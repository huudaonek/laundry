using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Laundry.Models;

namespace Laundry.Data
{
    public class LaundryContext : DbContext
    {
        public LaundryContext(DbContextOptions<LaundryContext> options)
            : base(options)
        {
        }

        public DbSet<LaundryItem> LaundryItems { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

        public List<OrderDetail> GetAllOrderDetails()
        {
            return OrderDetail.ToList();
        }
	}
}
