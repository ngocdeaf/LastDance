using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ngocdeaf.Models;

namespace Ngocdeaf.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ngocdeaf.Models.Genre>? Genre { get; set; }
        public DbSet<Ngocdeaf.Models.Book>? Book { get; set; }
        public DbSet<Ngocdeaf.Models.Cart>? Cart { get; set; }
        public DbSet<Ngocdeaf.Models.Order>? Order { get; set; }
    }
}