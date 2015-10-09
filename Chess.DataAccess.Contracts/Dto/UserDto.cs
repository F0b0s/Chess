using System.Collections.Generic;

namespace Chess.DataAccess.Contracts.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string PhotoRec { get; set; }
        public string Email { get; set; }
        public IEnumerable<LoginDto> Logins { get; set; }
    }
}