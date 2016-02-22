using System.Web.Optimization;

namespace Chess
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
//            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
//                        "~/Scripts/jquery-{version}.js"));

//            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
//                "~/Scripts/jquery-ui-{version}.js"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // используйте средство построения на сайте http://modernizr.com, чтобы выбрать только нужные тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                "~/Content/chessboard-0.3.0.css",
                "~/Content/themes/base/spinner.css",
                "~/Content/themes/base/all.css"
                ));
            //"~/Content/jquery-ui-1.10.4.css"));
        }
    }
}
