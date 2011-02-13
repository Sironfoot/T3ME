using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcLibrary.Bootstrapper;
using System.Web.Routing;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using MvcLibrary.Utils;
using T3ME.Business.Utils;

namespace T3ME.Business.StartUp
{
    [BootstrapperPriority(Priority = 2)]
    public class RegisterRoutes : IBootstrapperTask
    {
        protected readonly RouteCollection Routes = null;

        public RegisterRoutes() : this(RouteTable.Routes) { }

        public RegisterRoutes(RouteCollection routeCollection)
        {
            this.Routes = routeCollection;
        }

        public void Execute()
        {
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            Routes.MapRoute("LatestTweets", "",
                new { controller = "Display", action = "Latest", format = "html", page = 1 });
            Routes.MapRoute("LatestTweetsPaging", "page/{page}",
                new { controller = "Display", action = "Latest", format = "html" });
            Routes.MapRoute("LatestTweetsFormat", "latest.{format}",
                new { controller = "Display", action = "Latest",  page = 1 });
            Routes.MapRoute("LatestTweetsPagingFormat", "latest.{format}/page/{page}",
                new { controller = "Display", action = "Latest" });

            Routes.MapRouteWithFormat("RecentlyVoted", "recently-voted.{format}",
                new { controller = "Display", action = "RecentlyVoted", format = "html", page = 1 });
            Routes.MapRouteWithFormat("RecentlyVotedPaging", "recently-voted.{format}/page/{page}",
                new { controller = "Display", action = "RecentlyVoted", format = "html" });

            Routes.MapRouteWithFormat("TopRated", "top-rated.{format}",
                new { controller = "Display", action = "TopRated", format = "html", page = 1 });
            Routes.MapRouteWithFormat("TopRatedPaging", "top-rated.{format}/page/{page}",
                new { controller = "Display", action = "TopRated", format = "html" });

            Routes.MapRoute("Language", "Language/{action}", new { controller = "Language" });

            Routes.MapRoute("Status", "status/{twitterId}/{message}",
                new { controller = "Display", action = "Status" });

            Routes.MapRoute("Stats", "stats", new { controller = "Stats", action = "Index" });

            Routes.MapRoute("About", "about/{action}", new { controller = "About", action = "Index" });

            Routes.MapRoute("OAuth", "OAuth/{action}", new { controller = "OAuth", action = "" });
            Routes.MapRoute("Vote", "Vote/{action}", new { controller = "Vote", action = "" });

            Routes.MapRoute("Ajax", "Ajax/{action}", new { controller = "Ajax", action = "" });

            Routes.MapRoute("404", "{*url}", new { controller = "Display", action = "NotFound404" });

            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(Routes);
        }
    }
}