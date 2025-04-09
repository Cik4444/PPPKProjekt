using System.ComponentModel.DataAnnotations;

namespace PatientManager.ViewModels
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PrescriptionDate { get; set; } 
    }
}
