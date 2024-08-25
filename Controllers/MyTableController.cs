using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProjectOneMil.Data;
using System.Linq.Dynamic.Core;
using System.Text;
using ExcelDataReader;

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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Veritabanından verileri ID'ye göre sıralayarak çekiyoruz
            var data = await _context.onemildata.OrderBy(d => d.Id).ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("MyTableData");

                // Header row
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Column1";
                worksheet.Cells[1, 3].Value = "Column2";
                worksheet.Cells[1, 4].Value = "Column3";
                worksheet.Cells[1, 5].Value = "Column4";
                worksheet.Cells[1, 6].Value = "Column5";
                worksheet.Cells[1, 7].Value = "Column6";
                worksheet.Cells[1, 8].Value = "Column7";
                worksheet.Cells[1, 9].Value = "Column8";
                worksheet.Cells[1, 10].Value = "Column9";
                worksheet.Cells[1, 11].Value = "Column10";

                // Data rows
                for (int i = 0; i < data.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cells[row, 1].Value = data[i].Id;
                    worksheet.Cells[row, 2].Value = data[i].Column1;
                    worksheet.Cells[row, 3].Value = data[i].Column2;
                    worksheet.Cells[row, 4].Value = data[i].Column3;
                    worksheet.Cells[row, 5].Value = data[i].Column4.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"); // Tarih formatı
                    worksheet.Cells[row, 6].Value = data[i].Column5 ? "DOĞRU" : "YANLIŞ"; // Boolean formatı
                    worksheet.Cells[row, 7].Value = data[i].Column6;
                    worksheet.Cells[row, 8].Value = data[i].Column7;
                    worksheet.Cells[row, 9].Value = data[i].Column8;
                    worksheet.Cells[row, 10].Value = data[i].Column9;
                    worksheet.Cells[row, 11].Value = data[i].Column10;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"MyTableData-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    bool isHeaderSkipped = false;
                    while (reader.Read())
                    {
                        if (!isHeaderSkipped)
                        {
                            isHeaderSkipped = true;
                            continue;
                        }

                        try
                        {
                            var myTable = new MyTable
                            {
                                Column1 = reader.GetValue(1)?.ToString() ?? string.Empty,
                                Column2 = int.TryParse(reader.GetValue(2)?.ToString(), out int col2) ? col2 : 0,
                                Column3 = decimal.TryParse(reader.GetValue(3)?.ToString(), out decimal col3) ? col3 : 0,
                                Column4 = DateTime.TryParse(reader.GetValue(4)?.ToString(), out DateTime col4) ? col4 : DateTime.MinValue,
                                Column5 = reader.GetValue(5)?.ToString().ToLower() switch
                                {
                                    "doğru" => true,
                                    "true" => true,
                                    "yanlış" => false,
                                    "false" => false,
                                    _ => false
                                },
                                Column6 = reader.GetValue(6)?.ToString() ?? string.Empty,
                                Column7 = reader.GetValue(7)?.ToString() ?? string.Empty,
                                Column8 = int.TryParse(reader.GetValue(8)?.ToString(), out int col8) ? col8 : 0,
                                Column9 = decimal.TryParse(reader.GetValue(9)?.ToString(), out decimal col9) ? col9 : 0,
                                Column10 = Guid.TryParse(reader.GetValue(10)?.ToString(), out Guid col10) ? col10 : Guid.Empty
                            };

                            _context.onemildata.Add(myTable);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            // Hata loglama
                            Console.WriteLine($"Error parsing row: {ex.Message}");
                            return StatusCode(500, $"Error parsing row: {ex.Message}");
                        }
                    }
                }
            }

            return Ok("File imported successfully.");
        }


    }
}
