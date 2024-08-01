using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOneMil.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using OfficeOpenXml;
using System.IO;

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

        [HttpGet]
        public async Task<IActionResult> Download()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Lisans bağlamını ayarlayın

            var data = await _context.onemildata.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("MyTableData");
                worksheet.Cells.LoadFromCollection(data, true);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"MyTableData-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return BadRequest("No worksheet found in the Excel file.");
                        }

                        var rowCount = worksheet.Dimension.Rows;
                        var myTableList = new List<MyTable>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var myTable = new MyTable
                            {
                                Column1 = worksheet.Cells[row, 1].Text,
                                Column2 = int.Parse(worksheet.Cells[row, 2].Text),
                                Column3 = decimal.Parse(worksheet.Cells[row, 3].Text),
                                Column4 = DateTime.Parse(worksheet.Cells[row, 4].Text),
                                Column5 = bool.Parse(worksheet.Cells[row, 5].Text),
                                Column6 = worksheet.Cells[row, 6].Text,
                                Column7 = worksheet.Cells[row, 7].Text,
                                Column8 = int.Parse(worksheet.Cells[row, 8].Text),
                                Column9 = decimal.Parse(worksheet.Cells[row, 9].Text),
                                Column10 = Guid.Parse(worksheet.Cells[row, 10].Text)
                            };
                            myTableList.Add(myTable);
                        }

                        _context.onemildata.AddRange(myTableList);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while processing the file: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }
}
