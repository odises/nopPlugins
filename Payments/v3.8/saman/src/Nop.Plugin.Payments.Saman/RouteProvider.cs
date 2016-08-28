using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.Saman
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
            routes.MapRoute("SamanPaymentBeforeGateway", "saman/beforegateway", new { controller = "PaymentSaman", action = "BeforeGateway" }, new[] { "Nop.Plugin.Payments.Saman.Controllers" });
            routes.MapRoute("SamanPaymentCallback", "payment/saman/callback", new { controller = "PaymentSaman", action = "SamanCallback" }, new[] { "Nop.Plugin.Payments.Saman.Controllers" });
        }
    }
}
