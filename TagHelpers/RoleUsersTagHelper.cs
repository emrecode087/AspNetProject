using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ProjectOneMil.Models;

namespace ProjectOneMil.TagHelpers
{
	[HtmlTargetElement("td", Attributes = "asp-role-users")]
	public class RoleUsersTagHelper : TagHelper
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		[HtmlAttributeName("asp-role-users")]
		public string RoleId { get; set; } = null!;

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
					output.Content.SetHtmlContent(userNames.Count == 0 ? "No users" : setHtml(userNames));
				}
			}
			catch (Exception ex)
			{
				output.Content.SetContent($"An error occurred: {ex.Message}");
			}
		}

		private string setHtml(List<string> userNames)
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
