using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.ViewModel
{
    internal class CreateTournamentViewModel : INotifyPropertyChanged
    {

        private PersonModel? user;

        private TournamentModel tournament;
        public TournamentModel Tournament
        {
            get => tournament;
            set
            {
                tournament = value;
                OnPropertyChanged(nameof(Tournament));
            }
        }

        public ObservableCollection<CardModel> Cards { get; } = new ObservableCollection<CardModel>();

        private CardModel? selectedCard;

        public CardModel? SelectedCard
        {
            get => selectedCard;
            set
            {
                selectedCard = value;
                OnPropertyChanged(nameof(SelectedCard));
            }
        }

        private bool isCardReadOnly;
        public bool IsCardReadOnly
        {
            get => isCardReadOnly;
            set
            {
                isCardReadOnly = value;
                OnPropertyChanged(nameof(IsCardReadOnly));
            }
        }

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

        public ObservableCollection<PersonModel>? Players { get; set; }

        public ObservableCollection<PersonModel>? InvitedPlayers { get; set; }
        public PersonModel? SelectedPlayer { get; set; }

        public PersonModel? SelectedInvitedPlayer { get; set; }

        public List<PrizeModel>? Prizes { get; set; }
        public List<RoundModel>? Rounds { get; set; }

        public event Action? TournamentCreated;
        public event PropertyChangedEventHandler? PropertyChanged;

        public CreateTournamentViewModel(PersonModel user)
        {
            this.user = user;
            tournament = new TournamentModel();

            InvitedPlayers = new ObservableCollection<PersonModel>();

            IsCardReadOnly = true;
        }

        public async Task LoadData()
        {
            var players = await ServerConnection.GetAllPlayersAsync();

            if (players != null)
            {
                Players = new ObservableCollection<PersonModel>(players);
            }

            await LoadCardsAsync();
        }

        private async Task LoadCardsAsync()
        {
            if (user != null)
            {
                var cards = await ServerConnection.GetUserCardsAsync(user.Id);

                if (cards != null)
                {
                    foreach (var card in cards)
                    {
                        Cards.Add(card);
                    }

                    if (Cards.Count > 0)
                    {
                        SelectedCard = Cards[0];
                    }
                }
            }
        }


        #region Commands
        private RelayCommand? addToInvitedCommand;
        public RelayCommand AddToInvitedCommand
        {
            get => addToInvitedCommand ?? new RelayCommand(() =>
            {
                if (SelectedPlayer != null)
                {
                    InvitedPlayers?.Add(SelectedPlayer);
                    Players?.Remove(SelectedPlayer);
                }
            });
        }

        private RelayCommand? removeFromInvitedCommand;
        public RelayCommand RemoveFromInvitedCommand
        {
            get => removeFromInvitedCommand ?? new RelayCommand(() =>
            {
                if (SelectedInvitedPlayer != null)
                {
                    Players?.Add(SelectedInvitedPlayer);
                    InvitedPlayers?.Remove(SelectedInvitedPlayer);
                }
            });
        }

        private RelayCommand? addCardCmd;
        public RelayCommand AddCardCmd
        {
            get => addCardCmd ?? new RelayCommand(() =>
            {
                if (user == null)
                {
                    return;
                }

                CardModel card = new CardModel()
                {
                    UserId = (int)user.Id!
                };

                Cards.Add(card);
                SelectedCard = card;
                IsCardReadOnly = false;
            });
        }

        private RelayCommand? saveCardCmd;
        public RelayCommand SaveCardCmd
        {
            get => saveCardCmd ?? new RelayCommand(async () =>
            {
                if (user == null)
                {
                    Error = "User is not defined";
                    return;
                }

                if (SelectedCard == null)
                {
                    Error = "Card is not defined";
                    return;
                }

                if (await ServerConnection.SaveUserCard((int)user.Id!, SelectedCard))
                {
                    IsCardReadOnly = true;
                }

                Error = ServerConnection.LastError;
            });
        }
        #endregion


        public async Task<bool> CreateTournamentAsync()
        {
            try
            {
                if (Validate())
                {                   
                    if (Prizes != null)
                    {
                        Tournament.Prizes = Prizes.Where(prize => prize.PrizeAmount != null).ToList();
                    }

                    Tournament.RoundModel = Rounds!.Where(round => round.Frames != null).ToList();

                    if(Tournament.BuyIn != null)
                    {
                        Tournament.PaymentInfo = new PaymentInfoModel()
                        {
                            Card = SelectedCard!,
                            Sum = (decimal)Tournament.BuyIn
                        };
                    }
                    if (await ServerConnection.CreateTournamentAsync((int)user!.Id!, Tournament))
                    {
                        TournamentCreated?.Invoke();
                        return true;
                    }
                    else
                    {
                        Error = ServerConnection.LastError;
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Error = "Cannot connect to the server";
                return false;
            }
        }

        private bool Validate()
        {
            Error = String.Empty;

            if (user == null)
            {
                Error = "User is not defined";
                return false;
            }

            if (Tournament.Name == null || Tournament.Name.Length == 0)
            {
                Error = "Name is required";
                return false;
            }

            if (Tournament.StartDate != null && Tournament.EndDate != null && Tournament.StartDate > Tournament.EndDate)
            {
                Error = "Start date cannot be after the end date";
                return false;
            }

            if(Tournament.Garantee != null && Prizes == null)
            {
                Error = "Total prizes cannot be less than garantee";
                return false;
            }

            if (Tournament.PrizeMode == PrizesModeEnum.Percentage && Tournament.Garantee == null && Tournament.BuyIn == null)
            {
                Error = "Percentage prize mode requires a garantee or buy-ins";
                return false;
            }

            if (Rounds == null || Rounds.Where(round => round.Frames == null || round.Frames == 0).Count() == Rounds.Count())
            {
                Error = "Rounds are required";
                return false;
            }

            if (Tournament.BuyIn != null && SelectedCard == null)
            {
                Error = "Card is required";
                return false;
            }

            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
