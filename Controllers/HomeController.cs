using Microsoft.AspNetCore.Mvc;

namespace ProjectOneMil.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

         
    }
}
