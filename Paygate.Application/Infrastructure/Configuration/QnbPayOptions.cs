namespace Paygate.Application.Infrastructure.PaymentProviders;

public sealed class QnbPayOptions
{
    public string MerchantId { get; set; } = "";
    public string AppId { get; set; } = "";
    public string AppSecret { get; set; } = "";
    public string MerchantKey { get; set; } = "";
    public bool TestPlatform { get; set; } = true;
    public string BaseUrlTest { get; set; } = "https://test.qnbpay.com.tr/ccpayment";
    public string BaseUrlLive { get; set; } = "https://portal.qnbpay.com.tr/ccpayment";
}
