using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.DbModel
{
    public partial class PaymentInfo
    {
        public int Id { get; set; }

        public int CardId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Sum cannot be less than zero")]
        public decimal Sum { get; set; }

        public virtual Card Card { get; set; } = null!;

        public virtual ICollection<Tournament> Tournaments { get; } = new List<Tournament>();
    }
}
