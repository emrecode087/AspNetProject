using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Models;

namespace ProjectOneMil.Data
{
	/// <summary>
	/// The IdentityContext class is responsible for configuring the Entity Framework Core context for identity management.
	/// </summary>
	public class IdentityContext : IdentityDbContext<AppUser, AppRole, string>
	{
		/// <summary>
		/// Initializes a new instance of the IdentityContext class with the specified options.
		/// </summary>
		/// <param name="options">The options to be used by the DbContext.</param>
		public IdentityContext(DbContextOptions<IdentityContext> options)
			: base(options)
		{
		}
	}
}
