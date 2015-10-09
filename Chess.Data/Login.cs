namespace Chess.Data
{
    public class Login
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string LoginType { get; set; }

        public string ExternalUserId { get; set; }

        public virtual User User { get; set; }
    }
}