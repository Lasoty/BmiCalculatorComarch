using System;
using System.Collections;
using System.Collections.Generic;

namespace TestedProject
{
    public interface ICalculatorService
    {
        decimal GetGrossFromNet(decimal net, decimal tax);
        bool CheckDate(Invoice invoice, DateTime dateTime);

        Invoice CreateInvoice(ICollection<InvoiceItem> items);
    }
}
