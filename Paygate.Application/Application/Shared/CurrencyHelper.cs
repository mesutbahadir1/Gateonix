using Paygate.Application.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Application.Shared
{
    public static class CurrencyHelper
    {
        public static string GetCurrencySymbol(Currency currency)
        {
            return currency switch
            {
                Currency.TRY => "₺",
                Currency.EUR => "€",
                Currency.USD => "$",
                _ => throw new ArgumentException($"Unsupported currency code: {currency.ToString()}")
            };
        }

        public static string GetCurrencyISOCode(Currency currency)
        {
            return currency switch
            {
                Currency.TRY => "949",
                Currency.USD => "840",
                Currency.EUR => "978",
            };
        }
    }
}
