using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProjectOneMil.Data;
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




        /*
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
                            // Column4 (DateTime) dönüştürme
                            double oaDate;
                            if (!double.TryParse(worksheet.Cells[row, 4].Text.Replace(',', '.'), out oaDate))
                            {
                                return BadRequest($"Invalid OADate value at row {row}, column 4: '{worksheet.Cells[row, 4].Text}'.");
                            }
                            DateTime column4 = DateTime.FromOADate(oaDate);

                            // Column3 ve Column9 (Decimal) dönüştürme
                            decimal column3;
                            if (!decimal.TryParse(worksheet.Cells[row, 3].Text.Replace(',', '.'), out column3))
                            {
                                return BadRequest($"Invalid decimal value at row {row}, column 3: '{worksheet.Cells[row, 3].Text}'.");
                            }
                            decimal column9;
                            if (!decimal.TryParse(worksheet.Cells[row, 9].Text.Replace(',', '.'), out column9))
                            {
                                return BadRequest($"Invalid decimal value at row {row}, column 9: '{worksheet.Cells[row, 9].Text}'.");
                            }

                            var myTable = new MyTable
                            {
                                Column1 = worksheet.Cells[row, 1].Text,
                                Column2 = int.Parse(worksheet.Cells[row, 2].Text),
                                Column3 = column3,
                                Column4 = column4,
                                Column5 = bool.Parse(worksheet.Cells[row, 5].Text.ToUpper() == "DOĞRU" ? "true" : "false"),
                                Column6 = worksheet.Cells[row, 6].Text,
                                Column7 = worksheet.Cells[row, 7].Text,
                                Column8 = int.Parse(worksheet.Cells[row, 8].Text),
                                Column9 = column9,
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

        private bool VerifyMyTableData(MyTable myTable)
        {
            // Burada veri doğrulama işlemlerini yapabilirsiniz
            // Örnek olarak:
            if (string.IsNullOrEmpty(myTable.Column1) || myTable.Column2 <= 0 || myTable.Column3 <= 0)
            {
                return false;
            }
            return true;
        }
        */
    }
}
