using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Fanava
{
    public class FanavaPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }
        public string Passcode { get; set; }

    }
}
