using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Models;

namespace ProjectOneMil.Data
{
    public class IdentityContext : IdentityDbContext<AppUser,AppRole,string >

    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
            
        }

    }
}
