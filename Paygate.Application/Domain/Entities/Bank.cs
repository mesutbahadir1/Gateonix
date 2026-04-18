using Paygate.Application.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paygate.Application.Domain.Entities
{
    public class Bank : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public List<BankConfiguration> Configurations { get; set; }
    }
}
