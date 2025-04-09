using System;
using System.Collections.Generic;

namespace PatientManager.Models;

public partial class Medicalhistory
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DiseaseId { get; set; }

    public DateOnly DiseaseStart { get; set; }

    public DateOnly? DiseaseEnd { get; set; }

    public virtual Disease Disease { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
