using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerTournamentTrackerServer.DbModel
{
    public partial class Card
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Owner of the card is required")]
        [StringLength(255, ErrorMessage = "Max length of a name is 255 chars")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Card number is required")]
        [StringLength(16, ErrorMessage = "Max length of a card number is 16 chars")]
        public string CardNumber { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual ICollection<PaymentInfo> PaymentInfos { get; } = new List<PaymentInfo>();
    }
}
