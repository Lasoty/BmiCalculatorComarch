using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestedProject.Tests
{
    public class CalculatorServiceTests
    {
        Mock<IDiscountService> discountMock;
        ICalculatorService calculatorService;

        [SetUp]
        public void Setup()
        {
            Mock<ITaxProvider> mock = new Mock<ITaxProvider>();
            mock.Setup(m => m.GetTax(It.IsAny<RateType>())).Returns(0.23m);
            discountMock = new Mock<IDiscountService>();

            var factory = new Mock<IHttpClientFactory>();

            var dbOptions = new DbContextOptions<TestDbContext>();
            var dbContextMock = new DbContextMock<TestDbContext>(dbOptions);
            calculatorService = new CalculatorService(mock.Object, discountMock.Object, factory.Object, dbContextMock.Object);
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
            //Arrange 
            Invoice invoice = new()
            {
                Date = expected,
            };

            //Act
            var actual = calculatorService.CheckDate(invoice, testedDate);

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task CreateInvoiceShouldReturnCorrectNetValueInInvoice()
        {
            //Arrange
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            decimal expected = 60m;

            //Act 
            var actual = await calculatorService.CreateInvoice(items);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.TotalNet);
        }

        [Test]
        public async Task CreateInvoiceShouldReturnCorrectGrossValueInInvoice()
        {
            //Arrange
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            decimal expected = 73.8m;

            //Act 
            var actual = await calculatorService.CreateInvoice(items);

            Assert.AreEqual(expected, actual.TotalGross);
        }

        [Test]
        public async Task CreateInvoiceShouldThrowAnExceptionWhenItemsIsEmpty()
        {
            //Arrange 
            ICollection<InvoiceItem> items = new List<InvoiceItem>();

            //Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await calculatorService.CreateInvoice(items));
        }

        [Test]
        public async Task CreateInvoiceShouldReturnExactInvoiceItems()
        {
            //Arrange
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            //Act 
            var actual = await calculatorService.CreateInvoice(items);

            //Assert
            CollectionAssert.AllItemsAreNotNull(actual.Items);
            CollectionAssert.AllItemsAreInstancesOfType(actual.Items, typeof(InvoiceItem));
            CollectionAssert.AreEqual(items, actual.Items);
        }
    }
}