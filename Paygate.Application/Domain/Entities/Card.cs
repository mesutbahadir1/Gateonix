using Paygate.Application.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Entities
{
    public class Card : Entity, IAggregateRoot
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDateMonth { get; set; }
        public string ExpiryDateYear { get; set; }
        public string CVV { get; set; }
    }
}
