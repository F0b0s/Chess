using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Chess.Providers.OAuth.UriParsers;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Chess.Providers.OAuth
{
    public class ClaimsMapper
    {
        public static ClaimsIdentity CreateIdentity(IdentityUser user, string authenticationType)
        {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(OAuthClaimsParser.ClaimTypeUid, $"{user.Id}", null));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, $"{user.UserName}", null));

            return new ClaimsIdentity(claims, authenticationType);
        }

        public static IdentityUser CreateUser(ClaimsIdentity identity)
        {
            return new IdentityUser
            {
                Id = OAuthClaimsParser.ClaimTypeUid,
                UserName = identity.Claims.First(i => i.Type == ClaimTypes.Surname).Value,
                Email = identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value,
            };
        }
    }
}