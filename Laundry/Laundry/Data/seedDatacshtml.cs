using Laundry.Models;
using Laundry.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Laundry.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LaundryContext(
                serviceProvider.GetRequiredService<DbContextOptions<LaundryContext>>()))
            {
                if (!context.User.Any())
                {
                    var adminUser = new User
                    {
                        Name = "Admin",
                        Email = "admin@example.com",
                        Password = "admin",
                        Role = "ADMIN"
                    };

                    context.User.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
    }
}
