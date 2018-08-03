using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleHostBuilderAndLogTest.Data
{
    public class PageAccessLogContext : DbContext
    {
        public DbSet<Models.PageAccessLog> PageAccessLogs { get; set; }

        public PageAccessLogContext(DbContextOptions<PageAccessLogContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.PageAccessLog>().ToTable("PageAccessLog");
        }
    }
}
