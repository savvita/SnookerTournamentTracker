


namespace SnookerTournamentTracker.ConnectionLibrary
{
    public class Message
    {
        public int? Sender { get; set; }
        public ConnectionCode Code { get; set; }
        public string? Content { get; set; }
    }
}
