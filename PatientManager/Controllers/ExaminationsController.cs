using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;
using PatientManager.ViewModels;

namespace PatientManager.Controllers
{
    public class ExaminationsController : Controller
    {
        private readonly PostgresContext _context;

        public ExaminationsController(PostgresContext context)
        {
            _context = context;
        }

        // GET: Examinations
        public async Task<IActionResult> Index()
        {
            var examinations = _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Type);

            return View(await examinations.ToListAsync());
        }

        // GET: Examinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Type)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (examination == null) return NotFound();

            return View(examination);
        }

        // GET: Examinations/Create
        public IActionResult Create()
        {
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "LastName");
            ViewBag.Types = new SelectList(_context.Examinationtypes, "Id", "Name");

            return View();
        }

        // POST: Examinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExaminationDto dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new Examination
                {
                    PatientId = dto.PatientId,
                    TypeId = dto.TypeId,
                    Date = dto.Date.ToDateTime(TimeOnly.MinValue)
                };

                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = new SelectList(_context.Patients, "Id", "LastName", dto.PatientId);
            ViewBag.Types = new SelectList(_context.Examinationtypes, "Id", "Name", dto.TypeId);

            return View(dto);
        }

        // GET: Examinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null) return NotFound();

            ViewBag.Patients = new SelectList(_context.Patients, "Id", "LastName", examination.PatientId);
            ViewBag.Types = new SelectList(_context.Examinationtypes, "Id", "Name", examination.TypeId);

            return View(examination);
        }

        // POST: Examinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,Date,TypeId")] Examination examination)
        {
            if (id != examination.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExaminationExists(examination.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Patients = new SelectList(_context.Patients, "Id", "LastName", examination.PatientId);
            ViewBag.Types = new SelectList(_context.Examinationtypes, "Id", "Name", examination.TypeId);

            return View(examination);
        }

        // GET: Examinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Type)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (examination == null) return NotFound();

            return View(examination);
        }

        // POST: Examinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            if (examination != null)
            {
                _context.Examinations.Remove(examination);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExaminationExists(int id)
        {
            return _context.Examinations.Any(e => e.Id == id);
        }
    }
}
