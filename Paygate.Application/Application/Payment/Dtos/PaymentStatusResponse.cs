namespace Paygate.Application.Payment.Dtos;

public class PaymentStatusResponse
{
    public Guid TransactionId { get; set; }
    public string Status { get; set; } = default!;
}