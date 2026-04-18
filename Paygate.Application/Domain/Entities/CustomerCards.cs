using Paygate.Application.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Entities
{
    public class CustomerCards : ValueObject
    {
        public Guid CustomerId { get; set; }
        public Guid CardId { get; set; }
    }
}
