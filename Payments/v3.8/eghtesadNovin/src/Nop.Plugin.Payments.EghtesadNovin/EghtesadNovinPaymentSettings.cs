using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.EghtesadNovin
{
    public class EghtesadNovinPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }
        public string Passcode { get; set; }
    }
}
