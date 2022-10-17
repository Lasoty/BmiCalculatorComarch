using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TestedProject.Tests
{
    public class CalculatorServiceTests
    {
        ICalculatorService calculatorService;

        [SetUp]
        public void Setup()
        {
            calculatorService = new CalculatorService();
        }

        [TearDown]
        public void TearDown()
        {

        }

        [TestCase(10, 0.23, 12.3)]
        [TestCase(20, 0.23, 24.6)]
        public void GetGrossFromNetShouldReturnValidGrossValue(decimal netValue, decimal tax, decimal expected)
        {
            //Act
            decimal actual = calculatorService.GetGrossFromNet(netValue, tax);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase("2012.5.31", "2012.5.31")]
        [TestCase("2012.5.31", "2012.5.31 13:13:00")]
        public void CheckDateShouldReturnTrueForValidInvoiceDate(DateTime expected, DateTime testedDate)
        {
            //Assert 
            Invoice invoice = new()
            {
                Date = expected,
            };

            //Act
            var actual = calculatorService.CheckDate(invoice, testedDate);

            Assert.IsTrue(actual);
        }

        [Test]
        public void CreateInvoiceShouldReturnCorrectNetValueInInvoice()
        {
            //Assert
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            decimal expected = 60m;

            //Act 
            var actual = calculatorService.CreateInvoice(items);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.TotalNet);
        }

        [Test]
        public void CreateInvoiceShouldReturnCorrectGrossValueInInvoice()
        {
            //Assert
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            decimal expected = 73.8m;

            //Act 
            var actual = calculatorService.CreateInvoice(items);

            Assert.AreEqual(expected, actual.TotalGross);
        }
    }
}