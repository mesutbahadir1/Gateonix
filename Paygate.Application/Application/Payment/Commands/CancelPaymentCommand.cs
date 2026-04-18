using MediatR;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Payment.Dtos;
using Paygate.Application.Shared.Factories;
using Paygate.Application.Shared.Results;

namespace Paygate.Application.Payment.Commands;

public class CancelPaymentCommand : IRequest<Result<PaymentResponse>>
{
     public Guid TransactionId { get; set; }
}


public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, Result<PaymentResponse>>
{
     private readonly IPaymentProviderFactory _factory;
     
     public CancelPaymentCommandHandler(IPaymentProviderFactory factory)
     {
          _factory = factory;
     }

     public Task<Result<PaymentResponse>> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
     {
          try
          {
               var paymentProvider = _factory.GetProvider();
               Transaction tx = paymentProvider.CancelPayment(request.TransactionId);

               if (tx.Status!=PaymentStatus.Cancelled)
                    return Task.FromResult(Result<PaymentResponse>.Fail("Payment cancel failed"));
               var response = new PaymentResponse
               {
                    TransactionId = tx.Id,
                    Success = true,
                    Message = tx.Status.ToString()
               };
               return Task.FromResult(Result<PaymentResponse>.Ok(response,"Payment cancelled"));
          }
          catch (Exception e)
          {
               return Task.FromResult(Result<PaymentResponse>.Fail("Payment cancel failed."));
          }
     }
}