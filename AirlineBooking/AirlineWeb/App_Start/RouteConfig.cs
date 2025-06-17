using System.Web.Mvc;
using System.Web.Routing;

namespace AirlineWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Customer",
                url: "Customer/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "AirlineWeb.Areas.Customer.Controllers" }
            ).DataTokens.Add("area", "Customer");
        }
    }
}

