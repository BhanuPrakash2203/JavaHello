using System;
using System.Globalization;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class ValueFormatterTests
    {
        private ValueDefaultFormatter _defaultFormatter;

        private const string FRENCH_DATE_FORMAT = "d MMMM yyyy";
        private readonly CultureInfo FRENCH_CULTURE = CultureInfo.GetCultureInfo("fr-CA");

        [TestInitialize]
        public void Initialize()
        {
            var cultureAccessorMock = Substitute.For<ICultureAccessor>();
            cultureAccessorMock.GetCultureInfo().Returns(FRENCH_CULTURE);
            _defaultFormatter = new ValueDefaultFormatter(cultureAccessorMock, new DateBuilder(cultureAccessorMock));
        }

        [TestMethod]
        public void FormatInt()
        {
            object test = 10;
            _defaultFormatter.Format(test).Should().Be(test.ToString());
        }

        [TestMethod]
        public void FormatBool()
        {
            object test = true;
            _defaultFormatter.Format(test).Should().Be(test.ToString());
        }

        [TestMethod]
        public void FormatFloat()
        {
            object test = 99.9f;
            _defaultFormatter.Format(test).Should().Be(Convert.ToSingle(test).ToString(FRENCH_CULTURE));
        }

        [TestMethod]
        public void FormatDouble()
        {
            object test = 99.9;
            _defaultFormatter.Format(test).Should().Be(Convert.ToDouble(test).ToString(CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void FormatString()
        {
            object test = "test_string";
            _defaultFormatter.Format(test).Should().Be(test.ToString());
        }

        [TestMethod]
        public void FormatDate()
        {
            DateTime test = new DateTime();

            _defaultFormatter.Format(test).Should().Be(test.ToString(FRENCH_DATE_FORMAT));
        }
    }
}
