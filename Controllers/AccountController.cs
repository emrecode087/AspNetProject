using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOneMil.Models;
using ProjectOneMil.ViewModels;

namespace ProjectOneMil.Controllers
{
	/// <summary>
	/// The AccountController class handles user account-related actions such as login, registration, email confirmation, and logout.
	/// </summary>
	public class AccountController : Controller
	{
		private UserManager<AppUser> _userManager;
		private RoleManager<AppRole> _roleManager;
		private SignInManager<AppUser> _signInManager;
		private IEmailSender _emailSender;

		/// <summary>
		/// Initializes a new instance of the AccountController class.
		/// </summary>
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

		/// <summary>
		/// Displays the login view.
		/// </summary>
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		/// <summary>
		/// Displays the user registration view.
		/// </summary>
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// Handles user registration.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Create(CreateViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Check if the email is already registered
				var existingUser = await _userManager.FindByEmailAsync(model.Email);
				if (existingUser != null)
				{
					ModelState.AddModelError("", "This email is already registered.");
					return View(model);
				}

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

					await _emailSender.SendEmailAsync(user.Email,
						"Confirm your email", $"Please confirm your account by clicking this link: <a href='http://localhost:5264{url}'>link</a>");

					TempData["message"] = "Confirm your e-mail";
					return RedirectToAction("Login", "Account");
				}

				foreach (IdentityError err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}
			return View(model);
		}

		/// <summary>
		/// Confirms the user's email address.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string Id, string token)
		{
			if (Id == null || token == null)
			{
				TempData["message"] = "Invalid token";
				return View();
			}

			var user = await _userManager.FindByIdAsync(Id);

			if (user != null)
			{
				var result = await _userManager.ConfirmEmailAsync(user, token);

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

		/// <summary>
		/// Handles user login.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					await _signInManager.SignOutAsync();

					if (!await _userManager.IsEmailConfirmedAsync(user))
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
						var timeLeft = lockoutDate.Value - DateTimeOffset.UtcNow;
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

		/// <summary>
		/// Handles user logout.
		/// </summary>
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}

		/// <summary>
		/// Displays the access denied view.
		/// </summary>
		public IActionResult AccessDenied()
		{
			return View("Login");
		}
	}
}
