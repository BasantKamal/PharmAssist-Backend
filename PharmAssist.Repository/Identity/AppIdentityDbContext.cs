using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities.Identity;


namespace PharmAssist.Repository.Identity
{
	public class AppIdentityDbContext: IdentityDbContext<AppUser>
	{
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> Options) : base(Options)
        {

        }
    }
}
