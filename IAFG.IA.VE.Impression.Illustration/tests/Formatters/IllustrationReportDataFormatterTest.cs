using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business;
using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.Participations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using Unity.Lifetime;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Formatters
{
    [TestClass]
    public class IllustrationReportDataFormatterTest
    {
        private IUnityContainer _container;
        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessor;
        private readonly DateTime _currentDateTime = new DateTime(2017, 12, 31, 11, 58, 21);
        private readonly DonneesRapportIllustration _donnees = Auto.Create<DonneesRapportIllustration>();
        private CultureAccessor _cultureAccessor;
        private readonly CultureInfo _cultureEnCa = new CultureInfo("en-CA", false);
        private readonly CultureInfo _cultureFrCa = new CultureInfo("fr-CA", false);
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Setup()
        {
            _cultureAccessor = new CultureAccessor();
            _container = new UnityContainer();
            _container.RegisterType<IDateBuilder, DateBuilder>();
            _container.RegisterType<IResourcesAccessor, ImpressionResourcesAccessor>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IImpressionResourcesAccessor, ImpressionResourcesAccessor>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<ICultureAccessor>(_cultureAccessor);
            _container.RegisterType<ICurrencyFormatter, CurrencyFormatter>();
            _container.RegisterType<ICurrencyWithoutDecimalFormatter, CurrencyWithoutDecimalFormatter>();
            _container.RegisterType<IDateFormatter, DateFormatter>();
            _container.RegisterType<ILongDateFormatter, LongDateFormatter>();
            _container.RegisterType<IIllustrationReportDataFormatter, IllustrationReportDataFormatter>();
            _container.RegisterType<IDecimalFormatter, DecimalFormatter>();
            _container.RegisterType<INoDecimalFormatter, NoDecimalFormatter>();
            _container.RegisterType<IPercentageFormatter, PercentageFormatter>();
            _container.RegisterType<IPercentageWithoutSymbolFormatter, PercentageWithoutSymbolFormatter>();
            _container.RegisterType<IIllustrationResourcesAccessorFactory, ResourcesAccessorFactory>();
            _container.RegisterType<IVecteurManager, VecteurManager>();

        var systemInfo = Substitute.For<ISystemInformation>();
            systemInfo.CurrentDate.Returns(_currentDateTime);
            _container.RegisterInstance(systemInfo);

            var configRepo = Substitute.For<IConfigurationRepository>();
            configRepo.ObtenirLibelleEnum<StatutTabagisme>(Arg.Any<string>()).Returns(x => x[0]);
            configRepo.ObtenirLibelleRessource(Arg.Any<string>(), Arg.Any<string>()).Returns(x => x[1]);
            configRepo.ObtenirLibelleProvince(Arg.Any<string>()).Returns(x => x[0]);
            configRepo.ObtenirNomProduit(Arg.Any<Produit>()).Returns(x => x[0].ToString());
            configRepo.ObtenirConfigurationRapport(Arg.Any<Produit>(), Arg.Any<Etat>()).Returns(new ConfigurationRapport { AgeReferenceProjection = 100 });
            _container.RegisterInstance(configRepo);

            _formatter = _container.Resolve<IIllustrationReportDataFormatter>();
            _resourcesAccessor = new ResourcesAccessorFactory(_container.Resolve<IImpressionResourcesAccessor>()) { Contexte = ResourcesContexte.Illustration };

            _donnees.Langue = Language.English;
            _donnees.Produit = Produit.CapitalValeur;
            _donnees.ProvinceEtat = ProvinceEtat.Quebec;
        }

        [TestMethod]
        public void TestFormatDate()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("12/31/2017", _formatter.FormatCurrentDate());
            Assert.AreEqual("December 31, 2017", _formatter.FormatCurrentLongDate());
            Assert.AreEqual("December 31, 2017 - 11:58 AM", _formatter.FormatCurrentLongDateTime());
            Assert.AreEqual("", _formatter.FormatDate(null));
            Assert.AreEqual("12/31/2017", _formatter.FormatDate(_currentDateTime));
            _formatter.FormatDateForPrivacy(_currentDateTime).Should().Be("**/**/2017");
            Assert.AreEqual("12/31/2017 - 11:58 AM", _formatter.FormatDate(_currentDateTime, true));
            _formatter.FormatDate(_currentDateTime, true, true).Should().Be("**/**/2017 - 11:58 AM");
            Assert.AreEqual("12/31/2017", _formatter.FormatDate(_currentDateTime, false));
            Assert.AreEqual("12/31/2017 - 03:58 PM", _formatter.FormatDate(_currentDateTime.AddHours(4), true));
            Assert.AreEqual("December 31, 2017", _formatter.FormatLongDate(_currentDateTime));
            _formatter.FormatLongDateForPrivacy(_currentDateTime).Should().Be("******** **, 2017");
            Assert.AreEqual("", _formatter.FormatLongDate(null));
            Assert.AreEqual("December 31, 2017 - 11:58 AM", _formatter.FormatLongDate(_currentDateTime, true));
            _formatter.FormatLongDate(_currentDateTime, true, true).Should().Be("******** **, 2017 - 11:58 AM");
            Assert.AreEqual("December 31, 2017", _formatter.FormatLongDate(_currentDateTime, false));
            Assert.AreEqual("December 31, 2017 - 03:58 PM", _formatter.FormatLongDate(_currentDateTime.AddHours(4), true));
            _formatter.FormatLongDate(null, true, "NonFournie").Should().Be(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonFournie"));
            _formatter.FormatLongDate(null, true, string.Empty).Should().BeEmpty();
            _formatter.FormatLongDate(null, false, "NonFournie").Should().Be(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonFournie"));
            _formatter.FormatLongDate(null, false, string.Empty).Should().BeEmpty();
            _formatter.FormatLongDate(_currentDateTime, true, "NonFournie").Should().Be("******** **, 2017");
            _formatter.FormatLongDate(_currentDateTime, false, "NonFournie").Should().Be("December 31, 2017");

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("2017-12-31", _formatter.FormatCurrentDate());
            Assert.AreEqual("31 décembre 2017", _formatter.FormatCurrentLongDate());
            Assert.AreEqual("31 décembre 2017 - 11:58", _formatter.FormatCurrentLongDateTime());
            Assert.AreEqual("", _formatter.FormatDate(null));
            _formatter.FormatDateForPrivacy(_currentDateTime).Should().Be("2017-**-**");
            Assert.AreEqual("2017-12-31", _formatter.FormatDate(_currentDateTime));
            Assert.AreEqual("2017-12-31 - 11:58", _formatter.FormatDate(_currentDateTime, true));
            _formatter.FormatDate(_currentDateTime, true, true).Should().Be("2017-**-** - 11:58");
            Assert.AreEqual("2017-12-31", _formatter.FormatDate(_currentDateTime, false));
            Assert.AreEqual("2017-12-31 - 15:58", _formatter.FormatDate(_currentDateTime.AddHours(4), true));
            Assert.AreEqual("31 décembre 2017", _formatter.FormatLongDate(_currentDateTime));
            _formatter.FormatLongDateForPrivacy(_currentDateTime).Should().Be("** ******** 2017");
            Assert.AreEqual("", _formatter.FormatLongDate(null));
            Assert.AreEqual("31 décembre 2017 - 11:58", _formatter.FormatLongDate(_currentDateTime, true));
            _formatter.FormatLongDate(_currentDateTime, true, true).Should().Be("** ******** 2017 - 11:58");
            Assert.AreEqual("31 décembre 2017", _formatter.FormatLongDate(_currentDateTime, false));
            Assert.AreEqual("31 décembre 2017 - 15:58", _formatter.FormatLongDate(_currentDateTime.AddHours(4), true));
            _formatter.FormatLongDate(null, true, "NonFournie").Should().Be(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonFournie"));
            _formatter.FormatLongDate(null, true, string.Empty).Should().BeEmpty();
            _formatter.FormatLongDate(null, false, "NonFournie").Should().Be(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonFournie"));
            _formatter.FormatLongDate(null, false, string.Empty).Should().BeEmpty();
            _formatter.FormatLongDate(_currentDateTime, true, "NonFournie").Should().Be("** ******** 2017");
            _formatter.FormatLongDate(_currentDateTime, false, "NonFournie").Should().Be("31 décembre 2017");
        }

        [TestMethod]
        public void TestFormatCurrency()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatCurrency((int?)null));
            Assert.AreEqual("", _formatter.FormatCurrency((float?)null));
            Assert.AreEqual("", _formatter.FormatCurrency((double?)null));
            Assert.AreEqual("$256.00", _formatter.FormatCurrency(256));
            Assert.AreEqual("$34.76", _formatter.FormatCurrency((float)34.76));
            Assert.AreEqual("$200,987,678.25", _formatter.FormatCurrency((double)200987678.2456));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatCurrency((int?)null));
            Assert.AreEqual("", _formatter.FormatCurrency((float?)null));
            Assert.AreEqual("", _formatter.FormatCurrency((double?)null));
            Assert.AreEqual("256,00 $", _formatter.FormatCurrency(256));
            Assert.AreEqual("34,76 $", _formatter.FormatCurrency((float)34.76));
            Assert.AreEqual("200 987 678,25 $", _formatter.FormatCurrency((double)200987678.2456));
        }

        [TestMethod]
        public void TestFormatCurrencyWithoutDecimal()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("$256", _formatter.FormatCurrencyWithoutDecimal(256));
            Assert.AreEqual("$34", _formatter.FormatCurrencyWithoutDecimal((float)34.76));
            Assert.AreEqual("$200,987,678", _formatter.FormatCurrencyWithoutDecimal((double)200987678.2456));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("256 $", _formatter.FormatCurrencyWithoutDecimal(256));
            Assert.AreEqual("34 $", _formatter.FormatCurrencyWithoutDecimal((float)34.76));
            Assert.AreEqual("200 987 678 $", _formatter.FormatCurrencyWithoutDecimal((double)200987678.2456));
        }

        [TestMethod]
        public void TestFormatPercentage()
        {
            _cultureAccessor.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            _cultureAccessor.GetCultureInfo().NumberFormat.PercentPositivePattern = 1;
            Assert.AreEqual("1%", _formatter.FormatPercentage((int)1.256));
            Assert.AreEqual("100%", _formatter.FormatPercentage((int)100.256));
            Assert.AreEqual("0%", _formatter.FormatPercentage((int)0.256));
            Assert.AreEqual("7.60%", _formatter.FormatPercentage((float)0.076));
            Assert.AreEqual("2.46%", _formatter.FormatPercentage((double)0.02456));

            _cultureAccessor.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("1 %", _formatter.FormatPercentage((int)1.256));
            Assert.AreEqual("100 %", _formatter.FormatPercentage((int)100.256));
            Assert.AreEqual("0 %", _formatter.FormatPercentage((int)0.256));
            Assert.AreEqual("7,60 %", _formatter.FormatPercentage((float)0.076));
            Assert.AreEqual("2,46 %", _formatter.FormatPercentage((double)0.02456));
        }

        [TestMethod]
        public void TestFormatPercentageWithoutSymbol()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("1", _formatter.FormatPercentageWithoutSymbol((int)1.256));
            Assert.AreEqual("100", _formatter.FormatPercentageWithoutSymbol((int)100.256));
            Assert.AreEqual("0", _formatter.FormatPercentageWithoutSymbol((int)0.256));
            Assert.AreEqual("7.60", _formatter.FormatPercentageWithoutSymbol((float)0.076));
            Assert.AreEqual("2.46", _formatter.FormatPercentageWithoutSymbol((double)0.02456));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("1", _formatter.FormatPercentageWithoutSymbol((int)1.256));
            Assert.AreEqual("100", _formatter.FormatPercentageWithoutSymbol((int)100.256));
            Assert.AreEqual("0", _formatter.FormatPercentageWithoutSymbol((int)0.256));
            Assert.AreEqual("7,60", _formatter.FormatPercentageWithoutSymbol((float)0.076));
            Assert.AreEqual("2,46", _formatter.FormatPercentageWithoutSymbol((double)0.02456));
        }

        [TestMethod]
        public void TestFormatDecimal()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("256.00", _formatter.FormatDecimal(256));
            Assert.AreEqual("34.76", _formatter.FormatDecimal((float)34.76));
            Assert.AreEqual("200,987,678.25", _formatter.FormatDecimal((double)200987678.2456));
            Assert.AreEqual("200,987,678.25", _formatter.FormatDecimal((double?)200987678.2456));
            Assert.AreEqual(string.Empty, _formatter.FormatDecimal(null));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("256,00", _formatter.FormatDecimal(256));
            Assert.AreEqual("34,76", _formatter.FormatDecimal((float)34.76));
            Assert.AreEqual("200 987 678,25", _formatter.FormatDecimal((double)200987678.2456));
            Assert.AreEqual("200 987 678,25", _formatter.FormatDecimal((double?)200987678.2456));
            Assert.AreEqual(string.Empty, _formatter.FormatDecimal(null));
        }

        [TestMethod]
        public void TestFormatNoDecimal()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("256", _formatter.FormatNoDecimal(256));
            Assert.AreEqual("34", _formatter.FormatNoDecimal((float)34.76));
            Assert.AreEqual("200,987,678", _formatter.FormatNoDecimal((double)200987678.2456));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("256", _formatter.FormatNoDecimal(256));
            Assert.AreEqual("34", _formatter.FormatNoDecimal((float)34.76));
            Assert.AreEqual("200 987 678", _formatter.FormatNoDecimal((double)200987678.2456));
        }

        [TestMethod]
        public void TestFormatFullName()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            Assert.AreEqual("Donald W. Duck", _formatter.FormatFullName("Donald", "Duck", "W."));
            Assert.AreEqual("Duck, M., Directeur", _formatter.FormatFullName(string.Empty, "Duck", string.Empty, Genre.Masculin, "Directeur"));
            Assert.AreEqual("Duck", _formatter.FormatFullName(string.Empty, "Duck", string.Empty));
            Assert.AreEqual("Donald W. Duck, Directeur", _formatter.FormatFullName("Donald", "Duck", "W.", Genre.NonDefini, "Directeur"));
            Assert.AreEqual("Donald W. Duck, M., Directeur", _formatter.FormatFullName("Donald", "Duck", "W.", Genre.Masculin, "Directeur"));
            Assert.AreEqual("Daisy W. Duck, Mme.", _formatter.FormatFullName("Daisy", "Duck", "W.", Genre.Feminin, string.Empty));
        }

        [TestMethod]
        public void TestFormatNames()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            Assert.AreEqual(string.Empty, _formatter.FormatNames(null));
            Assert.AreEqual("Donald W. Duck", _formatter.FormatNames(new List<string> { "Donald W. Duck" }));
            Assert.AreEqual("Donald W. Duck et Daisy Duck", _formatter.FormatNames(new List<string> { "Donald W. Duck", "Daisy Duck" }));
            Assert.AreEqual("Donald W. Duck, Daisy Duck et Bebe Duck", _formatter.FormatNames(new List<string> { "Donald W. Duck", "Daisy Duck", "Bebe Duck" }));
        }

        [TestMethod]
        public void TestFormatAge()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("59 years", _formatter.FormatAge(59));
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("59 ans", _formatter.FormatAge(59));
        }

        [TestMethod]
        public void TestFormatStatutTabagisme()
        {
            Assert.AreEqual(StatutTabagisme.NonApplicable.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.NonApplicable));
            Assert.AreEqual(StatutTabagisme.NonDefini.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.NonDefini));
            Assert.AreEqual(StatutTabagisme.NonFumeur.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.NonFumeur));
            Assert.AreEqual(StatutTabagisme.NonFumeurElite.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.NonFumeurElite));
            Assert.AreEqual(StatutTabagisme.NonFumeurPrivilege.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.NonFumeurPrivilege));
            Assert.AreEqual(StatutTabagisme.Fumeur.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.Fumeur));
            Assert.AreEqual(StatutTabagisme.FumeurElite.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.FumeurElite));
            Assert.AreEqual(StatutTabagisme.FumeurPrivilege.ToString(), _formatter.FormatStatutTabagisme(StatutTabagisme.FumeurPrivilege));
        }

        [TestMethod]
        public void TestFormatUsageTabac()
        {
            const string tobaccoUse = "Tobacco use: ";
            const string usageDeTabac = "Usage de tabac : ";

            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            using (new AssertionScope())
            {
                _formatter.FormatUsageTabac(StatutTabagisme.NonApplicable).Should().BeNullOrEmpty();
                _formatter.FormatUsageTabac(StatutTabagisme.NonDefini).Should().BeNullOrEmpty();
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeur).Should().Be(tobaccoUse + StatutTabagisme.NonFumeur);
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeurElite).Should().Be(tobaccoUse + StatutTabagisme.NonFumeurElite);
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeurPrivilege).Should().Be(tobaccoUse + StatutTabagisme.NonFumeurPrivilege);
                _formatter.FormatUsageTabac(StatutTabagisme.Fumeur).Should().Be(tobaccoUse + StatutTabagisme.Fumeur);
                _formatter.FormatUsageTabac(StatutTabagisme.FumeurElite).Should().Be(tobaccoUse + StatutTabagisme.FumeurElite);
                _formatter.FormatUsageTabac(StatutTabagisme.FumeurPrivilege).Should().Be(tobaccoUse + StatutTabagisme.FumeurPrivilege);
            }

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            using (new AssertionScope())
            {
                _formatter.FormatUsageTabac(StatutTabagisme.NonApplicable).Should().BeNullOrEmpty();
                _formatter.FormatUsageTabac(StatutTabagisme.NonDefini).Should().BeNullOrEmpty();
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeur).Should().Be(usageDeTabac + StatutTabagisme.NonFumeur);
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeurElite).Should().Be(usageDeTabac + StatutTabagisme.NonFumeurElite);
                _formatter.FormatUsageTabac(StatutTabagisme.NonFumeurPrivilege).Should().Be(usageDeTabac + StatutTabagisme.NonFumeurPrivilege);
                _formatter.FormatUsageTabac(StatutTabagisme.Fumeur).Should().Be(usageDeTabac + StatutTabagisme.Fumeur);
                _formatter.FormatUsageTabac(StatutTabagisme.FumeurElite).Should().Be(usageDeTabac + StatutTabagisme.FumeurElite);
                _formatter.FormatUsageTabac(StatutTabagisme.FumeurPrivilege).Should().Be(usageDeTabac + StatutTabagisme.FumeurPrivilege);
            }
        }

        [TestMethod]
        public void TestFormatSexe()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Male", _formatter.FormatSexe(Sexe.Homme, TypeAffichageSexe.Genre));
            Assert.AreEqual("Male", _formatter.FormatSexe(Sexe.Homme, TypeAffichageSexe.Sexe));
            Assert.AreEqual("Female", _formatter.FormatSexe(Sexe.Femme, TypeAffichageSexe.Genre));
            Assert.AreEqual("Female", _formatter.FormatSexe(Sexe.Femme, TypeAffichageSexe.Sexe));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.Inconnu, TypeAffichageSexe.Genre));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.Inconnu, TypeAffichageSexe.Sexe));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.NonDefini, TypeAffichageSexe.Genre));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.NonDefini, TypeAffichageSexe.Sexe));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Homme", _formatter.FormatSexe(Sexe.Homme, TypeAffichageSexe.Genre));
            Assert.AreEqual("Masculin", _formatter.FormatSexe(Sexe.Homme, TypeAffichageSexe.Sexe));
            Assert.AreEqual("Femme", _formatter.FormatSexe(Sexe.Femme, TypeAffichageSexe.Genre));
            Assert.AreEqual("Féminin", _formatter.FormatSexe(Sexe.Femme, TypeAffichageSexe.Sexe));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.Inconnu, TypeAffichageSexe.Genre));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.Inconnu, TypeAffichageSexe.Sexe));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.NonDefini, TypeAffichageSexe.Genre));
            Assert.AreEqual("", _formatter.FormatSexe(Sexe.NonDefini, TypeAffichageSexe.Sexe));
        }

        [TestMethod]
        public void TestFormatTypeAssurance()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Individual", _formatter.FormatTypeAssurance(TypeAssurance.Individuelle));
            Assert.AreEqual("Joint first to die", _formatter.FormatTypeAssurance(TypeAssurance.Conjointe1erDec));
            Assert.AreEqual("Joint last to die", _formatter.FormatTypeAssurance(TypeAssurance.ConjointeDernierDec));
            Assert.AreEqual("Joint last to die, paid-up on first death", _formatter.FormatTypeAssurance(TypeAssurance.ConjointeDernierDecLib1er));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Individuelle", _formatter.FormatTypeAssurance(TypeAssurance.Individuelle));
            Assert.AreEqual("Conjointe au 1er décès", _formatter.FormatTypeAssurance(TypeAssurance.Conjointe1erDec));
            Assert.AreEqual("Conjointe au dernier décès", _formatter.FormatTypeAssurance(TypeAssurance.ConjointeDernierDec));
            Assert.AreEqual("Conjointe au dernier décès, libérée au 1er décès", _formatter.FormatTypeAssurance(TypeAssurance.ConjointeDernierDecLib1er));
        }

        [TestMethod]
        public void TestFormatterDuree()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("until age 5", _formatter.FormatterDuree(TypeDuree.AgeMaximum, 5));
            Assert.AreEqual("Until age 5", _formatter.FormatterDuree(TypeDuree.AgeMaximum, 5, true));
            Assert.AreEqual("until YYYY-MM-DD", _formatter.FormatterDuree(TypeDuree.DateTerminaison, "YYYY-MM-DD"));
            Assert.AreEqual("Until YYYY-MM-DD", _formatter.FormatterDuree(TypeDuree.DateTerminaison, "YYYY-MM-DD", true));
            Assert.AreEqual("for 1 year", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 1));
            Assert.AreEqual("For 1 year", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 1, true));
            Assert.AreEqual("for 5 years", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 5));
            Assert.AreEqual("For 5 years", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 5, true));
            Assert.AreEqual("for 1 year", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 1));
            Assert.AreEqual("For 1 year", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 1, true));
            Assert.AreEqual("for 5 years", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 5));
            Assert.AreEqual("For 5 years", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 5, true));
            Assert.AreEqual("5 years", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 5));
            Assert.AreEqual("5 years", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 5, true));
            Assert.AreEqual("per month", _formatter.FormatterDuree(TypeDuree.ParMois, 5));
            Assert.AreEqual("Per month", _formatter.FormatterDuree(TypeDuree.ParMois, 5, true));
            Assert.AreEqual("per month for 1 year", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 1));
            Assert.AreEqual("Per month for 1 year", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 1, true));
            Assert.AreEqual("per month for 5 years", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 5));
            Assert.AreEqual("Per month for 5 years", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 5, true));
            Assert.AreEqual("For life", _formatter.FormatterDuree(TypeDuree.Vie, 5));
            Assert.AreEqual("For life", _formatter.FormatterDuree(TypeDuree.Vie, 5, true));
            Assert.AreEqual("5", _formatter.FormatterDuree(TypeDuree.NonDefini, 5));
            Assert.AreEqual("5", _formatter.FormatterDuree(TypeDuree.NonDefini, 5, true));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("jusqu'à 5 ans", _formatter.FormatterDuree(TypeDuree.AgeMaximum, 5));
            Assert.AreEqual("Jusqu'à 5 ans", _formatter.FormatterDuree(TypeDuree.AgeMaximum, 5, true));
            Assert.AreEqual("jusqu'au YYYY-MM-DD", _formatter.FormatterDuree(TypeDuree.DateTerminaison, "YYYY-MM-DD"));
            Assert.AreEqual("Jusqu'au YYYY-MM-DD", _formatter.FormatterDuree(TypeDuree.DateTerminaison, "YYYY-MM-DD", true));
            Assert.AreEqual("durant 1 an", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 1));
            Assert.AreEqual("Durant 1 an", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 1, true));
            Assert.AreEqual("durant 5 ans", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 5));
            Assert.AreEqual("Durant 5 ans", _formatter.FormatterDuree(TypeDuree.DurantNombreAnnees, 5, true));
            Assert.AreEqual("pendant 1 an", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 1));
            Assert.AreEqual("Pendant 1 an", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 1, true));
            Assert.AreEqual("pendant 5 ans", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 5));
            Assert.AreEqual("Pendant 5 ans", _formatter.FormatterDuree(TypeDuree.PendantNombreAnnees, 5, true));
            Assert.AreEqual("1 an", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 1));
            Assert.AreEqual("1 an", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 1, true));
            Assert.AreEqual("5 ans", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 5));
            Assert.AreEqual("5 ans", _formatter.FormatterDuree(TypeDuree.NombreAnnees, 5, true));
            Assert.AreEqual("par mois", _formatter.FormatterDuree(TypeDuree.ParMois, 5));
            Assert.AreEqual("Par mois", _formatter.FormatterDuree(TypeDuree.ParMois, 5, true));
            Assert.AreEqual("par mois durant 1 an", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 1));
            Assert.AreEqual("Par mois durant 1 an", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 1, true));
            Assert.AreEqual("par mois durant 5 ans", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 5));
            Assert.AreEqual("Par mois durant 5 ans", _formatter.FormatterDuree(TypeDuree.ParMoisDurantNombreAnnees, 5, true));
            Assert.AreEqual("à vie", _formatter.FormatterDuree(TypeDuree.Vie, 5));
            Assert.AreEqual("À vie", _formatter.FormatterDuree(TypeDuree.Vie, 5, true));
            Assert.AreEqual("5", _formatter.FormatterDuree(TypeDuree.NonDefini, 5));
            Assert.AreEqual("5", _formatter.FormatterDuree(TypeDuree.NonDefini, 5, true));
        }

        [TestMethod]
        public void FormatNonAssurable_WhenTrue_ThenRessourceAssigne()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            _formatter.FormatNonAssurable(true).Should().Contain("non assurable");
        }

        [TestMethod]
        public void FormatNonAssurable_WhenFalse_ThenEmpty()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            _formatter.FormatNonAssurable(false).Should().BeEmpty();
        }

        [TestMethod]
        public void FormatterPeriodeAnneesDebutFin_WhenFRCACulture_ShouldFormatProperly()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            _formatter.FormatterPeriodeAnneesDebutFin(1, 10).Should().Be("Année(s) 1 à 10");
        }

        [TestMethod]
        public void FormatterPeriodeAnneesDebutFin_WhenENCACulture_ShouldFormatProperly()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            _formatter.FormatterPeriodeAnneesDebutFin(1, 10).Should().Be("Year(s) 1 to 10");
        }

        [TestMethod]
        public void TestFormatterProvince()
        {
            Assert.AreEqual("toto", _formatter.FormatterProvince("toto"));
        }

        [TestMethod]
        public void TestFormatterRoleAssure()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Insured", _formatter.FormatterRoleAssure(false));
            Assert.AreEqual("Applicant", _formatter.FormatterRoleAssure(true));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("Assuré", _formatter.FormatterRoleAssure(false));
            Assert.AreEqual("Contractant", _formatter.FormatterRoleAssure(true));
        }

        [TestMethod]
        public void TestFormatterEnum()
        {
            Assert.AreEqual(StatutTabagisme.Fumeur.ToString(), _formatter.FormatterEnum<StatutTabagisme>(StatutTabagisme.Fumeur.ToString()));
        }

        [TestMethod]
        public void TestFormatPhoneNumber()
        {
            Assert.AreEqual("(418) 555-1234", _formatter.FormatPhoneNumber("4185551234"));
            Assert.AreEqual("(418) 555-1234", _formatter.FormatPhoneNumber("418-555-1234"));
            Assert.AreEqual("(418) 555-1234 (8888)", _formatter.FormatPhoneNumber("418-555-1234 x8888"));
            Assert.AreEqual("(418) 555-1234 (8888)", _formatter.FormatPhoneNumber("418-555-1234 abdef 8888"));
            Assert.AreEqual("1 (418) 555-1234", _formatter.FormatPhoneNumber("14185551234"));
            Assert.AreEqual("1 (418) 555-1234 (8888)", _formatter.FormatPhoneNumber("1418-555-1234 x8888"));
            Assert.AreEqual("", _formatter.FormatPhoneNumber("abcdef"));
            Assert.AreEqual("", _formatter.FormatPhoneNumber(null));
        }

        [TestMethod]
        public void TestFormatterPeriodeAges()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("age 1", _formatter.FormatterPeriodeAges(1, null));
            Assert.AreEqual("age 1", _formatter.FormatterPeriodeAges(1, 0));
            Assert.AreEqual("age 1", _formatter.FormatterPeriodeAges(1, 1));
            Assert.AreEqual("ages 1 to 99", _formatter.FormatterPeriodeAges(1, 99));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("âge 1", _formatter.FormatterPeriodeAges(1, null));
            Assert.AreEqual("âge 1", _formatter.FormatterPeriodeAges(1, 0));
            Assert.AreEqual("âge 1", _formatter.FormatterPeriodeAges(1, 1));
            Assert.AreEqual("âges 1 à 99", _formatter.FormatterPeriodeAges(1, 99));
        }

        [TestMethod]
        public void TestFormatterPeriodeAnnees()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatterPeriodeAnnees(null, null));
            Assert.AreEqual("For life", _formatter.FormatterPeriodeAnnees(null, null, true));
            Assert.AreEqual("Year 1", _formatter.FormatterPeriodeAnnees(1, null));
            Assert.AreEqual("Year 1", _formatter.FormatterPeriodeAnnees(1, 0));
            Assert.AreEqual("Year 1", _formatter.FormatterPeriodeAnnees(1, 1));
            Assert.AreEqual("Years 1 to 99", _formatter.FormatterPeriodeAnnees(1, 99));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatterPeriodeAnnees(null, null));
            Assert.AreEqual("À vie", _formatter.FormatterPeriodeAnnees(null, null, true));
            Assert.AreEqual("Année 1", _formatter.FormatterPeriodeAnnees(1, null));
            Assert.AreEqual("Année 1", _formatter.FormatterPeriodeAnnees(1, 0));
            Assert.AreEqual("Année 1", _formatter.FormatterPeriodeAnnees(1, 1));
            Assert.AreEqual("Années 1 à 99", _formatter.FormatterPeriodeAnnees(1, 99));
        }

        [TestMethod]
        public void TestFormatterPeriodes()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatterPeriode(null, null));
            Assert.AreEqual("For life", _formatter.FormatterPeriode(null, null, true));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, null));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, 0));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, 1));
            Assert.AreEqual("1 to 99", _formatter.FormatterPeriode(1, 99));

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual("", _formatter.FormatterPeriode(null, null));
            Assert.AreEqual("À vie", _formatter.FormatterPeriode(null, null, true));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, null));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, 0));
            Assert.AreEqual("1", _formatter.FormatterPeriode(1, 1));
            Assert.AreEqual("1 à 99", _formatter.FormatterPeriode(1, 99));
        }

        [TestMethod]
        public void TestAddColon()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual(":", _formatter.AddColon());

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            Assert.AreEqual(" :", _formatter.AddColon());
        }

        [TestMethod]
        public void TestFormatterNomProduit()
        {
            Assert.AreEqual(Produit.CapitalValeur.ToString(), _formatter.FormatterNomProduit(Produit.CapitalValeur));
        }

        [TestMethod]
        public void TestGetValueFormatter()
        {
            //Test que la logique prevoie toutes les valeurs possibles.
            foreach (TypeAffichageValeur item in Enum.GetValues(typeof(TypeAffichageValeur)))
            {
                Assert.IsNotNull(_formatter.GetValueFormatter(item));
            }
        }

        [TestMethod]
        public void TestFormatterDescription()
        {
            var definition = new DefinitionTitreDescription
            {
                Description = "toto",
                DescriptionEn = "titi",
                DescriptionParams = new List<ParametreTexte>()
            };

            Assert.AreEqual(definition.DescriptionEn, _formatter.FormatterDescription(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(definition.Description, _formatter.FormatterDescription(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterDescriptionAvecParams()
        {
            var definition = new DefinitionTitreDescription
            {
                Description = "Exemple de description pour un produit {0} dans la province de {1}.",
                DescriptionEn = "Example of description for product {0} in the province of {1}.",
                DescriptionParams = new List<ParametreTexte> { ParametreTexte.NomProduit, ParametreTexte.Province }
            };

            Assert.AreEqual(string.Format(definition.DescriptionEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterDescription(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Description, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterDescription(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterAvisAvecParams()
        {
            var definition = new DefinitionAvis
            {
                Texte = "Exemple d'avis pour un produit {0} dans la province de {1}.",
                TexteEn = "Example of notice for product {0} in the province of {1}.",
                Params = new List<ParametreTexte> { ParametreTexte.NomProduit, ParametreTexte.Province }
            };

            Assert.AreEqual(string.Format(definition.TexteEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterAvis(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Texte, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterAvis(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterNoteAvecParams()
        {
            var definition = new DefinitionNote
            {
                Texte = "Exemple de notes pour un produit {0} dans la province de {1}.",
                TexteEn = "Example of notes for product {0} in the province of {1}.",
                Params = new List<ParametreTexte> { ParametreTexte.NomProduit, ParametreTexte.Province }
            };

            Assert.AreEqual(string.Format(definition.TexteEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterNote(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Texte, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterNote(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterTexteAvecParams()
        {
            var definition = new DefinitionTexte
            {
                Texte = "Exemple de texte pour un produit {0} dans la province de {1}.",
                TexteEn = "Example of text for product {0} in the province of {1}.",
                TexteParams = new List<ParametreTexte> { ParametreTexte.NomProduit, ParametreTexte.Province }
            };

            Assert.AreEqual(string.Format(definition.TexteEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTexte(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Texte, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTexte(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterTexteGlossaire()
        {
            var definition = new DefinitionTexteGlossaire
            {
                Texte = "Exemple de texte de glossaire.",
                TexteEn = "Example of text for glossary."
            };

            Assert.AreEqual(definition.TexteEn, _formatter.FormatterTexte(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(definition.Texte, _formatter.FormatterTexte(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterTexteGlossaireAvecRegles()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            var definition = new DefinitionTexteGlossaire
            {
                Texte = "{0} année, boni de {1}",
                TexteEn = "{0} year, bonus of {1}",
                Regles = new List<RegleGlossaire[]>
                {
                    new[]{RegleGlossaire.BoniFideliteInvestissement}
                }
            };

            _donnees.Boni = new Boni
            {
                TauxBoni = 0.0025,
                DebutBoniFidelite = 4
            };

            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            _donnees.Langue = Language.French;
            Assert.AreEqual("4e année, boni de 0,25 %", _formatter.FormatterTexte(definition, _donnees));

            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            accessorCulture.GetCultureInfo().NumberFormat.PercentPositivePattern = 1;
            _donnees.Langue = Language.English;
            Assert.AreEqual("4th year, bonus of 0.25%", _formatter.FormatterTexte(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterTitreDescription()
        {
            var definition = new DefinitionTitreDescription
            {
                Titre = "Exemple de titre pour un produit {0} dans la province de {1}.",
                TitreEn = "Example of title for product {0} in the province of {1}.",
                TitreParams = new List<ParametreTexte> { ParametreTexte.NomProduit, ParametreTexte.Province }
            };

            Assert.AreEqual(string.Format(definition.TitreEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTitre(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Titre, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTitre(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterTitre()
        {
            var definition = new DefinitionTitre
            {
                Titre = "Exemple de titre.",
                TitreEn = "Example of title."
            };

            Assert.AreEqual(string.Format(definition.TitreEn, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTitre(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(string.Format(definition.Titre, _donnees.Produit, _donnees.ProvinceEtat),
                            _formatter.FormatterTitre(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterLibellee()
        {
            var definition = new DefinitionLibelle
            {
                Libelle = "Libelle FR",
                LibelleEn = "Libelle EN"
            };

            Assert.AreEqual(definition.LibelleEn, _formatter.FormatterLibellee(definition, new ReportContext {Language = Language.English}));
            Assert.AreEqual(definition.Libelle, _formatter.FormatterLibellee(definition, new ReportContext { Language = Language.French }));
        }

        [TestMethod]
        public void TestFormatterLibelleeGlossaire()
        {
            var definition = new DefinitionTexteGlossaire
            {
                Libelle = "Libelle FR",
                LibelleEn = "Libelle EN"
            };

            Assert.AreEqual(definition.LibelleEn, _formatter.FormatterLibellee(definition, _donnees));

            _donnees.Langue = Language.French;
            Assert.AreEqual(definition.Libelle, _formatter.FormatterLibellee(definition, _donnees));
        }

        [TestMethod]
        public void TestFormatterParamsAvecNull()
        {
            Assert.AreEqual("", _formatter.FormatterParams("", null, null));
            Assert.AreEqual("", _formatter.FormatterParams("", new List<ParametreTexte>(), null));
            Assert.AreEqual("toto", _formatter.FormatterParams("toto", new List<ParametreTexte>(), null));
            Assert.AreEqual("toto", _formatter.FormatterParams("toto", new List<ParametreTexte>() { ParametreTexte.Aucun }, null));
        }

        [TestMethod]
        public void TestFormatterParams()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            _cultureAccessor.GetCultureInfo().NumberFormat.PercentPositivePattern = 1;
            accessorCulture.GetCultureInfo().NumberFormat.PercentNegativePattern = 1;

            const int anneeDebutProjection = 1;
            const int anneeFinProjection = 2;
            const int ageFinProjection = 99;
            const int anneeDebutVersementPrime = 10;
            const int anneeFinVersementPrime = 999;

            var projections = new Projections
            {
                AgeFinProjection = ageFinProjection,
                AgeReferenceFinProjection = ageFinProjection - 1,
                AnneeDebutProjection = anneeDebutProjection,
                AnneeFinProjection = anneeFinProjection,
                Projection = new Projection
                {
                    AnneesContrat = new[] { anneeDebutVersementPrime, anneeFinVersementPrime },
                    Columns = new List<Column>
                    {
                        new Column {Id = (int) ProjectionVectorId.TotalAnnualDeposit, Value = new double[] {1001, 1002}}
                    }
                }
            };

            _donnees.Projections = projections;

            var clients = new List<Types.Models.Client>
            {
                new Types.Models.Client {Nom = "Test1", AgeAssurance = 30},
                new Types.Models.Client {Nom = "Untel", Prenom = "Jean", AgeAssurance = 30, EstContractant = true}
            };

            _donnees.Clients = clients;

            var pdfProtections = new List<Types.ProtectionPdf>
            {
                new Types.ProtectionPdf
                {
                    Boundaries = new Types.LimiteProtection
                    {
                        AmountBoundaries = new List<Types.LimiteMontant>
                        {
                            new Types.LimiteMontant
                            {
                                MaximumIssueAmount = 300000,
                                MaximumIssueAge = 70,
                                MinimumIssueAge = 18,
                                MinimumIssueAmount = 10000
                            }
                        }
                    }
                }
            };

            _donnees.ProtectionsPDF = pdfProtections;

            var boni = Auto.Create<Boni>();
            _donnees.Boni = boni;
            _donnees.Participations = new Participations
            {
                OptionParticipation = TypeOptionParticipation.Comptant,
                ReductionBaremeParticipation = -.015
            };
            _donnees.BonusRate = 0.45;

            var projectionDefavorable = new Projection
            {
                Variances = new Variances { EcartCompteInteret = -.025 }
            };

            _donnees.FondsProtectionPrincipale = new Fonds
            {
                Id = string.Empty,
                Vehicule = string.Empty,
                DefaultRate = 0.0025
            };

            //Test que la logique prevoie toutes les valeurs possibles.
            using (new AssertionScope())
            {
                foreach (ParametreTexte item in Enum.GetValues(typeof(ParametreTexte)))
                {
                    switch (item)
                    {
                        case ParametreTexte.Aucun:
                            _formatter.FormatterParams("toto", new List<ParametreTexte> { ParametreTexte.Aucun }, null).Should().Be("toto");
                            break;
                        case ParametreTexte.NomProduit:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.NomProduit }, _donnees).Should().Be($"Result: {_donnees.Produit}");
                            break;
                        case ParametreTexte.AnneeDebutProjection:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AnneeDebutProjection }, _donnees).Should().Be($"Result: {anneeDebutProjection}");
                            break;
                        case ParametreTexte.AnneeFinProjection:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AnneeFinProjection }, _donnees).Should().Be($"Result: {anneeFinProjection}");
                            break;
                        case ParametreTexte.AnneeDebutVersementPrime:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AnneeDebutVersementPrime }, _donnees).Should().Be($"Result: {anneeDebutVersementPrime}");
                            break;
                        case ParametreTexte.AnneeFinVersementPrime:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AnneeFinVersementPrime }, _donnees).Should().Be($"Result: {anneeFinVersementPrime}");
                            break;
                        case ParametreTexte.AgeFinProjection:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AgeFinProjection }, _donnees).Should().Be($"Result: {ageFinProjection}");
                            break;
                        case ParametreTexte.AgeReferenceFinProjection:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.AgeReferenceFinProjection }, _donnees).Should().Be($"Result: {ageFinProjection - 1}");
                            break;
                        case ParametreTexte.PourcentageTaxe:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.PourcentageTaxe }, _donnees).Should().Be($"Result: {0}");
                            break;
                        case ParametreTexte.Province:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.Province }, _donnees).Should().Be($"Result: {_donnees.ProvinceEtat}");
                            break;
                        case ParametreTexte.MontantAssureMaxCombine:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.MontantAssureMaxCombine }, _donnees).Should().Be($"Result: {_donnees.ProtectionsPDF.First().Boundaries.AmountBoundaries.First().MaximumIssueAmount}");
                            break;
                        case ParametreTexte.ReductionBaremeParticipations:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.ReductionBaremeParticipations }, _donnees).Should().Be("Result: 1.50%");
                            _donnees.Projections.ProjectionDefavorable = projectionDefavorable;
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.ReductionBaremeParticipations }, _donnees).Should().Be("Result: 2.50%");
                            _donnees.Projections.ProjectionDefavorable = null;
                            break;
                        case ParametreTexte.BoniCommission:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.BoniCommission }, _donnees).Should().Be("Result: 45.00%");
                            break;
                        case ParametreTexte.NomContractants:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.NomContractants }, _donnees).Should().Be("Result: Jean Untel");
                            break;
                        case ParametreTexte.TauxBoniPAR:
                            _formatter.FormatterParams("Result: {0}", new List<ParametreTexte> { ParametreTexte.TauxBoniPAR }, _donnees).Should().Be("Result: 0.25%");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        [TestMethod]
        public void TestFormatterBoniFidelite()
        {
            var accessorCulture = _container.Resolve<ICultureAccessor>();
            accessorCulture.SetCultureInfo(_cultureFrCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());

            var configurationRepository = _container.Resolve<IConfigurationRepository>();
            configurationRepository.ObtenirLibelleEnum<BoniFidelite>(Arg.Any<string>()).Returns(
                "Boni d'investissement: disponible à la fin de la {0} année, selon les critères d’admissibilité précisés au contrat");

            Assert.AreEqual("Boni d'investissement: disponible à la fin de la 5e année, selon les critères d’admissibilité précisés au contrat",
                _formatter.FormatterBoniFidelite(BoniFidelite.Regle7, 5));

            accessorCulture.SetCultureInfo(_cultureEnCa, _container.Resolve<IIllustrationResourcesAccessorFactory>());
            configurationRepository.ObtenirLibelleEnum<BoniFidelite>(Arg.Any<string>()).Returns(
                "Investment Bonus: available at the end of the {0} year, based on eligibility criteria specified in the contract");

            Assert.AreEqual("Investment Bonus: available at the end of the 5th year, based on eligibility criteria specified in the contract",
                _formatter.FormatterBoniFidelite(BoniFidelite.Regle7, 5));
        }
    }
}
