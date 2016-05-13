using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Fanava.Models
{
    public class SamanCallbackResult
    {
        public string ReferenceNumber { get; set; }
        public string ReservationNumber { get; set; }
        public string State { get; set; }
        public string TraceNumber { get; set; }

        public override string ToString()
        {
            return string.Format("{{'ReferenceNumber':'{0}','ReservationNumber':'{1}','State':'{2}','TraceNumber':'{3}'}}", ReferenceNumber, ReservationNumber, State, TraceNumber);
        }
    }
}
