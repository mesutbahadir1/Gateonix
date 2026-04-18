using MediatR;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Paygate.Application.Domain.Ports;
using Paygate.Application.Infrastructure.PaymentProviders;
using Paygate.Application.Shared.Factories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/paygate-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "payment-logs-{0:yyyy.MM.dd}",
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
    })
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(typeof(Program).Assembly, typeof(IPaymentProvider).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKeyedScoped<IPaymentProvider, HalkbankPaymentProvider>("Halkbank");
builder.Services.AddKeyedScoped<IPaymentProvider, AkbankPaymentProvider>("Akbank");
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
