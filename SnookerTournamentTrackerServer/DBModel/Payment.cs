using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.DbModel
{
    public partial class Payment
    {
        public int Id { get; set; }

        public int TournamentId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount cannot be less than zero")]
        public decimal Amount { get; set; }

        public virtual Tournament Tournament { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
