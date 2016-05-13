using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.Fanava
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("FanavaPaymentBeforeGateway", "Fanava/beforegateway", new { controller = "PaymentFanava", action = "BeforeGateway" }, new[] { "Nop.Plugin.Payments.Fanava.Controllers" });
            routes.MapRoute("FanavaPaymentCallback", "payment/Fanava/callback", new { controller = "PaymentFanava", action = "FanavaCallback" }, new[] { "Nop.Plugin.Payments.Fanava.Controllers" });
        }
    }
}
