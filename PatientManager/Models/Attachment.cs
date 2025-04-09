using System;
using System.Collections.Generic;

namespace PatientManager.Models;

public partial class Attachment
{
    public int Id { get; set; }

    public int ExaminationId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual Examination Examination { get; set; } = null!;
}
