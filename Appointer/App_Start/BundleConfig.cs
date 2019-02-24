using System.Web.Optimization;

namespace Appointer
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JS
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/lib/twitter-bootstrap/js/bootstrap.min.js"));
            #endregion

            #region CSS
            bundles.Add(new StyleBundle("~/styles/css").Include(
                      "~/lib/twitter-bootstrap/css/bootstrap.min.css",
                      "~/lib/font-awesome/css/font-awesome.min.css",
                      "~/Content/font.css",
                      "~/Content/site.css"));
            #endregion
        }
    }
}
