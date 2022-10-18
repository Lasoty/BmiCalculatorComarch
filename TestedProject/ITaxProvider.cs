namespace TestedProject
{
    public interface ITaxProvider
    {
        decimal GetTax(RateType rate);
    }

    public enum RateType
    {
        General,
        Reduced
    }
}