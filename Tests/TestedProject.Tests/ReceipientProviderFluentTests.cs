using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace TestedProject.Tests
{
    public class ReceipientProviderFluentTests
    {
        IReceipientProvider receipientProvider;

        [SetUp]
        public void SetUp()
        {
            receipientProvider = new ReceipientProvider();
        }

        [Test]
        public void GetReceipientNameShouldNotBeEmptyAndStartWithNADALA()
        {
            string actual = receipientProvider.GetReceipientName();

            actual.Should().NotBeEmpty().And.StartWith("NADALA");
        }

        [Test]
        public void GetReceipientTaxIdShouldStartWithPLAndHaveLenght12()
        {
            string actual = receipientProvider.GetReceipientTaxId();

            actual.Should().NotBeEmpty().And.StartWith("PL").And.HaveLength(12);
        }
    }
}
