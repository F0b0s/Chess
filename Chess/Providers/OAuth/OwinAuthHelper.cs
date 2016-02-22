using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Chess.Providers.OAuth
{
    public class OwinAuthHelper
    {
        public static AuthenticationTicket SignIn(IOwinContext owinContext, IdentityUser user)
        {
            var properties = GetProperties(user);
            var claimsIdentity = ClaimsMapper.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            var cookieIdentity = ClaimsMapper.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            LogoutInternal(owinContext);
            owinContext.Authentication.SignIn(properties, /*claimsIdentity,*/ cookieIdentity);
            return new AuthenticationTicket(claimsIdentity, properties);
        }

        public static AuthenticationProperties GetProperties(IdentityUser user)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                Dictionary =
                {
                    {"name", user.UserName}
                },
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            return authenticationProperties;
        }

        public static void LogoutInternal(IOwinContext owinContext)
        {
            owinContext.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            owinContext.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}