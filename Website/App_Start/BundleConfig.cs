﻿namespace QOAM.Website
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/libraries").Include("~/Scripts/jquery-{version}.js", "~/Scripts/bootstrap.js", "~/Scripts/modernizr-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/form").Include("~/Scripts/jquery.validate.js", "~/Scripts/jquery.validate.unobtrusive.js", "~/Scripts/jquery.unobtrusive-ajax.js", "~/Scripts/globalize/globalize.js", "~/Scripts/globalize/cultures/globalize.cultures.js", "~/Scripts/jquery.validate.globalize.js"));
            bundles.Add(new ScriptBundle("~/scripts/application").Include("~/Scripts/Controllers/*.js", "~/Scripts/Helpers/*.js"));
            bundles.Add(new ScriptBundle("~/scripts/fancybox").Include("~/Scripts/jquery.ipicture.min.js", "~/Scripts/jquery.fancybox.js"));
            bundles.Add(new ScriptBundle("~/scripts/typeahead").Include("~/Scripts/typeahead.bundle.js"));
            bundles.Add(new ScriptBundle("~/scripts/slider").Include("~/Scripts/bootstrap-slider.js"));

            bundles.Add(new StyleBundle("~/styles/application").Include("~/Content/bootstrap.css", "~/Content/Site.css", "~/Content/slider.css"));
            bundles.Add(new StyleBundle("~/styles/fancybox").Include("~/Content/iPicture.css", "~/Content/jquery.fancybox.css"));
            bundles.Add(new StyleBundle("~/styles/typeahead").Include("~/Content/typeahead.js-bootstrap.css"));
        }
    }
}