namespace TournamentLibrary
{
    public class FrameModel
    {
        public int Id { get; set; }
        public List<FrameEntryModel> Entries { get; set; } = new List<FrameEntryModel>();
        public int? WinnerId { get; set; }

        public int MatchId { get; set; }
    }
}
