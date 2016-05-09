using System.Text;
using System.Web.Mvc;

namespace Chess.Controllers
{
    public class RouteController : Controller
    {
        public ContentResult RobotsText()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("user-agent: *");
            stringBuilder.AppendLine("allow: /");
            stringBuilder.AppendLine("allow: /Home/About");

            return Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}