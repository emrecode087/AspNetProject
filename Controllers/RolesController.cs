using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOneMil.Models;

namespace ProjectOneMil.Controllers
{
	public class RolesController : Controller
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		public RolesController(
			RoleManager<AppRole> roleManager,
			UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			return View(roles);
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

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
