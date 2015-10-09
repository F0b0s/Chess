using System.Security.Claims;
using System.Threading.Tasks;
using Chess.Providers.OAuth.UriParsers;
using Microsoft.Owin.Security.Google;

namespace Chess.Providers.OAuth
{
    public class GoogleOAuthProvider : GoogleOAuth2AuthenticationProvider
    {
        public override Task Authenticated(GoogleOAuth2AuthenticatedContext context)
        {
            string avatarUrl = context.User
                .SelectToken("image.url")
                .ToString()
                .Replace("sz=50", "sz=96");

            context.Identity.AddClaim(
                new Claim(OAuthClaimsParser.ClaimTypeAvatarUrl, avatarUrl));

            return base.Authenticated(context);
        }
    }
}