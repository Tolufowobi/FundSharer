using System.Web;
using System.Web.Optimization;

namespace FundSharer
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js",
                     "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                "~/Scripts/metisMenu.js", "~/Scripts/metisMenu.min.js",
                "~/Scripts/rapheal.js", "~/Scripts/rapheal.min.js",
                "~/Scripts/sb-admin-2.js", "~/Scripts/sb-admin-2.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/extras").Include(
                "~/Content/metisMenu.css", "~/Content/morris.css", "~/Content/sb-admin-2.css", "~/Content/animate.css"));
        }
    }
}
