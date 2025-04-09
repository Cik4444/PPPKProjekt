using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientManager.Models;

public partial class Examination
{

    public int Id { get; set; }

    public int PatientId { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    public int TypeId { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Medicalfile> Medicalfiles { get; set; } = new List<Medicalfile>();

    [Required(ErrorMessage = "Patient is required")]
    public virtual Patient Patient { get; set; } = null!;

    [Required(ErrorMessage = "Examination type is required")]
    [Display(Name = "Examination type")]
    public virtual Examinationtype Type { get; set; } = null!;

    [NotMapped]
    public DateOnly DateOnly
    {
        get => DateOnly.FromDateTime(Date);
        set => Date = value.ToDateTime(TimeOnly.MinValue);
    }
}

