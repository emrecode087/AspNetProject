using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOneMil.Models;
using ProjectOneMil.ViewModels;

namespace ProjectOneMil.Controllers
{
    public class AccountController: Controller
    {

		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;

		public AccountController(
            UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager ,
            SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
            _signInManager = signInManager;
		}

		[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
				
                if(user != null)
				{
					await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut) 
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutDate.Value-DateTimeOffset.UtcNow;
                        ModelState.AddModelError("", $"Your account is locked out. Please try again in {timeLeft.Minutes} minutes.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt");
                    }
				}
                else
				{
					ModelState.AddModelError("", "Invalid login attempt");
				}
            }
			return View();
		}

		public IActionResult Register()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }


    }
}
