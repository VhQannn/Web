using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Data;

public class WebIdentityContext : IdentityDbContext<User>
{
    public WebIdentityContext(DbContextOptions<WebIdentityContext> options)
        : base(options)
    {
    }
    public WebIdentityContext()
    {
        
    }
    protected WebIdentityContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
