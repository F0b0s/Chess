using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Chess.Providers.OAuth.UriParsers;
using Microsoft.AspNet.Identity;

namespace Chess.Providers.OAuth
{
    public class ExternalLoginModel
    {
        public string Email { get; set; }

        public string FullName
        {
            get { return FirstName + LastName; }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AvatarUrl { get; set; }

        public string Provider { get; set; }

        public string ProviderKey { get; set; }

        public bool IsRegistered { get; set; }

        public static ExternalLoginModel FromIdentity(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            var email = identity.FindFirst(ClaimTypes.Email);

            if (!IsOk(email))
                return null;

            var result = new ExternalLoginModel();
            result.IsRegistered = (email.Issuer == ClaimsIdentity.DefaultIssuer);
            result.Provider = email.OriginalIssuer;

            if (identity.AuthenticationType == DefaultAuthenticationTypes.ExternalCookie)
            {
                result.ProviderKey = identity.FindFirstValue(ClaimTypes.NameIdentifier);
                result.Email = identity.FindFirstValue(ClaimTypes.Email);
                result.FirstName = identity.FindFirstValue(ClaimTypes.GivenName);
                result.LastName = identity.FindFirstValue(ClaimTypes.Surname);
                result.AvatarUrl = identity.FindFirstValue(OAuthClaimsParser.ClaimTypeAvatarUrl);
            }
            else
            {
                
            }
            return result;
        }

        private static bool IsOk(Claim idClaim)
        {
            if (idClaim == null)
                return false;

            if (string.IsNullOrEmpty(idClaim.Issuer))
                return false;

            if (string.IsNullOrEmpty(idClaim.OriginalIssuer))
                return false;

            if (idClaim.OriginalIssuer == ClaimsIdentity.DefaultIssuer)
                return false;

            if (idClaim.Issuer != idClaim.OriginalIssuer && idClaim.Value == null)
                return false;

            return true;
        }
    }
}