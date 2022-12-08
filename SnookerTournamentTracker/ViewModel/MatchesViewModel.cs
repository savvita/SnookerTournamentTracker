using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TournamentLibrary;
using TournamentLibrary.Model;

namespace SnookerTournamentTracker.ViewModel
{
    internal class MatchesViewModel : INotifyPropertyChanged
    {
        private TournamentModel tournament;
        private PersonModel user;
        public ObservableCollection<RoundModel> Rounds { get; set; } = new ObservableCollection<RoundModel>();
        public ObservableCollection<MatchUpModel> Matches { get; set; } = new ObservableCollection<MatchUpModel>();

        private RoundModel? selectedRound;
        public RoundModel? SelectedRound
        {
            get => selectedRound;
            set
            {
                selectedRound = value;
                RefreshMatches();
            }
        }

        private MatchUpModel? selectedMatch;
        public MatchUpModel? SelectedMatch
        {
            get => selectedMatch;
            set
            {
                selectedMatch = value;
                RefreshFrames();
            }
        }

        private string? players;

        public string? Players
        {
            get => players;
            set
            {
                players = value;
                OnPropertyChanged(nameof(Players));
            }
        }

        private string? score;
        public string? Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged(nameof(Score));
            }
        }

        private string? error;
        public string? Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        private int totalFrames;
        public int TotalFrames
        {
            get => totalFrames;
            set
            {
                totalFrames = value;
                OnPropertyChanged(nameof(TotalFrames));
            }
        }

        private bool isAdmin;

        private bool isEditable;
        public bool IsEditable
        {
            get => isEditable;
            set
            {
                isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public ObservableCollection<FrameResultModel> Results { get; set; } = new ObservableCollection<FrameResultModel>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public MatchesViewModel(PersonModel user, TournamentModel tournament)
        {
            this.user = user;
            this.tournament = tournament;
        }

        public async Task LoadData()
        {
            this.tournament.Rounds = await ServerConnection.GetMatchesByTournamentIdAsync(tournament.Id) ?? new List<List<MatchUpModel>>();

            for (int i = tournament.RoundModel.Count - 1; i >= 0; i--)
            {
                Rounds.Add(tournament.RoundModel[i]);
            }

            if (Rounds.Count > 0)
            {
                SelectedRound = Rounds[0];
            }

            isAdmin = await ServerConnection.IsTournamentAdministratorAsync(user.Id, tournament.Id);
        }

        private RelayCommand<object>? saveBtnCmd;
        public RelayCommand<object> SaveBtnCmd
        {
            get => saveBtnCmd ?? new RelayCommand<object>(async (obj) =>
            {
                if (obj is FrameResultModel res)
                {
                    await SaveFrameResultAsync(res);
                }
            });
        }

        private void RefreshMatches()
        {
            Matches.Clear();

            if (SelectedRound != null)
            {
                var matches = tournament.Rounds.Where(r =>
                    {
                        if (r[0].MatchUpRound == null || r[0].MatchUpRound!.Round == null)
                        {
                            return false;
                        }

                        return r[0].MatchUpRound!.Round!.Equals(SelectedRound.Round);
                    })
                    .FirstOrDefault();

                if(matches == null)
                {
                    return;
                }

                foreach (var match in matches)
                {
                    Matches.Add(match);
                }
            }

            if(Matches.Count > 0)
            {
                SelectedMatch = Matches[0];
            }
        }

        private void RefreshFrames()
        {
            Results.Clear();
            
            if(SelectedMatch != null && SelectedMatch.MatchUpRound != null && SelectedMatch.MatchUpRound.Frames != null)
            {
                TotalFrames = (int)SelectedMatch.MatchUpRound.Frames!;

                if(SelectedMatch.Entries.Count < 2 || SelectedMatch.Entries[0].Player == null || SelectedMatch.Entries[1].Player == null)
                {
                    IsEditable = false;
                }
                else
                {
                    IsEditable = isAdmin;
                }

                string pl1 = SelectedMatch.Entries[0].Player != null ? 
                    $"{SelectedMatch.Entries[0].Player!.FirstName} {SelectedMatch.Entries[0].Player!.LastName}" : "Player 1";

                if (SelectedMatch.Entries.Count > 1)
                {
                    string pl2 = SelectedMatch.Entries[1].Player != null ?
                        $"{SelectedMatch.Entries[1].Player!.FirstName} {SelectedMatch.Entries[1].Player!.LastName}" : "Player 2";

                    Players = $"{pl1} vs {pl2}";

                    foreach(var frame in SelectedMatch.Frames)
                    {
                        if (frame.Entries.Count > 0)
                        {
                            FrameResultModel res = new FrameResultModel()
                            {
                                FirstPlayerScore = frame.Entries[0].Score ?? 0,
                                SecondPlayerScore = frame.Entries[1].Score ?? 0,
                                IsUnsaved = false
                            };

                            var breaks = new List<string>();

                            frame.Breaks.Where(x => x.PlayerId == frame.Entries[0].PlayerId).ToList().ForEach(x => breaks.Add(x.Score.ToString()!));
                            res.FirstPlayerBrakes = String.Join(',', breaks);

                            breaks.Clear();

                            frame.Breaks.Where(x => x.PlayerId == frame.Entries[1].PlayerId).ToList().ForEach(x => breaks.Add(x.Score.ToString()!));
                            res.SecondPlayerBrakes = String.Join(',', breaks);


                            Results.Add(res);

                            res.FirstPlayerMatchScore = Results.Count(r => r.FirstPlayerScore > r.SecondPlayerScore);
                            res.SecondPlayerMatchScore = Results.Count(r => r.FirstPlayerScore < r.SecondPlayerScore);
                        }
                    }

                    Score = $"{SelectedMatch.Frames.Where(f => f.WinnerId == SelectedMatch.Entries[0].Player!.Id).Count()} : " +
                        $"{SelectedMatch.Frames.Where(f => f.WinnerId == SelectedMatch.Entries[1].Player!.Id).Count()}";

                    if (IsEditable && SelectedRound != null && SelectedMatch.Winner == null)
                    {
                        SelectedMatch.Frames.Add(new FrameModel());
                        Results.Add(new FrameResultModel());
                    }

                }
                else
                {
                    Players = $"{pl1} (w/o)";
                }
            }
        }  

        private async Task SaveFrameResultAsync(FrameResultModel frameResult)
        {
            if (ValidateFrameResult(frameResult))
            {
                if(SelectedMatch == null)
                {
                    return;
                }

                if(SelectedMatch.MatchUpRound == null)
                {
                    return;
                }

                FrameModel frame = SelectedMatch!.Frames.Last();

                SetFrameResult(frame, frameResult);

                SetBreaks(frame, frameResult);

                await ServerConnection.SaveFrameResultAsync((int)user.Id!, frame);

                frameResult.FirstPlayerMatchScore = Results.Count(r => r.FirstPlayerScore > r.SecondPlayerScore);
                frameResult.SecondPlayerMatchScore = Results.Count(r => r.FirstPlayerScore < r.SecondPlayerScore);

                int score1 = SelectedMatch.Frames.Where(f => f.WinnerId == SelectedMatch.Entries[0].Player!.Id).Count();
                int score2 = SelectedMatch.Frames.Where(f => f.WinnerId == SelectedMatch.Entries[1].Player!.Id).Count();

                Score = $"{score1} : {score2}";

                if(score1 > SelectedMatch.MatchUpRound.Frames / 2)
                {
                    SelectedMatch.Winner = SelectedMatch.Entries[0].Player;
                }
                else if (score2 > SelectedMatch.MatchUpRound.Frames / 2)
                {
                    SelectedMatch.Winner = SelectedMatch.Entries[1].Player;
                }
                Error = string.Empty;
                frameResult.IsUnsaved = false;

                if (SelectedRound != null && SelectedMatch.Winner == null)
                {
                    SelectedMatch.Frames.Add(new FrameModel());
                    Results.Add(new FrameResultModel());
                }
                else
                {
                    tournament.Rounds = await ServerConnection.GetMatchesByTournamentIdAsync(tournament.Id) ?? new List<List<MatchUpModel>>();
                }
            }
        }

        private bool ValidateFrameResult(FrameResultModel frameResult)
        {
            if (SelectedRound == null)
            {
                Error = "Round is undefined";
                return false;
            }

            if (SelectedMatch == null)
            {
                Error = "Match is undefined";
                return false;
            }

            if (SelectedMatch.Entries[0].Player == null || SelectedMatch.Entries[1].Player == null)
            {
                Error = "Players are undefined";
                return false;
            }

            if (frameResult.FirstPlayerScore == frameResult.SecondPlayerScore)
            {
                Error = "Scores cannot be equals";
                return false;
            }

            if(frameResult.FirstPlayerBrakes != null && !ValidateBreaks(frameResult.FirstPlayerScore, frameResult.FirstPlayerBrakes))
            {
                return false;
            }

            if (frameResult.SecondPlayerBrakes != null && !ValidateBreaks(frameResult.SecondPlayerScore, frameResult.SecondPlayerBrakes))
            {
                return false;
            }

            return true;
        }

        private void SetFrameResult(FrameModel frame, FrameResultModel frameResult)
        {
            frame.MatchId = SelectedMatch!.Id;

            frame.Entries.Add(new FrameEntryModel()
            {
                Score = frameResult.FirstPlayerScore,
                PlayerId = SelectedMatch!.Entries[0].Player!.Id
            });

            frame.Entries.Add(new FrameEntryModel()
            {
                Score = frameResult.SecondPlayerScore,
                PlayerId = SelectedMatch.Entries[1].Player!.Id
            });

            frame.WinnerId = frame.Entries[0].Score > frame.Entries[1].Score ? frame.Entries[0].PlayerId : frame.Entries[1].PlayerId;
        }

        private bool ValidateBreaks(int score, string breaks)
        {
            var breaksArray = breaks.Split(',');

            foreach(var br in breaksArray)
            {
                if(!int.TryParse(br.Trim(), out int b))
                {
                    Error = "Break must be a number";
                    return false;
                }

                if(b < 50)
                {
                    Error = "Break cannot be less than 50";
                    return false;
                }
                if (b > score)
                {
                    Error = "Break cannot be grater than score";
                    return false;
                }
            }

            return true;
        }

        private void SetBreaks(FrameModel frame, FrameResultModel frameResult)
        {     
            if(frameResult.FirstPlayerBrakes != null)
            {
                var breaksArray1 = frameResult.FirstPlayerBrakes.Split(',');

                foreach (var br in breaksArray1)
                {
                    frame.Breaks.Add(new BreakModel()
                    {
                        PlayerId = (int)frame.Entries[0].PlayerId!,
                        Score = short.Parse(br)
                    });
                }
            }
            
            if(frameResult.SecondPlayerBrakes != null)
            {
                var breaksArray2 = frameResult.SecondPlayerBrakes.Split(',');

                foreach (var br in breaksArray2)
                {
                    frame.Breaks.Add(new BreakModel()
                    {
                        PlayerId = (int)frame.Entries[1].PlayerId!,
                        Score = short.Parse(br)
                    });
                }
            }
        }



        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
