using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.EghtesadNovin.Models
{
    public class BeforeGatewayModel
    {
        public string Mid { get; set; }
        public string ResNum { get; set; }
        public string Amount { get; set; }
        public string RedirectUrl { get; set; }
    }
}
