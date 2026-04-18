using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Infrastructure.PaymentProviders
{
    public class HalkbankPaymentProvider : IPaymentProvider
    {
        public string Name => "Halkbank";

        public Guid BankId => Guid.NewGuid();

        public bool IsAvailable => true;

        public Transaction CancelPayment(Guid transactionId)
        {
            return new Transaction()
            {
                Id = transactionId,
                BankId = BankId,
                CardId = Guid.NewGuid(),
                Amount = 0,
                Currency = "TRY",
                Status = PaymentStatus.Cancelled
            };
        }

        public Transaction ConfirmPayment(Guid transactionId)
        {
            return new Transaction()
            {
                Id = transactionId,
                BankId = BankId,
                CardId = Guid.NewGuid(),
                Amount = 0,
                Currency = "TRY",
                Status = PaymentStatus.Completed
            };
        }

        public Transaction GetPaymentStatus(Guid transactionId)
        {
            return new Transaction()
            {
                Id = transactionId,
                BankId = BankId,
                CardId = Guid.NewGuid(),
                Amount = 0,
                Currency = "TRY",
                Status = PaymentStatus.Completed
            };
        }

        public Transaction ProcessPayment(Guid cardId, decimal amount, string currency)
        {
            return new Transaction()
            {
                Id = Guid.NewGuid(),
                BankId = BankId,
                CardId = cardId,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Pending
            };
        }

        public Transaction RefundPayment(Guid transactionId, decimal amount)
        {
            return new Transaction()
            {
                Id = transactionId,
                BankId = BankId,
                CardId = Guid.NewGuid(),
                Amount = amount,
                Currency = "TRY",
                Status = PaymentStatus.Refunded
            };
        }
    }
}
