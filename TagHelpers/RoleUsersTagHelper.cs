using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ProjectOneMil.Models;

namespace ProjectOneMil.TagHelpers
{
	/// <summary>
	/// The RoleUsersTagHelper class is a custom TagHelper that displays the users associated with a specific role.
	/// </summary>
	[HtmlTargetElement("td", Attributes = "asp-role-users")]
	public class RoleUsersTagHelper : TagHelper
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of the RoleUsersTagHelper class with the specified role and user managers.
		/// </summary>
		public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		/// <summary>
		/// Gets or sets the role ID for which the users will be displayed.
		/// </summary>
		[HtmlAttributeName("asp-role-users")]
		public string RoleId { get; set; } = null!;

		/// <summary>
		/// Processes the tag helper to display the users associated with the specified role.
		/// </summary>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			try
			{
				var userNames = new List<string>();
				var role = await _roleManager.FindByIdAsync(RoleId);

				if (role != null && role.Name != null)
				{
					var users = _userManager.Users.ToList();
					foreach (var user in users)
					{
						if (await _userManager.IsInRoleAsync(user, role.Name))
						{
							userNames.Add(user.UserName ?? "");
						}
					}
					output.Content.SetHtmlContent(userNames.Count == 0 ? "No users" : SetHtml(userNames));
				}
			}
			catch (Exception ex)
			{
				output.Content.SetContent($"An error occurred: {ex.Message}");
			}
		}

		/// <summary>
		/// Generates the HTML content for the list of user names.
		/// </summary>
		private string SetHtml(List<string> userNames)
		{
			var html = "<ul>";
			foreach (var item in userNames)
			{
				html += $"<li>{item}</li>";
			}
			html += "</ul>"; // Add closing tag for <ul>
			return html;
		}
	}
}
