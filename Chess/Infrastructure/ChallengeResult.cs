using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Chess.Infrastructure
{
    public class ChallengeResult : HttpUnauthorizedResult
    {
        private readonly IController _controller;
        private readonly string _redirectUrl;

        public ChallengeResult(string provider, string redirectUrl)
        {
            _redirectUrl = redirectUrl;
            LoginProvider = provider;
        }

        public string LoginProvider { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = _redirectUrl };
            // this line did the trick
            //context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            var owinContext = context.HttpContext.GetOwinContext();
            owinContext.Authentication.Challenge(properties, LoginProvider);
        }
    }
}