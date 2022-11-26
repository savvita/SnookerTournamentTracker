using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class RoundModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(30)]
        public string? Round { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? Frames { get; set; }
    }
}
