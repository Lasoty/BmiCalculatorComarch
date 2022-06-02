using BMICalculator.Model.DTO;

namespace BMICalculator.Services.Interfaces
{
    public interface IBmiDeterminator
    {
        BmiClassification DetermineBmi(double bmi);
    }
}
