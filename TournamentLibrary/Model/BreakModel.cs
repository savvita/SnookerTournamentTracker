using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class BreakModel
    {
        public int? Id { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [Required]
        [Range(50, 156)]
        public short Score { get; set; }
    }
}
