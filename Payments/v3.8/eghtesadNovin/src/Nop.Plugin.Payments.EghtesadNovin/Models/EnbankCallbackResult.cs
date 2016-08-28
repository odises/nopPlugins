using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.EghtesadNovin.Models
{
    public class EnbankCallbackResult
    {
        public string ReferenceNumber { get; set; }
        public string ReservationNumber { get; set; }
        public string State { get; set; }
        public string Mid { get; set; }
        public string Language { get; set; }
        public string CardPanHash { get; set; }

        public override string ToString()
        {
            return string.Format("{{'ReferenceNumber':'{0}','ReservationNumber':'{1}','State':'{2}','Mid':'{3}','Language':'{4}','CardPanHash':'{5}'}}", ReferenceNumber, ReservationNumber, State, Mid, Language, CardPanHash);
        }
    }
}
