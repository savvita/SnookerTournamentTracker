namespace TournamentLibrary
{
    public class MatchUpModel
    {
        public int Id { get; set; }
        public PersonModel? Winner { get; set; }
        public int? MatchUpRound { get; set; }
        public List<PersonModel> Players { get; set; } = new List<PersonModel>();
        public int? MaxFrames { get; set; }
        public List<FrameModel> Frames { get; set; } = new List<FrameModel>();
    }
}
