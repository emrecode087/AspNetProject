using Microsoft.AspNetCore.Identity;

namespace ProjectOneMil.Models
{
	public class AppUser : IdentityUser
	{ 
		public string FullName { get; set; } = String.Empty;
			
	}
}
