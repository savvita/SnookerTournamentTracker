namespace TournamentLibrary
{
    public class TournamentModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal? EntryFee { get; set; }
        public decimal? Garantee { get; set; }
        public PrizesModeEnum PrizeMode { get; set; }
        public List<PrizeModel>? Prizes { get; set; }
        public List<RoundModel> RoundModel { get; set; } = new List<RoundModel>();
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<PersonModel> Players { get; set; } = new List<PersonModel>();
        public List<List<MatchUpModel>> Rounds { get; set; } = new List<List<MatchUpModel>>();
    }
}
