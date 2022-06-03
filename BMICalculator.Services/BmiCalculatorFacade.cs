using BMICalculator.Model.DTO;
using BMICalculator.Services.Enums;
using BMICalculator.Services.Interfaces;
using System;

namespace BMICalculator.Services
{
    public class BmiCalculatorFacade : IBmiCalculatorFacade
    {
        private readonly IBmiDeterminator bmiDeterminator;
        private readonly IBmiCalculatorFactory bmiCalculatorFactory;

        public BmiCalculatorFacade(IBmiDeterminator bmiDeterminator, IBmiCalculatorFactory bmiCalculatorFactory)
        {
            this.bmiDeterminator = bmiDeterminator;
            this.bmiCalculatorFactory = bmiCalculatorFactory;
        }

        private string GetSummary(BmiClassification classification)
            => classification switch
            {
                BmiClassification.Underweight => "You are underweight, you should put on some weight",
                BmiClassification.Normal => "Your weight is normal, keep it up",
                BmiClassification.Overweight => "You are a bit overweight",
                BmiClassification.Obesity => "You should take care of your obesity",
                BmiClassification.ExtremeObesity => "Your extreme obesity might cause health problems",
                _ => throw new NotImplementedException(),
            };

        public BmiResult GetResult(double weight, double height, UnitSystem unitSystem)
        {
            var bmiCalculator = bmiCalculatorFactory.CreateCalculator(unitSystem);
            var bmi = bmiCalculator.CalculateBmi(weight, height);
            BmiClassification classification = bmiDeterminator.DetermineBmi(bmi);

            return new BmiResult()
            {
                Bmi = bmi,
                BmiClassification = classification,
                Summary = GetSummary(classification)
            };

        }
    }

    public interface IBmiCalculatorFacade
    {
        BmiResult GetResult(double weight, double height, UnitSystem unitSystem);
    }
}
