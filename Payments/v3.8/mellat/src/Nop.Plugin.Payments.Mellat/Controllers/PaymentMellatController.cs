using Nop.Web.Framework.Controllers;
using Nop.Services.Payments;
using System.Collections.Generic;
using Nop.Plugin.Payments.Mellat.Models;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.Web.Mvc;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Payments.Mellat.Controllers
{
    public class PaymentMellatController : BasePaymentController
    {
        private readonly IStoreService _storeService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentMellatController(
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService, IOrderService orderService)
        {
            _storeService = storeService;
            _workContext = workContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mellatPaymentSettings = _settingService.LoadSetting<MellatPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.TerminalId = mellatPaymentSettings.TerminalId;
            model.Username = mellatPaymentSettings.Username;
            model.Password = mellatPaymentSettings.Password;
            

            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.TerminalId_OverrideForStore = _settingService.SettingExists(mellatPaymentSettings, x => x.TerminalId, storeScope);
                model.Username_OverrideForStore = _settingService.SettingExists(mellatPaymentSettings, x => x.Username, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(mellatPaymentSettings, x => x.Password, storeScope);
            }

            return View("~/Plugins/Payments.Mellat/Views/PaymentMellat/Configure.cshtml", model);
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
            var mellatPaymentSettings = _settingService.LoadSetting<MellatPaymentSettings>(storeScope);

            //save settings
            mellatPaymentSettings.TerminalId = model.TerminalId;
            mellatPaymentSettings.Username = model.Username;
            mellatPaymentSettings.Password = model.Password;


            if (model.TerminalId_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(mellatPaymentSettings, x => x.TerminalId, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(mellatPaymentSettings, x => x.TerminalId, storeScope);
            }

            if (model.Username_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(mellatPaymentSettings, x => x.Username, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(mellatPaymentSettings, x => x.Username, storeScope);
            }

            if (model.Password_OverrideForStore || storeScope == 0)
            {
                _settingService.SaveSetting(mellatPaymentSettings, x => x.Password, storeScope, false);
            }
            else if (storeScope > 0)
            {
                _settingService.DeleteSetting(mellatPaymentSettings, x => x.Password, storeScope);
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
            return View("~/Plugins/Payments.Mellat/Views/PaymentMellat/PaymentInfo.cshtml");
        }

        public ActionResult BeforeGateway(string refId)
        {
            BeforeGatewayModel model = new BeforeGatewayModel();
            model.RefId = refId;
            

            return View("~/Plugins/Payments.Mellat/Views/PaymentMellat/BeforeGateway.cshtml", model);
        }

        [HttpPost]
        public ActionResult MellatCallback(MellatCallbackResult data)
        {
            // load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mellatPaymentSettings = _settingService.LoadSetting<MellatPaymentSettings>(storeScope);
            var orderId = int.Parse(data.SaleOrderId.ToString());
            var order = _orderService.GetOrderById(orderId);

            if (Session["RefId"].ToString() == data.RefId)
            {
                if (data.ResCode == "0")
                {
                    var requestVerificationResult = SubmitVerifyRequest(mellatPaymentSettings.Username, 
                        mellatPaymentSettings.Password, 
                        long.Parse(mellatPaymentSettings.TerminalId), 
                        orderId, 
                        data.SaleOrderId, 
                        data.SaleReferenceId);
                    if (requestVerificationResult == "0")
                    {
                        var settlementResult = SubmitSettleRequest(long.Parse(mellatPaymentSettings.TerminalId),
                            mellatPaymentSettings.Username, mellatPaymentSettings.Password, orderId, data.SaleOrderId,
                            data.SaleReferenceId);
                        if (settlementResult == "0" || settlementResult == "45")
                        {
                            // ok
                            _orderProcessingService.MarkOrderAsPaid(order);
                            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                        }
                        else
                        {
                            // order cancelled
                            _logger.Error("Settlement request failed.");
                            return RedirectToAction("Index", "Home", new { area = "" });
                        }

                    }
                    else
                    {
                        _logger.Error("Request verification failed.");
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
                else if (data.ResCode == "17")
                {
                    _logger.Error("Request cancelled by customer.");
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    _logger.Error("ResCode != 0");
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                _logger.Error("RefId mismatch.");
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        protected virtual string SubmitVerifyRequest(string username, string password, long terminalId, long orderId, long saleOrderId, long saleReferenceId)
        {
            var bpService = new Bpm.Mellat.PaymentGatewayImplService();
            var result = bpService.bpVerifyRequest(terminalId, username, password, orderId, saleOrderId, saleReferenceId);

            return result;
        }

        protected string SubmitSettleRequest(long terminalId, string username, string password, long orderId, long saleOrderId, long saleReferenceId)
        {
            var bpService = new Bpm.Mellat.PaymentGatewayImplService();
            var result = bpService.bpSettleRequest(terminalId, username, password, orderId, saleOrderId, saleReferenceId);

            return result;
        }
    }
}
