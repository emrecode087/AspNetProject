using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectOneMil.Controllers
{
	/// <summary>
	/// The HomeController class handles the main actions for the home page of the application.
	/// </summary>
	[Authorize]
	public class HomeController : Controller
	{
		/// <summary>
		/// Displays the home page view.
		/// </summary>
		public IActionResult Index()
		{
			return View();
		}
	}
}
