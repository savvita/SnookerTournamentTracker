using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTracker.Model
{
    internal class RoundViewModel
    {
        public string? RoundName { get; set; }
        public int? FrameCount { get; set; }
        public bool IsSaved { get; set; }
    }
}
