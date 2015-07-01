using Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Core.DAL
{
    public class DBContext : DbContext
    {
        public DBContext()
            : base("DBContextPubSub")
        {

        }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


        }
    }
}