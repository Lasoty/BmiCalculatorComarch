using System;
using System.Collections.Generic;
using System.Linq;

namespace TestedProject
{
    public class CalculatorService : ICalculatorService
    {
        public decimal GetGrossFromNet(decimal net, decimal tax)
        {
            decimal result = net * (1 + tax);
            return result;
        }

        public bool CheckDate(Invoice invoice, DateTime dateTime)
        {
            return invoice.Date == dateTime.Date;
        }

        public Invoice CreateInvoice(ICollection<InvoiceItem> items)
        {
            Invoice invoice = new Invoice()
            {
                Date = DateTime.Now,
                Items = items
            };

            invoice.TotalNet = items.Sum(x => x.NetValue * x.Quantity);
            invoice.TotalGross = GetGrossFromNet(invoice.TotalNet, 0.23m);

            return invoice;
        }
    }
}
