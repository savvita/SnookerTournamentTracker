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
        public static void CloseTournamentRegistration(int administratorId, TournamentModel tournament)
        {
            // TODO
            // create other rounds
            // save to bd
            // send emails
            var players = RandomizePlayers(tournament.Players);
            int rounds = GetNumberOfRounds(players.Count);

            if (rounds > tournament.RoundModel.Count)
            {
                AddTournamentRounds(administratorId, tournament, rounds);
            }

            int byes = GetNumberOfByes(rounds, players.Count);

            tournament.Rounds.Add(CreateFirstRound(players, byes, tournament.RoundModel.Last()));
            CreateOtherRounds(tournament, rounds);

            ServerConnection.SaveTournamentDraw(administratorId, tournament);
        }

        private static void AddTournamentRounds(int administratorId, TournamentModel tournament, int roundCount)
        {
            RoundModel lastRound = tournament.RoundModel[tournament.RoundModel.Count - 1];
            List<string>? rounds = ServerConnection.GetAllRounds();

            if (rounds == null)
            {
                return;
            }

            while (tournament.RoundModel.Count < roundCount && tournament.RoundModel.Count < rounds.Count)
            {
                tournament.RoundModel.Add(new RoundModel()
                {
                    Frames = lastRound.Frames,
                    Round = rounds[tournament.RoundModel.Count]
                });
            }

            ServerConnection.AddTournamentRounds(administratorId, tournament);
        }


        private static List<PersonModel> RandomizePlayers(List<PersonModel> players)
        {
            return players.OrderBy(player => Guid.NewGuid()).ToList();
        }

        private static int GetNumberOfRounds(int playerCount)
        {
            int rounds = 1;

            int curr = 2;

            while (curr < playerCount)
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

        private static List<MatchUpModel> CreateFirstRound(List<PersonModel> players, int byes, RoundModel round)
        {
            List<MatchUpModel> matchups = new List<MatchUpModel>();

            MatchUpModel curr = new MatchUpModel();
            int currNumber = 1;

            foreach (PersonModel player in players)
            {
                curr.Entries.Add(new MatchUpEntryModel() { Player = player });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchNumber = currNumber;
                    curr.MatchUpRound = round;
                    matchups.Add(curr);
                    curr = new MatchUpModel();

                    if (byes > 0)
                    {
                        curr.Winner = player;
                        byes--;
                    }

                    currNumber++;
                }
            }

            return matchups;
        }

        private static void CreateOtherRounds(TournamentModel tournament, int rounds)
        {
            int round = 2;
            var prevRound = tournament.Rounds[0];
            var currRound = new List<MatchUpModel>();

            MatchUpModel currMatch = new MatchUpModel();
            int currNumber = tournament.Rounds[0].Count + 1;

            while (round <= rounds)
            {
                foreach (MatchUpModel match in prevRound)
                {
                    currMatch.Entries.Add(new MatchUpEntryModel() 
                    { 
                        ParentMatchUp = match,
                        Player = match.Winner
                    });

                    if (currMatch.Entries.Count > 1)
                    {
                        currMatch.MatchNumber = currNumber;
                        currMatch.MatchUpRound = tournament.RoundModel[tournament.RoundModel.Count - round];
                        currRound.Add(currMatch);
                        currMatch = new MatchUpModel();

                        currNumber++;
                    }
                }

                tournament.Rounds.Add(currRound);
                prevRound = currRound;
                currRound = new List<MatchUpModel>();

                round++;
            }
        }
    }
}
