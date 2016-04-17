using Nop.Web.Framework.Controllers;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.Mellat.Models;
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

namespace Nop.Plugin.Payments.Mellat.Controllers
{
    public class PaymentMellatController : BasePaymentController
    {
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentMellatController(IOrderService orderService,
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
            var samanPaymentSettings = _settingService.LoadSetting<MellatPaymentSettings>(storeScope);

            throw new NotImplementedException();
        }
    }
}
