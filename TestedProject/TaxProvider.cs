namespace TestedProject
{
    public class TaxProvider : ITaxProvider
    {
        public decimal GetTax(RateType rate)
        {
            switch (rate)
            {
                case RateType.General:
                    return 0.23m;
                case RateType.Reduced:
                    return 0.08m;
                default:
                    return 0.23m;
            }
        }
    }
}