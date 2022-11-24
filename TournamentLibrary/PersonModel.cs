using System.Security;

namespace TournamentLibrary
{
    public class PersonModel
    {
        public int? Id { get; set; }
        public SecureString? Password { get; set; }
        public string? OpenPassword { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
