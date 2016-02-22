using System;
using Chess.Providers.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

namespace Chess
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
                                        {
                                            ExpireTimeSpan = TimeSpan.FromMinutes(30),
                                            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                                            LoginPath = new PathString("/Account/Signin"),
                                            AuthenticationMode = AuthenticationMode.Active
                                        });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            EnableGoogleAuth(app);
        }

        private static void EnableGoogleAuth(IAppBuilder app)
        {
            var authOptions = new GoogleOAuth2AuthenticationOptions
                              {
                                  ClientId = "1057175655112-0n7cnhcnak2rfpdm9j8bd4aethe8bpaf.apps.googleusercontent.com",
                                  ClientSecret = "YKb5Bk-2R0ga0V1E6v7UX6DY"
                              };
            authOptions.Scope.Add("email");
            authOptions.Provider = new GoogleOAuthProvider();

            app.UseGoogleAuthentication(authOptions);
        }
    }
}