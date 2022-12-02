using SnookerTournamentTracker.ConnectionLibrary;
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
        private static ClientModel client = new ClientModel();

        public static string? LastError = String.Empty;
        public static async Task<bool> SignIn(PersonModel user)
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.SignIn, Content = JsonSerializer.Serialize<PersonModel>(user) });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            PersonModel? receivedUser = JsonSerializer.Deserialize<PersonModel>(response.Content);

            if (receivedUser == null)
            {
                LastError = "Server answer is damages";
                return false;
            }

            LastError = String.Empty;

            user.Id = receivedUser.Id;
            user.FirstName = receivedUser.FirstName;
            user.SecondName = receivedUser.SecondName;
            user.LastName = receivedUser.LastName;
            user.EmailAddress = receivedUser.EmailAddress;
            user.PhoneNumber = receivedUser.PhoneNumber;

            return true;
        }

        public static async Task<bool> SignUp(PersonModel user)
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.SignUp, Content = JsonSerializer.Serialize<PersonModel>(user) });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            PersonModel? receivedUser = JsonSerializer.Deserialize<PersonModel>(response.Content);

            if (receivedUser == null)
            {
                LastError = "Server answer is damages";
                return false;
            }

            LastError = String.Empty;

            user.Id = receivedUser.Id;

            return true;
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

        public static List<PrizeModel>? GetAllPlaces()
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.AllPlaces });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<PrizeModel>? places = JsonSerializer.Deserialize<List<PrizeModel>>(response.Content);

            if (places == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return places;
        }

        public static List<string>? GetAllRounds()
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.AllRounds });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<string>? rounds = JsonSerializer.Deserialize<List<string>>(response.Content);

            if (rounds == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return rounds;
        }

        public static List<PersonModel>? GetAllPlayers()
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.AllPlayers });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<PersonModel>? players = JsonSerializer.Deserialize<List<PersonModel>>(response.Content);

            if (players == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return players;
        }

        public static bool CreateTournament(int senderId, TournamentModel tournament)
        {
            //TODO add creation and creation rounds
            //Save prizes
            //Save rounds
            //Save invites
            //Save tournament

            //TODO Remove this
            //tournaments.Add(tournament);
            //tournament.Id = tournaments.Count;
            //

            Message? response = client.GetResponse(new Message() { Sender = senderId, Code = ConnectionCode.CreateTournament, Content = JsonSerializer.Serialize<TournamentModel>(tournament) });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            TournamentModel? receivedTournament = JsonSerializer.Deserialize<TournamentModel>(response.Content);

            if (receivedTournament == null)
            {
                LastError = "Server answer is damages";
                return false;
            }

            LastError = String.Empty;

            tournament.Id = receivedTournament.Id;

            return true;
        }

        // Finish this
        private static List<PersonModel> RandomizePlayers(List<PersonModel> players)
        {
            return players.OrderBy(player => Guid.NewGuid()).ToList();
        }

        private static int GetNumberOfRounds(int playerCount)
        {
            int rounds = 0;

            int curr = 2;

            while(curr < playerCount)
            {
                rounds++;
                curr *= 2;
            }

            return rounds;
        }

        private static int GetNumberOfByes(int rounds, int playerCount)
        {
            int totalPlayers = 1;

            for (int i = 0; i < rounds; i++)
            {
                totalPlayers *= 2;
            }

            return totalPlayers - playerCount;
        }

        private static List<MatchUpModel> CreateFirstRound(List<PersonModel> players, int byes)
        {
            List<MatchUpModel> matchups = new List<MatchUpModel>();

            MatchUpModel curr = new MatchUpModel();

            foreach(PersonModel player in players)
            {
                curr.Players.Add(player);

                if(byes > 0 || curr.Players.Count > 1)
                {
                    //curr.MatchUpRound = 1;
                    matchups.Add(curr);
                    curr = new MatchUpModel();

                    if(byes > 0)
                    {
                        byes--;
                    }
                }
            }

            return matchups;
        }

        //=======================

        public static List<PrizeModel>? GetPrizes(int? id)
        {
            if(id == null)
            {
                return null;
            }

            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.PrizesByTournamentId, Content = id.ToString() });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<PrizeModel>? prizes = JsonSerializer.Deserialize<List<PrizeModel>>(response.Content);

            if (prizes == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return prizes;
        }

        public static List<RoundModel>? GetRounds(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.RoundsByTournamentId, Content = id.ToString() });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<RoundModel>? rounds = JsonSerializer.Deserialize<List<RoundModel>>(response.Content);

            if (rounds == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return rounds;
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

        public static List<TournamentModel>? GetAllTournaments(bool activeOnly = true)
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.AllTournaments });

            if (response == null)
            {
                LastError = "Server does not response";
                return null;
            }

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return null;
            }

            List<TournamentModel>? tournaments = JsonSerializer.Deserialize<List<TournamentModel>>(response.Content);

            if (tournaments == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return tournaments;
        }

        public static bool UpdateProfile(PersonModel person)
        {
            Message? response = client.GetResponse(new Message() { Code = ConnectionCode.UpdateProfile, Content = JsonSerializer.Serialize<PersonModel>(person) });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error)
            {
                LastError = response.Content ?? "Cannot update the profile";
                return false;
            }

            LastError = String.Empty;

            return true;
        }
    }
}
