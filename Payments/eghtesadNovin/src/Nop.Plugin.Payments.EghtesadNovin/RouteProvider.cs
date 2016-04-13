using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.EghtesadNovin
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
            routes.MapRoute("EghtesadNovinPaymentBeforeGateway", "enbank/beforegateway", new { controller = "PaymentEghtesadNovin", action = "BeforeGateway" }, new[] { "Nop.Plugin.Payments.EghtesadNovin.Controllers" });
            routes.MapRoute("EghtesadNovinPaymentCallback", "payment/enbank/callback", new { controller = "PaymentEghtesadNovin", action = "EghtesadNovinCallback" }, new[] { "Nop.Plugin.Payments.EghtesadNovin.Controllers" });
        }
    }
}
