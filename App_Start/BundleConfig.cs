using System.Web;
using System.Web.Optimization;

namespace PCBookWebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/underscore-min.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap3.5/css/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angularLib").Include(
                        "~/Scripts/angular/angular.min.js",
                        "~/Scripts/angular/angular-route.min.js",
                        "~/Scripts/angular/angular-messages.min.js",
                        "~/Scripts/angular/angular-animate.min.js",
                        "~/Scripts/angular/angular-aria.min.js",
                        "~/Scripts/angular/angular-loader.min.js",
                        "~/Scripts/angular/angular-touch.min.js",
                        "~/Scripts/angular/angular-resource.min.js",
                        "~/Scripts/angular/angular-mocks.js",
                        "~/Scripts/angular/angular-locale_bn-bd.js",
                        "~/Scripts/angular/angular-cookies.min.js",
                        "~/Scripts/angular/angular-local-storage.min.js",
                        "~/Scripts/angular-dropdown-multiselect/angularjs-dropdown-multiselect.min.js",
                        //"~/Scripts/angular-ui/ui-bootstrap-tpls1.2.4.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls-0.12.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularApp")
                    .Include("~/AngApp/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularControllers")
                .IncludeDirectory("~/AngApp/Controllers", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/angularBookControllers")
                .IncludeDirectory("~/AngApp/BookModule/Controllers", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/angularBankControllers")
                .IncludeDirectory("~/AngApp/BankModule/Controllers", "*.js", true));


            bundles.Add(new ScriptBundle("~/bundles/angularSaleControllers")
                .IncludeDirectory("~/AngApp/SalesModule/Controllers", "*.js", true));


            bundles.Add(new ScriptBundle("~/bundles/angularDirectives")
                    .IncludeDirectory("~/AngApp/Directives", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/angularFactories")
                    .IncludeDirectory("~/AngApp/Factories", "*.js", true));
        }
    }
}
