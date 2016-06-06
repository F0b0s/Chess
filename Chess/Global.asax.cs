using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Chess
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // workaroud for elastic beanstalk
            ConfigureConnectionStrings();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private static void ConfigureConnectionStrings()
        {
            var fieldInfo = typeof(ConfigurationElementCollection).GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(ConfigurationManager.ConnectionStrings, false);

                // Check for AppSetting and Create
                var connectionString = ConfigurationManager.AppSettings["UCIProxy.DAL.PositionAnalysisContext"];
                if (connectionString != null)
                {
                    var positionAnalysisContext = new ConnectionStringSettings("UCIProxy.DAL.PositionAnalysisContext", connectionString)
                                                  {
                                                      ProviderName = "System.Data.EntityClient"
                                                  };
                    ConfigurationManager.ConnectionStrings.Add(positionAnalysisContext);
                }
            }
        }
    }
}