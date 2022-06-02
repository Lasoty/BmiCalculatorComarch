using BMICalculator.Model.DTO;
using BMICalculator.Model.Repositories;
using System.Threading.Tasks;

namespace BMICalculator.Services
{
    public class ResultService
    {
        public BmiResult RecentOverweightResult { get; private set; }
        private readonly IResultRepository resultRepository;

        public ResultService(IResultRepository resultRepository)
        {
            this.resultRepository = resultRepository;
        }

        public void SetRecentOverweightResult(BmiResult result)
        {
            if (result.BmiClassification == BmiClassification.Overweight)
            {
                RecentOverweightResult = result;
            }
        }

        public async Task SaveUnderweightResultAsync(BmiResult result)
        {
            if (result.BmiClassification == BmiClassification.Underweight)
            {
                await resultRepository.SaveResultAsync(result);
            }
        }
    }
}
