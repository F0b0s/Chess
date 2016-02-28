using System;
using System.Configuration;
using Chess.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using UCIProxy;
using UCIProxy.DAL;

namespace Chess
{
    public class UnityConfig
    {
        #region Unity Container
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
        #endregion

        public static void RegisterTypes(IUnityContainer container)
        {
            var connectionString = ConfigurationManager.AppSettings["ChessDbContext"];
            container.RegisterType<AnalysisRepository, AnalysisRepository>(new InjectionConstructor(new InjectionParameter(connectionString)));
            
            var maxAnalisysyDepth = Int32.Parse(ConfigurationManager.AppSettings["MaxAnalysisDepth"]);
            var maxOutputLines = Int32.Parse(ConfigurationManager.AppSettings["MaxOutputLines"]);
            container.RegisterType<UciProxy, UciProxy>(new InjectionConstructor(new ResolvedParameter<AnalysisRepository>(), maxAnalisysyDepth, maxOutputLines));
            
            var dbConnection = new IdentityDbContext<IdentityUser>(connectionString);
            container.RegisterType<IUserStore<IdentityUser>, UserStore<IdentityUser>>(new InjectionConstructor(new InjectionParameter(dbConnection)));
            container.RegisterType<UserManager<IdentityUser>, CustomUserManager>(); 
        }
    }
}