using System;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class LongDateFormatterTest
    {
        private const string FRENCH_DATE_FORMAT = "d MMMM yyyy";
        private const string DEFAULT_DATE_FORMAT = "MMMM d, yyyy";
        private const string FRENCH_DATEHEURE_FORMAT = "d MMMM yyyy - HH:mm";
        private const string DEFAULT_DATEHEURE_FORMAT = "MMMM d, yyyy - hh:mm tt";

        private const string EN_CA = "en-CA";
        private const string FR_CA = "fr-CA";

        private static readonly IFixture _auto = AutoFixtureFactory.Create();

        private readonly ICultureAccessor _cultureAccessorMock = Substitute.For<ICultureAccessor>();
        private ILongDateFormatter _subject;

        [TestInitialize]
        public void Initialize()
        {
            _subject = new LongDateFormatter(_cultureAccessorMock, new DateBuilder(_cultureAccessorMock));
        }

        [TestMethod]
        public void Format_WhenOneParameterAndEnCulture_ThenReturnDateWithNoTimeEnFormat()
        {
            CultureSwitcher.SwitchTo(EN_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(EN_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate);

            result.Should().Be(aDate.ToString(DEFAULT_DATE_FORMAT));
        }

        [TestMethod]
        public void Format_WhenOneParameterAndFrCulture_ThenReturnDateWithNoTimeFrFormat()
        {
            CultureSwitcher.SwitchTo(FR_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(FR_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate);

            result.Should().Be(aDate.ToString(FRENCH_DATE_FORMAT));
        }

        [TestMethod]
        public void Format_WhenTwoParametersAndInlcudeTimeFalseAndEnCulture_ThenReturnDateWithNoTimeEnFormat()
        {
            CultureSwitcher.SwitchTo(EN_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(EN_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate, false);

            result.Should().Be(aDate.ToString(DEFAULT_DATE_FORMAT));
        }

        [TestMethod]
        public void Format_WhenTwoParametersAndInlcudeTimeFalseAndFrCulture_ThenReturnDateWithNoTimeFrFormat()
        {
            CultureSwitcher.SwitchTo(FR_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(FR_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate, false);

            result.Should().Be(aDate.ToString(FRENCH_DATE_FORMAT));
        }

        [TestMethod]
        public void Format_WhenTwoParametersAndIncludeTimeTrueAndEnCulture_ThenReturnDateWithTimeEnFormat()
        {
            CultureSwitcher.SwitchTo(EN_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(EN_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate, true);

            result.Should().Be(aDate.ToString(DEFAULT_DATEHEURE_FORMAT));
        }

        [TestMethod]
        public void Format_WhenTwoParametersAndIncludeTimeTrueAndFrCulture_ThenReturnDateWithTimeFrFormat()
        {
            CultureSwitcher.SwitchTo(FR_CA);
            _cultureAccessorMock.GetCultureInfo().Returns(new CultureInfo(FR_CA));

            DateTime aDate = _auto.Create<DateTime>();
            string result = _subject.Format(aDate, true);

            result.Should().Be(aDate.ToString(FRENCH_DATEHEURE_FORMAT));
        }
    }
}
