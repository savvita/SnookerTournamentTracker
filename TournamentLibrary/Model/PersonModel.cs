using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace TournamentLibrary
{
    public class PersonModel
    {
        public int? Id { get; set; }

        public SecureString? Password { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [StringLength(100, ErrorMessage = "Maximum length for a password is 100 chars")]
        public string? OpenPassword { get; set; }

        [Required(ErrorMessage = "A first name is required")]
        [StringLength(100, ErrorMessage = "Maximum length for a first name is 100 chars")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum length for a second name is 100 chars")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "A last name is required")]
        [StringLength(100, ErrorMessage = "Maximum length for a last name is 100 chars")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "An email is required")]
        [StringLength(255, ErrorMessage = "Maximum length for an email is 255 chars")]
        [EmailAddress(ErrorMessage = "The email is not a valid e-mail address")]
        public string? EmailAddress { get; set; }

        [StringLength(20, ErrorMessage = "Maximum length for a phone number is 20 chars")]
        public string? PhoneNumber { get; set; }
    }
}
