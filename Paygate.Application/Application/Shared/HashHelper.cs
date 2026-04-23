using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Application.Shared
{
    public static class HashHelper
    {
        public static string ComputeHash(Dictionary<string,string> formParams, string storeKey)
        {
            var orderedValues = formParams
                                .Where(x => !string.Equals(x.Key, "encoding",StringComparison.OrdinalIgnoreCase) && 
                                            !string.Equals(x.Key, "hash",StringComparison.OrdinalIgnoreCase))
                                .OrderBy(x => x.Key, StringComparer.Ordinal)
                                .Select(x => EscapeForHash(x.Value))
                                .ToList();

            var hashString = string.Join("|", orderedValues) + "|" + EscapeForHash(storeKey);

            using var sha512 = SHA512.Create();
            var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            var hashValue = Convert.ToBase64String(hashBytes);

            return hashValue;



        }
        private static string EscapeForHash(string value)
        {
            if(string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("\\", "\\\\").Replace("|", "\\|");
        }
    }
}
