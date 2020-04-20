using Microsoft.EntityFrameworkCore;
using Ripl.EF.Tests.DataContext;
using Ripl.EF.Tests.Entities;
using Ripl.EF.Tests.Fixtures;
using Ripl.EF.Tests.Reporsitories;
using Xunit;

namespace Ripl.EF.Tests
{
    public class CommandTests : IClassFixture<TestCommandFixture>
    {
        private readonly TestCommandFixture _testCommandFixture;
        private readonly DbContextOptions<StoreContext> _options;
        public CommandTests(TestCommandFixture commandFixture)
        {
            _testCommandFixture = commandFixture;
            _options = commandFixture.ContextOptions;
        }

        [Fact]
        public void ShouldAddRecord()
        {
            using (var context = new StoreContext(_options))
            {
                var repository = new TestCommandRepository(context);
                repository.AddEntity<Product>(new Product { Code = "A00001", Name = "Product A", QuantityInStock = 100 });
                _ = repository.Save().Result;
            }

            using (var context = new StoreContext(_options))
            {
                var count = context.Products.CountAsync().Result;
                Assert.Equal(1, count);
                Assert.Equal("A00001", context.Products.SingleAsync().Result.Code);
            }
        }
    }
}