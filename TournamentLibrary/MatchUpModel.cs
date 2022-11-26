using System.ComponentModel.DataAnnotations;

namespace TournamentLibrary
{
    public class MatchUpModel
    {
        public int Id { get; set; }

        public PersonModel? Winner { get; set; }

        [Required]
        public RoundModel? MatchUpRound { get; set; }

        public List<PersonModel> Players { get; set; } = new List<PersonModel>();
        public List<FrameModel> Frames { get; set; } = new List<FrameModel>();
    }
}
