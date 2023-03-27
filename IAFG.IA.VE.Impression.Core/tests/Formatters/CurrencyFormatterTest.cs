using System.Threading;
using AutoFixture;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class CurrencyFormatterTest
    {
        private const string EN_CA_CULTURE = "en-CA";
        private const string FR_CA_CULTURE = "fr-CA";
        private IDateBuilder _dateBuilder;
        private ICultureAccessor _cultureAccessor;

        private static readonly IFixture _auto = AutoFixtureFactory.Create();


        [TestInitialize]
        public void Setup()
        {
            _cultureAccessor = new CultureAccessor();
            _dateBuilder = new DateBuilder(_cultureAccessor);
        }

        [TestMethod]
        public void TestMethodEnglish()
        {
            _cultureAccessor.SetCultureInfo(EN_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());

            var formatter = new CurrencyFormatter(_cultureAccessor, _dateBuilder);
            var resultInt = formatter.Format((int)256);
            var resultFloat = formatter.Format((float)34.76);
            var resultDouble = formatter.Format((double)200987678.2456);

            Assert.AreEqual("$256.00", resultInt);
            Assert.AreEqual("$34.76", resultFloat);
            Assert.AreEqual("$200,987,678.25", resultDouble);
        }

        [TestMethod]
        public void TestMethodEnglishWithoutDecimal()
        {
            _cultureAccessor.SetCultureInfo(EN_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());

            var formatter = new CurrencyWithoutDecimalFormatter(_cultureAccessor, _dateBuilder);
            var resultInt = formatter.Format((int)256);
            var resultFloat = formatter.Format((float)34.76);
            var resultDouble = formatter.Format((double)200987678.2456);

            Assert.AreEqual("$256", resultInt);
            Assert.AreEqual("$34", resultFloat);
            Assert.AreEqual("$200,987,678", resultDouble);
        }

        [TestMethod]
        public void TestMethodFrench()
        {
            _cultureAccessor.SetCultureInfo(FR_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());

            var formatter = new CurrencyFormatter(_cultureAccessor, _dateBuilder);
            var resultInt = formatter.Format((int)256);
            var resultFloat = formatter.Format((float)34.76);
            var resultDouble = formatter.Format((double)200987678.2456);

            var decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            Assert.AreEqual("256.00 $".Replace(".", decimalSeparator), resultInt);
            Assert.AreEqual("34.76 $".Replace(".", decimalSeparator), resultFloat);
            Assert.AreEqual("200 987 678.25 $".Replace(".", decimalSeparator), resultDouble);
        }

        [TestMethod]
        public void TestMethodFrenchWithoutDecimal()
        {
            _cultureAccessor.SetCultureInfo(FR_CA_CULTURE, _auto.Create<IResourcesAccessorFactory>());

            var formatter = new CurrencyWithoutDecimalFormatter(_cultureAccessor, _dateBuilder);
            var resultInt = formatter.Format((int)256);
            var resultFloat = formatter.Format((float)34.76);
            var resultDouble = formatter.Format((double)200987678.2456);

            Assert.AreEqual("256 $", resultInt);
            Assert.AreEqual("34 $", resultFloat);
            Assert.AreEqual("200 987 678 $", resultDouble);
        }
    }
}
