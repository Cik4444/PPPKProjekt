using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;
using System.IO.Compression;

namespace PatientManager.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly PostgresContext _context;
        private readonly IWebHostEnvironment _env;

        public AttachmentsController(PostgresContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int examinationId, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var attachment = new Attachment
                    {
                        ExaminationId = examinationId,
                        FileName = file.FileName,
                        FilePath = "/uploads/" + fileName,
                        UploadedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)

                    };

                    _context.Attachments.Add(attachment);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Details", "Examinations", new { id = examinationId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return RedirectToAction("Error", "Home", new { message = "File upload failed." });
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

        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null)
                return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath, attachment.FilePath.TrimStart('/'));
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, "application/octet-stream", attachment.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                var path = Path.Combine(_env.WebRootPath, attachment.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Examinations", new { id = attachment.ExaminationId });
            }

            return NotFound();
        }

        public async Task<IActionResult> List(int examinationId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.ExaminationId == examinationId)
                .OrderByDescending(a => a.UploadedAt)
                .ToListAsync();

            ViewBag.ExaminationId = examinationId;
            return View(attachments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            return View(attachment);
        }

        public async Task<IActionResult> Preview(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath, attachment.FilePath.TrimStart('/'));
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            var contentType = GetContentType(fullPath);
            return File(fileBytes, contentType);
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }

        public async Task<IActionResult> DownloadAll(int examinationId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.ExaminationId == examinationId)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var attachment in attachments)
                {
                    var filePath = Path.Combine(_env.WebRootPath, attachment.FilePath.TrimStart('/'));
                    if (!System.IO.File.Exists(filePath)) continue;

                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    var zipEntry = zip.CreateEntry(attachment.FileName);

                    using var entryStream = zipEntry.Open();
                    await entryStream.WriteAsync(fileBytes);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream.ToArray(), "application/zip", $"attachments_{examinationId}.zip");
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachmentsJson(int examinationId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.ExaminationId == examinationId)
                .Select(a => new {
                    a.Id,
                    a.FileName,
                    a.FilePath,
                    a.UploadedAt
                })
                .ToListAsync();

            return Json(attachments);
        }
    }
}
