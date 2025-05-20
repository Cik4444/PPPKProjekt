using System;
using System.Collections.Generic;

namespace PatientManager.Models;

public partial class Medicalfile
{
    public int Id { get; set; }

    public int ExaminationId { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual Examination Examination { get; set; } = null!;

    public int medicalNumber { get; set; }
}
