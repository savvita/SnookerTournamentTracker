using SnookerTournamentTracker.ConnectionLibrary;
using SnookerTournamentTrackerServer.DBAccess;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer
{
    internal class Server : IDisposable
    {
        private TcpListener? listener;
        private Dictionary<ConnectionCode, Func<Message, Message>> handlers;
        private IDBAccess db;

        public Server(IDBAccess db)
        {
            this.db = db;
            listener = new TcpListener(IPAddress.Any, Connection.Port);

            handlers = new Dictionary<ConnectionCode, Func<Message, Message>>();
            InitializeHandlers();
            Task.Factory.StartNew(Listen);
        }

        private void InitializeHandlers()
        {
            handlers.Add(ConnectionCode.SignIn, db.SignIn);
            handlers.Add(ConnectionCode.SignUp, db.SignUp);
            handlers.Add(ConnectionCode.AllTournaments, db.GetAllTournaments);
            handlers.Add(ConnectionCode.AllRounds, db.GetAllRounds);
            handlers.Add(ConnectionCode.AllPlaces, db.GetAllPlaces);
            handlers.Add(ConnectionCode.AllPlayers, db.GetAllPlayers);
            handlers.Add(ConnectionCode.PrizesByTournamentId, db.GetPrizesByTournament);
            handlers.Add(ConnectionCode.RoundsByTournamentId, db.GetRoundsByTournament);
            handlers.Add(ConnectionCode.PlayersByTournamentId, db.GetPlayersByTournament);
            handlers.Add(ConnectionCode.MatchesByTournamentId, db.GetMatchesByTournament);
            handlers.Add(ConnectionCode.TournamentsByPlayerId, db.GetTournamentsByPlayer);
            handlers.Add(ConnectionCode.TournamentsByAdministratorId, db.GetTournamentsByAdministrator);
            handlers.Add(ConnectionCode.UpdateProfile, db.UpdateProfile);
            handlers.Add(ConnectionCode.CreateTournament, db.CreateTournament);
            handlers.Add(ConnectionCode.RegisterAtTournament, db.RegisterAtTournament);
            handlers.Add(ConnectionCode.UnregisterFromTournament, db.UnregisterFromTournament);
            handlers.Add(ConnectionCode.IsTournamentAdministrator, db.IsTournamentAdministrator);
            handlers.Add(ConnectionCode.AddTournamentRounds, db.AddTournamentRounds);
            handlers.Add(ConnectionCode.SaveTournamentDraw, db.SaveTournamentDraw);
            handlers.Add(ConnectionCode.SaveFrameResult, db.SaveFrameResult);
            handlers.Add(ConnectionCode.UserCards, db.GetUserCards);
            handlers.Add(ConnectionCode.SaveUserCard, db.SaveUserCard);
            handlers.Add(ConnectionCode.ConfirmPlayerRegistration, db.ConfirmPlayerRegistration);
            handlers.Add(ConnectionCode.CancelTournament, db.CancelTournament);
            handlers.Add(ConnectionCode.UpdateTournament, db.UpdateTournament);
        }

        private void Listen()
        {
            if (listener == null)
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

                    if (msg == null)
                    {
                        return;
                    }

                    if (handlers.ContainsKey(msg.Code))
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
                catch
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

        public void Dispose()
        {
            listener?.Stop();
        }
    }
}
