using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.DataAccess.Contracts.Dto
{
    public class NewUserCommandDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginType { get; set; }
        public string ExternalLoginId { get; set; }
        public string PhotoRec { get; set; }
        public string Email { get; set; }
    }
}
