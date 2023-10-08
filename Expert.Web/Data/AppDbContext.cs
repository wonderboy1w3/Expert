using Expert.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace Expert.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Grade> Grades { get; set; }
}
