using System;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class DateFormatterTest
    {
        private IDateBuilder _dateBuilder;
        private ICultureAccessor _cultureAccessor;
        private readonly IFixture _auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Setup()
        {
            _cultureAccessor = new CultureAccessor();
            _dateBuilder = new DateBuilder(_cultureAccessor);
        }

        [TestMethod]
        public void TestMethodEnglish()
        {
            _cultureAccessor.SetCultureInfo(CultureHelper.DefaultCulture, _auto.Create<IResourcesAccessorFactory>());

            var date = new DateTime(2017, 12, 31, 11, 58, 21);
            var formatter = new DateFormatter(_cultureAccessor, _dateBuilder);
            var resultDate = formatter.Format(date);
            var resultDateTime = formatter.Format(date, true);
            var resultDateTime24H = formatter.Format(date.AddHours(4), true);

            using (new AssertionScope())
            {
                resultDate.Should().Be("12/31/2017");
                resultDateTime.Should().Be("12/31/2017 - 11:58 AM");
                resultDateTime24H.Should().Be("12/31/2017 - 03:58 PM");
            }
        }

        [TestMethod]
        public void TestMethodEnglishLong()
        {
            _cultureAccessor.SetCultureInfo(CultureHelper.DefaultCulture, _auto.Create<IResourcesAccessorFactory>());

            var date = new DateTime(2017, 12, 31, 11, 58, 21);
            var formatter = new LongDateFormatter(_cultureAccessor, _dateBuilder);
            var resultDate = formatter.Format(date);
            var resultDateTime = formatter.Format(date, true);
            var resultDateTime24H = formatter.Format(date.AddHours(4), true);

            using (new AssertionScope())
            {
                resultDate.Should().Be("December 31, 2017");
                resultDateTime.Should().Be("December 31, 2017 - 11:58 AM");
                resultDateTime24H.Should().Be("December 31, 2017 - 03:58 PM");
            }
        }

        [TestMethod]
        public void TestMethodFrench()
        {
            _cultureAccessor.SetCultureInfo(CultureHelper.FrenchCulture, _auto.Create<IResourcesAccessorFactory>());

            var date = new DateTime(2017, 12, 31, 11, 58, 21);
            var formatter = new DateFormatter(_cultureAccessor, _dateBuilder);
            var resultDate = formatter.Format(date);
            var resultDateTime = formatter.Format(date, true);
            var resultDateTime24H = formatter.Format(date.AddHours(4), true);

            using (new AssertionScope())
            {
                resultDate.Should().Be("2017-12-31");
                resultDateTime.Should().Be("2017-12-31 - 11:58");
                resultDateTime24H.Should().Be("2017-12-31 - 15:58");
            }
        }

        [TestMethod]
        public void TestMethodFrenchLong()
        {
            _cultureAccessor.SetCultureInfo(CultureHelper.FrenchCulture, _auto.Create<IResourcesAccessorFactory>());

            var date = new DateTime(2017, 12, 31, 11, 58, 21);
            var formatter = new LongDateFormatter(_cultureAccessor, _dateBuilder);
            var resultDate = formatter.Format(date);
            var resultDateTime = formatter.Format(date, true);
            var resultDateTime24H = formatter.Format(date.AddHours(4), true);

            using (new AssertionScope())
            {
                resultDate.Should().Be("31 décembre 2017");
                resultDateTime.Should().Be("31 décembre 2017 - 11:58");
                resultDateTime24H.Should().Be("31 décembre 2017 - 15:58");
            }
        }
    }
}
