using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;

namespace PatientManager.Controllers
{
    public class PatientsController : Controller
    {
        private readonly PostgresContext _context;

        public PatientsController(PostgresContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string? searchString)
        {
            var patients = from p in _context.Patients
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p =>
                    p.LastName.ToLower().Contains(searchString.ToLower()) ||
                    p.Oib.Contains(searchString));
            }

            return View(await patients.ToListAsync());
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Examinations)
                    .ThenInclude(e => e.Type) // ako želiš prikazati naziv tipa
                .Include(p => p.Medicalhistories)
                    .ThenInclude(m => m.Disease) // ako koristiš entitet Disease
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }


        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Oib,BirthDate,Spol")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);

                if (_context.Patients.Any(p => p.Oib == patient.Oib && p.Id != patient.Id))
                {
                    ModelState.AddModelError("Oib", "Patient with this OIB already exists.");
                    return View(patient);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Oib,BirthDate,Spol")] Patient patient)
        {
            if (id != patient.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);

                    if (_context.Patients.Any(p => p.Oib == patient.Oib && p.Id != patient.Id))
                    {
                        ModelState.AddModelError("Oib", "Patient with this OIB already exists.");
                        return View(patient);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
