using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Member> Member { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<Book> Book { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
    }
}
