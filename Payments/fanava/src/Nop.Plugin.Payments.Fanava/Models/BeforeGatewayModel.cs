using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Fanava.Models
{
    public class BeforeGatewayModel
    {
        public string Amount { get; set; }
        public string ResNum { get; set; }
        public string Mid { get; set; }
        public string RedirectUrl { get; set; }
    }
}
