using Nop.Web.Framework.Controllers;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Plugin.Payments.EghtesadNovin.Models;
using System;

namespace Nop.Plugin.Payments.EghtesadNovin.Controllers
{
    public class PaymentEghtesadNovinController : BasePaymentController
    {
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentEghtesadNovinController(IOrderService orderService,
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
            var eghtesadNovinPaymentSettings = _settingService.LoadSetting<EghtesadNovinPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.MerchantId = eghtesadNovinPaymentSettings.MerchantId;
            model.Passcode = eghtesadNovinPaymentSettings.Passcode;

            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = _settingService.SettingExists(eghtesadNovinPaymentSettings, x => x.MerchantId, storeScope);
                model.Passcode_OverrideForStore = _settingService.SettingExists(eghtesadNovinPaymentSettings, x => x.Passcode, storeScope);
            }

            return View("~/Plugins/Payments.EghtesadNovin/Views/PaymentEghtesadNovin/Configure.cshtml", model);
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
            var eghtesadNovinPaymentSettings = _settingService.LoadSetting<EghtesadNovinPaymentSettings>(storeScope);

            //save settings
            eghtesadNovinPaymentSettings.MerchantId = model.MerchantId;
            eghtesadNovinPaymentSettings.Passcode = model.Passcode;

            if (model.MerchantId_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(eghtesadNovinPaymentSettings, x => x.MerchantId, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(eghtesadNovinPaymentSettings, x => x.MerchantId, storeScope);
            }

            if (model.Passcode_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(eghtesadNovinPaymentSettings, x => x.Passcode, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(eghtesadNovinPaymentSettings, x => x.Passcode, storeScope);
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
            return View("~/Plugins/Payments.EghtesadNovin/Views/PaymentEghtesadNovin/PaymentInfo.cshtml");
        }

        public ActionResult BeforeGateway(string mid, string resNum, string amount, string redirectUrl)
        {
            BeforeGatewayModel model = new BeforeGatewayModel();
            model.Mid = mid;
            model.ResNum = resNum;
            model.Amount = amount;
            model.RedirectUrl = redirectUrl;

            return View("~/Plugins/Payments.EghtesadNovin/Views/PaymentEghtesadNovin/BeforeGateway.cshtml", model);
        }

        [HttpPost]
        public ActionResult EghtesadNovinCallback(string refNum, string resNum, string state, string mid, string language, string cardPanHash)
        {
            var callbackResult = new EnbankCallbackResult
            {
                ReferenceNumber = refNum,
                ReservationNumber = resNum,
                State = state,
                Mid = mid,
                Language = language,
                CardPanHash = cardPanHash
            };

            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var eghtesadNovinPaymentSettings = _settingService.LoadSetting<EghtesadNovinPaymentSettings>(storeScope);

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

            order.AuthorizationTransactionId = callbackResult.ToString();

            _orderService.UpdateOrder(order);

            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            if (callbackResult.State.ToUpper() == "OK")
            {
                _orderProcessingService.MarkAsAuthorized(order);

                Pna.Shaparak.PaymentWebServiceService modernService = new Pna.Shaparak.PaymentWebServiceService();
                modernService.AllowAutoRedirect = true;
                Pna.Shaparak.loginRequest loginRequest = new Pna.Shaparak.loginRequest();
                loginRequest.username = eghtesadNovinPaymentSettings.MerchantId;
                loginRequest.password = eghtesadNovinPaymentSettings.Passcode;

                var sessionId = modernService.login(loginRequest);
                Pna.Shaparak.wsContext context = new Pna.Shaparak.wsContext() { data = new Pna.Shaparak.wsContextEntry[1] };
                context.data[0] = new Pna.Shaparak.wsContextEntry();
                context.data[0].key = "SESSION_ID";
                context.data[0].value = sessionId;

                Pna.Shaparak.verifyResponseResult[] result = modernService.verifyTransaction(context, new string [] {callbackResult.ReferenceNumber});
                var transactionVerificationResult = new TransactionVerificationResultModel();
                transactionVerificationResult.Amount = result[0].amount;
                transactionVerificationResult.AmountSpecified = result[0].amountSpecified;
                transactionVerificationResult.ReferenceNumber = result[0].refNum;
                transactionVerificationResult.VerificationError = result[0].verificationError.ToString();
                transactionVerificationResult.VerificationErrorSpecified = result[0].verificationErrorSpecified;

                order.AuthorizationTransactionResult = transactionVerificationResult.ToString();
                _orderService.UpdateOrder(order);

                if (transactionVerificationResult.Amount == order.OrderTotal)
                {
                    _orderProcessingService.MarkOrderAsPaid(order);
                    modernService.logout(context);
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });    
                }
                else
                {
                    _logger.Error("Transaction verification failed. amounts mismatch.");
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                _logger.Error("Transaction verification failed.");
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
    }
}
