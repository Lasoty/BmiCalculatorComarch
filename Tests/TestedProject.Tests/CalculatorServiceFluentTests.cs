using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Contrib.HttpClient;
using NUnit.Framework;


namespace TestedProject.Tests
{
    public class CalculatorServiceFluentTests
    {
        ICalculatorService calculatorService;
        Mock<IDiscountService> discountMock;
        Mock<HttpMessageHandler> handler;
        DbContextMock<TestDbContext> dbContextMock;

        [SetUp]
        public void StartUp()
        {
            Mock<ITaxProvider> taxMock = new Mock<ITaxProvider>();
            taxMock.Setup(m => m.GetTax(It.IsAny<RateType>())).Returns(0.23m);
            taxMock.Setup(m => m.ForTest).Returns(123);

            handler = new Mock<HttpMessageHandler>();
            var factory = new Mock<IHttpClientFactory>();
            handler.SetupAnyRequest().ReturnsResponse(System.Net.HttpStatusCode.NotFound);
            var client = new HttpClient(handler.Object);
            factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
            
            discountMock = new Mock<IDiscountService>();

            var dbOptions = new DbContextOptions<TestDbContext>();
            dbContextMock = new DbContextMock<TestDbContext>(dbOptions);

            calculatorService = new CalculatorService(taxMock.Object, discountMock.Object, factory.Object, dbContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void StartPeriodDateShouldReturnValidDate()
        {
            DateTime dateTime = 23.March(2022).At(8, 55).AsLocal();
            DateTime actual = calculatorService.StartPeriodDate(dateTime);

            actual.Should().Be(1.March(2022).AsLocal()).And.HaveDay(1);
        }

        [Test]
        public void GetInvoiceDateShouldReturnDateWithoutHoursAndAfter1stDayOfMonth()
        {
            DateTime dateTime = 23.March(2022).At(8, 55).AsLocal();
            Invoice invoice = new Invoice
            {
                Date = dateTime
            };

            DateTime actual = calculatorService.GetInvoiceDate(invoice);
            actual.Should().HaveHour(0).And.HaveMinute(0).And.BeAfter(1.March(2022));
        }

        [Test]
        public async Task CreateInvoiceShouldReturnInvoiceWithExactItems()
        {
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };

            Invoice actual = await calculatorService.CreateInvoice(items, RateType.General);

            actual.Should().NotBeNull();

            actual.Items.Should().NotBeEmpty().And.HaveCount(3).And.ContainItemsAssignableTo<InvoiceItem>()
                .And.Equal(items);
        }

        [Test]
        public async Task CreateInvoiceShouldReturnInvoiceIncludeDiscount()
        {
            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
                new InvoiceItem { Name = "Item test 2", NetValue = 10, Quantity = 2 },
                new InvoiceItem { Name = "Item test 3", NetValue = 10, Quantity = 3 },
            };
            discountMock.Setup(m => m.GetDiscount(It.IsAny<string>())).Returns(0.1m);

            Invoice actual = await calculatorService.CreateInvoice(items, RateType.General);

            actual.Should().NotBeNull();

            actual.TotalNet.Should().BePositive().And.Be(54);
        }


        [Test]
        public async Task CreateInvoiceShouldThrowAnExceptionWhenItemsIsEmpty()
        {
            ICollection<InvoiceItem> items = new List<InvoiceItem>();

            await calculatorService.Invoking(async cs => await cs.CreateInvoice(items))
                .Should().ThrowAsync<ArgumentNullException>();
        }


        [Test]
        public async Task CreateInvoiceShouldThrowAnExceptionWhenItemsIsEmpty2()
        {
            ICollection<InvoiceItem> items = new List<InvoiceItem>();

            Func<Task> act = async () => await calculatorService.CreateInvoice(items);

            await act.Should().ThrowAsync<ArgumentNullException>();
            //act.Should().NotThrowAfter(5.Seconds(), 100.Microseconds());
        }

        [Test]
        public async Task CreateInvoiceShouldRaiseInvoiceCreatedEvent()
        {
            using var monitoredCalculatorService = calculatorService.Monitor();

            ICollection<InvoiceItem> items = new List<InvoiceItem>()
            {
                new InvoiceItem { Name = "Item test 1", NetValue = 10, Quantity = 1 },
            };

            //calculatorService.InvoiceCreated += (sender, args) => { };
            Invoice actual = await calculatorService.CreateInvoice(items);
            monitoredCalculatorService.Should().Raise(nameof(CalculatorService.InvoiceCreated));
        }

        [Test]
        public async Task SendInvoiceShouldCallToPostAsync()
        {
            handler.SetupRequest(HttpMethod.Post, "http://example.api/api/Invoices")
                .ReturnsResponse(System.Net.HttpStatusCode.OK)
                .Verifiable();

            Invoice invoice = new();
            await calculatorService.SendInvoice(invoice);
            handler.VerifyAnyRequest(Times.AtLeastOnce());
        }

        [Test]
        public async Task SaveInvoiceShouldSaveEntity()
        {
            var dbSetmock = dbContextMock.CreateDbSetMock(x => x.Invoices);
            dbContextMock.Setup(dbContext => dbContext.SaveChanges()).Verifiable();

            Invoice invoice = new()
            {
                ReceipientName = "Test 1"
            };

            await calculatorService.SaveInvoice(invoice);

            dbSetmock.Object.Should().Contain(invoice);
            dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }
    }
}
