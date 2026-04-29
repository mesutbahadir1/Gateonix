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
        public static string ComputeHashForZiraat(Dictionary<string, string> formParams, string storeKey)
        {
            var plain = string.Concat(
                formParams["clientid"],
                formParams["oid"],
                formParams["amount"],
                formParams["okUrl"],
                formParams["failUrl"],
                formParams["rnd"],
                storeKey
            );

            using var sha1 = SHA1.Create();
            var bytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(plain);
            var hashBytes = sha1.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
        private static string EscapeForHash(string value)
        {
            if(string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("\\", "\\\\").Replace("|", "\\|");
        }
    }
}
