using System;
using System.Configuration;
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
                                  ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                                  ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
            };
            authOptions.Scope.Add("email");
            authOptions.Provider = new GoogleOAuthProvider();

            app.UseGoogleAuthentication(authOptions);
        }
    }
}