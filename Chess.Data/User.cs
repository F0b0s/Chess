using System.Collections.Generic;

namespace Chess.Data
{
    public class User
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PhotoRec { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Login> Logins { get; set; }
    }
}