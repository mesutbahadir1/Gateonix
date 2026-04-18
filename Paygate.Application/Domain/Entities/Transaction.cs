using Paygate.Application.Domain.Enums;
using Paygate.Application.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Entities
{
    public class Transaction : Entity, IAggregateRoot
    {
        public Guid BankId { get; set; }
        public Guid CardId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
