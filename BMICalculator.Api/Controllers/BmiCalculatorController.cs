using BMICalculator.Model.DTO;
using BMICalculator.Services;
using BMICalculator.Services.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BMICalculator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BmiCalculatorController : ControllerBase
    {
        private readonly IBmiCalculatorFacade bmiCalculatorFacade;

        public BmiCalculatorController(
            IBmiCalculatorFacade bmiCalculatorFacade
            )
        {
            this.bmiCalculatorFacade = bmiCalculatorFacade;
        }

        [HttpGet("[action]/{weight}/{height}/{unit:int=0}")]
        public IActionResult Calculate(double weight, double height, UnitSystem unit)
        {
            BmiResult result = bmiCalculatorFacade.GetResult(weight, height, unit);
            return Ok(result);
        }
    }
}
