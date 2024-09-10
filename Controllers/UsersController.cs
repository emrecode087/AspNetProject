using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Models;
using ProjectOneMil.ViewModels;

namespace ProjectOneMil.Controllers
{
	/// <summary>
	/// The UsersController class handles actions related to user management such as listing, editing, and deleting users.
	/// </summary>
	[Authorize(Roles = "admin")]
	public class UsersController : Controller
	{
		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;

		/// <summary>
		/// Initializes a new instance of the UsersController class.
		/// </summary>
		public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		/// <summary>
		/// Displays the list of users.
		/// </summary>
		public IActionResult Index()
		{
			return View(_userManager.Users);
		}

		/// <summary>
		/// Displays the view for editing an existing user.
		/// </summary>
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}
			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
				return View(new EditViewModel
				{
					Id = user.Id,
					FullName = user.FullName,
					Email = user.Email,
					SelectedRoles = await _userManager.GetRolesAsync(user)
				});
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Handles the editing of an existing user.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Edit(string id, EditViewModel model)
		{
			if (id != model.Id)
			{
				return RedirectToAction("Index");
			}

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(id);

				if (user != null)
				{
					user.FullName = model.FullName;
					user.Email = model.Email;

					var result = await _userManager.UpdateAsync(user);

					if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
					{
						await _userManager.RemovePasswordAsync(user);
						await _userManager.AddPasswordAsync(user, model.Password);
					}
					if (result.Succeeded)
					{
						await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

						if (model.SelectedRoles != null)
						{
							await _userManager.AddToRolesAsync(user, model.SelectedRoles);
						}
						return RedirectToAction("Index");
					}

					foreach (IdentityError err in result.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}
				}
				return RedirectToAction("Index");
			}

			return View(model);
		}

		/// <summary>
		/// Handles the deletion of a user.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					ModelState.AddModelError("", "User could not be deleted");
				}
			}
			else
			{
				ModelState.AddModelError("", "User not found");
			}
			return View("Index", _userManager.Users);
		}
	}
}
