using Ripl.EF.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ripl.EF.Tests.Entities
{
    public class OrderItem : BaseEntity
    {

        [ForeignKey(nameof(Order))]
        public long OrderId { get; set; }
        [ForeignKey(nameof(Product))]
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
