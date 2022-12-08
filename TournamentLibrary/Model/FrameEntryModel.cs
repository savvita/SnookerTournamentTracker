using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentLibrary
{
    public class FrameEntryModel
    {
        public int Id { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Score must be grater than or equal to 0")]
        public int? Score { get; set; }

        public int? PlayerId { get; set; } 
    }
}
