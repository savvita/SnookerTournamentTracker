using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.ConnectionLibrary
{
    public class Message
    {
        public int? Sender { get; set; }
        public byte Code { get; set; }
        public string? Content { get; set; }
    }
}
