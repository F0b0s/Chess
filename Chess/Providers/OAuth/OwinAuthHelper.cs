using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Chess.Providers.OAuth
{
    public class OwinAuthHelper
    {
        public static AuthenticationTicket SignIn(IOwinContext owinContext, User user)
        {
            var properties = GetProperties(user);
            var claimsIdentity = ClaimsMapper.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            var cookieIdentity = ClaimsMapper.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            LogoutInternal(owinContext);
            owinContext.Authentication.SignIn(properties, /*claimsIdentity,*/ cookieIdentity);
            return new AuthenticationTicket(claimsIdentity, properties);
        }

        public static AuthenticationProperties GetProperties(User user)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                Dictionary =
                {
                    {"name", user.LastName != null ? $"{user.LastName} {user.FirstName}" : string.Empty},
                    {"ava", user.PhotoRec}
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