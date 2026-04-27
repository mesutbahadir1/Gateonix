using MediatR;
using Paygate.Application.Application.Payment.Commands;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Application.Shared;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;
using System.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Paygate.Application.Infrastructure.PaymentProviders
{
    public class HalkbankPaymentProvider : IPaymentProvider
    {
        public string Name => "Halkbank";

        public Guid BankId => Guid.NewGuid();

        public bool IsAvailable => true;
        public BankConfiguration Configuration = new BankConfiguration()
        {
            ApiEndpoint = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate",
            ClientId = "190100000",
            StoreKey = "123456"
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
            var rnd = Guid.NewGuid().ToString("N")[..20];
            var amount = request.Amount.ToString("0.00", CultureInfo.InvariantCulture);
            var currency = CurrencyHelper.GetCurrencyISOCode(request.Currency);
            var pan = request.Card.CardNumber.Replace("-", "").Replace(" ", "");
            var expiryMonth = request.Card.ExpiryDateMonth.PadLeft(2, '0');
            var expiryYear = request.Card.ExpiryDateYear.Length == 2
                ? request.Card.ExpiryDateYear
                : request.Card.ExpiryDateYear.Substring(request.Card.ExpiryDateYear.Length - 2);
            var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLowerInvariant();

            var formParams = new Dictionary<string, string>
            {
                ["clientid"] = Configuration.ClientId,
                ["storetype"] = PaymentStoreType.GetPaymentStoreType(request.StoreType),
                ["rnd"] = rnd,
                ["amount"] = amount,
                ["currency"] = currency,
                ["pan"] = pan,
                ["cv2"] = request.Card.CVV,
                ["Ecom_Payment_Card_ExpDate_Year"] = expiryYear,
                ["Ecom_Payment_Card_ExpDate_Month"] = expiryMonth,
                ["islemtipi"] = request.TransactionType.ToString(),
                ["oid"] = request.OrderId,
                ["lang"] = lang,
                ["failUrl"] = request.FailURL,
                ["okUrl"] = request.OkURL,
                ["countryName"] = "TR"
            };

            var hash =  HashHelper.ComputeHashForZiraat(formParams, Configuration.StoreKey);
            formParams["hash"] = hash;

            return new PaymentInitiateResponse
            {
                IsSuccess = true,
                OrderId = request.OrderId,
                ProviderName = Name,
                PaymentHtmlContent = BuildSubmitFormHtml(Configuration.ApiEndpoint, formParams),
                ResponseDate = DateTime.UtcNow
            };
        }

        private static string BuildSubmitFormHtml(string actionUrl, IReadOnlyDictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html><head><meta charset=\"utf-8\" /></head><body>");
            sb.AppendLine($"<form id=\"halkbank3dForm\" method=\"post\" action=\"{WebUtility.HtmlEncode(actionUrl)}\">");

            foreach (var field in fields)
            {
                sb.AppendLine($"<input type=\"hidden\" name=\"{WebUtility.HtmlEncode(field.Key)}\" value=\"{WebUtility.HtmlEncode(field.Value)}\" />");
            }

            sb.AppendLine("</form>");
            sb.AppendLine("<script>document.getElementById('halkbank3dForm').submit();</script>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
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
