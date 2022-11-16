using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.Model
{
    internal static class ConnectionClientModel
    {
        //private static ClientModel client = new ClientModel();
        //public static async Task<bool> SignIn(PersonModel user, out string error)
        public static async Task<bool> SignIn(PersonModel user)
        {
            // TODO - Add request to the server
            //TODO - Add error text

            //error = "Error";
            user.FirstName = "John";
            user.SecondName = "Ivanovich";
            user.LastName = "Smith";
            user.EmailAddress = "Smith@gmail.com";
            user.PhoneNumber = "0123456789";
            user.Id = 1;

            //client.SendMessage(new Message() { Sender = user.Id, Code = 2, Content = "Some text" });
            return true;


            //error = "Error";
            //if (!File.Exists("users.json"))
            //{
            //    return false;
            //}

            //string json = File.ReadAllText("users.json");

            //List<PersonModel>? players = JsonSerializer.Deserialize<List<PersonModel>>(json);
            //if(players == null)
            //{
            //    return false;
            //}

            //return players.FindIndex(player => {
            //    if(player == null || player.Login == null)
            //    {
            //        return false;
            //    }
            //    return player.Login.Equals(user.Login);
            //}) != -1;
        }

        public static bool SignUp(PersonModel user, out string error)
        {
            // TODO - Add request to the server
            //TODO - Add error text
            error = "Error";
            user.Id = 2;

            return true;
            //if (!File.Exists("users.json"))
            //{
            //    File.Create("users.json");
            //}

            //string json = File.ReadAllText("users.json");


            //List<PersonModel>? players = new List<PersonModel>();

            //try
            //{
            //    players = JsonSerializer.Deserialize<List<PersonModel>>(json);
            //}
            //catch
            //{
            //}

            //if(players == null)
            //{
            //    players = new List<PersonModel>();
            //}

            //players.Add(user);

            //json = JsonSerializer.Serialize<List<PersonModel>>(players);
            //File.WriteAllText("users.json", json);

            ////TODO - Add checks
            //return true;
        }

        private static string? SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static List<PrizeModel> GetAllPlaces()
        {
            //TODO - Load from DB
            List<PrizeModel> places = new List<PrizeModel>();
            places.Add(new PrizeModel() { PlaceName = "Winner" });
            places.Add(new PrizeModel() { PlaceName = "Round Up" });
            places.Add(new PrizeModel() { PlaceName = "Semi final" });
            places.Add(new PrizeModel() { PlaceName = "Quarter final" });
            places.Add(new PrizeModel() { PlaceName = "Last 16" });
            places.Add(new PrizeModel() { PlaceName = "Last 32" });
            places.Add(new PrizeModel() { PlaceName = "Last 64" });
            places.Add(new PrizeModel() { PlaceName = "Last 128" });
            places.Add(new PrizeModel() { PlaceName = "Rest" });

            return places;
        }

        public static List<string> GetAllRounds()
        {
            //TODO - Load from DB
            List<string> rounds = new List<string>();

            rounds.Add("Final");
            rounds.Add("Semi final");
            rounds.Add("Quarter final");
            rounds.Add("Last 16");
            rounds.Add("Last 32");
            rounds.Add("Last 64");
            rounds.Add("Last 128");
            rounds.Add("Rest");
            return rounds;
        }

        public static List<PersonModel> GetAllPlayers()
        {
            //TODO - Load from DB
            List<PersonModel> players = new List<PersonModel>();

            players.Add(new PersonModel()
            {
                FirstName = "John",
                SecondName = "J.",
                LastName = "Smith",
                EmailAddress = "smith@mail.com"
            });

            players.Add(new PersonModel()
            {
                FirstName = "Ivan",
                SecondName = "Ivanovich",
                LastName = "Petrov",
                EmailAddress = "petrov@mail.com"
            });

            players.Add(new PersonModel()
            {
                FirstName = "John",
                LastName = "Doe"
            });
            return players;
        }

        public static bool CreateTournament(TournamentModel tournament)
        {
            //TODO add creation and creation rounds
            tournaments.Add(tournament);
            tournament.Id = tournaments.Count;
            return true;
        }

        public static List<PrizeModel>? GetPrizes(int? id)
        {
            if(id == null)
            {
                return null;
            }

            //TODO load from db
            return new List<PrizeModel>()
            {
                new PrizeModel()
                {
                    PlaceName = "Winner",
                    PrizeAmount = 60
                },
                new PrizeModel()
                {
                    PlaceName = "Round Up",
                    PrizeAmount = 40
                },
                new PrizeModel()
                {
                    PlaceName = "Semi final",
                    PrizeAmount = 20
                }
            };
        }

        public static List<RoundModel>? GetRounds(int? id)
        {
            if (id == null)
            {
                return null;
            }

            //TODO load from db
            return new List<RoundModel>()
            {
                new RoundModel()
                {
                    Round = "Final",
                    Frames = 15
                },
                new RoundModel()
                {
                    Round = "Semi final",
                    Frames = 13
                },
                new RoundModel()
                {
                    Round = "Quarter final",
                    Frames = 13
                },
                 new RoundModel()
                {
                    Round = "Last 16",
                    Frames = 11
                }
            };
        }

        //TODO Remove this
        private static List<TournamentModel> tournaments = new List<TournamentModel>()
            {
                new TournamentModel()
                {
                    Id = 1,
                    Name = "Champion of Champions",
                    IsActive = false,
                    EntryFee = 20,
                    Garantee = 200000,
                    Rounds = new List<List<MatchUpModel>>(),
                    Players = new List<PersonModel>(),
                    PrizeMode = PrizesModeEnum.Percentage,
                    StartDate = DateTime.Now
                },
                new TournamentModel()
                {
                    Id = 2,
                    Name = "UK Championship",
                    IsActive = true,
                    EntryFee = 200,
                    Garantee = 250000,
                    Rounds = new List<List<MatchUpModel>>(),
                    Players = new List<PersonModel>()
                },
                new TournamentModel()
                {
                    Id = 3,
                    Name = "Welsh Open",
                    IsActive = true,
                    Rounds = new List<List<MatchUpModel>>(),
                    Players = new List<PersonModel>()
                }
            };

        public static List<TournamentModel> GetAllTournaments(bool activeOnly = true)
        {
            // TODO load from db

            return tournaments;

            //return new List<TournamentModel>()
            //{
            //    new TournamentModel()
            //    {
            //        Id = 1,
            //        Name = "Champion of Champions",
            //        IsActive = false,
            //        EntryFee = 20,
            //        Garantee = 200000,
            //        Rounds = new List<List<MatchUpModel>>(),
            //        Players = new List<PersonModel>()
            //    },
            //    new TournamentModel()
            //    {
            //        Id = 2,
            //        Name = "UK Championship",
            //        IsActive = true,
            //        EntryFee = 200,
            //        Garantee = 250000,
            //        Rounds = new List<List<MatchUpModel>>(),
            //        Players = new List<PersonModel>()
            //    },
            //    new TournamentModel()
            //    {
            //        Id = 3,
            //        Name = "Welsh Open",
            //        IsActive = true,
            //        Rounds = new List<List<MatchUpModel>>(),
            //        Players = new List<PersonModel>()
            //    }
            //};
        }

        public static bool UpdateProfile(PersonModel person)
        {
            // TODO add request to the server
            return true;
        }
    }
}
