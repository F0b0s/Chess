using System.Web;
using System.Web.Mvc;
using Chess.Infrastructure.Attributes;
using Microsoft.AspNet.Identity;

namespace Chess
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HostAuthenticationAttribute(DefaultAuthenticationTypes.ExternalCookie));
        }
    }
}
