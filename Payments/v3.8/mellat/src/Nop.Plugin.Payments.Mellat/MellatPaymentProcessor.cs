using Nop.Core.Plugins;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using System.Web.Routing;
using Nop.Plugin.Payments.Mellat.Controllers;
using System.Web;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Core;
using System.Linq;
using Nop.Core.Data;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Mellat
{
    public class MellatPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        private readonly MellatPaymentSettings _mellatPaymentSettings;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        public MellatPaymentProcessor(MellatPaymentSettings mellatPaymentSettings,
            ISettingService settingService,
            HttpContextBase httpContext,
            IWebHelper webHelper, IOrderService orderService)
        {
            this._mellatPaymentSettings = mellatPaymentSettings;
            this._settingService = settingService;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
            _orderService = orderService;
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
            actionName = "Configure";
            controllerName = "PaymentMellat";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.Mellat.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentMellatController);
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentMellat";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.Mellat.Controllers" }, { "area", null } };
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var result = SubmitPayRequest(postProcessPaymentRequest);

            if (ValidatePayResponse(result))
            {
                var postData = PopulatePostData(result);

                _httpContext.Session["RefId"] = postData.FirstOrDefault().Value;

                _httpContext.Response.RedirectToRoute("MellatPaymentBeforeGateway", new { refId = postData.FirstOrDefault().Value });
            }
            else
            {
                // log
                _httpContext.Response.RedirectToRoute("MellatPaymentFailure");
            }
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
            var settings = new MellatPaymentSettings
            {
                TerminalId = "terminalId",
                Username = "username",
                Password = "password"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Mellat.Fields.TerminalId", "Terminal Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Mellat.Fields.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Mellat.Fields.Password", "Password");

            base.Install();
        }

        public override void Uninstall()
        {
            _settingService.DeleteSetting<MellatPaymentSettings>();

            this.DeletePluginLocaleResource("Plugins.Payments.Mellat.Fields.TerminalId");
            this.DeletePluginLocaleResource("Plugins.Payments.Mellat.Fields.Username");
            this.DeletePluginLocaleResource("Plugins.Payments.Mellat.Fields.Password");

            base.Uninstall();
        }

        #endregion

        #region Utilites
        protected string SubmitPayRequest(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order;
            var callbackUrl = string.Concat(_webHelper.GetStoreLocation(false), "payment/mellat/callback");

            var payDate = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
            var payTime = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

            var bankService = new Bpm.Mellat.PaymentGatewayImplService();
            var result = bankService.bpPayRequest(long.Parse(_mellatPaymentSettings.TerminalId), _mellatPaymentSettings.Username, _mellatPaymentSettings.Password, order.Id, long.Parse(postProcessPaymentRequest.Order.OrderTotal.ToString("#")), payDate, payTime, order.Id.ToString(), callbackUrl, 0);

            return result;
        }

        protected bool ValidatePayResponse(string result)
        {
            var splits = result.Split(',');
            if (splits.Length != 2 || splits[0] != "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected Dictionary<string, object> PopulatePostData(string result)
        {
            var parts = result.Split(',');
            var postData = new Dictionary<string, object>
            {
                {"RefId", parts[1]}
            };
            return postData;
        }
        #endregion
    }
}
