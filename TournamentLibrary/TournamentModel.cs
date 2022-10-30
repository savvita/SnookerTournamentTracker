namespace TournamentLibrary
{
    public class TournamentModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal? EntryFee { get; set; }
        public decimal? Garantee { get; set; }
        public List<PersonModel> Players { get; set; } = new List<PersonModel>();
        public List<List<MatchUpModel>> Rounds { get; set; } = new List<List<MatchUpModel>>();
    }
}
