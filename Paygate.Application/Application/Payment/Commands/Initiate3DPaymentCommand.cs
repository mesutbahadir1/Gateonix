using MediatR;
using Microsoft.Extensions.Logging;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Application.Payment.Dtos.Cards;
using Paygate.Application.Application.Shared;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Task<bool> Handle(Initiate3DPaymentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.OrderId))
            {
                _logger.LogError("OrderId is required for initiating 3D payment.");
                throw new ApplicationException("OrderId is required for initiating 3D payment.");
            }

            _logger.LogInformation("Initiating 3D payment for OrderId: {OrderId}, Amount: {Amount} {Currency}", request.OrderId, request.Amount, request.Currency);



        }

        
    }
}
