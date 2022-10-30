namespace TournamentLibrary
{
    public class FrameModel
    {
        public int Id { get; set; }
        public List<FrameEntryModel> Entries { get; set; } = new List<FrameEntryModel>();
        public PersonModel? Winner { get; set; }
    }
}
