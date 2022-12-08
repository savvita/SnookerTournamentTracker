using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class PaymentInfoModel
    {
        public int Id { get; set; }

        public CardModel Card { get; set; } = null!;

        public decimal Sum { get; set; }
    }
}
