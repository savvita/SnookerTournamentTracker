using System.ComponentModel.DataAnnotations;

namespace TournamentLibrary
{
    public class MatchUpModel
    {
        public int Id { get; set; }

        public int? MatchNumber { get; set; }

        public PersonModel? Winner { get; set; }

        [Required(ErrorMessage = "A matchup round is required")]
        public RoundModel? MatchUpRound { get; set; }

        public List<MatchUpEntryModel> Entries { get; set; } = new List<MatchUpEntryModel>();
        public List<FrameModel> Frames { get; set; } = new List<FrameModel>();
    }
}
