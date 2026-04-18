using Paygate.Application.Shared.Base;

namespace Paygate.Application.Domain.Entities
{
    public class Customer : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<CustomerAddress> Addresses { get; set; }
        public List<CustomerCards> Cards { get; set; }
    }
}
