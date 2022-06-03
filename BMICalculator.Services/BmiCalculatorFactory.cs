using BMICalculator.Services.Enums;
using BMICalculator.Services.Interfaces;

namespace BMICalculator.Services
{
    public class BmiCalculatorFactory : IBmiCalculatorFactory
    {
        private readonly ImperialBmiCalculator imperialBmiCalculator;
        private readonly MetricBmiCalculator metricBmiCalculator;

        public BmiCalculatorFactory(ImperialBmiCalculator imperialBmiCalculator, MetricBmiCalculator metricBmiCalculator)
        {
            this.imperialBmiCalculator = imperialBmiCalculator;
            this.metricBmiCalculator = metricBmiCalculator;
        }

        public IBmiCalculator CreateCalculator(UnitSystem unitSystem)
        {
            return unitSystem switch
            {
                UnitSystem.Metric => metricBmiCalculator,
                UnitSystem.Imperial => imperialBmiCalculator,
                _ => null,
            };
        }
    }

    public interface IBmiCalculatorFactory
    {
        IBmiCalculator CreateCalculator(UnitSystem unitSystem);
    }
}
