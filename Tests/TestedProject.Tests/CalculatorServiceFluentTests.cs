﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;


namespace TestedProject.Tests
{
    public class CalculatorServiceFluentTests
    {
        ICalculatorService calculatorService;
        Mock<IDiscountService> discountMock; 

        [SetUp]
        public void StartUp()
        {
            Mock<ITaxProvider> taxMock = new Mock<ITaxProvider>();
            taxMock.Setup(m => m.GetTax(It.IsAny<RateType>())).Returns(0.23m);
            taxMock.Setup(m => m.ForTest).Returns(123);

            discountMock = new Mock<IDiscountService>();
            calculatorService = new CalculatorService(taxMock.Object, discountMock.Object);
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
    }
}
