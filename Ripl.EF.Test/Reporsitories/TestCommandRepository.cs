using Ripl.EF.Tests.DataContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ripl.EF.Tests.Reporsitories
{
    public class TestCommandRepository : CommandRepository<StoreContext>
    {
        public TestCommandRepository(StoreContext dataContext)
            : base(dataContext)
        {
        }
    }
}