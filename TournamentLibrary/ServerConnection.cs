using SnookerTournamentTracker.ConnectionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public static class ServerConnection
    {

        public static string LastError = String.Empty;

        private static Message? GetResponse(Message request)
        {
            TcpClient client = new TcpClient();
            client.Connect(Connection.Host, Connection.Port);
            Connection.SendMessage(client.GetStream(), request);

            Message? response = Connection.ReceiveMessage(client.GetStream());
            client.Close();

            return response;
        }

        private static async Task<Message?> GetResponseAsync(Message request)
        {
            return await Task.FromResult<Message?>(GetResponse(request));
        }

        public static async Task<bool> SignInAsync(PersonModel user)
        {
            Message? response = await GetResponseAsync(new Message() 
            { 
                Code = ConnectionCode.SignIn, 
                Content = JsonSerializer.Serialize<PersonModel>(user) 
            });

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

        public static async Task<bool> SignUpAsync(PersonModel user)
        {
            Message? response = await GetResponseAsync(new Message() 
            { 
                Code = ConnectionCode.SignUp, 
                Content = JsonSerializer.Serialize<PersonModel>(user) 
            });

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

        public static async Task<bool> UpdateProfileAsync(int senderId, PersonModel person)
        {
            return await HandleRequestAsync<PersonModel>(ConnectionCode.UpdateProfile, senderId, person);
        }

        public static async Task<List<CardModel>?> GetUserCardsAsync(int? userId)
        {
            if (userId == null)
            {
                return null;
            }

            return await QueryRequestAsync<CardModel>(ConnectionCode.UserCards, null, JsonSerializer.Serialize<int?>(userId));
        }

        public static async Task<bool> SaveUserCard(int senderId, CardModel card)
        {
            Message? response = await GetResponseAsync(new Message()
            {
                Sender = senderId,
                Code = ConnectionCode.SaveUserCard,
                Content = JsonSerializer.Serialize<CardModel>(card)
            });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            if (!int.TryParse(response.Content, out int id))
            {
                LastError = "Server's answer is damaged";
                return false;
            }

            card.Id = id;

            LastError = String.Empty;

            return true;
        }

        public static async Task<List<TournamentModel>?> GetAllTournamentsAsync(bool activeOnly = true)
        {
            return await QueryRequestAsync<TournamentModel>(ConnectionCode.AllTournaments, null, null);
        }

        public static async Task<List<string>?> GetAllRoundNamesAsync()
        {
            return await QueryRequestAsync<string>(ConnectionCode.AllRounds, null, null);
        }

        public static async Task<List<PersonModel>?> GetAllPlayersAsync()
        {
             return await QueryRequestAsync<PersonModel>(ConnectionCode.AllPlayers, null, null);
        } 

        public static async Task<List<PrizeModel>?> GetAllPlacesAsync()
        {
            return await QueryRequestAsync<PrizeModel>(ConnectionCode.AllPlaces, null, null);
        }

        public static async Task<List<PrizeModel>?> GetPrizesByTournamentIdAsync(int? tournamentId)
        {
            if(tournamentId == null)
            {
                return null;
            }

            return await QueryRequestAsync<PrizeModel>(ConnectionCode.PrizesByTournamentId, null, JsonSerializer.Serialize<int?>(tournamentId)); 
        }

        public static async Task<List<RoundModel>?> GetRoundsByTournamentIdAsync(int? tournamentId)
        {
            if (tournamentId == null)
            {
                return null;
            }

            return await QueryRequestAsync<RoundModel>(ConnectionCode.RoundsByTournamentId, null, JsonSerializer.Serialize<int?>(tournamentId));
        }

        public static async Task<List<TournamentPlayer>?> GetPlayersByTournamentIdAsync(int? tournamentId)
        {
            if (tournamentId == null)
            {
                return null;
            }

            return await QueryRequestAsync<TournamentPlayer>(ConnectionCode.PlayersByTournamentId, null, JsonSerializer.Serialize<int?>(tournamentId));
        }

        public static async Task <List<TournamentModel>?> GetTournamentsByAdministratorIdAsync(int? userId)
        {
            if (userId == null)
            {
                return null;
            }

            return await QueryRequestAsync<TournamentModel>(ConnectionCode.TournamentsByAdministratorId, null, JsonSerializer.Serialize<int?>(userId));
        }

        public static async Task<List<TournamentModel>?> GetTournamentsByPlayerIdAsync(int? userId)
        {
            if (userId == null)
            {
                return null;
            }

            return await QueryRequestAsync<TournamentModel>(ConnectionCode.TournamentsByPlayerId, null, JsonSerializer.Serialize<int?>(userId));
        }

        public static async Task <List<List<MatchUpModel>>?> GetMatchesByTournamentIdAsync(int? tournamentId)
        {
            if (tournamentId == null)
            {
                return null;
            }

            return await QueryRequestAsync<List<MatchUpModel>>(ConnectionCode.MatchesByTournamentId, null, JsonSerializer.Serialize<int?>(tournamentId));
        }
 
        public static async Task<bool> IsTournamentAdministratorAsync(int? userId, int? tournamentId)
        {
            if (userId == null || tournamentId == null)
            {
                return false;
            }

            Message? response = await GetResponseAsync(new Message()
            {
                Sender = userId,
                Code = ConnectionCode.IsTournamentAdministrator,
                Content = JsonSerializer.Serialize<int?>(tournamentId)
            });

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

            var result = JsonSerializer.Deserialize<bool>(response.Content);

            LastError = String.Empty;

            return result;
        }

        public static async Task<bool> RegisterAtTournamentAsync(int? userId, int? tournamentId)
        {
            if (userId == null || tournamentId == null)
            {
                return false;
            }

            return await HandleRequestAsync<int?>(ConnectionCode.RegisterAtTournament, (int)userId!, tournamentId);
        }

        public static async Task<bool> UnregisterFromTournamentAsync(int? userId, int? tournamentId)
        {
            if (userId == null || tournamentId == null)
            {
                return false;
            }

            return await HandleRequestAsync<int?>(ConnectionCode.UnregisterFromTournament, (int)userId!, tournamentId);
        }

        public static async Task<bool> ConfirmPlayerRegistrationAsync(int senderId, TournamentPlayer player)
        {
            return await HandleRequestAsync<TournamentPlayer>(ConnectionCode.ConfirmPlayerRegistration, senderId, player);
        }
        public static async Task<bool> CancelTournamentAsync(int? userId, int? tournamentId)
        {
            if (userId == null || tournamentId == null)
            {
                return false;
            }

            return await HandleRequestAsync<int?>(ConnectionCode.CancelTournament, (int)userId!, tournamentId);
        }

        public static async Task<bool> UpdateTournamentAsync(int? userId, TournamentModel tournament)
        {
            if (userId == null)
            {
                return false;
            }

            return await HandleRequestAsync<TournamentModel>(ConnectionCode.UpdateTournament, (int)userId!, tournament);
        }

        public static async Task<bool> CreateTournamentAsync(int senderId, TournamentModel tournament)
        {
            //TODO Save invites

            Message? response = await GetResponseAsync(new Message()
            {
                Sender = senderId,
                Code = ConnectionCode.CreateTournament,
                Content = JsonSerializer.Serialize<TournamentModel>(tournament)
            });

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

        
        public static async Task<bool> AddTournamentRoundsAsync(int senderId, TournamentModel tournament)
        {
            return await HandleRequestAsync<TournamentModel>(ConnectionCode.AddTournamentRounds, senderId, tournament);
        }

        public static async Task<bool> SaveTournamentDrawAsync(int senderId, TournamentModel tournament)
        {
            return await HandleRequestAsync<TournamentModel>(ConnectionCode.SaveTournamentDraw, senderId, tournament);
        }

        public static async Task<bool> SaveFrameResultAsync(int senderId, FrameModel frame)
        {
            return await HandleRequestAsync<FrameModel>(ConnectionCode.SaveFrameResult, senderId, frame);
        }


        private static async Task<List<T>?> QueryRequestAsync<T>(ConnectionCode code, int? senderId, string? content)
        {
            Message? response = await GetResponseAsync(new Message()
            {
                Sender = senderId,
                Code = code,
                Content = content
            });

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

            List<T>? result = JsonSerializer.Deserialize<List<T>>(response.Content);

            if (result == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return result;
        }

        private static async Task<bool> HandleRequestAsync<T>(ConnectionCode code, int senderId, T obj)
        {
            Message? response = await GetResponseAsync(new Message()
            {
                Sender = senderId,
                Code = code,
                Content = JsonSerializer.Serialize<T>(obj)
            });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            LastError = String.Empty;

            return true;
        }
    }
}
