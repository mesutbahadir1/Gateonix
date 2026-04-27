using MediatR;
using Microsoft.Extensions.Logging;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Application.Payment.Dtos.Cards;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Shared.Factories;

namespace Paygate.Application.Application.Payment.Commands
{
    public class Initiate3DPaymentCommand : IRequest<PaymentInitiateResponse>
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public CardDto Card { get; set; }
        public string OkURL { get; set; }
        public string FailURL { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentStoreTypeEnum StoreType { get; set; }


    }
    public class Initiate3DPaymentCommandHandler : IRequestHandler<Initiate3DPaymentCommand, PaymentInitiateResponse>
    {
        private readonly ILogger<Initiate3DPaymentCommandHandler> _logger;
        private readonly IPaymentProviderFactory _factory;

        public Initiate3DPaymentCommandHandler(
            ILogger<Initiate3DPaymentCommandHandler> logger,
            IPaymentProviderFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public Task<PaymentInitiateResponse> Handle(Initiate3DPaymentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.OrderId))
            {
                _logger.LogError("OrderId is required for initiating 3D payment.");
                throw new ApplicationException("OrderId is required for initiating 3D payment.");
            }
            
            if (request.Card is null ||
                string.IsNullOrWhiteSpace(request.Card.CardNumber) ||
                string.IsNullOrWhiteSpace(request.Card.ExpiryDateMonth) ||
                string.IsNullOrWhiteSpace(request.Card.ExpiryDateYear) ||
                string.IsNullOrWhiteSpace(request.Card.CVV))
            {
                _logger.LogError("Card details are required for initiating 3D payment.");
                throw new ApplicationException("CardNumber, ExpiryDateMonth, ExpiryDateYear and CVV are required.");
            }

            if (string.IsNullOrWhiteSpace(request.OkURL))
            {
                _logger.LogError("OkURL is required for initiating 3D payment.");
                throw new ApplicationException("OkURL is required for initiating 3D payment.");
            }
            
            if (string.IsNullOrWhiteSpace(request.FailURL))
            {
                _logger.LogError("FailURL is required for initiating 3D payment.");
                throw new ApplicationException("FailURL is required for initiating 3D payment.");
            }

            _logger.LogInformation("Initiating 3D payment for OrderId: {OrderId}, Amount: {Amount} {Currency}", request.OrderId, request.Amount, request.Currency);

            var paymentProvider = _factory.GetProvider();
            var response = paymentProvider.Initiate3DPayment(request);
            return Task.FromResult(response);
        }
    }
}
