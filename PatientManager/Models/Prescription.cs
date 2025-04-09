using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace PatientManager.Models;

public partial class Prescription
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int MedicationId { get; set; }

    public DateOnly PrescriptionDate { get; set; }

    public virtual Medication Medication { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
