using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;
using System.Text;

namespace PatientManager.Controllers
{
    public class MedicalFilesController : Controller
    {
        private readonly PostgresContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MedicalFilesController> _logger;

        public MedicalFilesController(PostgresContext context, IWebHostEnvironment env, ILogger<MedicalFilesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // GET: MedicalFiles
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.ToListAsync();
            return View(patients);
        }

        // GET: MedicalFiles/ExportToCsv/5
        public async Task<IActionResult> ExportToCsv(int id)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.Medicalhistories)
                        .ThenInclude(mh => mh.Disease)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                    return NotFound();

                var csv = new StringBuilder();
                csv.AppendLine("Name,LastName,OIB,BirthDate,Gender,Diseases");

                var diseases = string.Join(" | ", patient.Medicalhistories.Select(m => m.Disease.Name));
                csv.AppendLine($"\"{patient.Name}\",\"{patient.LastName}\",\"{patient.Oib}\",\"{patient.BirthDate:yyyy-MM-dd}\",\"{patient.Spol}\",\"{diseases}\"");

                var utf8Bom = Encoding.UTF8.GetPreamble();
                var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
                var bytes = utf8Bom.Concat(csvBytes).ToArray();

                var fileName = $"patient_{patient.Id}_{patient.LastName}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export CSV for patient with ID {PatientId}", id);
                LogError(ex);
                return RedirectToAction("Error", "Home", new { message = "CSV export failed." });
            }
        }

        private void LogError(Exception ex)
        {
            var logPath = Path.Combine(_env.WebRootPath, "logs");
            Directory.CreateDirectory(logPath);

            var filePath = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            var error = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.Message}\n{ex.StackTrace}";
            System.IO.File.WriteAllText(filePath, error);
        }


    }
}
