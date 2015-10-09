using System;
using Chess.Providers.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Chess
{
    public partial class Startup
    {
        static Startup ()
        {
            PublicClientId = "self";
        }

        public static string PublicClientId { get; }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Signin"),
                AuthenticationMode = AuthenticationMode.Active
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            //EnableFacebookAuth(app);
            EnableGoogleAuth(app);
            //EnableVk(app);
        }

        private void EnableGoogleAuth(IAppBuilder app)
        {
            var authOptions = new GoogleOAuth2AuthenticationOptions();
            authOptions.ClientId = "996683506861-7bc0h31boqbqjnrr1j9gj2d60uovbevu.apps.googleusercontent.com";
            authOptions.ClientSecret = "KG2GOvHFVDKegb9Lz-MYlITb";
            authOptions.Scope.Add("email");
            authOptions.Provider = new GoogleOAuthProvider();

            app.UseGoogleAuthentication(authOptions);
        }
    }
}