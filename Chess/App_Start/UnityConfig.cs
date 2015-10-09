using System;
using System.Configuration;
using Chess.DataAccess;
using Chess.DataAccess.Contracts;
using Chess.Domain;
using Microsoft.Practices.Unity;
using UCIProxy;
using UCIProxy.DAL;

namespace Chess.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType<AnalysisRepository, AnalysisRepository>();
            
            var maxAnalisysyDepth = Int32.Parse(ConfigurationManager.AppSettings["MaxAnalysisDepth"]);
            var maxOutputLines = Int32.Parse(ConfigurationManager.AppSettings["MaxOutputLines"]);
            container.RegisterType<UciProxy, UciProxy>(new InjectionConstructor(new ResolvedParameter<AnalysisRepository>(), maxAnalisysyDepth, maxOutputLines));

            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterType<IUserDataAccess, UserDataAccess>();
        }
    }
}
