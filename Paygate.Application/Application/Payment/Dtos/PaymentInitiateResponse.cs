using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Application.Payment.Dtos
{
    public class PaymentInitiateResponse
    {
        public bool IsSuccess { get; set; }
        public string OrderId { get; set; }
        public string ProviderName { get; set; }
        public string PaymentHtmlContent { get; set; }
        public DateTime ResponseDate { get; set; }
    }
}
