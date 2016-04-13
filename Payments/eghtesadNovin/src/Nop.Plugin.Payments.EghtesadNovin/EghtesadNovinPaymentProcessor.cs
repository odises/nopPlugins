using Nop.Core.Plugins;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using System.Web.Routing;
using System.Web;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Plugin.Payments.EghtesadNovin.Controllers;

namespace Nop.Plugin.Payments.EghtesadNovin
{
    public class EghtesadNovinPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        private readonly EghtesadNovinPaymentSettings _eghtesadNovinPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        public EghtesadNovinPaymentProcessor(EghtesadNovinPaymentSettings eghtesadNovinPaymentSettings,
            ISettingService settingService,
            HttpContextBase httpContext,
            IWebHelper webHelper)
        {
            this._eghtesadNovinPaymentSettings = eghtesadNovinPaymentSettings;
            this._settingService = settingService;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            /// TODO 
            actionName = "Configure";
            controllerName = "PaymentEghtesadNovin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.EghtesadNovin.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentEghtesadNovinController);
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentEghtesadNovin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.EghtesadNovin.Controllers" }, { "area", null } };
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var resNum = postProcessPaymentRequest.Order.OrderGuid.ToString();
            var amount = (long)postProcessPaymentRequest.Order.OrderTotal;
            var mid = _eghtesadNovinPaymentSettings.MerchantId;

            _httpContext.Response.RedirectToRoute("EghtesadNovinPaymentBeforeGateway", new { mid = mid, resNum = resNum, amount = amount, redirectUrl = string.Concat(_webHelper.GetStoreLocation(false), "payment/enbank/callback") });
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        public override void Install()
        {
            var settings = new EghtesadNovinPaymentSettings
            {
                MerchantId = "merchantId",
                Passcode = "passcode"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.EghtesadNovin.Fields.MerchantId", "Merchant Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.EghtesadNovin.Fields.Passcode", "Passcode");

            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<EghtesadNovinPaymentSettings>();

            this.DeletePluginLocaleResource("Plugins.Payments.EghtesadNovin.Fields.MerchantId");
            this.DeletePluginLocaleResource("Plugins.Payments.EghtesadNovin.Fields.Passcode");

            base.Uninstall();
        }

        #endregion
    }
}
