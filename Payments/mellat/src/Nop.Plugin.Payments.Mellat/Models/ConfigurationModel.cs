using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Mellat.Models
{
    public class ConfigurationModel : BaseNopModel 
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Mellat.Fields.TerminalId")]
        public string TerminalId { get; set; }
        public bool TerminalId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Mellat.Fields.Username")]
        public string Username { get; set; }
        public bool Username_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Mellat.Fields.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }
    }
}
