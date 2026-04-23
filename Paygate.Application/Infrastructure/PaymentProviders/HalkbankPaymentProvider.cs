using MediatR;
using Paygate.Application.Application.Payment.Commands;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Application.Shared;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public BankConfiguration Configuration = new BankConfiguration()
        {
            ApiEndpoint = "https://api.halkbank.com/payment",
            ClientId = "your-client-id",
            StoreKey = "your-store-key"
        };

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

        public PaymentInitiateResponse Initiate3DPayment(Initiate3DPaymentCommand request)
        {
            var rnd = Guid.NewGuid().ToString("N").Substring(0, 20);

            var amount = request.Amount.ToString("F2", CultureInfo.InvariantCulture).Replace(",", ".");

            var currency = CurrencyHelper.GetCurrencyISOCode(request.Currency);

            var pan = request.Card.CardNumber.Replace("-", "").Replace(" ", "");
            var Ecom_Payment_Card_ExpDate_Year = request.Card.ExpiryDateYear.ToString();
            var Ecom_Payment_Card_ExpDate_Month = DateHelper.FormatExpiryYear(request.Card.ExpiryDateMonth.ToString());

            var islemtipi = TransactionType.Auth.ToString();

            var oid = request.OrderId;

            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();

            var formParams = new Dictionary<string, string>
            {
                ["clientid"] = Configuration.ClientId,
                ["storekey"] = Configuration.StoreKey,
                ["storeType"] = PaymentStoreType.GetPaymentStoreType(request.StoreType),
                ["rnd"] = rnd,
                ["amount"] = amount,
                ["currency"] = currency,
                ["pan"] = pan,
                ["Ecom_Payment_Card_ExpDate_Year"] = Ecom_Payment_Card_ExpDate_Year,
                ["Ecom_Payment_Card_ExpDate_Month"] = Ecom_Payment_Card_ExpDate_Month,
                ["islemtipi"] = islemtipi,
                ["oid"] = oid,
                ["lang"] = lang,
                ["failUrl"] = request.FailURL,
                ["okUrl"] = request.OkURL
            };

            var hash = HashHelper.ComputeHash(formParams, Configuration.StoreKey);
            formParams["hash"] = hash;





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
