using System;
using System.Collections.Generic;

namespace TestedProject
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<InvoiceItem> Items { get; internal set; }
        public decimal TotalNet { get; internal set; }
        public decimal TotalGross { get; internal set; }

        public string ReceipientName { get; set; }
    }

    public class InvoiceItem
    {
        public string Name { get; set; }

        public decimal NetValue { get; set; }

        public decimal Quantity { get; set; }
    }
}