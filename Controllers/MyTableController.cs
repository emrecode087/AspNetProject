using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Data;
using System.Threading.Tasks;

namespace ProjectOneMil.Controllers
{
    [Authorize(Roles = "admin")]
    public class MyTableController : Controller
    {
        private readonly AppDbContext _context;

        public MyTableController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetMyTableData()
        {
            using (_context)
            {
                var _data = await _context.onemildata.ToListAsync();
                return Json(new { data = _data });
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyTable myTable)
        {
            if (ModelState.IsValid)
            {
                _context.onemildata.Add(myTable);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(myTable);
        }
    }
}
