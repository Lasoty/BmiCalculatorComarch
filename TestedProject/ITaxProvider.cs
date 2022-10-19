namespace TestedProject
{
    public interface ITaxProvider
    {
        decimal GetTax(RateType rate);

        int ForTest { get; set; }
    }

    public enum RateType
    {
        General,
        Reduced
    }
}