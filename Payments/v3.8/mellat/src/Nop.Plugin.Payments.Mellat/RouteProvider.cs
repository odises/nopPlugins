using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.Mellat
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
            routes.MapRoute("MellatPaymentBeforeGateway", "mellat/beforegateway", new { controller = "PaymentMellat", action = "BeforeGateway" }, new[] { "Nop.Plugin.Payments.Mellat.Controllers" });
            routes.MapRoute("MellatPaymentCallback", "payment/mellat/callback", new { controller = "PaymentMellat", action = "MellatCallback" }, new[] { "Nop.Plugin.Payments.Mellat.Controllers" });
        }
    }
}
