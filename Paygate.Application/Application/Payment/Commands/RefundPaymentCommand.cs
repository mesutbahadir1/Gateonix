using MediatR;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Payment.Dtos;
using Paygate.Application.Shared.Factories;
using Paygate.Application.Shared.Results;

namespace Paygate.Application.Payment.Commands;

public class RefundPaymentCommand : IRequest<Result<PaymentResponse>>
{
    public Guid TransactionId { get; set; }
    
    public decimal Amount { get; set; }
}


public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<PaymentResponse>>
{
    private readonly IPaymentProviderFactory _factory;

    public RefundPaymentCommandHandler(IPaymentProviderFactory factory)
    {
        _factory = factory;
    }

    public Task<Result<PaymentResponse>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentProvider = _factory.GetProvider();
            Transaction tx = paymentProvider.RefundPayment(request.TransactionId, request.Amount);
            if (tx.Status!=PaymentStatus.Refunded)
                return Task.FromResult(Result<PaymentResponse>.Fail("Payment refund failed"));
            var response = new PaymentResponse
            {
                TransactionId = tx.Id,
                Success = true,
                Message = tx.Status.ToString()
            };
            return Task.FromResult(Result<PaymentResponse>.Ok(response,"Payment refunded"));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<PaymentResponse>.Fail("Payment refund failed"));
        }
    }
}