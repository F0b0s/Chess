using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Chess.Providers.OAuth.UriParsers
{
    public class OAuthClaimsParser
    {
        public static string ClaimTypeAvatarUrl = "avatarUrl";
        public static string ClaimTypeUid = "urn:chess:id";

        protected readonly ClaimsIdentity Identity;

        public OAuthClaimsParser(ClaimsIdentity identity)
        {
            Identity = identity;
        }

        protected string GetPartName(string fullName, int namePart)
        {
            if (!String.IsNullOrWhiteSpace(fullName) && fullName.Contains(" "))
            {
                var strings = fullName.Split(' ');
                return strings[namePart];
            }

            return String.Empty;
        }

        public virtual string GetName()
        {
            return Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public virtual string GetFirstName()
        {
            return Identity.FindFirst(ClaimTypes.GivenName)?.Value;
        }

        public virtual string GetLasttName()
        {
            return Identity.FindFirst(ClaimTypes.Surname)?.Value;
        }

        public virtual string GetEmail()
        {
            return Identity.FindFirst(ClaimTypes.Email)?.Value;
        }

        public virtual string GetAvatarUrl()
        {
            return Identity.FindFirst(OAuthClaimsParser.ClaimTypeAvatarUrl)?.Value;
        }

        public virtual string GetProviderKey()
        {
            return Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}