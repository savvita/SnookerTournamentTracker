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

        public static string? LastError = String.Empty;

        private static Message? GetResponse(Message request)
        {
            TcpClient client = new TcpClient();
            client.Connect(Connection.Host, Connection.Port);
            Connection.SendMessage(client.GetStream(), request);

            Message? response = Connection.ReceiveMessage(client.GetStream());
            client.Close();

            return response;
        }

        public static async Task<bool> SignIn(PersonModel user)
        {
            Message? response = GetResponse(new Message() { Code = ConnectionCode.SignIn, Content = JsonSerializer.Serialize<PersonModel>(user) });

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
            Message? response = GetResponse(new Message() { Code = ConnectionCode.SignUp, Content = JsonSerializer.Serialize<PersonModel>(user) });

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

        public static bool UpdateProfile(PersonModel person)
        {
            Message? response = GetResponse(new Message() { Code = ConnectionCode.UpdateProfile, Content = JsonSerializer.Serialize<PersonModel>(person) });

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

        public static List<TournamentModel>? GetAllTournaments(bool activeOnly = true)
        {
            //Message? response = GetResponse(new Message() { Code = ConnectionCode.AllTournaments });

            //if (response == null)
            //{
            //    LastError = "Server does not response";
            //    return null;
            //}

            //if (response.Code == ConnectionCode.Error || response.Content == null)
            //{
            //    LastError = response.Content ?? "Server does not response";
            //    return null;
            //}

            //List<TournamentModel>? tournaments = JsonSerializer.Deserialize<List<TournamentModel>>(response.Content);

            //if (tournaments == null)
            //{
            //    LastError = "Server answer is damages";
            //    return null;
            //}

            //LastError = String.Empty;

            //return tournaments;

            return QueryRequest<TournamentModel>(ConnectionCode.AllTournaments, null);
        }

        public static List<string>? GetAllRounds()
        {
            //Message? response = GetResponse(new Message() { Code = ConnectionCode.AllRounds });

            //if (response == null)
            //{
            //    LastError = "Server does not response";
            //    return null;
            //}

            //if (response.Code == ConnectionCode.Error || response.Content == null)
            //{
            //    LastError = response.Content ?? "Server does not response";
            //    return null;
            //}

            //List<string>? rounds = JsonSerializer.Deserialize<List<string>>(response.Content);

            //if (rounds == null)
            //{
            //    LastError = "Server answer is damages";
            //    return null;
            //}

            //LastError = String.Empty;

            //return rounds;

            return QueryRequest<string>(ConnectionCode.AllRounds, null);
        }

        public static List<PersonModel>? GetAllPlayers(int senderId)
        {
             return QueryRequest<PersonModel>(ConnectionCode.AllPlayers, senderId);
        } 

        public static List<PrizeModel>? GetAllPlaces()
        {
            //Message? response = GetResponse(new Message() { Code = ConnectionCode.AllPlaces });

            //if (response == null)
            //{
            //    LastError = "Server does not response";
            //    return null;
            //}

            //if (response.Code == ConnectionCode.Error || response.Content == null)
            //{
            //    LastError = response.Content ?? "Server does not response";
            //    return null;
            //}

            //List<PrizeModel>? places = JsonSerializer.Deserialize<List<PrizeModel>>(response.Content);

            //if (places == null)
            //{
            //    LastError = "Server answer is damages";
            //    return null;
            //}

            //LastError = String.Empty;

            //return places;

            return QueryRequest<PrizeModel>(ConnectionCode.AllPlaces, null);
        }

        public static List<PrizeModel>? GetPrizesByTournamentId(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() 
            { 
                Code = ConnectionCode.PrizesByTournamentId, 
                Content = id.ToString() 
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

            List<PrizeModel>? prizes = JsonSerializer.Deserialize<List<PrizeModel>>(response.Content);

            if (prizes == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return prizes;
        }

        public static List<RoundModel>? GetRoundsByTournamentId(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() { Code = ConnectionCode.RoundsByTournamentId, Content = id.ToString() });

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

        public static List<PersonModel>? GetPlayersByTournamentId(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() { Code = ConnectionCode.PlayersByTournamentId, Content = id.ToString() });

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

        public static List<TournamentModel>? GetTournamentsByAdministrator(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() { Code = ConnectionCode.TournamentsByAdministratorId, Content = id.ToString() });

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

        public static List<TournamentModel>? GetTournamentsByPlayerId(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() { Code = ConnectionCode.TournamentsByPlayerId, Content = id.ToString() });

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

        public static List<List<MatchUpModel>>? GetMatchesByTournamentId(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Message? response = GetResponse(new Message() { Code = ConnectionCode.MatchesByTournamentId, Content = id.ToString() });

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

            var matches = JsonSerializer.Deserialize<List<List<MatchUpModel>>>(response.Content);

            if (matches == null)
            {
                LastError = "Server answer is damages";
                return null;
            }

            LastError = String.Empty;

            return matches;
        }

        public static bool IsTournamentAdministrator(int? userId, int? tournamentId)
        {
            if (userId == null || tournamentId == null)
            {
                return false;
            }

            Message? response = GetResponse(new Message() { Sender = userId, Code = ConnectionCode.IsTournamentAdministrator, Content = tournamentId.ToString() });

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

            bool? isAdmin = JsonSerializer.Deserialize<bool>(response.Content);

            LastError = String.Empty;

            return isAdmin ?? false;
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

            Message? response = GetResponse(new Message()
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

        public static bool RegisterAtTournament(PersonModel player, TournamentModel tournament)
        {
            Message? response = GetResponse(new Message() { Code = ConnectionCode.RegisterAtTournament, Sender = player.Id, Content = tournament.Id.ToString() });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error)
            {
                LastError = response.Content ?? "Cannot register at the tournament";
                return false;
            }

            LastError = String.Empty;

            return true;
        }

        public static bool UnregisterFromTournament(PersonModel player, TournamentModel tournament)
        {
            Message? response = GetResponse(new Message() { Code = ConnectionCode.UnregisterFromTournament, Sender = player.Id, Content = tournament.Id.ToString() });

            if (response == null)
            {
                LastError = "Server does not response";
                return false;
            }

            if (response.Code == ConnectionCode.Error)
            {
                LastError = response.Content ?? "Cannot unregister from the tournament";
                return false;
            }

            LastError = String.Empty;

            return true;
        }

        public static bool AddTournamentRounds(int senderId, TournamentModel tournament)
        {
            //Message? response = GetResponse(new Message()
            //{
            //    Sender = senderId,
            //    Code = ConnectionCode.AddTournamentRounds,
            //    Content = JsonSerializer.Serialize<TournamentModel>(tournament)
            //});

            //if (response == null)
            //{
            //    LastError = "Server does not response";
            //    return false;
            //}

            //if (response.Code == ConnectionCode.Error || response.Content == null)
            //{
            //    LastError = response.Content ?? "Server does not response";
            //    return false;
            //}

            //LastError = String.Empty;

            //return true;

            return HandleRequest<TournamentModel>(ConnectionCode.AddTournamentRounds, senderId, tournament);
        }

        public static bool SaveTournamentDraw(int senderId, TournamentModel tournament)
        {
            //Message? response = GetResponse(new Message()
            //{
            //    Sender = senderId,
            //    Code = ConnectionCode.SaveTournamentDraw,
            //    Content = JsonSerializer.Serialize<TournamentModel>(tournament)
            //});

            //if (response == null)
            //{
            //    LastError = "Server does not response";
            //    return false;
            //}

            //if (response.Code == ConnectionCode.Error || response.Content == null)
            //{
            //    LastError = response.Content ?? "Server does not response";
            //    return false;
            //}

            //LastError = String.Empty;

            //return true;

            return HandleRequest<TournamentModel>(ConnectionCode.SaveTournamentDraw, senderId, tournament);
        }

        public static bool SaveFrameResult(int senderId, FrameModel frame)
        {
            return HandleRequest<FrameModel>(ConnectionCode.SaveFrameResult, senderId, frame);
        }

        private static List<T>? QueryRequest<T> (ConnectionCode code, int? senderId)
        {
            Message? response = GetResponse(new Message() 
            { 
                Sender = senderId,
                Code = code 
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

        private static bool HandleRequest<T> (ConnectionCode code, int senderId, T obj)
        {
            Message? response = GetResponse(new Message()
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

            if (response.Code == ConnectionCode.Error || response.Content == null)
            {
                LastError = response.Content ?? "Server does not response";
                return false;
            }

            LastError = String.Empty;

            return true;
        }
    }
}
