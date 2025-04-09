using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PatientManager.Helpers
{
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter()
            : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
        {
        }
    }
}