using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class MatchUpEntryModel
    {
        public int Id { get; set; }
        public PersonModel? Player { get; set; }
        public MatchUpModel? ParentMatchUp { get; set; }
    }
}
