using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Paygate.Application.Application.Payment.Commands;
using Paygate.Application.Application.Payment.Dtos;
using Paygate.Application.Domain.Entities;
using Paygate.Application.Domain.Enums;
using Paygate.Application.Domain.Ports;

namespace Paygate.Application.Infrastructure.PaymentProviders;

public sealed class QnbPayPaymentProvider : IPaymentProvider
{
    private readonly QnbPayOptions _options;

    public QnbPayPaymentProvider(IOptions<QnbPayOptions> options)
    {
        _options = options.Value;
    }

    public string Name => "QNBpay";

    public Guid BankId => Guid.NewGuid();

    public bool IsAvailable => true;

    public PaymentInitiateResponse Initiate3DPayment(Initiate3DPaymentCommand request)
    {
        string token;
        try
        {
            token = GetAccessToken();
        }
        catch (Exception ex)
        {
            return Fail(request.OrderId, ex.Message);
        }

        var baseUrl = _options.TestPlatform ? _options.BaseUrlTest : _options.BaseUrlLive;
        var pan = request.Card.CardNumber.Replace("-", "").Replace(" ", "");
        var expiryMonth = request.Card.ExpiryDateMonth.PadLeft(2, '0');
        var expiryYear = NormalizeExpiryYear(request.Card.ExpiryDateYear);
        var holder = string.IsNullOrWhiteSpace(request.Card.CardHolderName)
            ? "Test Kart"
            : request.Card.CardHolderName.Trim();

        var totalStr = request.Amount.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")).Replace(".", "").Replace(",", ".");
        const string installmentStr = "1";
        var currencyCode = request.Currency.ToString();
        var invoiceId = request.OrderId;

        var req = new Dictionary<string, object>
        {
            ["cc_holder_name"] = holder,
            ["cc_no"] = pan,
            ["expiry_month"] = expiryMonth,
            ["expiry_year"] = expiryYear,
            ["cvv"] = request.Card.CVV,
            ["currency_code"] = currencyCode,
            ["installments_number"] = installmentStr,
            ["invoice_id"] = invoiceId,
            ["invoice_description"] = $"{invoiceId} nolu sipariş ödemesi",
            ["name"] = "[boş]",
            ["surname"] = "[boş]",
            ["total"] = totalStr,
            ["merchant_key"] = _options.MerchantKey,
            ["transaction_type"] = "Auth",
            ["items"] = new List<object>
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Tahsilat",
                    ["price"] = totalStr,
                    ["quantity"] = 1,
                    ["description"] = "Tahsilat"
                }
            },
            ["hash_key"] = "",
            ["response_method"] = "POST",
            ["payment_completed_by"] = "merchant",
            ["ip"] = "127.0.0.1",
            ["cancel_url"] = request.FailURL,
            ["return_url"] = request.OkURL
        };

        req["hash_key"] = QnbPaySigning.GenerateHashKeySale(
            totalStr,
            installmentStr,
            currencyCode,
            _options.MerchantKey,
            invoiceId,
            _options.AppSecret);

        var link = $"{baseUrl.TrimEnd('/')}/api/paySmart3D";
        string htmlOrBody;
        try
        {
            htmlOrBody = PostJson(link, req, token);
        }
        catch (Exception ex)
        {
            return Fail(request.OrderId, ex.Message);
        }

        if (string.IsNullOrWhiteSpace(htmlOrBody))
            return Fail(request.OrderId, "QNBpay boş yanıt döndü.");

        var trimmed = htmlOrBody.TrimStart();
        if (LooksLikeHtml(trimmed))
        {
            return new PaymentInitiateResponse
            {
                IsSuccess = true,
                OrderId = request.OrderId,
                ProviderName = Name,
                PaymentHtmlContent = htmlOrBody,
                ResponseDate = DateTime.UtcNow
            };
        }

        var errMsg = TryParseApiError(htmlOrBody);
        return Fail(request.OrderId, errMsg ?? htmlOrBody);
    }

    private static bool LooksLikeHtml(string s) =>
        s.StartsWith("<", StringComparison.Ordinal) ||
        s.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
        s.Contains("<html", StringComparison.OrdinalIgnoreCase);

    private static PaymentInitiateResponse Fail(string orderId, string message) =>
        new()
        {
            IsSuccess = false,
            OrderId = orderId,
            ProviderName = "QNBpay",
            PaymentHtmlContent =
                "<!doctype html><html><head><meta charset=\"utf-8\"/></head><body><pre>" +
                System.Net.WebUtility.HtmlEncode(message) +
                "</pre></body></html>",
            ResponseDate = DateTime.UtcNow
        };

    private static string? TryParseApiError(string body)
    {
        try
        {
            var jo = JObject.Parse(body);
            var desc = jo["status_description"]?.ToString();
            if (!string.IsNullOrEmpty(desc))
                return desc;
            return jo["message"]?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private string GetAccessToken()
    {
        if (string.IsNullOrWhiteSpace(_options.AppId) || string.IsNullOrWhiteSpace(_options.AppSecret))
            throw new InvalidOperationException("QnbPay AppId ve AppSecret yapılandırılmalıdır.");

        var baseUrl = _options.TestPlatform ? _options.BaseUrlTest : _options.BaseUrlLive;
        var link = $"{baseUrl.TrimEnd('/')}/api/token";
        var postData = new Dictionary<string, object>
        {
            ["app_id"] = _options.AppId,
            ["app_secret"] = _options.AppSecret
        };

        var loginResponse = PostJson(link, postData, bearerToken: null);
        JObject jo;
        try
        {
            jo = JObject.Parse(loginResponse);
        }
        catch (JsonReaderException)
        {
            throw new InvalidOperationException("QNBpay token yanıtı JSON değil: " + loginResponse[..Math.Min(200, loginResponse.Length)]);
        }
        var statusCode = jo["status_code"]?.ToString();
        if (statusCode == "100" && jo["data"] != null)
        {
            var token = jo["data"]?["token"]?.ToString();
            if (!string.IsNullOrWhiteSpace(token))
                return token;
        }

        var errorMsg = "QNBpay token error";
        var desc = jo["status_description"]?.ToString();
        if (!string.IsNullOrEmpty(desc))
            errorMsg += " - " + desc;
        throw new InvalidOperationException(errorMsg);
    }

    private static string PostJson(string link, Dictionary<string, object> param, string? bearerToken)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        var jsonContent = JsonConvert.SerializeObject(param);
        using var req = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        req.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = Encoding.UTF8.WebName };

        if (!string.IsNullOrEmpty(bearerToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var response = client.PostAsync(link, req).GetAwaiter().GetResult();
        var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        return Encoding.UTF8.GetString(bytes);
    }

    private static string NormalizeExpiryYear(string year)
    {
        var y = year.Trim();
        if (y.Length == 2)
            return $"20{y}";
        return y;
    }

    public Transaction CancelPayment(Guid transactionId) =>
        new()
        {
            Id = transactionId,
            BankId = BankId,
            CardId = Guid.NewGuid(),
            Amount = 0,
            Currency = "TRY",
            Status = PaymentStatus.Cancelled
        };

    public Transaction ConfirmPayment(Guid transactionId) =>
        new()
        {
            Id = transactionId,
            BankId = BankId,
            CardId = Guid.NewGuid(),
            Amount = 0,
            Currency = "TRY",
            Status = PaymentStatus.Completed
        };

    public Transaction GetPaymentStatus(Guid transactionId) =>
        new()
        {
            Id = transactionId,
            BankId = BankId,
            CardId = Guid.NewGuid(),
            Amount = 0,
            Currency = "TRY",
            Status = PaymentStatus.Completed
        };

    public Transaction ProcessPayment(Guid cardId, decimal amount, string currency) =>
        new()
        {
            Id = Guid.NewGuid(),
            BankId = BankId,
            CardId = cardId,
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Pending
        };

    public Transaction RefundPayment(Guid transactionId, decimal amount) =>
        new()
        {
            Id = transactionId,
            BankId = BankId,
            CardId = Guid.NewGuid(),
            Amount = amount,
            Currency = "TRY",
            Status = PaymentStatus.Refunded
        };
}

internal static class QnbPaySigning
{
    internal static string GenerateHashKeySale(string total, string installment, string currencyCode, string merchantKey,
        string invoiceId, string appSecret)
    {
        var text = total + "|" + installment + "|" + currencyCode + "|" + merchantKey + "|" + invoiceId;
        return GenerateHashKey(text, appSecret);
    }

    private static string GenerateHashKey(string text, string appSecret)
    {
        var mtRand = new Random();
        var iv = Sha1Hash(mtRand.Next().ToString()).Substring(0, 16);
        var password = Sha1Hash(appSecret);
        var salt = Sha1Hash(mtRand.Next().ToString()).Substring(0, 4);
        string saltWithPassword;
        using (var sha256Hash = SHA256.Create())
            saltWithPassword = GetHash(sha256Hash, password + salt);
        var encrypted = Encryptor(text, saltWithPassword.Substring(0, 32), iv);
        var msgEncryptedBundle = iv + ":" + salt + ":" + encrypted;
        return msgEncryptedBundle.Replace("/", "__");
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();
        for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));
        return sBuilder.ToString();
    }

    private static string Sha1Hash(string password) =>
        string.Join("", SHA1.HashData(Encoding.UTF8.GetBytes(password)).Select(b => b.ToString("x2")));

    private static string Encryptor(string textToEncrypt, string strKey, string strIV)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(textToEncrypt);
        using var aesProvider = Aes.Create();
        aesProvider.BlockSize = 128;
        aesProvider.KeySize = 256;
        aesProvider.Key = Encoding.UTF8.GetBytes(strKey);
        aesProvider.IV = Encoding.UTF8.GetBytes(strIV);
        aesProvider.Padding = PaddingMode.PKCS7;
        aesProvider.Mode = CipherMode.CBC;
        var cryptoTransform = aesProvider.CreateEncryptor(aesProvider.Key, aesProvider.IV);
        var encryptedBytes = cryptoTransform.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
        return Convert.ToBase64String(encryptedBytes);
    }
}
