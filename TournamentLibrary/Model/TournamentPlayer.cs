using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class TournamentPlayer
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public PersonModel? Player { get; set; }
        public bool IsFinished { get; set; } = false;
        public decimal? Payment { get; set; }

        public string Status { get; set; } = null!;
    }
}
