using GalaSoft.MvvmLight.CommandWpf;
using SnookerTournamentTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class CreateRoundsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private string? error = String.Empty;

        public string? Error
        {
            get => error;
            set
            {
                error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public ObservableCollection<RoundViewModel> Rounds { get; set; } = new ObservableCollection<RoundViewModel>();
        private List<string>? roundNames;

        private List<RoundModel> rounds;
        
        public CreateRoundsViewModel(List<RoundModel>? rounds)
        {
            LoadData(rounds);
            //roundNames = ServerConnection.GetAllRoundNames();

            //if (roundNames == null)
            //{
            //    Error = ServerConnection.LastError;
            //}

            //else
            //{
            //    if(rounds != null)
            //    {
            //        this.rounds = rounds;

            //        foreach(RoundModel round in rounds)
            //        {
            //            Rounds.Add(new RoundViewModel()
            //            {
            //                RoundName = round.Round,
            //                FrameCount = (int)round.Frames!,
            //                IsSaved = true
            //            });
            //        }
            //    }
            //    else
            //    {
            //        this.rounds = new List<RoundModel>();
            //    }

            //    if(Rounds.Count < roundNames.Count)
            //    {
            //        Rounds.Add(new RoundViewModel()
            //        {
            //            RoundName = roundNames[Rounds.Count]
            //        });
            //    }
            //}
        }

        private async Task LoadData(List<RoundModel>? rounds)
        {
            roundNames = await ServerConnection.GetAllRoundNamesAsync();

            if (roundNames == null)
            {
                Error = ServerConnection.LastError;
            }

            else
            {
                if (rounds != null)
                {
                    this.rounds = rounds;

                    foreach (RoundModel round in rounds)
                    {
                        Rounds.Add(new RoundViewModel()
                        {
                            RoundName = round.Round,
                            FrameCount = (int)round.Frames!,
                            IsSaved = true
                        });
                    }
                }
                else
                {
                    this.rounds = new List<RoundModel>();
                }

                if (Rounds.Count < roundNames.Count)
                {
                    Rounds.Add(new RoundViewModel()
                    {
                        RoundName = roundNames[Rounds.Count]
                    });
                }
            }
        }

        private RelayCommand<object>? saveBtnCmd;
        public RelayCommand<object> SaveBtnCmd
        {
            get => saveBtnCmd ?? new RelayCommand<object>((obj) =>
            {
                if (obj is RoundViewModel res)
                {
                    SaveRound(res);
                }
            });
        }

        public bool Validate(RoundViewModel round)
        {
            Error = String.Empty;

            if (Rounds == null)
            {
                return false;
            }

            if(round.FrameCount == null || round.FrameCount <= 0)
            {
                Error = "Number of frames is required";
                return false;
            }

            if (round.FrameCount % 2 == 0)
            {
                Error = "Match must consist of odd number of frames";
                return false;
            }

            return true;
        }

        private void SaveRound(RoundViewModel round)
        {
            if (Validate(round))
            {
                rounds.Add(new RoundModel()
                {
                    Round = round.RoundName,
                    Frames = round.FrameCount
                });

                if (roundNames != null && Rounds.Count < roundNames.Count)
                {
                    Rounds.Add(new RoundViewModel()
                    {
                        RoundName = roundNames[Rounds.Count]
                    });
                }

                Error = string.Empty;
                round.IsSaved = true;
            }
        }


        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public List<RoundModel> GetRounds()
        {
            return rounds;
        }
    }
}
