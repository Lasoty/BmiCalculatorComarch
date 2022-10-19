using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TestedProject
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ITaxProvider taxProvider;
        private readonly IDiscountService discountService;
        private readonly TestDbContext dbContext;
        private readonly HttpClient httpClient;

        public CalculatorService(
            ITaxProvider taxProvider,
            IDiscountService discountService,
            IHttpClientFactory httpClientFactory,
            TestDbContext dbContext
           )
        {
            IReceipientProvider receipientProvider = new ReceipientProvider();
            this.taxProvider = taxProvider;
            this.discountService = discountService;
            this.dbContext = dbContext;
            this.httpClient = httpClientFactory.CreateClient();
        }

        public event EventHandler InvoiceCreated;

        public decimal GetGrossFromNet(decimal net, decimal tax)
        {
            decimal result = net * (1 + tax);
            return result;
        }

        public bool CheckDate(Invoice invoice, DateTime dateTime)
        {
            return invoice.Date == dateTime.Date;
        }

        public async Task<Invoice> CreateInvoice(ICollection<InvoiceItem> items, RateType rate)
        {
            decimal rateValue = taxProvider.GetTax(rate);
            if (items == null || items.Count == 0)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Invoice invoice = new Invoice()
            {
                Date = DateTime.Now,
                Items = items,
            };

            var totalNet = items.Sum(x => x.NetValue * x.Quantity);
            var discount = discountService.GetDiscount(invoice.ReceipientName);

            if (discount > 0)
                totalNet = totalNet - (totalNet * discount);

            invoice.TotalNet = totalNet;
            invoice.TotalGross = GetGrossFromNet(invoice.TotalNet, rateValue);

            InvoiceCreated?.Invoke(this, new EventArgs());
            return invoice;
        }

        public DateTime StartPeriodDate(DateTime dateTime)
        {
            return new(dateTime.Year, dateTime.Month, 1);
        }

        public DateTime GetInvoiceDate(Invoice invoice)
        {
            return invoice.Date.Date;
        }

        public async Task SendInvoice(Invoice invoice)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(invoice), Encoding.Unicode);
            await httpClient.PostAsync("http://example.api/api/Invoices2", content);
        }

        public async Task SaveInvoice(Invoice invoice)
        {
            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();
        }
    }
}
