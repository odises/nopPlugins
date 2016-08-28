using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Saman
{
    public class SamanPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }
        public string Passcode { get; set; }

    }
}
