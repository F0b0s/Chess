namespace Chess.Models.Account
{
    public class ExternalLoginProviderModel
    {
        public string Provider { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }
}