namespace BMICalculator.Model.DTO
{
    public class BmiResult
    {
        public double Bmi { get; set; }
        public BmiClassification BmiClassification { get; set; }
        public string Summary { get; set; }
    }

    public enum BmiClassification
    {
        Underweight,
        Normal,
        Overweight,
        Obesity,
        ExtremeObesity
    }
}
