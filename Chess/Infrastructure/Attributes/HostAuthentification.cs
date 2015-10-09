using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc.Filters;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Chess.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HostAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private readonly IAuthenticationFilter _innerFilter;
        private readonly string _authenticationType;

        public bool AllowMultiple
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the authentication type of the OWIN middleware to use.
        /// </summary>
        public string AuthenticationType
        {
            get { return this._authenticationType; }
        }

        internal IAuthenticationFilter InnerFilter
        {
            get { return this._innerFilter; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.HostAuthenticationAttribute"/> class.
        /// </summary>
        /// <param name="authenticationType">The authentication type of the OWIN middleware to use.</param>
        public HostAuthenticationAttribute(string authenticationType)
            : this((IAuthenticationFilter) new HostAuthenticationFilter(authenticationType))
        {
            this._authenticationType = authenticationType;
        }

        internal HostAuthenticationAttribute(IAuthenticationFilter innerFilter)
        {
            if (innerFilter == null)
                throw new ArgumentNullException("innerFilter");
            this._innerFilter = innerFilter;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            this._innerFilter.OnAuthentication(filterContext);
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            this._innerFilter.OnAuthenticationChallenge(filterContext);
        }
    }

    public class HostAuthenticationFilter : IAuthenticationFilter
    {
        private readonly string _authenticationType;

        /// <summary>
        /// Gets the authentication type of the OWIN middleware to use.
        /// </summary>
        /// 
        /// <returns>
        /// The authentication type of the OWIN middleware to use.
        /// </returns>
        public string AuthenticationType
        {
            get { return this._authenticationType; }
        }

        /// <summary>
        /// Gets a value indicating whether the filter allows multiple authentication.
        /// </summary>
        /// 
        /// <returns>
        /// true if the filter allows multiple authentication; otherwise, false.
        /// </returns>
        public bool AllowMultiple
        {
            get { return true; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.HostAuthenticationFilter"/> class.
        /// </summary>
        /// <param name="authenticationType">The authentication type of the OWIN middleware to use.</param>
        public HostAuthenticationFilter(string authenticationType)
        {
            if (authenticationType == null)
                throw new ArgumentNullException("authenticationType");
            this._authenticationType = authenticationType;
        }

        private static AuthenticationResponseChallenge AddChallengeAuthenticationType(
            AuthenticationResponseChallenge challenge, string authenticationType)
        {
            List<string> list = new List<string>();
            AuthenticationProperties properties;
            if (challenge != null)
            {
                string[] authenticationTypes = challenge.AuthenticationTypes;
                if (authenticationTypes != null)
                    list.AddRange((IEnumerable<string>) authenticationTypes);
                properties = challenge.Properties;
            }
            else
                properties = new AuthenticationProperties();
            list.Add(authenticationType);
            return new AuthenticationResponseChallenge(list.ToArray(), properties);
        }

        private static IAuthenticationManager GetAuthenticationManagerOrThrow(IOwinContext context)
        {
            IAuthenticationManager authenticationManager = context.Authentication;
            if (authenticationManager == null)
                throw new InvalidOperationException("qwe");
            return authenticationManager;
        }

        public async void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("context");
            if (filterContext.HttpContext == null)
                throw new InvalidOperationException("qwe");
            IAuthenticationManager authenticationManager =
                HostAuthenticationFilter.GetAuthenticationManagerOrThrow(filterContext.HttpContext.GetOwinContext());
            AuthenticateResult result = await authenticationManager.AuthenticateAsync(this._authenticationType);
            if (result != null)
            {
                IIdentity identity = (IIdentity) result.Identity;
                if (identity != null)
                    filterContext.HttpContext.User = new ClaimsPrincipal(identity);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }
    }
}