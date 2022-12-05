using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.DbModel
{
    public partial class MatchUpEntry
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int MatchId { get; set; }

        public int? ParentMatchId { get; set; }

        public int? Score { get; set; }

        public virtual Match Match { get; set; } = null!;

        public virtual Match? ParentMatch { get; set; }

        public virtual User? User { get; set; }
    }
}
