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

        [Required(ErrorMessage = "A place name is required")]
        [StringLength(20, ErrorMessage = "Maximum length for a place name is 100 chars")]
        public string? PlaceName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Prize amount must be grater than or equal to 0")]
        public double? PrizeAmount { get; set; }
    }
}
