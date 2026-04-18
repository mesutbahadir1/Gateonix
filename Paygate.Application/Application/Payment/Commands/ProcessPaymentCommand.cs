using MediatR;
using Microsoft.Extensions.Logging;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Payment.Dtos;
using Paygate.Application.Shared.Factories;
using Paygate.Application.Shared.Results;

namespace Paygate.Application.Payment.Commands;

public class ProcessPaymentCommand : IRequest<Result<PaymentResponse>>
{
    public Guid CardId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
}

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PaymentResponse>>
{
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly IPaymentProviderFactory _factory;

    public ProcessPaymentCommandHandler(ILogger<ProcessPaymentCommandHandler> logger, IPaymentProviderFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public Task<Result<PaymentResponse>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Payment successful...");
            var paymentProvider = _factory.GetProvider();
            Transaction tx = paymentProvider.ProcessPayment(request.CardId, request.Amount, request.Currency);
            if (tx.Status!=PaymentStatus.Pending)
                return Task.FromResult(Result<PaymentResponse>.Fail("Payment process failed"));
            var response = new PaymentResponse
            {
                TransactionId = tx.Id,
                Success = true,
                Message = tx.Status.ToString()
            };
            

            return Task.FromResult(Result<PaymentResponse>.Ok(response,"Payment processed"));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<PaymentResponse>.Fail("Payment process failed"));
        }
    }
}