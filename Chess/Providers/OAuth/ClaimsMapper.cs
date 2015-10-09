using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Chess.Domain;
using Chess.Providers.OAuth.UriParsers;

namespace Chess.Providers.OAuth
{
    public class ClaimsMapper
    {
        public static ClaimsIdentity CreateIdentity(User user, string authenticationType)
        {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(OAuthClaimsParser.ClaimTypeUid, $"{user.Id}", null));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, $"{user.FullName}", null));
            claims.Add(new Claim(ClaimTypes.GivenName, $"{user.FirstName}", null));
            claims.Add(new Claim(ClaimTypes.Surname, $"{user.LastName}", null));
            claims.Add(new Claim(OAuthClaimsParser.ClaimTypeAvatarUrl, $"{user.PhotoRec}", null));

            return new ClaimsIdentity(claims, authenticationType);
        }

        public static User CreateUser(ClaimsIdentity identity)
        {
            return new User()
            {
                Id = Int64.Parse(identity.Claims.First(i => i.Type == OAuthClaimsParser.ClaimTypeUid).Value),
                LastName = identity.Claims.First(i => i.Type == ClaimTypes.Surname).Value,
                FirstName = identity.Claims.First(i => i.Type == ClaimTypes.GivenName).Value,
                PhotoRec = identity.Claims.First(i => i.Type == OAuthClaimsParser.ClaimTypeAvatarUrl).Value,
                Email = identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value,
            };
        }
    }
}