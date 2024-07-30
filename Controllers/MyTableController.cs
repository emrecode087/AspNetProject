using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace ProjectOneMil.Controllers
{
    [Authorize(Roles = "admin,manager")]
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var myTable = await _context.onemildata.FindAsync(id);
            if (myTable == null)
            {
                return NotFound();
            }
            return View(myTable);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MyTable myTable)
        {
            if (id != myTable.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.onemildata.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(myTable);
        }

        [HttpPost]
        public JsonResult GetMyTableData()
        {
            var data = _context.onemildata.AsQueryable();

            var draw = Request.Form["draw"].FirstOrDefault();
            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var orderDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var orderColumnName = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int recordTotal = data.Count();
            int recordsFiltered = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.Column1.ToLower().Contains(searchValue.ToLower()) ||
                                       x.Column6.ToLower().Contains(searchValue.ToLower()) ||
                                       x.Column7.ToLower().Contains(searchValue.ToLower()));
                recordsFiltered = data.Count();
            }

            if (!string.IsNullOrEmpty(orderColumnName) && !string.IsNullOrEmpty(orderDir))
            {
                data = data.OrderBy($"{orderColumnName} {(orderDir == "asc" ? "ascending" : "descending")}");
            }

            var myTableData = data.Skip(start).Take(length).ToList();

            var result = new
            {
                draw = draw,
                recordsTotal = recordTotal,
                recordsFiltered = recordsFiltered,
                data = myTableData
            };

            return Json(result);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var myTable = _context.onemildata.Find(id);
            if (myTable == null)
            {
                return NotFound();
            }

            _context.onemildata.Remove(myTable);
            _context.SaveChanges();
            return Ok();
        }
    }
}
