using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;

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
                        UploadedAt = DateTime.UtcNow
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
    }
}
