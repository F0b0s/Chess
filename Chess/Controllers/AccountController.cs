using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chess.Domain;
using Chess.Infrastructure;
using Chess.Models.Account;
using Chess.Providers;
using Chess.Providers.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chess.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public AccountController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        // GET: Account
        public ActionResult Signin()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ExternalLinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("GetExternalLogin", new
            {
                provider
            }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var externalLogin = ExternalLoginModel.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            if (externalLogin.Provider != provider)
            {
                Request.GetOwinContext().Authentication.SignOut(
                    DefaultAuthenticationTypes.ExternalBearer,
                    OAuthDefaults.AuthenticationType);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            var user = _usersRepository.GetExternalUserByLogin(externalLogin.Provider, externalLogin.ProviderKey);
            if (user != null)
            {
                OwinAuthHelper.SignIn(Request.GetOwinContext(), user);
            }
            else
            {
                user = await _usersRepository.CreateExternalUser(externalLogin.ProviderKey, externalLogin.FirstName,
                    externalLogin.LastName, externalLogin.AvatarUrl, externalLogin.Provider, externalLogin.Email);
                OwinAuthHelper.SignIn(Request.GetOwinContext(), user);
            }

            return Redirect("/");
        }

        public string GetExternalLogins(string returnUrl, bool generateState = false)
        {
            var descriptions =
                Request.GetOwinContext().Authentication.GetExternalAuthenticationTypes();
            IList<ExternalLoginProviderModel> providers = new List<ExternalLoginProviderModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (var description in descriptions)
            {
                var model = new ExternalLoginProviderModel
                {
                    Provider = description.Caption,
                    Url = Url.Action("ExternalLinkLogin", "Account", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = Url.Action("GetExternalLogin","Account"),
                        state
                    }),
                    State = state
                };

                providers.Add(model);
            }

            return JsonConvert.SerializeObject(providers);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            OwinAuthHelper.LogoutInternal(Request.GetOwinContext());

            return new RedirectResult("/");
        }
    }
}