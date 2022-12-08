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

        [Required(ErrorMessage = "A round name is required")]
        [StringLength(30, ErrorMessage = "Maximum length for a round name is 30 chars")]
        public string? Round { get; set; }

        [Required(ErrorMessage = "The number of frames is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of frames must be grater than or equal to 1")]
        public int? Frames { get; set; }
    }
}
