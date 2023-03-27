using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtectionsIllustration
{
    [TestClass]
    public class DetailPrimesVerseesMapperExtensionTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        private IIllustrationReportDataFormatter _illustrationReportDataFormatter;
        private IIllustrationResourcesAccessorFactory _illustrationResourcesAccessorFactory;

        [TestInitialize]
        public void Setup()
        {
            _illustrationReportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
            _illustrationResourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        }

        [TestMethod]
        public void FormatterDescription_WhenFacteurMultiplicateurIsGreaterThanZeroAndTypeScenarioPrimeIsMinimale_ThenReturnAppropriateDescription()
        {
            string xMinimale = "{0} X Minimale";
            string formattedMultiplicateur = "1.00";

            _illustrationResourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("XMinimale").Returns(xMinimale);
            _illustrationReportDataFormatter.FormatDecimal((double)1).Returns(formattedMultiplicateur);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { FacteurMultiplicateur = 1, TypeScenarioPrime = TypeScenarioPrime.Variable_Minimale };

            string description = detailPrimesVersees.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            description.Should().Be(string.Format(xMinimale, formattedMultiplicateur));
        }

        [TestMethod]
        public void FormatterDescription_WhenFacteurMultiplicateurIsGreaterThanZeroAndTypeScenarioPrimeIsReference_ThenReturnAppropriateDescription()
        {
            string xReference = "{0} X Référence";
            string formattedMultiplicateur = "1.00";

            _illustrationResourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("XReference").Returns(xReference);
            _illustrationReportDataFormatter.FormatDecimal((double)1).Returns(formattedMultiplicateur);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { FacteurMultiplicateur = 1, TypeScenarioPrime = TypeScenarioPrime.Variable_Reference };

            string description = detailPrimesVersees.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            description.Should().Be(string.Format(xReference, formattedMultiplicateur));
        }

        [TestMethod]
        public void FormatterDescription_WhenFacteurMultiplicateurIsZero_ThenReturnFormattedTypeScenarioPrime()
        {
            string description = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(TypeScenarioPrime.Variable_Minimale.ToString()).Returns(description);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { FacteurMultiplicateur = 0, TypeScenarioPrime = TypeScenarioPrime.Variable_Minimale };

            string result = detailPrimesVersees.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(description);
        }

        [DataTestMethod]
        [DynamicData(nameof(TypeScenariosPrimeWithoutMinimaleOrReference), DynamicDataSourceType.Method)]
        public void FormatterDescription_WhenTypeScenarioPrimeIsNotMinimaleOrReference_ThenReturnFormattedTypeScenarioPrime(TypeScenarioPrime typeScenarioPrime)
        {
            string description = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(typeScenarioPrime.ToString()).Returns(description);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { FacteurMultiplicateur = 1, TypeScenarioPrime = typeScenarioPrime };

            string result = detailPrimesVersees.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(description);
        }

        [TestMethod]
        public void FormatterMontant_ThenReturnAppropriateFormattedMontant()
        {
            double? montant = Auto.Create<double>();
            var formattedMontant = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatCurrency(montant).Returns(formattedMontant);
            var detailPrimesVersees = new DetailPrimesVersees {TypeScenarioPrime = TypeScenarioPrime.Modale, Montant = montant };

            var result = detailPrimesVersees.FormatterMontant(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(formattedMontant);
        }

        [TestMethod]
        public void FormatterMontantAvecODS_ThenReturnAppropriateFormattedMontant()
        {
            double? montant = Auto.Create<double>();
            var formattedMontant = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatCurrency(montant).Returns(formattedMontant);
            _illustrationResourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("Prime").Returns("Prime");
            var detailPrimesVersees = new DetailPrimesVersees { TypeScenarioPrime = TypeScenarioPrime.ModalePlusODE, Montant = montant };

            var result = detailPrimesVersees.FormatterMontant(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be("Prime + " + formattedMontant);
        }

        [TestMethod]
        public void FormatterPeriode_WhenDureeHasValue_ThenReturnAppropriateFormattedPeriode()
        {
            int annee = 2019;
            int? duree = Auto.Create<int>();
            string formattedPeriode = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterPeriodeAnneesDebutFin(annee, duree.Value).Returns(formattedPeriode);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { Duree = duree, Annee = annee };

            string result = detailPrimesVersees.FormatterPeriode(_illustrationReportDataFormatter);

            result.Should().Be(formattedPeriode.FirstCharToUpper());
        }

        [TestMethod]
        public void FormatterPeriode_WhenDureeHasNoValue_ThenReturnAppropriateFormattedPeriode()
        {
            int annee = 2019;
            int? mois = 12;
            string formattedPeriode = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterPeriodeAnneeMois(annee, mois).Returns(formattedPeriode);
            DetailPrimesVersees detailPrimesVersees = new DetailPrimesVersees { Duree = null, Annee = annee, Mois = mois };

            string result = detailPrimesVersees.FormatterPeriode(_illustrationReportDataFormatter);

            result.Should().Be(formattedPeriode.FirstCharToUpper());
        }

        [TestMethod]
        public void FirstCharToUpper_WhenStringIsEmpty_ThenReturnEmpty()
        {
            string s = string.Empty;

            string result = s.FirstCharToUpper();

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FirstCharToUpper_WhenStringIsNotEmpty_ThenReturnAppropriateString()
        {
            string s = "anyString";

            string result = s.FirstCharToUpper();

            result.Should().Be("AnyString");
        }

        private static IEnumerable<object[]> TypeScenariosPrimeWithoutMinimaleOrReference()
        {
            var typeScenariosPrimeWithoutMinimaleOrReference =
                ((TypeScenarioPrime[])Enum.GetValues(typeof(TypeScenarioPrime))).Where(x =>
                    x != TypeScenarioPrime.Variable_Reference && x != TypeScenarioPrime.Variable_Minimale);
            foreach (var scenarioPrime in typeScenariosPrimeWithoutMinimaleOrReference)
            {
                yield return new object[] { scenarioPrime };
            }
        }
    }
}
