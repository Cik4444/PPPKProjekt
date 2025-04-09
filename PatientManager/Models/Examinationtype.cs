using System;
using System.Collections.Generic;

namespace PatientManager.Models;

public partial class Examinationtype
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();
}
