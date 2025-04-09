using System.ComponentModel.DataAnnotations;

namespace PatientManager.ViewModels
{
    public class MedicalHistoryDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        [Required]
        public int DiseaseId { get; set; }

        [Required]
        [Display(Name = "Disease Start")]
        [DataType(DataType.Date)]
        public DateTime DiseaseStart { get; set; }

        [Display(Name = "Disease End")]
        [DataType(DataType.Date)]
        public DateTime? DiseaseEnd { get; set; }
    }
}
