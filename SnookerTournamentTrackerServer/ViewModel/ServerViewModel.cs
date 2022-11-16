using SnookerTournamentTracker.ConnectionLibrary;
using SnookerTournamentTrackerServer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.ViewModel
{
    internal class ServerViewModel : INotifyPropertyChanged
    {
        private ServerModel model = new ServerModel(); 
        //private string? code;
        //public string? Code
        //{
        //    get => code;
        //    set
        //    {
        //        code = value;
        //        OnPropertyChanged(nameof(Code));
        //    }
        //}

        //private string? message;
        //public string? Message
        //{
        //    get => message;
        //    set
        //    {
        //        message = value;
        //        OnPropertyChanged(nameof(Message));
        //    }
        //}


        public ServerViewModel()
        {
            //Connection.f2();
            //Message? m = Connection.f();
            //if(m != null)
            //{
            //    Code = m.Code.ToString();
            //    Message = m.Content;
            //}
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
