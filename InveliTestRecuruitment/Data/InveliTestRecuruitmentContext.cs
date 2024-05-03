using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InveliTestRecuruitment.Models;

namespace InveliTestRecuruitment.Data
{
    public class InveliTestRecuruitmentContext : DbContext
    {
        public InveliTestRecuruitmentContext (DbContextOptions<InveliTestRecuruitmentContext> options)
            : base(options)
        {
        }

        public DbSet<InveliTestRecuruitment.Models.SupplierModel> SupplierModel { get; set; } = default!;
        public DbSet<InveliTestRecuruitment.Models.ProductModel> ProductModel { get; set; } = default!;
        public DbSet<InveliTestRecuruitment.Models.PurchaseOrderModel> PurchaseOrderModel { get; set; } = default!;
        public DbSet<InveliTestRecuruitment.Models.PurchaseOrderDetail> PurchaseOrderDetail { get; set; } = default!;
    }
}
