using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using EBuy.Models;

namespace EBuy.Repository
{
    public class EBuyContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<GoodModel> Goods { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<OrderModel> Order { get; set; }

        public EBuyContext()
            : base("DefaultConnection")
        {
            Database.CreateIfNotExists();
        }
    }
}