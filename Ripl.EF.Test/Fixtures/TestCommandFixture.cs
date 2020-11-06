using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Ripl.EF.Contracts;
using Ripl.EF.Tests.DataContext;
using Ripl.EF.Tests.Reporsitories;
using System;

namespace Ripl.EF.Tests.Fixtures
{
    public class TestCommandFixture
    {
        public DbContextOptions<StoreContext> ContextOptions { get; private set; }
        public TestCommandFixture()
        {
            ContextOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: "TestStoreDb")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }
    }
}
