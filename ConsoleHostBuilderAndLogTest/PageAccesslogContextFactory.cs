using ConsoleHostBuilderAndLogTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleHostBuilderAndLogTest
{
    public class PageAccesslogContextFactory : IDesignTimeDbContextFactory<PageAccessLogContext>
    {
        public PageAccessLogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PageAccessLogContext>();
            optionsBuilder.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = SIS; user id = sa; password = 1q2w3e4r; MultipleActiveResultSets = true");

            return new PageAccessLogContext(optionsBuilder.Options);
        }
    }
}
