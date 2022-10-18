namespace TestedProject
{
    public class ReceipientProvider : IReceipientProvider
    {
        public ReceipientProvider()
        {
        }

        public string GetReceipientName()
        {
            return "NADALA Software & Consulting";
        }

        public string GetReceipientTaxId()
        {
            return "PL8921426699";
        }
    }

    public interface IReceipientProvider
    {
        string GetReceipientName();
        string GetReceipientTaxId();
    }
}