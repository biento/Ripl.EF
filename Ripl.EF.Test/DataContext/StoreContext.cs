using Microsoft.EntityFrameworkCore;
using Ripl.EF.Tests.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ripl.EF.Tests.DataContext
{
    public class StoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public StoreContext()
        {
        }

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }
    }
}
