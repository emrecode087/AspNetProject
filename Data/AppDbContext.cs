using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ProjectOneMil.Data
{
	/// <summary>
	/// The AppDbContext class is responsible for configuring the Entity Framework Core context for the application.
	/// </summary>
	public class AppDbContext : DbContext
	{
		/// <summary>
		/// Gets the configuration settings for the application.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Initializes a new instance of the AppDbContext class with the specified configuration.
		/// </summary>
		/// <param name="configuration">The configuration settings for the application.</param>
		public AppDbContext(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Configures the database to be used for this context.
		/// </summary>
		/// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
		}

		/// <summary>
		/// Gets or sets the DbSet for the MyTable entity.
		/// </summary>
		public DbSet<MyTable> onemildata { get; set; }

		/// <summary>
		/// Configures the model that was discovered by convention from the entity types exposed in DbSet properties on your derived context.
		/// </summary>
		/// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MyTable>().Property(e => e.Column4)
				.HasConversion(
					v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
					v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
				);
		}
	}
}
