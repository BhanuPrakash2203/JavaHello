using System.Globalization;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class PercentageFormatterTest
    {
        private const string EN_CA_CULTURE = "en-CA";
        private const string FR_CA_CULTURE = "fr-CA";
        private const double DOUBLE_VALUE_TO_FORMAT_100 = 100.0;
        private const double DOUBLE_VALUE_TO_FORMAT_50 = 50.0;
        private const double DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_0 = 0.0;
        private const double DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_50 = 0.5;
        private const double DOUBLE_VALUE_ONE_TO_FORMAT_100 = 1.0;
        private const string EXPECTED_FORMATTED_VALUE_EMPTY = "";
        private const float FLOAT_VALUE_TO_FORMAT_100 = 100.0f;
        private const float FLOAT_VALUE_TO_FORMAT_50 = 50.0f;
        private const float FLOAT_VALUE_UNDER_ONE_TO_FORMAT_0 = 0.0f;
        private const float FLOAT_VALUE_UNDER_ONE_TO_FORMAT_50 = 0.5f;
        private const int INT_VALUE_TO_FORMAT_0 = 0;
        private const int INT_VALUE_TO_FORMAT_1 = 1;
        private const int INT_VALUE_TO_FORMAT_100 = 100;
        private const int INT_VALUE_TO_FORMAT_50 = 50;
        private const string STRING_VALUE_TO_FORMAT_0 = "0";
        private const string STRING_VALUE_TO_FORMAT_100 = "100";
        private const string STRING_VALUE_TO_FORMAT_50 = "50";
        private readonly string EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS = "50%";
        private readonly string EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS = "100%";
        private readonly string EXPECTED_FORMATTED_VALUE_0_NO_DECIMALS = "0%";
        private readonly string EXPECTED_FORMATTED_VALUE_1_NO_DECIMALS = "1%";
        private readonly string EXPECTED_FORMATTED_VALUE_50 = "50.00%";
        private readonly string EXPECTED_FORMATTED_VALUE_100 = "100.00%";
        private readonly string EXPECTED_FORMATTED_VALUE_0 = "0.00%";
        private IDateBuilder _dateBuilder;
        private ICultureAccessor _cultureAccessor;
        private IPercentageFormatter _percentageFormatter;

        private static readonly IFixture _auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Setup()
        {
            _cultureAccessor = new CultureAccessor();
            _dateBuilder = new DateBuilder(_cultureAccessor);
            _percentageFormatter = new PercentageFormatter(_cultureAccessor, _dateBuilder);

            _cultureAccessor.SetCultureInfo(EN_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());
            _cultureAccessor.GetCultureInfo().NumberFormat.PercentPositivePattern = 1;
        }

        [TestMethod]
        public void TestMethodEnglish()
        {
            var resultIntUn = _percentageFormatter.Format((int)1.256);
            var resultIntZero = _percentageFormatter.Format((int)0.256);
            var resultIntCent = _percentageFormatter.Format((int)100.256);
            var resultFloat = _percentageFormatter.Format((float)0.076, false);
            var resultDouble = _percentageFormatter.Format((double)0.02456, false);

            Assert.AreEqual("1%", resultIntUn);
            Assert.AreEqual("0%", resultIntZero);
            Assert.AreEqual("100%", resultIntCent);
            Assert.AreEqual("7.60%", resultFloat);
            Assert.AreEqual("2.46%", resultDouble);
        }

        [TestMethod]
        public void TestMethodFrench()
        {
            _cultureAccessor.SetCultureInfo(FR_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());
            _cultureAccessor.GetCultureInfo().NumberFormat.PercentDecimalSeparator = ",";

            var resultIntUn = _percentageFormatter.Format((int)1.256);
            var resultIntZero = _percentageFormatter.Format((int)0.256);
            var resultIntCent = _percentageFormatter.Format((int)100.256);
            var resultFloat = _percentageFormatter.Format((float)0.076, false);
            var resultDouble = _percentageFormatter.Format((double)0.02456, false);

            Assert.AreEqual("1 %", resultIntUn);
            Assert.AreEqual("0 %", resultIntZero);
            Assert.AreEqual("100 %", resultIntCent);
            Assert.AreEqual("7,60 %", resultFloat);
            Assert.AreEqual("2,46 %", resultDouble);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne0WithoutDecimals_ThenStringUnderOne0()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_0, DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_0 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne0WithDecimals_ThenStringUnderOne0()
        {
            var formattedValue = _percentageFormatter.Format(DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_0, DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_0 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne50WithoutDecimals_ThenStringUnderOne50()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_50, DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne50WithDecimals_ThenStringUnderOne50()
        {
            var formattedValue = _percentageFormatter.Format(DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_50, DOUBLE_VALUE_UNDER_ONE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleIsOneWithoutDecimals_ThenString100()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(DOUBLE_VALUE_ONE_TO_FORMAT_100, DOUBLE_VALUE_ONE_TO_FORMAT_100 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne100WithoutDecimals_ThenStringUnderOne100()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(DOUBLE_VALUE_TO_FORMAT_100, DOUBLE_VALUE_TO_FORMAT_100 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDoubleUnderOne100WithDecimals_ThenStringUnderOne100()
        {
            var formattedValue = _percentageFormatter.Format(DOUBLE_VALUE_TO_FORMAT_100, DOUBLE_VALUE_TO_FORMAT_100 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDouble50WithoutDecimals_ThenString50()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(DOUBLE_VALUE_TO_FORMAT_50, DOUBLE_VALUE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenDouble50WithDecimals_ThenString50()
        {
            var formattedValue = _percentageFormatter.Format(DOUBLE_VALUE_TO_FORMAT_50, DOUBLE_VALUE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloatUnderOne0WithoutDecimals_ThenStringUnderOne0()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(FLOAT_VALUE_UNDER_ONE_TO_FORMAT_0, FLOAT_VALUE_UNDER_ONE_TO_FORMAT_0 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloatUnderOne0WithDecimals_ThenStringUnderOne0()
        {
            var formattedValue = _percentageFormatter.Format(FLOAT_VALUE_UNDER_ONE_TO_FORMAT_0, FLOAT_VALUE_UNDER_ONE_TO_FORMAT_0 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloatUnderOne50WithoutDecimals_ThenStringUnderOne50()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(FLOAT_VALUE_UNDER_ONE_TO_FORMAT_50, FLOAT_VALUE_UNDER_ONE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloatUnderOne50WithDecimals_ThenStringUnderOne50()
        {
            var formattedValue = _percentageFormatter.Format(FLOAT_VALUE_UNDER_ONE_TO_FORMAT_50, FLOAT_VALUE_UNDER_ONE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloat100WithoutDecimals_ThenString100()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(FLOAT_VALUE_TO_FORMAT_100, FLOAT_VALUE_TO_FORMAT_100 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloat100WithDecimals_ThenString100()
        {
            var formattedValue = _percentageFormatter.Format(FLOAT_VALUE_TO_FORMAT_100, FLOAT_VALUE_TO_FORMAT_100 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloat50WithoutDecimals_ThenString50()
        {
            var formattedValue = _percentageFormatter.FormatWithoutDecimals(FLOAT_VALUE_TO_FORMAT_50, FLOAT_VALUE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenFloat50WithDecimals_ThenString50()
        {
            var formattedValue = _percentageFormatter.Format(FLOAT_VALUE_TO_FORMAT_50, FLOAT_VALUE_TO_FORMAT_50 > 1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50, formattedValue);
        }

        [TestMethod]
        public void Format_WhenStringUnderOne0_ThenStringUnderOne0()
        {
            var formattedValue = _percentageFormatter.Format(STRING_VALUE_TO_FORMAT_0, true);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenStringUnderOne50_ThenStringUnderOne50()
        {
            var formattedValue = _percentageFormatter.Format(0.5.ToString(CultureInfo.CurrentCulture), false);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenString50_ThenString50()
        {
            var formattedValue = _percentageFormatter.Format(STRING_VALUE_TO_FORMAT_50, true);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenString100_ThenString100()
        {
            var formattedValue = _percentageFormatter.Format(STRING_VALUE_TO_FORMAT_100, true);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenValueStringEmpty_ThenStringEmpty()
        {
            var formattedValueVide = _percentageFormatter.Format(EXPECTED_FORMATTED_VALUE_EMPTY, true);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_EMPTY, formattedValueVide);
        }

        [TestMethod]
        public void Format_WhenValueDoubleNull_ThenStringEmpty()
        {
            var formattedValueNull = _percentageFormatter.Format(null, true);

            formattedValueNull.Should().BeEmpty();
        }

        [TestMethod]
        public void Format_WhenInt0_ThenString0()
        {
            var formattedValue = _percentageFormatter.Format(INT_VALUE_TO_FORMAT_0);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_0_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenInt1_ThenString1()
        {
            var formattedValue = _percentageFormatter.Format(INT_VALUE_TO_FORMAT_1);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_1_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenInt50_ThenString50()
        {
            var formattedValue = _percentageFormatter.Format(INT_VALUE_TO_FORMAT_50);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_50_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenInt100_ThenString100()
        {
            var formattedValue = _percentageFormatter.Format(INT_VALUE_TO_FORMAT_100);

            Assert.AreEqual(EXPECTED_FORMATTED_VALUE_100_NO_DECIMALS, formattedValue);
        }

        [TestMethod]
        public void Format_WhenValeurInvalide_ThenReturnValueUnchanged()
        {
            var formattedValue = _percentageFormatter.Format("abc", true);

            Assert.AreEqual("abc", formattedValue);
        }
    }
}
