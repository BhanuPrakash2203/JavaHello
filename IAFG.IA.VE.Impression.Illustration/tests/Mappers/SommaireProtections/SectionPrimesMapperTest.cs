using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtections
{
    [TestClass]
    public class SectionPrimesMapperTest
    {
        private const string LibelleTitreColonnePrimesVersees = "Primes et contributions mensuelles illustrées¹";

        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestInitialize]
        public void Initialize()
        {
            _resourceAccessorFactory.GetResourcesAccessor().GetStringResourceById(LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionneesPAR).Returns(LibelleTitreColonnePrimesVersees);
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void Map_WhenSectionPrimeWrapper_ThenShouldBeAsExpected()
        {           
            var section = Auto.Create<SectionPrimesModel>();
            section.FrequenceFacturation = TypeFrequenceFacturation.Annuelle;
            section.TitreColonnePrimesVersees = LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionneesPAR;

            var context = Auto.Create<IReportContext>();
            var subject = new SectionPrimesMapper(_autoMapperFactory);
            var viewModel = new ProtectionPrimesViewModel();
            subject.Map(section, viewModel, context);
            viewModel.FrequenceFacturation.Should().Be(TypeFrequenceFacturation.Annuelle);
            viewModel.TitreColonnePrimesVersees.Should().Be(LibelleTitreColonnePrimesVersees);
        }
    }
}
