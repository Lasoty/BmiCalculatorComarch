using BMICalculator.Services.Interfaces;
using System;

namespace BMICalculator.Services
{
    public class ImperialBmiCalculator : IBmiCalculator
    {
        public double CalculateBmi(double weight, double height)
        {
            if (weight <= 0)
                throw new ArgumentException("Weight is not a valid number");


            if (height <= 0)
                throw new ArgumentException("Height is not a valid number");

            var bmi = weight / Math.Pow(height, 2) * 703;
            return Math.Round(bmi, 2);
        }
    }
}
