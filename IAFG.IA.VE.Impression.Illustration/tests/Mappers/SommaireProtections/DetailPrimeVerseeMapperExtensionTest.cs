using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtections
{
    [TestClass]
    public class DetailPrimeVerseeMapperExtensionTest
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
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { FacteurMultiplicateur = 1, TypeScenarioPrime = TypeScenarioPrime.Variable_Minimale };

            string description = detailPrimeVersee.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            description.Should().Be(string.Format(xMinimale, formattedMultiplicateur));
        }

        [TestMethod]
        public void FormatterDescription_WhenFacteurMultiplicateurIsGreaterThanZeroAndTypeScenarioPrimeIsReference_ThenReturnAppropriateDescription()
        {
            string xReference = "{0} X Référence";
            string formattedMultiplicateur = "1.00";

            _illustrationResourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("XReference").Returns(xReference);
            _illustrationReportDataFormatter.FormatDecimal((double)1).Returns(formattedMultiplicateur);
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { FacteurMultiplicateur = 1, TypeScenarioPrime = TypeScenarioPrime.Variable_Reference };

            string description = detailPrimeVersee.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            description.Should().Be(string.Format(xReference, formattedMultiplicateur));
        }

        [TestMethod]
        public void FormatterDescription_WhenFacteurMultiplicateurIsZero_ThenReturnFormattedTypeScenarioPrime()
        {
            string description = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(TypeScenarioPrime.Variable_Minimale.ToString()).Returns(description);
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { FacteurMultiplicateur = 0, TypeScenarioPrime = TypeScenarioPrime.Variable_Minimale };

            string result = detailPrimeVersee.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(description);
        }

        [DataTestMethod]
        [DynamicData(nameof(TypeScenariosPrimeWithoutMinimaleOrReference), DynamicDataSourceType.Method)]
        public void FormatterDescription_WhenTypeScenarioPrimeIsNotMinimaleOrReference_ThenReturnFormattedTypeScenarioPrime(TypeScenarioPrime typeScenarioPrime)
        {
            string description = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(typeScenarioPrime.ToString()).Returns(description);
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { FacteurMultiplicateur = 1, TypeScenarioPrime = typeScenarioPrime };

            string result = detailPrimeVersee.FormatterDescription(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(description);
        }

        [TestMethod]
        public void FormatterMontant_ThenReturnAppropriateFormattedMontant()
        {
            double? montant = Auto.Create<double>();
            var formattedMontant = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatCurrency(montant).Returns(formattedMontant);
            var detailPrimeVersee = new DetailPrimeVersee { TypeScenarioPrime = TypeScenarioPrime.Modale, Montant = montant };
            var result = detailPrimeVersee.FormatterMontant(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be(formattedMontant);
        }

        [TestMethod]
        public void FormatterMontantAvecODS_ThenReturnAppropriateFormattedMontant()
        {
            double? montant = Auto.Create<double>();
            var formattedMontant = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatCurrency(montant).Returns(formattedMontant);
            _illustrationResourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("Prime").Returns("Prime");
            var detailPrimesVersees = new DetailPrimeVersee { TypeScenarioPrime = TypeScenarioPrime.ModalePlusODE, Montant = montant };

            var result = detailPrimesVersees.FormatterMontant(_illustrationReportDataFormatter, _illustrationResourcesAccessorFactory);

            result.Should().Be("Prime + " + formattedMontant);
        }

        [TestMethod]
        public void FormatterPeriode_WhenDureeHasValue_ThenReturnAppropriateFormattedPeriode()
        {
            int? duree = Auto.Create<int>();
            string formattedPeriode = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterDuree(TypeDuree.PendantNombreAnnees, duree.Value).Returns(formattedPeriode);
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { Duree = duree };

            string result = detailPrimeVersee.FormatterPeriode(_illustrationReportDataFormatter);

            result.Should().Be(formattedPeriode);
        }

        [TestMethod]
        public void FormatterPeriode_WhenDureeHasNoValue_ThenReturnAppropriateFormattedPeriode()
        {
            int annee = 2019;
            int? mois = 12;
            string formattedPeriode = Auto.Create<string>();
            _illustrationReportDataFormatter.FormatterPeriodeAnneeMois(annee, mois).Returns(formattedPeriode);
            DetailPrimeVersee detailPrimeVersee = new DetailPrimeVersee { Duree = null, Annee = annee, Mois = mois };

            string result = detailPrimeVersee.FormatterPeriode(_illustrationReportDataFormatter);

            result.Should().Be(formattedPeriode);
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
