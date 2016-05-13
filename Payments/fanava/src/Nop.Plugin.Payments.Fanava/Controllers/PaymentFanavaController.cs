using Nop.Web.Framework.Controllers;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.Fanava.Models;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;

namespace Nop.Plugin.Payments.Fanava.Controllers
{
    public class PaymentFanavaController : BasePaymentController
    {
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentFanavaController(IOrderService orderService,
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService)
        {
            _orderService = orderService;
            _storeService = storeService;
            _workContext = workContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var fanavaPaymentSettings = _settingService.LoadSetting<FanavaPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.MerchantId = fanavaPaymentSettings.MerchantId;
            model.Passcode = fanavaPaymentSettings.Passcode;

            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = _settingService.SettingExists(fanavaPaymentSettings, x => x.MerchantId, storeScope);
                model.Passcode_OverrideForStore = _settingService.SettingExists(fanavaPaymentSettings, x => x.Passcode, storeScope);
            }

            return View("~/Plugins/Payments.Fanava/Views/PaymentFanava/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var fanavaPaymentSettings = _settingService.LoadSetting<FanavaPaymentSettings>(storeScope);

            //save settings
            fanavaPaymentSettings.MerchantId = model.MerchantId;
            fanavaPaymentSettings.Passcode = model.Passcode;

            if (model.MerchantId_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(fanavaPaymentSettings, x => x.MerchantId, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(fanavaPaymentSettings, x => x.MerchantId, storeScope);
            }

            if (model.Passcode_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(fanavaPaymentSettings, x => x.Passcode, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(fanavaPaymentSettings, x => x.Passcode, storeScope);
            }

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.Fanava/Views/PaymentFanava/PaymentInfo.cshtml");
        }

        public ActionResult BeforeGateway(string amount, string resNum, string mid, string redirectUrl)
        {
            BeforeGatewayModel model = new BeforeGatewayModel();
            model.Amount = amount;
            model.ResNum = resNum;
            model.Mid = mid;
            model.RedirectUrl = redirectUrl;

            return View("~/Plugins/Payments.Fanava/Views/PaymentFanava/BeforeGateway.cshtml", model);
        }

        [HttpPost]
        public ActionResult FanavaCallback(string refNum, string resNum, string state, string traceNo)
        {
            var callbackResult = new SamanCallbackResult
            {
                ReferenceNumber = refNum,
                ReservationNumber = resNum,
                State = state,
                TraceNumber = traceNo
            };

            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var fanavaPaymentSettings = _settingService.LoadSetting<FanavaPaymentSettings>(storeScope);
            Guid orderGuid;
            try
            {
                orderGuid = Guid.Parse(resNum);
            }
            catch (Exception ex)
            {
                _logger.Error("Callback failed.", ex);
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var order = _orderService.GetOrderByGuid(orderGuid);

            if (order == null)
            {
                _logger.Error("Callback failed.", new NopException("Order is null."));
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            order.AuthorizationTransactionResult = callbackResult.ToString();

            _orderService.UpdateOrder(order);

            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            var service = new ir.shaparak.fanava.PaymentWebServiceService();
            //var result = service.verifyTransaction(refNum, fanavaPaymentSettings.MerchantId);

            //order.AuthorizationTransactionCode = result.ToString();

            _orderService.UpdateOrder(order);

            //if (result > 0)
            //{
            //    _orderProcessingService.MarkAsAuthorized(order);

            //    if (result == (double)order.OrderTotal)
            //    {
            //        _orderProcessingService.MarkOrderAsPaid(order);
            //        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            //    }
            //    else
            //    {
            //        var reverseId = service.reverseTransaction(refNum, fanavaPaymentSettings.MerchantId, fanavaPaymentSettings.MerchantId, fanavaPaymentSettings.Passcode);
            //        order.CaptureTransactionId = reverseId.ToString();
            //        _orderService.UpdateOrder(order);

            //        if (reverseId == 1)
            //        {
            //            _orderProcessingService.Refund(order);
            //            _logger.Error("Transaction reverse process was successful.", new NopException(reverseId.ToString()));
            //        }
            //        else
            //        {
            //            _logger.Error("Transaction reverse process was not successful.", new NopException(reverseId.ToString()));
            //        }

            //        return RedirectToAction("Index", "Home", new { area = "" });
            //    }
            //}
            //else
            //{
            //    _logger.Error("Transaction verification failed.", new NopException(result.ToString()));
            //    return RedirectToAction("Index", "Home", new { area = "" });
            //}

            return null;
        }
    }
}
