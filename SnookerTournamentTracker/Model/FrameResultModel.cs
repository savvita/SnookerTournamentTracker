using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary.Model
{
    public class FrameResultModel : INotifyPropertyChanged
    {
        public int FirstPlayerScore { get; set; }
        public int SecondPlayerScore { get; set; }

        private int firstPlayerMatchScore;
        public int FirstPlayerMatchScore
        {
            get => firstPlayerMatchScore;
            set
            {
                firstPlayerMatchScore = value;
                OnPropertyChanged(nameof(FirstPlayerMatchScore));
            }
        }
        private int secondPlayerMatchScore;
        public int SecondPlayerMatchScore
        {
            get => secondPlayerMatchScore;
            set
            {
                secondPlayerMatchScore = value;
                OnPropertyChanged(nameof(SecondPlayerMatchScore));
            }
        }

        public string? FirstPlayerBrakes { get; set; }
        public string? SecondPlayerBrakes { get; set; }

        private bool isUnsaved = true;
        public bool IsUnsaved
        {
            get => isUnsaved;
            set
            {
                isUnsaved = value;
                OnPropertyChanged(nameof(IsUnsaved));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
