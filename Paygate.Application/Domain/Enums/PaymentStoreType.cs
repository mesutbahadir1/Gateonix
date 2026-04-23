using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Enums
{
    public static class PaymentStoreType
    {
        public static string GetPaymentStoreType(PaymentStoreTypeEnum type)
        {
            return type switch
            {
                PaymentStoreTypeEnum.ThreeD => ThreeD,
                PaymentStoreTypeEnum.ThreeDPay => ThreeDPay,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected payment store type value: {type}"),
            };
        }
        public const string ThreeD = "3d";
        public const string ThreeDPay = "3d_pay";
    }
    public enum PaymentStoreTypeEnum
    {
        ThreeD,
        ThreeDPay
    }
}
