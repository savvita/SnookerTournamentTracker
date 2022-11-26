using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class PrizeModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string? PlaceName { get; set; }

        [Range(0, double.MaxValue)]
        public double? PrizeAmount { get; set; }
    }
}
