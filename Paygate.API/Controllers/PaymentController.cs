using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paygate.Application.Application.Payment.Commands;
using Paygate.Application.Application.Payment.Queries;
using Paygate.Application.Payment.Commands;

namespace Paygate.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{transactionId}/status")]
    public async Task<IActionResult> GetPaymentStatus([FromRoute] Guid transactionId)
    {
        var query = new GetPaymentStatusQuery { TransactionId = transactionId };
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("{transactionId}/cancel")]
    public async Task<IActionResult> CancelPayment([FromRoute] Guid transactionId)
    {
        var command = new CancelPaymentCommand() { TransactionId = transactionId };
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("{transactionId}/confirm")]
    public async Task<IActionResult> ConfirmPayment([FromRoute] Guid transactionId)
    {
        var command = new ConfirmPaymentCommand() { TransactionId = transactionId };
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("refund")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    [HttpGet("ok")]
    public async Task<IActionResult> Initiate3DPayment([FromBody] Dictionary<string,string> command)
    {
        return Ok(await _mediator.Send(command));
    }
    [HttpGet("fail")]
    public async Task<IActionResult> Initiate3DPayment2([FromBody] Dictionary<string, string> command)
    {
        return Ok(await _mediator.Send(command));
    }
    [HttpPost("ok")]
    public async Task<IActionResult> Initiate3DPaymentPost([FromBody] Dictionary<string, string> command)
    {
        return Ok(await _mediator.Send(command));
    }
    [HttpPost("fail")]
    public async Task<IActionResult> Initiate3DPaymentPost2([FromBody] Dictionary<string, string> command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("3d/initiate")]
    public async Task<IActionResult> Initiate3DPayment([FromBody] Initiate3DPaymentCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}