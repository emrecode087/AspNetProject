using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOneMil.Models;

namespace ProjectOneMil.Controllers
{
	/// <summary>
	/// The RolesController class handles actions related to user roles such as creating, editing, and deleting roles.
	/// </summary>
	public class RolesController : Controller
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		/// <summary>
		/// Initializes a new instance of the RolesController class.
		/// </summary>
		public RolesController(
			RoleManager<AppRole> roleManager,
			UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		/// <summary>
		/// Displays the list of roles.
		/// </summary>
		[HttpGet]
		public IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			return View(roles);
		}

		/// <summary>
		/// Displays the view for creating a new role.
		/// </summary>
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Handles the creation of a new role.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> CreateAsync(AppRole model)
		{
			if (ModelState.IsValid)
			{
				var result = await _roleManager.CreateAsync(model);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(model);
		}

		/// <summary>
		/// Displays the view for editing an existing role.
		/// </summary>
		public async Task<IActionResult> Edit(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);

			if (role != null && role.Name != null)
			{
				ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
				return View(role);
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// Handles the editing of an existing role.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Edit(AppRole model)
		{
			if (ModelState.IsValid)
			{
				var role = await _roleManager.FindByIdAsync(model.Id);
				if (role != null)
				{
					role.Name = model.Name;
					var result = await _roleManager.UpdateAsync(role);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					foreach (IdentityError err in result.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}
					if (role.Name != null)
					{
						ViewBag.Users = await _userManager.GetUsersInRoleAsync(role.Name);
					}
				}
			}
			return View(model);
		}

		/// <summary>
		/// Handles the deletion of a role.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role != null)
			{
				var result = await _roleManager.DeleteAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return RedirectToAction("Index");
		}
	}

}
