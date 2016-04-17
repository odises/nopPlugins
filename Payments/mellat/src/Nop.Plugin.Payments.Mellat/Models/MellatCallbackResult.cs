using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Mellat.Models
{
    public class MellatCallbackResult
    {
        public string RefId { get; set; }
        public string ResCode { get; set; }
        public long SaleOrderId { get; set; }
        public long SaleReferenceId { get; set; }
        public string CardHolderInfo { get; set; }
        public string CardHolderPan { get; set; }

        public override string ToString()
        {
            return null;
            //return string.Format("{{'ReferenceNumber':'{0}','ReservationNumber':'{1}','State':'{2}','TraceNumber':'{3}'}}", ReferenceNumber, ReservationNumber, State, TraceNumber);
        }
    }
}
