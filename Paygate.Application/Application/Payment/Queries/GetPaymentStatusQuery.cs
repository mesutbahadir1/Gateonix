using MediatR;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Payment.Dtos;
using Paygate.Application.Shared.Factories;
using Paygate.Application.Shared.Results;

namespace Paygate.Application.Application.Payment.Queries;

public class GetPaymentStatusQuery : IRequest<Result<PaymentStatusResponse>>
{
    public Guid TransactionId { get; set; }
}


public class GetPaymentStatusQueryHandler : IRequestHandler<GetPaymentStatusQuery, Result<PaymentStatusResponse>>
{
    private readonly IPaymentProviderFactory _factory;

    public GetPaymentStatusQueryHandler(IPaymentProviderFactory factory)
    {
        _factory = factory;
    }

    public Task<Result<PaymentStatusResponse>> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentProvider = _factory.GetProvider();
            Transaction tx = paymentProvider.GetPaymentStatus(request.TransactionId);
            if (tx.Status!=PaymentStatus.Completed)
                return Task.FromResult(Result<PaymentStatusResponse>.Fail("Payment status retrieve failed"));
            var response = new PaymentStatusResponse
            {
                TransactionId = tx.Id,
                Status = tx.Status.ToString()
            };
            return Task.FromResult(Result<PaymentStatusResponse>.Ok(response,"Payment status retrieved"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}