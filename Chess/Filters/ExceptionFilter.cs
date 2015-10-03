using System.Web.Mvc;
using UCIProxy.Common;

namespace Chess.Filters
{
    internal class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            LogHelper.LogError(filterContext.Exception);
            
            filterContext.Result = new RedirectResult("/Home/Error");
            filterContext.ExceptionHandled = true;
        }
    }
}