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
		private IEmailSender _emailSender;

		public AccountController(
            UserManager<AppUser> userManager,
			RoleManager<AppRole> roleManager,
			SignInManager<AppUser> signInManager,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
		}

		[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new AppUser
				{
					UserName = model.UserName,
					Email = model.Email,
					FullName = model.FullName
				};

				IdentityResult result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

					//email
					await _emailSender.SendEmailAsync(user.Email, 
						"Confirm your email", $"Please confirm your account by clicking this link: <a href='http://localhost:5264{url}'>link</a>");

					TempData["message"] = "Confirm your e-mail";
					return RedirectToAction("Login","Account");
				}

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}

			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string Id,string token)
		{
			if(Id == null || token == null)
			{
				TempData["message"] = "Invalid token";
				return View(); 
			}

			var user = _userManager.FindByIdAsync(Id).Result;

			if (user != null)
			{
				var result = _userManager.ConfirmEmailAsync(user, token).Result;

				if (result.Succeeded)
				{
					TempData["message"] = "Email confirmed";
					return RedirectToAction("Login", "Account");
				}
				else
				{
					TempData["message"] = "Email could not be confirmed";
				}
			}
			TempData["message"] = "User not found";
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

                    if(!await _userManager.IsEmailConfirmedAsync(user))
					{
						ModelState.AddModelError("", "Please confirm your email address.");
						return View(model);
					}

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

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}

		public IActionResult AccessDenied()
		{
			   return View();
		}
    }
}
