using BMICalculator.Model.DTO;
using System.Threading.Tasks;

namespace BMICalculator.Model.Repositories
{
    public interface IResultRepository
    {
        Task SaveResultAsync(BmiResult result);
    }

    public class ResultRepository : IResultRepository
    {
        public Task SaveResultAsync(BmiResult result)
        {
            return Task.CompletedTask;
        }
    }
}
