using System.Web;
using System.Web.Optimization;

namespace ShareWeb
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

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/CommonCSS").Include(
                    //Bootstrap Core CSS
                    "~/Content/bootstrap/css/bootstrap.min.css",
                    //MetisMenu CSS
                    "~/Content/metisMenu/metisMenu.min.css",
                    //Custom CSS
                    "~/Content/dist/css/sb-admin-2.min.css",
                    //Custom Fonts
                    "~/Content/font-awesome/css/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/Content/CommonJS").Include(
                    //jQuery
                    "~/Content/jquery/jquery.min.js",
                    //Bootstrap Core JavaScript
                    "~/Content/bootstrap/js/bootstrap.min.js",
                    //Metis Menu Plugin JavaScript
                    "~/Content/metisMenu/metisMenu.min.js",
                    //Custom Theme JavaScript
                    "~/Content/dist/js/sb-admin-2.min.js"));

            bundles.Add(new StyleBundle("~/Content/ChartsCSS").Include(
                    //Morris Charts CSS
                    "~/Content/morrisjs/morris.css"));

            bundles.Add(new ScriptBundle("~/Content/ChartsJS").Include(
                    //Morris Charts JavaScript
                    "~/Content/raphael/raphael.min.js",
                    "~/Content/morrisjs/morris.min.js",
                    "~/Content/data/morris-data.js"));
        }
    }
}
