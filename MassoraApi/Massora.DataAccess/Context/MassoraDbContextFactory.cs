using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massora.DataAccess.Context
{
    public class MassoraDbContextFactory : IDesignTimeDbContextFactory<MassoraDbContext>
    {

        public MassoraDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MassoraDbContext>();

            // Buraya kendi connection string'ini koy:
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MassoraDb;Trusted_Connection=true;MultipleActiveResultSets=true");

            return new MassoraDbContext(optionsBuilder.Options);
        }
    }
}
