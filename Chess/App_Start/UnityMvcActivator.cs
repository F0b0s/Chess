using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Chess;
using Microsoft.Practices.Unity.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(UnityWebActivator), "Shutdown")]

namespace Chess
{    
    public static class UnityWebActivator
    {
        public static void Start() 
        {
            // workaroud for elastic beanstalk
            ConfigureConnectionStrings();

            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));            
        }
                
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
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

                    ConfigurationManager.ConnectionStrings.Remove("UCIProxy.DAL.PositionAnalysisContext");
                    ConfigurationManager.ConnectionStrings.Add(positionAnalysisContext);
                }
            }
        }
    }
}