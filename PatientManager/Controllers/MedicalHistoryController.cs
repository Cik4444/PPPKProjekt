using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;
using PatientManager.ViewModels;

namespace PatientManager.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly PostgresContext _context;

        public MedicalHistoryController(PostgresContext context)
        {
            _context = context;
        }

        // GET: MedicalHistory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var history = await _context.Medicalhistories
                .Include(m => m.Disease)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (history == null) return NotFound();

            return View(history);
        }

        // GET: MedicalHistory/Create?patientId=1
        public IActionResult Create(int patientId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.Diseases = new SelectList(_context.Diseases.ToList(), "Id", "Name");

            var model = new MedicalHistoryDto
            {
                PatientId = patientId,
                DiseaseStart = DateTime.Today
            };

            return View(model);
        }


        // POST: MedicalHistory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalHistoryDto model)
        {
            if (ModelState.IsValid)
            {
                var history = new Medicalhistory
                {
                    PatientId = model.PatientId,
                    DiseaseId = model.DiseaseId,
                    DiseaseStart = DateOnly.FromDateTime(model.DiseaseStart),
                    DiseaseEnd = model.DiseaseEnd.HasValue ? DateOnly.FromDateTime(model.DiseaseEnd.Value) : null
                };

                _context.Medicalhistories.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Patients", new { id = model.PatientId });
            }

            ViewBag.PatientId = model.PatientId;
            ViewBag.Diseases = new SelectList(_context.Diseases.ToList(), "Id", "Name", model.DiseaseId);
            return View(model);
        }


        // GET: MedicalHistory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var history = await _context.Medicalhistories.FindAsync(id);
            if (history == null) return NotFound();

            var model = new MedicalHistoryDto
            {
                Id = history.Id,
                PatientId = history.PatientId,
                DiseaseId = history.DiseaseId,
                DiseaseStart = history.DiseaseStart.ToDateTime(TimeOnly.MinValue),
                DiseaseEnd = history.DiseaseEnd?.ToDateTime(TimeOnly.MinValue)
            };

            ViewBag.Diseases = new SelectList(_context.Diseases.ToList(), "Id", "Name", model.DiseaseId);
            return View(model);
        }


        // POST: MedicalHistory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalHistoryDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var history = await _context.Medicalhistories.FindAsync(id);
                if (history == null) return NotFound();

                history.DiseaseId = model.DiseaseId;
                history.DiseaseStart = DateOnly.FromDateTime(model.DiseaseStart);
                history.DiseaseEnd = model.DiseaseEnd.HasValue ? DateOnly.FromDateTime(model.DiseaseEnd.Value) : null;

                _context.Update(history);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Patients", new { id = model.PatientId });
            }

            ViewBag.Diseases = new SelectList(_context.Diseases.ToList(), "Id", "Name", model.DiseaseId);
            return View(model);
        }

    }
}
