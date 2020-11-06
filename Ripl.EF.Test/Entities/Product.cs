using Ripl.EF.Contracts;

namespace Ripl.EF.Tests.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int QuantityInStock { get; set; }
    }
}
