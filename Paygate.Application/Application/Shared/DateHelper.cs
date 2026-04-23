using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Application.Shared
{
    public static class DateHelper
    {
        public static string FormatExpiryYear(string year)
        {
            if (string.IsNullOrEmpty(year))
                throw new ArgumentException("Expiry year cannot be null or empty.");
            if (year.Length == 4)
                return year.Substring(2);
            return year;

        }
    }
}
