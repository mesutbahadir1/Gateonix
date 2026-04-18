using MediatR;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Payment.Dtos;
using Paygate.Application.Shared.Factories;
using Paygate.Application.Shared.Results;

namespace Paygate.Application.Payment.Commands;

public class ConfirmPaymentCommand : IRequest<Result<PaymentResponse>>
{
    public Guid TransactionId { get; set; }
}


public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<PaymentResponse>>
{
    private readonly IPaymentProviderFactory _factory;
    
    public ConfirmPaymentCommandHandler(IPaymentProviderFactory factory)
    {
        _factory = factory;
    }

    public Task<Result<PaymentResponse>> Handle(ConfirmPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentProvider = _factory.GetProvider();
            Transaction tx = paymentProvider.ConfirmPayment(request.TransactionId);
            if (tx.Status!=PaymentStatus.Completed)
                return Task.FromResult(Result<PaymentResponse>.Fail("Payment confirm failed"));
            var response = new PaymentResponse
            {
                TransactionId = tx.Id,
                Success = true,
                Message = tx.Status.ToString()
            };
            return Task.FromResult(Result<PaymentResponse>.Ok(response,"Payment confirmed"));
        }
        catch (Exception e)
        { 
            return Task.FromResult(Result<PaymentResponse>.Fail("Payment confirm failed"));
        }
    }
}