using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectOneMil.Controllers
{
    public class MyTableController : Controller
    {
        private readonly AppDbContext _context;

        public MyTableController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var myTableList = await _context.onemildata.Take(10).ToListAsync();

            // Veri çekildiğini kontrol etmek için
            if (myTableList != null && myTableList.Any())
            {
                Console.WriteLine("Veriler çekildi.");
                foreach (var item in myTableList)
                {
                    Console.WriteLine($"{item.Id} - {item.Column1}");
                }
            }
            else
            {
                Console.WriteLine("Veritabanında veri bulunamadı.");
            }

            return View(myTableList);
        }

        // Yeni veri eklemek için formu gösterir
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Yeni veri eklemek için formdan gelen verileri işler
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
