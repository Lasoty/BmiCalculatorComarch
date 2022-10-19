using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestedProject
{
    public interface ICalculatorService
    {
        decimal GetGrossFromNet(decimal net, decimal tax);
        bool CheckDate(Invoice invoice, DateTime dateTime);

        Task<Invoice> CreateInvoice(ICollection<InvoiceItem> items, RateType rate = RateType.General);

        DateTime StartPeriodDate(DateTime dateTime);

        DateTime GetInvoiceDate(Invoice invoice);

        Task SendInvoice(Invoice invoice);

        event EventHandler InvoiceCreated;
    }
}
