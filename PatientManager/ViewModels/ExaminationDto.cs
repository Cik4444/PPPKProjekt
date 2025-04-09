namespace PatientManager.ViewModels;

public class ExaminationDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateOnly Date { get; set; }

    public int TypeId { get; set; }
}
