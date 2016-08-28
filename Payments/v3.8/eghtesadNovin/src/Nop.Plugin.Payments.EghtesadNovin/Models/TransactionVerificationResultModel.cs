using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.EghtesadNovin.Models
{
    public class TransactionVerificationResultModel
    {
        public decimal Amount { get; set; }
        public bool AmountSpecified { get; set; }
        public string ReferenceNumber { get; set; }
        public string VerificationError { get; set; }
        public bool VerificationErrorSpecified { get; set; }

        public override string ToString()
        {
            return string.Format("{{'Amount':'{0}','AmountSpecified':'{1}','ReferenceNumber':'{2}','VerificationError':'{3}','VerificationErrorSpecified':'{4}'}}", Amount.ToString(), AmountSpecified.ToString(), ReferenceNumber, VerificationError, VerificationErrorSpecified.ToString());
        }
    }

   
}
