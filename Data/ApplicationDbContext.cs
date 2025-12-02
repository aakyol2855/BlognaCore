using Microsoft.EntityFrameworkCore;
using BlognaCorev2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BlognaCorev2.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    
    // Post tablosu (Identity tablolarının yanında varlığını sürdürüyor)
    public DbSet<Post> Posts { get; set; } = default!;
}