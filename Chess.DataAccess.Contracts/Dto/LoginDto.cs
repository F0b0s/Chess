using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.DataAccess.Contracts.Dto
{
    public class LoginDto
    {
        public string LoginType { get; set; }

        public string ExternalUserId { get; set; }
    }
}
