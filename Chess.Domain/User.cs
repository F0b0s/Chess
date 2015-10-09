using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain
{
    public class User     {         public long Id { get; set; }          public string Email { get; set; }          public string FullName => FirstName + LastName;          public string FirstName { get; set; }         public string LastName { get; set; }         public string Login { get; set; }         public string PhotoRec { get; set; }          public IEnumerable<ExternalLoginInfo> ExternalLogins { get; set; }     }
}
