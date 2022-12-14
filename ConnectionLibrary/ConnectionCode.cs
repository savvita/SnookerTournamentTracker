using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.ConnectionLibrary
{
    public enum ConnectionCode
    {
        Ok,
        Error,
        SignIn,
        SignUp,
        AllPlayers,
        AllTournaments,
        AllRounds,
        AllPlaces,
        CreateTournament,
        PrizesByTournamentId,
        RoundsByTournamentId,
        PlayersByTournamentId,
        MatchesByTournamentId,
        TournamentsByAdministratorId,
        TournamentsByPlayerId,
        UpdateProfile,
        RegisterAtTournament,
        UnregisterFromTournament,
        IsTournamentAdministrator,
        AddTournamentRounds,
        SaveTournamentDraw,
        SaveFrameResult,
        UserCards,
        SaveUserCard,
        ConfirmPlayerRegistration,
        CancelTournament,
        UpdateTournament
    }
}
