using Microsoft.EntityFrameworkCore;
using SnookerTournamentTracker.ConnectionLibrary;
using SnookerTournamentTrackerServer.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTrackerServer.Model
{
    internal class ServerModel : IDisposable
    {
        private TcpListener? listener;
        private Dictionary<ConnectionCode, Func<Message, Message>> handlers;
        private DbSnookerTournamentTrackerContext db;

        private Dictionary<string, int> registrationStatuses;
        private Dictionary<string, int> tournamentStatuses;
        private Dictionary<string, int> roundNames;

        public ServerModel()
        {
            listener = new TcpListener(IPAddress.Any, Connection.Port);

            db = new DbSnookerTournamentTrackerContext();
            db.Places.Load();
            db.Rounds.Load();
            db.TournamentStatuses.Load();
            db.RegistrationStatuses.Load();

            registrationStatuses = new Dictionary<string, int>();

            foreach(var st in db.RegistrationStatuses) 
            {
                registrationStatuses.Add(st.Status, st.Id);
            }

            tournamentStatuses = new Dictionary<string, int>();

            foreach (var st in db.TournamentStatuses)
            {
                tournamentStatuses.Add(st.Status, st.Id);
            }

            roundNames = new Dictionary<string, int>();

            foreach (var r in db.Rounds)
            {
                roundNames.Add(r.RoundName, r.Id);
            }

            handlers = new Dictionary<ConnectionCode, Func<Message, Message>>();
            InitializeHandlers();
            Task.Factory.StartNew(Listen);
        }

        private void InitializeHandlers()
        {
            handlers.Add(ConnectionCode.SignIn, SignIn);
            handlers.Add(ConnectionCode.SignUp, SignUp);
            handlers.Add(ConnectionCode.AllTournaments, GetAllTournaments);
            handlers.Add(ConnectionCode.AllRounds, GetAllRounds);
            handlers.Add(ConnectionCode.AllPlaces, GetAllPlaces);
            handlers.Add(ConnectionCode.AllPlayers, GetAllPlayers);
            handlers.Add(ConnectionCode.PrizesByTournamentId, GetPrizesByTournament);
            handlers.Add(ConnectionCode.RoundsByTournamentId, GetRoundsByTournament);
            handlers.Add(ConnectionCode.PlayersByTournamentId, GetPlayersByTournament);
            handlers.Add(ConnectionCode.MatchesByTournamentId, GetMatchesByTournament);
            handlers.Add(ConnectionCode.TournamentsByPlayerId, GetTournamentsByPlayer);
            handlers.Add(ConnectionCode.TournamentsByAdministratorId, GetTournamentsByAdministrator);
            handlers.Add(ConnectionCode.UpdateProfile, UpdateProfile);
            handlers.Add(ConnectionCode.CreateTournament, CreateTournament);
            handlers.Add(ConnectionCode.RegisterAtTournament, RegisterAtTournament);
            handlers.Add(ConnectionCode.UnregisterFromTournament, UnregisterFromTournament);
            handlers.Add(ConnectionCode.IsTournamentAdministrator, IsTournamentAdministrator);
            handlers.Add(ConnectionCode.AddTournamentRounds, AddTournamentRounds);
            handlers.Add(ConnectionCode.SaveTournamentDraw, SaveTournamentDraw);
            handlers.Add(ConnectionCode.SaveFrameResult, SaveFrameResult);
        }

        private void Listen()
        {
            if(listener == null)
            {
                return;
            }

            listener.Start();

            do
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Factory.StartNew((obj) => HandleClient(obj), client);
            } while (true);
        }

        private void HandleClient(object? obj)
        {
            if (obj is TcpClient tcpClient)
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    Message? msg = Connection.ReceiveMessage(stream);

                    if(msg == null)
                    {
                        return;
                    }

                    if(handlers.ContainsKey(msg.Code))
                    {
                        Message response = handlers[msg.Code](msg);
                        Connection.SendMessage(stream, response);
                    }
                    else
                    {
                        Message errorMsg = new Message()
                        {
                            Code = ConnectionCode.Error,
                            Content = "Unrecognized command"
                        };
                        Connection.SendMessage(tcpClient.GetStream(), errorMsg);
                    }

                }
                catch (Exception ex)
                {
                    Message errorMsg = new Message()
                    {
                        Code = ConnectionCode.Error,
                        Content = "Server internal error"
                    };
                    Connection.SendMessage(tcpClient.GetStream(), errorMsg);
                }
            }
        }


        private T? GetObjectFromMessage<T>(Message msg, bool validate, out Message? response) where T : class
        {
            response = null;

            if(msg.Content == null)
            {
                response = new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };

                return null;
            }

            T? obj = JsonSerializer.Deserialize<T>(msg.Content);

            if (obj == null)
            {
                response = new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };

                return null;
            }

            if (validate)
            {
                var validationResult = ValidateObject<T>(obj);

                if (validationResult.Count > 0)
                {
                    response = GetValidationErrorMessage(validationResult);
                    return null;
                }
            }

            return obj;
        }

        private List<ValidationResult> ValidateObject<T>(T obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }

            var context = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            Validator.TryValidateObject(obj, context, validationResult, true);

            return validationResult;
        }

        private Message GetValidationErrorMessage(List<ValidationResult> result)
        {
            return new Message()
            {
                Code = ConnectionCode.Error,
                Content = String.Join(Environment.NewLine, result)
            };
        }
        
        private Message SignIn(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, false, out Message? response);

            if (user == null)
            {
                return response!;
            }

            var dbUser = db.Users.Where(u => u.Email.Equals(user.EmailAddress)).FirstOrDefault();

            if (dbUser == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            if (user.OpenPassword == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Password is required"
                };
            }

            if (!VerifyHashedPassword(dbUser.Password, user.OpenPassword))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Login or/and password are incorrect"
                };
            }

            user.FirstName = dbUser.FirstName;
            user.SecondName = dbUser.SecondName;
            user.LastName = dbUser.LastName;
            user.EmailAddress = dbUser.Email;

            if(dbUser.PhoneNumbers.Count > 0)
            {
                user.PhoneNumber = dbUser.PhoneNumbers.ElementAt(0).Number;
            }
            user.Id = dbUser.Id;

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<PersonModel>(user) };
        }

        private Message SignUp(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, true, out Message? response);

            if (user == null)
            {
                return response!;
            }

            if (db.Users.Count() > 0)
            {

                bool isRegistered = db.Users.ToList().Any(u => u.Email.Equals(user.EmailAddress, StringComparison.OrdinalIgnoreCase));

                if (isRegistered)
                {
                    return new Message()
                    {
                        Code = ConnectionCode.Error,
                        Content = "This email is already registered"
                    };
                }
            }

            User newUser = new User()
            {
                FirstName = user.FirstName!,
                SecondName = user.SecondName,
                LastName = user.LastName!,
                Email = user.EmailAddress!,
                Password = HashPassword(user.OpenPassword!),
                UserRoleId = 1
            };

            var validationResult = ValidateObject<User>(newUser);

            if(validationResult.Count > 0)
            {
                return GetValidationErrorMessage(validationResult);
            }

            db.Users.Add(newUser);

            if (user.PhoneNumber != null)
            {
                newUser.PhoneNumbers.Add(new PhoneNumber()
                {
                    User = newUser,
                    Number = user.PhoneNumber
                });
            }

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<PersonModel>(new PersonModel() {  Id = newUser.Id }) };
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer;

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer = bytes.GetBytes(0x20);
            }

            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer, 0, dst, 0x11, 0x20);

            return Convert.ToBase64String(dst);
        }

        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer;

            if (hashedPassword == null)
            {
                return false;
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] src = Convert.FromBase64String(hashedPassword);

            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }

            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);

            byte[] buffer2 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer2, 0, 0x20);

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer = bytes.GetBytes(0x20);
            }

            return ByteArraysEqual(buffer2, buffer);
        }

        private bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        private Message CreateTournament(Message request)
        {
            if(request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
                };
            }
            TournamentModel? tournament = GetObjectFromMessage<TournamentModel>(request, true, out Message? response);

            if (tournament == null)
            {
                return response!;
            }

            // TODO check saving

            Tournament newTourney = new Tournament()
            {
                Name = tournament.Name!,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                Garantee = tournament.Garantee,
                EntreeFee = tournament.BuyIn,
                PrizeMode = (byte)tournament.PrizeMode,
                TournamentStatusId = 1
            };

            db.Tournaments.Add(newTourney);
            db.SaveChanges();

            SaveAdministrator(newTourney, (int)request.Sender);

            if (tournament.Prizes != null)
            {
                SavePrizes(newTourney, tournament.Prizes);
            }

            SaveRoundModel(newTourney, tournament.RoundModel);

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<TournamentModel>(new TournamentModel() { Id = newTourney.Id }) };
        }

        private void SaveAdministrator(Tournament tourney, int administratorId)
        {
            tourney.Administrators.Add(db.Users.Where(u => u.Id == administratorId).Select(u => new Administrator()
            {
                TournamentId = tourney.Id,
                UserId = u.Id
            }).First());
        }

        private void SavePrizes(Tournament tourney, List<PrizeModel> prizes)
        {
            prizes.ForEach(prize =>
            {
                if (prize.PrizeAmount != null)
                {
                    tourney.Prizes.Add(new Prize()
                    {
                        Amount = (decimal)prize.PrizeAmount,
                        PlaceId = db.Places.Where(place => place.PlaceName.Equals(prize.PlaceName)).Select(place => place.Id).First()
                    });
                }
            });
        }

        private void SaveRoundModel(Tournament tourney, List<RoundModel> rounds)
        {
            rounds.ForEach(round =>
            {
                if (round.Frames != null)
                {
                    tourney.TournamentsRounds.Add(new TournamentsRound()
                    {
                        Round = db.Rounds.Where(r => r.RoundName.Equals(round.Round)).First(),
                        FrameCount = (int)round.Frames
                    });
                }
            });
        }

        private Message GetAllTournaments(Message request)
        {
            //TODO prize mode
            var tournaments = db.Tournaments.Select(t => new TournamentModel()
            {
                Id = t.Id,
                Name = t.Name,
                Garantee = t.Garantee,
                BuyIn = t.EntreeFee,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.TournamentStatus.Status
                //IsActive = !t.TournamentStatus.Status.Equals("Finished")
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        private Message GetAllRounds(Message request)
        {
            var rounds = db.Rounds.Select(round => round.RoundName).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<string>>(rounds) };
        }

        private Message GetAllPlaces(Message request)
        {
            var places = db.Places.Select(place => new PrizeModel()
            {
                Id = place.Id,
                PlaceName = place.PlaceName
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PrizeModel>>(places) };
        }

        private Message GetAllPlayers(Message request)
        {
            var players = db.Users.Select(user => new PersonModel()
            {
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PersonModel>>(players) };
        }

        private Message GetPrizesByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if(!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            Tournament? tourney = db.Tournaments.Include(t => t.Prizes).Where(t => t.Id == id).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found"
                };
            }

            var prizes = tourney.Prizes.Select(prize => new PrizeModel()
            {
                Id = prize.Id,
                PlaceName = prize.Place.PlaceName,
                PrizeAmount = (double)prize.Amount
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PrizeModel>>(prizes) };
        }

        private Message GetRoundsByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            Tournament? tourney = db.Tournaments.Include(t => t.TournamentsRounds).Where(t => t.Id == id).FirstOrDefault();

            if(tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found"
                };
            }

            var rounds = tourney.TournamentsRounds.Select(round => new RoundModel()
            {
                Id = round.Id,
                Round = round.Round.RoundName,
                Frames = round.FrameCount
            }).ToList();
            
            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<RoundModel>>(rounds) };
        }

        private Message GetMatchesByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            Tournament? tourney = db.Tournaments
                .Include(t => t.TournamentsRounds)
                .Include(t => t.Matches)
                .ThenInclude(t => t.MatchUpEntries)
                .Where(t => t.Id == id).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found"
                };
            }

            var matches = new List<List<MatchUpModel>>();

            var dbMatches = tourney.Matches.GroupBy(m => m.RoundId).OrderByDescending(g => g.Key).ToList();

            foreach(var group in dbMatches)
            {
                List<MatchUpModel> round = new List<MatchUpModel>();

                foreach(var match in group)
                {
                    MatchUpModel model = new MatchUpModel()
                    {
                        Id = match.Id,
                        MatchNumber = match.MatchNumber
                    };

                    var r = db.TournamentsRounds.Where(r => r.Id == match.RoundId).First();

                    model.Winner = match.Winner == null ? null : new PersonModel()
                    {
                        Id = match.Winner.Id,
                        FirstName = match.Winner.FirstName,
                        SecondName = match.Winner.SecondName,
                        LastName = match.Winner.LastName
                    };

                    model.MatchUpRound = new RoundModel()
                    {
                        Id = match.RoundId,
                        Round = r.Round.RoundName,
                        Frames = match.Round.FrameCount,
                    };

                    var frames = db.Frames.Where(f => f.MatchId == match.Id).Include(f => f.FrameEntities).ToList();

                    foreach(var frame in frames)
                    {
                        FrameModel f = new FrameModel()
                        {
                            Id = frame.Id,
                            MatchId = frame.MatchId,
                            WinnerId = frame.WinnerId
                        };

                        foreach(var entry in frame.FrameEntities)
                        {
                            f.Entries.Add(new FrameEntryModel()
                            {
                                Id = entry.Id,
                                PlayerId = entry.PlayerId,
                                Score = entry.Score
                            });
                        }

                        model.Frames.Add(f);
                    }

                    foreach (var entry in match.MatchUpEntries)
                    {
                        var ent = new MatchUpEntryModel()
                        {
                            Id = entry.Id
                        };

                        ent.ParentMatchUp = new MatchUpModel()
                        {
                            Id = entry.ParentMatchId ?? 0,
                            MatchNumber = entry.ParentMatch != null ? entry.ParentMatch.MatchNumber : null
                        };

                        var user = db.Users.Where(u => u.Id == entry.UserId).FirstOrDefault();

                        if (user != null)
                        {
                            ent.Player = new PersonModel()
                            {
                                Id = user.Id,
                                FirstName = user.FirstName,
                                SecondName = user.SecondName,
                                LastName = user.LastName
                            };
                        }

                        model.Entries.Add(ent);
                    }

                    round.Add(model);
                }

                matches.Add(round);
            }


            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<List<MatchUpModel>>>(matches) };
        }

        private Message GetPlayersByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            Tournament? tourney = db.Tournaments
                .Include(t => t.TournamentsPlayers)
                .ThenInclude(pl => pl.User)
                .Where(t => t.Id == id).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found"
                };
            }

            var players = tourney.TournamentsPlayers
                .Where(pl => pl.RegistrationStatusId == registrationStatuses["Registered"])
                .Select(player => new PersonModel()
            {
                FirstName = player.User.FirstName,
                SecondName = player.User.SecondName,
                LastName = player.User.LastName
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PersonModel>>(players) };
        }

        private Message GetTournamentsByAdministrator(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            List<TournamentModel> tournaments = new List<TournamentModel>();

            var dbTournaments = db.Administrators.Where(adm => adm.UserId == id).Select(t => t.Tournament);

            foreach (var tournament in dbTournaments)
            {
                tournaments.Add(new TournamentModel()
                {
                    Id = tournament.Id,
                    Name = tournament.Name,
                    StartDate = tournament.StartDate,
                    EndDate = tournament.EndDate,
                    Garantee = tournament.Garantee,
                    BuyIn = tournament.EntreeFee,
                    Status = tournament.TournamentStatus.Status
                    //IsActive = !tournament.TournamentStatus.Equals("Finished") // TODO change this like registration statuses
                });
            }


            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        private Message GetTournamentsByPlayer(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            List<TournamentModel> tournaments = new List<TournamentModel>();

            var dbTournaments = db.TournamentsPlayers
                .Where(pl => pl.UserId == id)
                .Select(t => t.Tournament);

            foreach (var tournament in dbTournaments)
            {
                tournaments.Add(new TournamentModel()
                {
                    Id = tournament.Id,
                    Name = tournament.Name,
                    StartDate = tournament.StartDate,
                    EndDate = tournament.EndDate,
                    Garantee = tournament.Garantee,
                    BuyIn = tournament.EntreeFee,
                    Status = tournament.TournamentStatus.Status
                    //IsActive = !tournament.TournamentStatus.Equals("Finished") // TODO change this like registration statuses
                });
            }


            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        private Message IsTournamentAdministrator(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Sender cannot be null"
                };
            }

            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            bool isAdmin = db.Administrators.Any(adm => adm.UserId == request.Sender && adm.TournamentId == id);

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<bool>(isAdmin) };
        }

        private Message RegisterAtTournament(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Sender cannot be null"
                };
            }

            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            var tourney = db.Tournaments.Where(t => t.Id == id && !t.TournamentStatus.Status.Equals("Finished")).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is completed"
                };
            }

            var user = db.Users.Where(u => u.Id == request.Sender).FirstOrDefault();

            if (user == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            tourney.TournamentsPlayers.Add(new TournamentsPlayer()
            {
                RegistrationStatusId = registrationStatuses["Registered"], //TODO deepnd on entree fee
                Tournament = tourney,
                User = user
            });

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        private Message UnregisterFromTournament(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Sender cannot be null"
                };
            }

            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            if (!int.TryParse(request.Content, out int id))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            var tourney = db.Tournaments.Where(t => t.Id == id && !t.TournamentStatus.Status.Equals("Finished")).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is completed"
                };
            }

            var player = tourney.TournamentsPlayers.Where(pl => pl.UserId == request.Sender).FirstOrDefault();

            if (player == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            //tourney.TournamentsPlayers.Remove(player);
            player.RegistrationStatusId = registrationStatuses["Unregistered"];

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        private Message AddTournamentRounds(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
                };
            }
            TournamentModel? tournament = GetObjectFromMessage<TournamentModel>(request, true, out Message? response);

            if (tournament == null)
            {
                return response!;
            }

            var tourney = db.Tournaments.Where(t => t.Id == tournament.Id && !t.TournamentStatus.Status.Equals("Finished")).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is completed"
                };
            }

            tournament.RoundModel.ForEach(round =>
            {
                if(!tourney.TournamentsRounds.Any(r => r.Round.RoundName.Equals(round.Round)))
                {
                    tourney.TournamentsRounds.Add(new TournamentsRound()
                    {
                        RoundId = db.Rounds.First(r => r.RoundName.Equals(round.Round)).Id,
                        FrameCount = (int)round.Frames!,
                        TournamentId = (int)tournament.Id!
                    });
                }
            });

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        private Message SaveTournamentDraw (Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
                };
            }
            TournamentModel? tournament = GetObjectFromMessage<TournamentModel>(request, true, out Message? response);

            if (tournament == null)
            {
                return response!;
            }

            var tourney = db.Tournaments.Where(t => t.Id == tournament.Id && !t.TournamentStatus.Status.Equals("Finished")).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is completed"
                };
            }

            SaveMatchesDraw(tournament, tourney);

            SaveMatchUpEntries(tournament);

            tourney.TournamentStatusId = tournamentStatuses["On play"];

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        private Message SaveFrameResult(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
                };
            }
            FrameModel? frame = GetObjectFromMessage<FrameModel>(request, true, out Message? response);

            if (frame == null)
            {
                return response!;
            }

            //TODO handle breaks

            Frame dbFrame = new Frame()
            {
                MatchId = frame.MatchId,
                WinnerId = frame.WinnerId
            };

            db.Frames.Add(dbFrame);

            db.SaveChanges();

            dbFrame.FrameEntities.Add(new FrameEntity()
            {
                FrameId = dbFrame.Id,
                PlayerId = (int)frame.Entries[0].PlayerId!,
                Score = (short)frame.Entries[0].Score!
            });

            dbFrame.FrameEntities.Add(new FrameEntity()
            {
                FrameId = dbFrame.Id,
                PlayerId = (int)frame.Entries[1].PlayerId!,
                Score = (short)frame.Entries[1].Score!
            });

            db.SaveChanges();

            try
            {
                HandleFrameResults(frame);
            }
            catch
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Invalid match draw"
                };
            }

            return new Message() { Code = ConnectionCode.Ok };
        }

        private void HandleFrameResults(FrameModel frame)
        {
            Match currMatch = db.Matches.Where(m => m.Id == frame.MatchId).First();
            int score1 = currMatch.Frames.Where(f => f.WinnerId == frame.Entries[0].PlayerId).Count();
            int score2 = currMatch.Frames.Where(f => f.WinnerId == frame.Entries[1].PlayerId).Count();
            int frameCount = currMatch.Round.FrameCount;

            if(score1 >= frameCount / 2 || score2 >= frameCount / 2)
            {
                currMatch.WinnerId = score1 > score2 ? frame.Entries[0].PlayerId : frame.Entries[1].PlayerId;

                db.MatchUpEntries.Where(m => m.MatchId == currMatch.Id && m.UserId == frame.Entries[0].PlayerId).First().Score = score1;
                db.MatchUpEntries.Where(m => m.MatchId == currMatch.Id && m.UserId == frame.Entries[1].PlayerId).First().Score = score2;

                if(currMatch.Round.Round.RoundName.Equals("Final"))
                {
                    db.Tournaments.Where(t => t.Id == currMatch.TournamentId).First().TournamentStatusId = tournamentStatuses["Finished"];
                }
                else
                {
                    MatchUpEntry nextMatchEntry = db.MatchUpEntries.Where(m => m.ParentMatchId == currMatch.Id).First();
                    nextMatchEntry.UserId = currMatch.WinnerId;
                }
            }

            db.SaveChanges();
        }

        private void SaveMatchesDraw(TournamentModel tournament, Tournament tourney)
        {
            tournament.Rounds.ForEach(round =>
            {
                round.ForEach(match =>
                {
                    if(match.MatchUpRound == null || match.MatchUpRound.Round == null || match.MatchNumber == null)
                    {
                        throw new Exception("Invalid match information");
                    }

                    Match m = new Match()
                    {
                        TournamentId = (int)tournament.Id!,
                        RoundId = roundNames[match.MatchUpRound.Round],
                        WinnerId = match.Winner == null ? null : match.Winner.Id,
                        MatchNumber = (int)match.MatchNumber
                    };

                    tourney.Matches.Add(m);
                });
            });

            db.SaveChanges();
        }



        private void SaveMatchUpEntries(TournamentModel tournament)
        {
            // GET matches from tourney

            var matches = db.Matches.Where(m => m.TournamentId == tournament.Id).ToList();

            tournament.Rounds.ForEach(round =>
            {
                round.ForEach(match =>
                {
                    match.Entries.ForEach(entry =>
                    {
                        MatchUpEntry ent = new MatchUpEntry();

                        ent.MatchId = matches.Where(m => m.MatchNumber == match.MatchNumber).First().Id;
                        //ent.ParentMatchId = entry.ParentMatchUp != null ? entry.ParentMatchUp.Id : null;
                        ent.UserId = entry.Player != null ? entry.Player.Id : null;

                        db.MatchUpEntries.Add(ent);
                    });
                });
            });

            db.SaveChanges();
        }

        private Message UpdateProfile(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, true, out Message? response);

            if (user == null)
            {
                return response!;
            }

            User? dbUser = db.Users.Where(user => user.Id == request.Sender).FirstOrDefault();

            if(dbUser == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            dbUser.FirstName = user.FirstName!;
            dbUser.SecondName = user.SecondName;
            dbUser.LastName = user.LastName!;

            if (user.PhoneNumber != null)
            {
                if (dbUser.PhoneNumbers.Count() == 0)
                {
                    dbUser.PhoneNumbers.Add(new PhoneNumber()
                    {
                        Number = user.PhoneNumber,
                        User = dbUser
                    });
                }
                else
                {
                    dbUser.PhoneNumbers.ElementAt(0).Number = user.PhoneNumber;
                }
            }

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public void Dispose()
        {
            listener?.Stop();
        }
    }
}
