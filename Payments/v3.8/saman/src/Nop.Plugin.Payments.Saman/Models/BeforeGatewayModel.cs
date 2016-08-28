using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Saman.Models
{
    public class BeforeGatewayModel
    {
        public string Token { get; set; }
        public string RedirectUrl { get; set; }
    }
}
