namespace Paygate.Application.Payment.Dtos;

public class PaymentResponse
{
    public Guid TransactionId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
}