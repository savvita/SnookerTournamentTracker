using System.ComponentModel.DataAnnotations;

namespace TournamentLibrary
{
    public class TournamentModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "A tournament name is required")]
        [StringLength(500, ErrorMessage = "Maximum length for a tournament name is 500 chars")]
        public string? Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Entree fee must be grater than or equal to zero")]
        public decimal? BuyIn { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Garantee must be grater than or equal to zero")]
        public decimal? Garantee { get; set; }

        public PrizesModeEnum PrizeMode { get; set; }

        public PaymentInfoModel? PaymentInfo { get; set; }

        public List<PrizeModel>? Prizes { get; set; }

        public List<RoundModel> RoundModel { get; set; } = new List<RoundModel>();

        //public bool IsActive { get; set; } = true;
        public string? Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<PersonModel> Players { get; set; } = new List<PersonModel>();

        public List<List<MatchUpModel>> Rounds { get; set; } = new List<List<MatchUpModel>>();
    }
}
