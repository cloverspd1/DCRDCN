namespace BEL.DCRDCNWorkflow
{
    using System.Web;
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            if (bundles != null)
            {
                bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-1.10.2.min.js",
                            "~/Scripts/jquery-ui.js",
                            "~/Scripts/jquery-migrate-1.2.1.min.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate*",
                            "~/Scripts/jquery.unobtrusive-ajax.min.js"));

                // Use the development version of Modernizr to develop with and learn from. Then, when you're
                // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

                bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                            "~/Scripts/moment.js",
                            "~/Scripts/bootstrap.js",
                            "~/Scripts/bootstrap-datetimepicker.min.js",
                            "~/Scripts/bootstrap-hover-dropdown.js",
                            "~/Scripts/bootstrap-multiselect.js",
                            "~/Scripts/respond.js",
                            "~/Scripts/jquery.menu.js",
                            "~/Scripts/jquery.tokeninput.js",
                            "~/Scripts/responsive-tabs.js"));

                bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                            "~/Scripts/jquery.datatable.js",
                            "~/Scripts/spcontext.js",
                            "~/Scripts/fileuploader.js",
                            "~/Scripts/ProjectScripts/SessionUpdater.js",
                            "~/Scripts/ProjectScripts/common.js",
                            "~/Scripts/ProjectScripts/resources.js"));

                //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                //            "~/Scripts/jquery-{version}.js"));
                //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                //          "~/Scripts/jquery-1.10.2.min.js",
                //          "~/Scripts/jquery-ui.js",
                //          "~/Scripts/jquery-migrate-1.2.1.min.js"));

                //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                //            "~/Scripts/jquery.validate*"));

                //// Use the development version of Modernizr to develop with and learn from. Then, when you're
                //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                //              "~/Scripts/modernizr-*"));

                //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                //          "~/Scripts/moment.js",
                //          "~/Scripts/bootstrap.js",
                //          "~/Scripts/bootstrap-datetimepicker.min.js",
                //          "~/Scripts/bootstrap-hover-dropdown.js",
                //          "~/Scripts/respond.js",
                //          "~/Scripts/jquery.menu.js",
                //          "~/Scripts/jquery.tokeninput.js",
                //          "~/Scripts/responsive-tabs.js"));

                //bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                //            "~/Scripts/jquery.datatable.js",
                //            "~/Scripts/spcontext.js",
                //            "~/Scripts/fileuploader.js",
                //            "~/Scripts/ProjectScripts/common.js",
                //            "~/Scripts/ProjectScripts/resources.js"));

                //DCR
                bundles.Add(new ScriptBundle("~/bundles/dcrindex").Include("~/Scripts/ProjectScripts/dcr/dcr.js"));

                //DCR
                bundles.Add(new ScriptBundle("~/bundles/dcnindex").Include("~/Scripts/ProjectScripts/dcn/dcn.js"));

                //ADMIN
                bundles.Add(new ScriptBundle("~/bundles/adminindex").Include("~/Scripts/ProjectScripts/Admin/admin.js"));

                //Report
                bundles.Add(new ScriptBundle("~/bundles/reportindex").Include("~/Scripts/ProjectScripts/Reports/report.js"));

                
                bundles.Add(new StyleBundle("~/Content/css").Include(
                          "~/Content/jquery-ui-1.10.4.custom.min.css",
                          "~/Content/fileuploader.css",
                          "~/Content/font-awesome.min.css",
                          "~/Content/bootstrap.min.css",
                          "~/Content/animate.css",
                          "~/Content/bootstrap-datetimepicker.css",
                          "~/Content/bootstrap-multiselect.css",
                          "~/Content/main.css",
                          "~/Content/token-input.css",
                          "~/Content/style-responsive.css"));
            }
        }
    }
}
