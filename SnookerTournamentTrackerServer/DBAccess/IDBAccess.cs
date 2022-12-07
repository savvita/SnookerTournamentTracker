using SnookerTournamentTracker.ConnectionLibrary;

namespace SnookerTournamentTrackerServer.DBAccess
{
    internal interface IDBAccess
    {
        Message SignIn(Message request);

        Message SignUp(Message request);

        Message CreateTournament(Message request);

        Message GetAllTournaments(Message request);

        Message GetAllRounds(Message request);

        Message GetAllPlaces(Message request);

        Message GetAllPlayers(Message request);

        Message GetPrizesByTournament(Message request);

        Message GetUserCards(Message request);

        Message GetRoundsByTournament(Message request);

        Message GetMatchesByTournament(Message request);

        Message GetPlayersByTournament(Message request);

        Message GetTournamentsByAdministrator(Message request);

        Message GetTournamentsByPlayer(Message request);

        Message IsTournamentAdministrator(Message request);

        Message RegisterAtTournament(Message request);

        Message ConfirmPlayerRegistration(Message request);

        Message UnregisterFromTournament(Message request);

        Message AddTournamentRounds(Message request);

        Message SaveTournamentDraw(Message request);

        Message SaveFrameResult(Message request);

        Message SaveUserCard(Message request);

        Message UpdateProfile(Message request);

    }
}
