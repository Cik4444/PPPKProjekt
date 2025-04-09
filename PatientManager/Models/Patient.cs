using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientManager.Models;

public partial class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "OIB is required")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB must have exactly 11 characters")]
    public string Oib { get; set; } = null!;

    [Required(ErrorMessage = "Birth date is required")]
    [Display(Name = "Birth date")]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [Display(Name = "Gender")]
    public string Spol { get; set; } = null!;

    public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

    public virtual ICollection<Medicalhistory> Medicalhistories { get; set; } = new List<Medicalhistory>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
