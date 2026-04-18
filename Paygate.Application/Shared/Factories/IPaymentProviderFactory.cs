using Paygate.Application.Domain.Ports;

namespace Paygate.Application.Shared.Factories;

public interface IPaymentProviderFactory
{
    IPaymentProvider GetProvider();
}