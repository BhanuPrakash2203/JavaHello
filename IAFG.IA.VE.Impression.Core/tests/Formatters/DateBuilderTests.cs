using System;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class DateBuilderTests
    {
        private DateBuilder _sujet;
        private ICultureAccessor _cultureAccessor;
        private readonly DateTime _initialDate = new DateTime(2020, 09, 11, 16, 32, 22);

        [TestInitialize]
        public void Initialize()
        {
            _cultureAccessor = Substitute.For<ICultureAccessor>();

            _sujet = new DateBuilder(_cultureAccessor);
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().Build(_initialDate);

            result.Should().Be("2020-09-11");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().Build(_initialDate);

            result.Should().Be(_initialDate.ToString("M/d/yyyy"));
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("2020-09-11");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCultureAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("9/11/2020");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureAndTime_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithTime().Build(_initialDate);

            result.Should().Be("2020-09-11 - 16:32");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCultureAndTime_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().WithTime().Build(_initialDate);

            result.Should().Be(_initialDate.ToString("M/d/yyyy - hh:mm tt"));
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureAndTimeAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("2020-09-11 - 16:32");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCultureAndTimeAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("9/11/2020 - 04:32 PM");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().Build(_initialDate);

            result.Should().Be("11 septembre 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().Build(_initialDate);

            result.Should().Be("septembre 11, 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCultureAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("11 September 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("September 11, 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCultureAndTime_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().WithTime().Build(_initialDate);

            result.Should().Be("11 septembre 2020 - 16:32");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureAndTime_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithTime().Build(_initialDate);

            result.Should().Be("septembre 11, 2020 - 04:32 ");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCultureAndTimeAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("11 September 2020 - 16:32");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureAndTimeAndInvariantCulture_ThenReturnDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("September 11, 2020 - 04:32 PM");
        }

        [TestMethod]
        public void Build_WhenShortAndLongDateFormat_ThenException()
        {
            Action act = () => _sujet.WithShortDateFormat().WithLongDateFormat().Build(_initialDate);

            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureWithPrivacy_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().Build(_initialDate);

            result.Should().Be("2020-**-**");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureWithPrivacyAndTime_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().WithTime().Build(_initialDate);

            result.Should().Be("2020-**-** - 16:32");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureWithPrivacyAndTimeAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("2020-**-** - 16:32");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenFrenchCultureWithPrivacyAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("2020-**-**");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCultureWithPrivacy_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().Build(_initialDate);

            result.Should().Be("** ******** 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenFrenchCultureWithPrivacyAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.FrenchCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("** ******** 2020");
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCultureWithPrivacy_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().Build(_initialDate);

            result.Should().Be(_initialDate.ToString("**/**/yyyy"));
        }

        [TestMethod]
        public void WithShortDateFormat_WhenDefaultCultureWithPrivacyAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithShortDateFormat().WithPrivacy().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("**/**/2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureWithPrivacy_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().Build(_initialDate);

            result.Should().Be("******** **, 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureWithPrivacyAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("******** **, 2020");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureWithPrivacyAndTime_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().WithTime().Build(_initialDate);

            result.Should().Be("******** **, 2020 - 04:32 ");
        }

        [TestMethod]
        public void WithLongDateFormat_WhenDefaultCultureWithPrivacyAndTimeAndInvariantCulture_ThenMasqueDate()
        {
            _cultureAccessor.GetCultureInfo().Returns(CultureHelper.DefaultCulture);

            var result = _sujet.WithLongDateFormat().WithPrivacy().WithTime().WithInvariantCulture().Build(_initialDate);

            result.Should().Be("******** **, 2020 - 04:32 PM");
        }
    }
}