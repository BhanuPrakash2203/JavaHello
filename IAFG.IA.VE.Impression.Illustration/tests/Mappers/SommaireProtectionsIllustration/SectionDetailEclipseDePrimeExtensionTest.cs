using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers.SommaireProtectionsIllustration
{
    [TestClass]
    public class SectionDetailEclipseDePrimeExtensionTest
    {
        private IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailEclipseDePrimeModel _model;
        private IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private IResourcesAccessor _resourcesAccessor;
        private IIllustrationReportDataFormatter _formatter;

        [TestInitialize]
        public void Initialize()
        {
            _model = new SectionDetailEclipseDePrimeModel();

            _resourcesAccessor = Substitute.For<IResourcesAccessor>();
            _resourcesAccessor.GetStringResourceById("EclipseDePrimeDescription").Returns("Description eclipse de prime");
            _resourcesAccessor.GetStringResourceById("EclipseDePrimeActivation").Returns("Activation eclipse de prime");
            _resourcesAccessor.GetStringResourceById("BaremeAlternatif").Returns("Bareme Alternatif");
            _resourcesAccessor.GetStringResourceById("BaremeCourant").Returns("Bareme Courant");

            _resourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
            _resourcesAccessorFactory.GetResourcesAccessor().Returns(_resourcesAccessor);

            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.AddColon().Returns(":");
            _formatter.FormatterPeriodeAnnees(15, null).Returns("Année 15");
            _formatter.FormatterPeriodeAnnees(27, null).Returns("Année 27");

        }

        [TestMethod]
        public void FormatterDescriptionActivationEclipseDePrime_Valid()
        {
            _model.FormatterDescriptionActivationEclipseDePrime(_formatter, _resourcesAccessorFactory).Should().Be("Activation eclipse de prime:");
        }

        [TestMethod]
        public void FormatterDiminutionBareme_WHEN_Diminution_Valid()
        {
            var bareme = new Bareme()
            {
                Diminution = 1.55
            };

            bareme.FormatterDiminutionBareme(_resourcesAccessorFactory).Should().Be("Bareme Alternatif");
        }

        [TestMethod]
        public void FormatterDiminutionBareme_WHEN_DiminutionIsNull_Valid()
        {
            var bareme = new Bareme();
            bareme.FormatterDiminutionBareme(_resourcesAccessorFactory).Should().Be("Bareme Courant");
        }

        [TestMethod]
        public void FormatterBaremes_WHEN_BaremesISNull_RETURNS_EmptyList()
        {
            var result = _model.FormatterBaremes(_formatter, _resourcesAccessorFactory);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void FormatterBaremes_WHENBaremesISNOTNull_RETURNS_Valid()
        {
            _model.Baremes = new List<Bareme>()
            {
                new Bareme(){Diminution = null, Annee = 15},
                new Bareme(){Diminution = 1.55, Annee = 27}
            };

            var result = _model.FormatterBaremes(_formatter, _resourcesAccessorFactory);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);

                result.First().Should().HaveCount(2);
                result.First().First().Should().Be("Bareme Courant");
                result.First().Skip(1).First().Should().Be("Année 15");

                result.Skip(1).First().First().Should().Be("Bareme Alternatif");
                result.Skip(1).First().Skip(1).First().Should().Be("Année 27");
            }
        }
    }
}
