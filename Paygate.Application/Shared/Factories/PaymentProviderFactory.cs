using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Infrastructure.PaymentProviders;
using Paygate.Application.Shared.Factories;

public class PaymentProviderFactory : IPaymentProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public PaymentProviderFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public IPaymentProvider GetProvider()
    {
        var providerName = _configuration["PaymentProvider"];

        return _serviceProvider.GetRequiredKeyedService<IPaymentProvider>(providerName);
    }
}