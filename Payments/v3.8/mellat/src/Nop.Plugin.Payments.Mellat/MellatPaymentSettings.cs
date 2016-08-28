using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Mellat
{
    public class MellatPaymentSettings : ISettings
    {
        public string TerminalId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
