using System.ComponentModel.DataAnnotations;
using System.Security;

namespace TournamentLibrary
{
    public class PersonModel
    {
        public int? Id { get; set; }

        [Required]
        public SecureString? Password { get; set; }

        [Required]
        [StringLength(100)]
        public string? OpenPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? SecondName { get; set; }

        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }
}
