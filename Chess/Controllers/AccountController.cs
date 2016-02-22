using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chess.Infrastructure;
using Chess.Models.Account;
using Chess.Providers;
using Chess.Providers.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace Chess.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Signin()
        {
            return View();
        }
        
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult ExternalLinkLogin(string provider)
        {
            var returnUrl = Url.Action("Index", "Home", null);
            var redirectUrl = Url.Action("ExternalLoginCallback", new { returnUrl });
            return new ChallengeResult(provider, redirectUrl);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("Signin");

            IdentitySignin(loginInfo);

            var storedUser = await _userManager.FindByEmailAsync(loginInfo.Email);
            if (storedUser == null)
            {
                var user = new IdentityUser
                           {
                               Email = loginInfo.Email,
                               UserName = loginInfo.ExternalIdentity.Name
                           };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return RedirectToAction("Signin");
                }
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            OwinAuthHelper.LogoutInternal(Request.GetOwinContext());

            return new RedirectResult("/");
        }

        private void IdentitySignin(ExternalLoginInfo loginInfo)
        {
            var identity = new ClaimsIdentity(loginInfo.ExternalIdentity.Claims, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties
                                         {
                                             AllowRefresh = true,
                                             IsPersistent = false,
                                             ExpiresUtc = DateTime.UtcNow.AddDays(7)
                                         }, identity);
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                            DefaultAuthenticationTypes.ExternalCookie);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
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
                        client_id = "self",
                        //redirect_uri = Url.Action("GetExternalLogin", "Account"),
                        state
                    }),
                    State = state
                };

                providers.Add(model);
            }

            return JsonConvert.SerializeObject(providers);
        }
    }
}