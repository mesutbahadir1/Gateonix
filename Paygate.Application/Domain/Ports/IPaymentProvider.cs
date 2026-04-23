using Paygate.Application.Application.Payment.Commands;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Domain.Entities;

namespace Paygate.Application.Domain.Ports
{
    public interface IPaymentProvider
    {
        public string Name { get; }
        public Guid BankId { get; }
        public bool IsAvailable { get; }
        Transaction GetPaymentStatus(Guid transactionId);
        Transaction ProcessPayment(Guid cardId, decimal amount, string currency);
        Transaction ConfirmPayment(Guid transactionId);
        Transaction RefundPayment(Guid transactionId, decimal amount);
        Transaction CancelPayment(Guid transactionId);
        PaymentInitiateResponse Initiate3DPayment(Initiate3DPaymentCommand request);

    }
}
