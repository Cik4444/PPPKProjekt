using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientManager.Models;
using PatientManager.ViewModels;

namespace PatientManager.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly PostgresContext _context;

        public PrescriptionsController(PostgresContext context)
        {
            _context = context;
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .Include(p => p.Medication)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prescription == null) return NotFound();

            return View(prescription);
        }

        // GET: Prescriptions/Create?patientId=1
        public IActionResult Create(int patientId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.Medications = new SelectList(_context.Medications.ToList(), "Id", "Name");

            var viewModel = new PrescriptionDto
            {
                PatientId = patientId,
                PrescriptionDate = DateTime.Today
            };

            return View(viewModel);
        }


        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PrescriptionDto model)
        {
            if (ModelState.IsValid)
            {
                var prescription = new Prescription
                {
                    PatientId = model.PatientId,
                    MedicationId = model.MedicationId,
                    PrescriptionDate = DateOnly.FromDateTime(model.PrescriptionDate)
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Patients", new { id = model.PatientId });
            }

            ViewBag.Medications = new SelectList(_context.Medications.ToList(), "Id", "Name", model.MedicationId);
            return View(model);
        }


        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            var model = new PrescriptionDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                MedicationId = prescription.MedicationId,
                PrescriptionDate = prescription.PrescriptionDate.ToDateTime(TimeOnly.MinValue)
            };

            ViewBag.Medications = new SelectList(_context.Medications.ToList(), "Id", "Name", model.MedicationId);
            return View(model);
        }


        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PrescriptionDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var prescription = await _context.Prescriptions.FindAsync(id);
                if (prescription == null) return NotFound();

                prescription.MedicationId = model.MedicationId;
                prescription.PrescriptionDate = DateOnly.FromDateTime(model.PrescriptionDate);

                _context.Update(prescription);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Patients", new { id = model.PatientId });
            }

            ViewBag.Medications = new SelectList(_context.Medications.ToList(), "Id", "Name", model.MedicationId);
            return View(model);
        }

    }
}
