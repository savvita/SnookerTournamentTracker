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

        public ServerModel()
        {
            listener = new TcpListener(IPAddress.Any, Connection.Port);
            db = new DbSnookerTournamentTrackerContext();

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
            handlers.Add(ConnectionCode.UpdateProfile, UpdateProfile);
            handlers.Add(ConnectionCode.CreateTournament, CreateTournament);
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


        private T? GetObjectFromMessage<T>(Message msg, out Message? response) where T : class
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

            var validationResult = ValidateObject<T>(obj);

            if (validationResult.Count > 0)
            {
                response = GetValidationErrorMessage(validationResult);
                return null;
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

            Validator.TryValidateObject(obj, context, validationResult);

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
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, out Message? response);

            if (user == null)
            {
                return response!;
            }

            // TODO add check in the db

            user.FirstName = "John";
            user.SecondName = "Ivanovich";
            user.LastName = "Smith";
            user.EmailAddress = "Smith@gmail.com";
            user.PhoneNumber = "0123456789";
            user.Id = 1;

            return new Message() { Content = JsonSerializer.Serialize<PersonModel>(user) };
        }

        private Message SignUp(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, out Message? response);

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

            return new Message() { Content = JsonSerializer.Serialize<PersonModel>(new PersonModel() {  Id = newUser.Id }) };
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer;

            if (password == null)
            {
                throw new ArgumentNullException("password");
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
                throw new ArgumentNullException("password");
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
            TournamentModel? tournament = GetObjectFromMessage<TournamentModel>(request, out Message? response);

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
                EntreeFee = tournament.EntryFee,
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

            return new Message() { Content = JsonSerializer.Serialize<TournamentModel>(new TournamentModel() { Id = newTourney.Id }) };
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
            //TODO - check this
            // TODO - load rounds and prizes separatly by request from the view
            var tournaments = db.Tournaments.Select(t => new TournamentModel()
            {
                Id = t.Id,
                Name = t.Name,
                Garantee = t.Garantee,
                EntryFee = t.EntreeFee,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                IsActive = !t.Equals("Finished")
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

            Tournament? tourney = db.Tournaments.Where(t => t.Id == id).FirstOrDefault();

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
                PlaceName = prize.Place.PlaceName,
                PrizeAmount = (double)prize.Amount
            }).ToList();

            return new Message() { Content = JsonSerializer.Serialize<List<PrizeModel>>(prizes) };
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

            Tournament? tourney = db.Tournaments.Where(t => t.Id == id).FirstOrDefault();

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
                Round = round.Round.RoundName,
                Frames = round.FrameCount
            }).ToList();
            
            return new Message() { Content = JsonSerializer.Serialize<List<RoundModel>>(rounds) };
        }

        private Message UpdateProfile(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, out Message? response);

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
