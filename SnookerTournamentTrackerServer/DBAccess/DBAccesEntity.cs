using Microsoft.EntityFrameworkCore;
using SnookerTournamentTracker.ConnectionLibrary;
using SnookerTournamentTrackerServer.DbModel;
using SnookerTournamentTrackerServer.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTrackerServer.DBAccess
{
    internal class DBAccesEntity : IDBAccess
    {
        //private DbSnookerTournamentTrackerContext db;
        private DbSnookerTournamentTrackerSmarterContext db;
        private Dictionary<string, int> registrationStatuses;
        private Dictionary<string, int> tournamentStatuses;
        private Dictionary<string, int> roundNames;

        public DBAccesEntity()
        {
            db = new DbSnookerTournamentTrackerSmarterContext();
            db.Places.Load();
            db.Rounds.Load();
            db.TournamentStatuses.Load();
            db.RegistrationStatuses.Load();

            registrationStatuses = new Dictionary<string, int>();

            foreach (var st in db.RegistrationStatuses)
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
        }
        
        public Message SignIn(Message request)
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

            if (!Passwords.VerifyHashedPassword(dbUser.Password, user.OpenPassword))
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

            if (dbUser.PhoneNumbers.Count > 0)
            {
                user.PhoneNumber = dbUser.PhoneNumbers.ElementAt(0).Number;
            }
            user.Id = dbUser.Id;

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<PersonModel>(user) };
        }

        public Message SignUp(Message request)
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
                Password = Passwords.HashPassword(user.OpenPassword!),
                UserRoleId = 1
            };

            var validationResult = ValidateObject<User>(newUser);

            if (validationResult.Count > 0)
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

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<PersonModel>(new PersonModel() { Id = newUser.Id }) };
        }

        public Message UpdateProfile(Message request)
        {
            PersonModel? user = GetObjectFromMessage<PersonModel>(request, true, out Message? response);

            if (user == null)
            {
                return response!;
            }

            if (request.Sender != user.Id)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User cannot change other user"
                };
            }

            User? dbUser = db.Users.Where(user => user.Id == request.Sender).FirstOrDefault();

            if (dbUser == null)
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

        public Message GetUserCards(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            User? user = db.Users.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            List<CardModel> cards = db.Cards.Where(c => c.UserId == id).Select(c => new CardModel()
            {
                Id = c.Id,
                UserId = c.UserId,
                CardNumber = c.CardNumber,
                Name = c.Name
            }).ToList();


            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<CardModel>>(cards) };
        }

        public Message SaveUserCard(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User cannot be null"
                };
            }
            CardModel? card = GetObjectFromMessage<CardModel>(request, true, out Message? response);

            if (card == null)
            {
                return response!;
            }

            if (card.CardNumber.Any(ch => !Char.IsDigit(ch)))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Card number is incorrect"
                };
            }

            Card c = new Card()
            {
                CardNumber = card.CardNumber,
                Name = card.Name,
                UserId = card.UserId
            };

            db.Cards.Add(c);

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok, Content = c.Id.ToString() };
        }

        public Message GetAllTournaments(Message request)
        {
            var t = db.Tournaments.ToList();

            var tournaments = db.Tournaments.Select(t => new TournamentModel()
            {
                Id = t.Id,
                Name = t.Name,
                Garantee = t.Garantee,
                BuyIn = t.EntreeFee,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.TournamentStatus.Status,
                PrizeMode = t.PrizeMode != null ? (PrizesModeEnum)t.PrizeMode : 0,
                PaymentInfo = t.PaymentInfo == null ? null : new PaymentInfoModel()
                {
                    Id = t.PaymentInfo.Id,
                    Card = new CardModel()
                    {
                        Id = t.PaymentInfo.CardId,
                        CardNumber = db.Cards.Where(c => c.Id == t.PaymentInfo.CardId).First().CardNumber,
                        Name = t.PaymentInfo.Card.Name
                    },
                    Sum = t.PaymentInfo.Sum
                }
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        public Message GetAllRounds(Message request)
        {
            var rounds = db.Rounds.Select(round => round.RoundName).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<string>>(rounds) };
        }

        public Message GetAllPlaces(Message request)
        {
            var places = db.Places.Select(place => new PrizeModel()
            {
                Id = place.Id,
                PlaceName = place.PlaceName
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PrizeModel>>(places) };
        }

        public Message GetAllPlayers(Message request)
        {
            var players = db.Users.Select(user => new PersonModel()
            {
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName
            }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<PersonModel>>(players) };
        }

        public Message GetPrizesByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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

        public Message GetRoundsByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            Tournament? tourney = db.Tournaments.Include(t => t.TournamentsRounds).Where(t => t.Id == id).FirstOrDefault();

            if (tourney == null)
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

        public Message GetMatchesByTournament(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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

            foreach (var group in dbMatches)
            {
                List<MatchUpModel> round = new List<MatchUpModel>();

                foreach (var match in group)
                {
                    MatchUpModel model = new MatchUpModel()
                    {
                        Id = match.Id,
                        MatchNumber = match.MatchNumber
                    };

                    var r = db.TournamentsRounds.Where(r => r.TournamentId == tourney.Id && r.RoundId == match.RoundId).First();


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
                        Frames = r.FrameCount
                    };

                    var frames = db.Frames.Where(f => f.MatchId == match.Id).Include(f => f.FrameEntities).ToList();

                    foreach (var frame in frames)
                    {
                        FrameModel f = new FrameModel()
                        {
                            Id = frame.Id,
                            MatchId = frame.MatchId,
                            WinnerId = frame.WinnerId
                        };

                        foreach (var entry in frame.FrameEntities)
                        {
                            f.Entries.Add(new FrameEntryModel()
                            {
                                Id = entry.Id,
                                PlayerId = entry.PlayerId,
                                Score = entry.Score
                            });
                        }

                        foreach (var br in db.Brakes.Where(x => x.FrameId == frame.Id))
                        {
                            f.Breaks.Add(new BreakModel()
                            {
                                Id = br.Id,
                                PlayerId = br.PlayerId,
                                Score = br.Score
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

        public Message GetPlayersByTournament(Message request)
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
                .Where(pl => pl.RegistrationStatusId != registrationStatuses["Unregistered"])
                .Select(player =>
                {
                    TournamentPlayer pl = new TournamentPlayer()
                    {
                        Player = new PersonModel()
                        {
                            Id = player.User.Id,
                            FirstName = player.User.FirstName,
                            SecondName = player.User.SecondName,
                            LastName = player.User.LastName
                        },
                        IsFinished = player.IsFinished ?? true,
                        TournamentId = tourney.Id,
                        Status = player.RegistrationStatus.Status
                    };

                    var payment = db.Payments.Where(p => p.UserId == player.User.Id && p.TournamentId == tourney.Id).FirstOrDefault();

                    if (payment != null)
                    {
                        pl.Payment = payment.Amount;
                    }

                    return pl;
                }).ToList();

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentPlayer>>(players) };
        }

        public Message GetTournamentsByAdministrator(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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
                });
            }

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        public Message GetTournamentsByPlayer(Message request)
        {
            if (request.Content == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Content was null"
                };
            }

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            List<TournamentModel> tournaments = new List<TournamentModel>();

            var dbTournaments = db.TournamentsPlayers
                .Where(pl => pl.UserId == id && pl.RegistrationStatusId == registrationStatuses["Registered"])
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
                });
            }

            return new Message() { Code = ConnectionCode.Ok, Content = JsonSerializer.Serialize<List<TournamentModel>>(tournaments) };
        }

        public Message IsTournamentAdministrator(Message request)
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

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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

        public Message RegisterAtTournament(Message request)
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

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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

            if (tourney.TournamentsPlayers.Any(pl => pl.UserId == request.Sender && pl.RegistrationStatusId != registrationStatuses["Unregistered"]))
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Player is already registered"
                };
            }

            tourney.TournamentsPlayers.Add(new TournamentsPlayer()
            {
                RegistrationStatusId = tourney.EntreeFee == null ? registrationStatuses["Registered"] : registrationStatuses["Waiting for paying"], //TODO deepnd on entree fee
                Tournament = tourney,
                User = user,
                IsFinished = false
            });

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public Message ConfirmPlayerRegistration(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
                };
            }
            TournamentPlayer? player = GetObjectFromMessage<TournamentPlayer>(request, true, out Message? response);

            if (player == null)
            {
                return response!;
            }

            var tourney = db.Tournaments.Where(t => t.Id == player.TournamentId && !t.TournamentStatus.Status.Equals("Finished")).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is completed"
                };
            }

            var dbPlayer = tourney.TournamentsPlayers.Where(pl => pl.UserId == player.Id).FirstOrDefault();

            if (dbPlayer == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Player not found or is not registered to this tournament"
                };
            }

            dbPlayer.RegistrationStatusId = registrationStatuses["Registered"];

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public Message UnregisterFromTournament(Message request)
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

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
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

            var player = tourney.TournamentsPlayers.Where(pl => pl.UserId == request.Sender && pl.RegistrationStatusId != registrationStatuses["Unregistered"]).FirstOrDefault();

            if (player == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "User not found"
                };
            }

            player.RegistrationStatusId = registrationStatuses["Unregistered"];

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public Message CancelTournament(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administartor of the tournament cannot be null"
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

            int? id = JsonSerializer.Deserialize<int>(request.Content);

            if (id == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Request has an incorrect format"
                };
            }

            var tourney = db.Tournaments.Where(t => t.Id == id && t.TournamentStatusId == tournamentStatuses["Registration"]).FirstOrDefault();

            if(tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is not on registration"
                };
            }

            var admin = db.Administrators.Where(a => a.TournamentId == id && a.UserId == request.Sender).FirstOrDefault();

            if(admin == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Only administrator of the tournament can cancel tournament"
                };
            }

            tourney.TournamentStatusId = tournamentStatuses["Cancelled"];

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public Message UpdateTournament(Message request)
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
           

            var tourney = db.Tournaments.Where(t => t.Id == tournament.Id && t.TournamentStatusId == tournamentStatuses["Registration"]).FirstOrDefault();

            if (tourney == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Tournament not found or is not on registration"
                };
            }

            var admin = db.Administrators.Where(a => a.TournamentId == tournament.Id && a.UserId == request.Sender).FirstOrDefault();

            if (admin == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Only administrator of the tournament can edit a tournament"
                };
            }

            tourney.Name = tournament.Name!;
            tourney.StartDate = tournament.StartDate;
            tourney.EndDate = tournament.EndDate;

            db.SaveChanges();

            return new Message() { Code = ConnectionCode.Ok };
        }

        public Message CreateTournament(Message request)
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

            int? payment = null;

            if (tournament.PaymentInfo != null)
            {
                PaymentInfo info = new PaymentInfo()
                {
                    CardId = tournament.PaymentInfo.Card.Id,
                    Sum = tournament.PaymentInfo.Sum
                };
                db.PaymentInfos.Add(info);

                db.SaveChanges();

                payment = info.Id;
            }


            Tournament newTourney = new Tournament()
            {
                Name = tournament.Name!,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate,
                Garantee = tournament.Garantee,
                EntreeFee = tournament.BuyIn,
                PrizeMode = (byte)tournament.PrizeMode,
                TournamentStatusId = 1,
                PaymentInfoId = payment
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

        public Message AddTournamentRounds(Message request)
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
                if (!tourney.TournamentsRounds.Any(r => r.Round.RoundName.Equals(round.Round)))
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

        public Message SaveTournamentDraw(Message request)
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

        private void SaveMatchesDraw(TournamentModel tournament, Tournament tourney)
        {
            tournament.Rounds.ForEach(round =>
            {
                round.ForEach(match =>
                {
                    if (match.MatchUpRound == null || match.MatchUpRound.Round == null || match.MatchNumber == null)
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
            var matches = db.Matches.Where(m => m.TournamentId == tournament.Id).ToList();

            tournament.Rounds.ForEach(round =>
            {
                round.ForEach(match =>
                {
                    match.Entries.ForEach(entry =>
                    {
                        MatchUpEntry ent = new MatchUpEntry();

                        ent.MatchId = matches.Where(m => m.MatchNumber == match.MatchNumber).First().Id;

                        if (entry.ParentMatchUp == null)
                        {
                            ent.ParentMatchId = null;
                        }
                        else
                        {
                            ent.ParentMatchId = matches.First(m => m.MatchNumber == entry.ParentMatchUp.MatchNumber).Id;
                        }

                        ent.UserId = entry.Player != null ? entry.Player.Id : null;


                        db.MatchUpEntries.Add(ent);
                    });
                });
            });

            db.SaveChanges();
        }

        public Message SaveFrameResult(Message request)
        {
            if (request.Sender == null)
            {
                return new Message()
                {
                    Code = ConnectionCode.Error,
                    Content = "Administrator of the tournament cannot be null"
                };
            }
            FrameModel? frame = GetObjectFromMessage<FrameModel>(request, true, out Message? response);

            if (frame == null)
            {
                return response!;
            }


            Frame dbFrame = new Frame()
            {
                MatchId = frame.MatchId,
                WinnerId = frame.WinnerId
            };

            db.Frames.Add(dbFrame);

            db.SaveChanges();

            SaveFrameEntries(dbFrame, frame);
            SaveFrameBreaks(dbFrame, frame);

            try
            {
                HandleFrameResults(dbFrame, frame);
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

        private void SaveFrameEntries(Frame dbFrame, FrameModel frame)
        {
            foreach (var entry in frame.Entries)
            {
                dbFrame.FrameEntities.Add(new FrameEntity()
                {
                    FrameId = dbFrame.Id,
                    PlayerId = (int)entry.PlayerId!,
                    Score = (short?)entry.Score
                });
            }

            db.SaveChanges();
        }

        private void SaveFrameBreaks(Frame dbFrame, FrameModel frame)
        {
            foreach (var br in frame.Breaks)
            {
                db.Brakes.Add(new Brake()
                {
                    FrameId = dbFrame.Id,
                    PlayerId = br.PlayerId,
                    Score = br.Score
                });
            }

            db.SaveChanges();
        }

        private void HandleFrameResults(Frame dbFrame, FrameModel frame)
        {
            var tourney = db.Matches.Where(m => m.Id == dbFrame.MatchId).First().Tournament;

            Match currMatch = db.Matches.Where(m => m.Id == frame.MatchId).First();

            var round = tourney.TournamentsRounds.First(r => r.RoundId == currMatch.RoundId);

            if (frame.Entries.Count > 1)
            {
                int score1 = currMatch.Frames.Where(f => f.WinnerId == frame.Entries[0].PlayerId).Count();
                int score2 = currMatch.Frames.Where(f => f.WinnerId == frame.Entries[1].PlayerId).Count();

                var frameCount = round.FrameCount;

                if (score1 > frameCount / 2 || score2 > frameCount / 2)
                {
                    currMatch.IsCompleted = true;
                    currMatch.WinnerId = score1 > score2 ? frame.Entries[0].PlayerId : frame.Entries[1].PlayerId;

                    var looser = currMatch.WinnerId == frame.Entries[0].PlayerId ? frame.Entries[1].PlayerId : frame.Entries[0].PlayerId;
                    db.TournamentsPlayers.Where(pl => pl.TournamentId == tourney.Id && pl.UserId == looser).First().IsFinished = true;

                    db.MatchUpEntries.Where(m => m.MatchId == currMatch.Id && m.UserId == frame.Entries[0].PlayerId).First().Score = score1;
                    db.MatchUpEntries.Where(m => m.MatchId == currMatch.Id && m.UserId == frame.Entries[1].PlayerId).First().Score = score2;
                }
            }

            else
            {
                currMatch.IsCompleted = true;
                currMatch.WinnerId = frame.Entries[0].PlayerId;
            }

            if (round.Round.RoundName.Equals("Final"))
            {
                db.TournamentsPlayers.Where(pl => pl.TournamentId == tourney.Id && pl.UserId == currMatch.WinnerId).First().IsFinished = true;
                db.Tournaments.Where(t => t.Id == currMatch.TournamentId).First().TournamentStatusId = tournamentStatuses["Finished"];
            }
            else
            {
                MatchUpEntry nextMatchEntry = db.MatchUpEntries.Where(m => m.ParentMatchId == currMatch.Id).First();
                nextMatchEntry.UserId = currMatch.WinnerId;
            }

            if (currMatch.WinnerId != null)
            {
                SetPrizes(tourney, currMatch);
            }

            db.SaveChanges();
        }

        private void SetPrizes(Tournament tournament, Match match)
        {
            if(tournament.Prizes == null)
            {
                return;
            }

            var round = tournament.TournamentsRounds.First(r => r.RoundId == match.RoundId);

            var prizes = tournament.Prizes.Where(pr => pr.Place.RoundId == round.RoundId);

            var looser = match.MatchUpEntries.Count > 1 ? match.MatchUpEntries.First(e => e.UserId != match.WinnerId).UserId : null;

            if (round.Round.RoundName.Equals("Final"))
            {
                db.TournamentsPlayers.Where(pl => pl.TournamentId == tournament.Id && pl.UserId == match.WinnerId).First().IsFinished = true;
                db.Tournaments.Where(t => t.Id == match.TournamentId).First().TournamentStatusId = tournamentStatuses["Finished"];

                if (prizes != null)
                {
                    var prize = prizes.Where(pr => pr.Place.PlaceName.Equals("Winner")).FirstOrDefault();
                    if (prize != null)
                    {
                        Payment payment = new Payment()
                        {
                            TournamentId = tournament.Id,
                            UserId = (int)match.WinnerId!
                        };

                        if (tournament.PrizeMode == 0)
                        {
                            payment.Amount = prize.Amount;
                        }
                        else
                        {
                            decimal total = 0;
                            if (tournament.Garantee != null)
                            {
                                total = prize.Amount;
                            }
                            else if (tournament.EntreeFee != null)
                            {
                                total = tournament.TournamentsPlayers.Count() * (decimal)tournament.EntreeFee;
                            }
                            payment.Amount = prize.Amount * total / 100;
                        }

                        db.Payments.Add(payment);
                    }

                    if (looser != null)
                    {
                        prize = prizes.Where(pr => pr.Place.PlaceName.Equals("Finalist")).FirstOrDefault();
                        if (prize != null)
                        {
                            Payment payment = new Payment()
                            {
                                TournamentId = tournament.Id,
                                UserId = (int)looser!
                            };

                            if (tournament.PrizeMode == 0)
                            {
                                payment.Amount = prize.Amount;
                            }
                            else
                            {
                                decimal total = 0;
                                if (tournament.Garantee != null)
                                {
                                    total = prize.Amount;
                                }
                                else if (tournament.EntreeFee != null)
                                {
                                    total = tournament.TournamentsPlayers.Count() * (decimal)tournament.EntreeFee;
                                }
                                payment.Amount = prize.Amount * total / 100;
                            }

                            db.Payments.Add(payment);
                        }
                    }
                }
            }
            else if (looser != null)
            {
                MatchUpEntry nextMatchEntry = db.MatchUpEntries.Where(m => m.ParentMatchId == match.Id).First();
                nextMatchEntry.UserId = match.WinnerId;

                if (prizes != null)
                {
                    var prize = prizes.FirstOrDefault();
                    if (prize != null)
                    {
                        Payment payment = new Payment()
                        {
                            TournamentId = tournament.Id,
                            UserId = (int)looser!
                        };

                        if (tournament.PrizeMode == 0)
                        {
                            payment.Amount = prize.Amount;
                        }
                        else
                        {
                            payment.Amount = tournament.Garantee != null ? prize.Amount * (decimal)tournament.Garantee / 100 : 0;
                        }

                        db.Payments.Add(payment);
                    }
                }
            }
        }

        private T? GetObjectFromMessage<T>(Message msg, bool validate, out Message? response) where T : class
        {
            response = null;

            if (msg.Content == null)
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
            if (obj == null)
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
    }
}
