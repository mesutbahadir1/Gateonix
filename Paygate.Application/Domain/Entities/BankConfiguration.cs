using Paygate.Application.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Entities
{
    public class BankConfiguration : Entity
    {
        public Guid BankId { get; set; }
        public string ApiEndpoint { get; set; }
        public string ClientId { get; set; }
        public string StoreKey { get; set; }
        
    }
}
