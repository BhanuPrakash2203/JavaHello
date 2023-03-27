using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Test.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.HypothesesInvestissement
{
    [TestClass]
    public class SectionFondsCapitalisationMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestMethod]
        public void AutoMappingConfigurationShouldBeValid()
        {
            AutoMapperAssertions.AssertConfigurationIsValid<SectionFondsCapitalisationMapper.ReportProfile>();
        }

        [TestMethod]
        public void Map_ShouldMapFondsCapitalisationViewModel()
        {
            var context = Auto.Create<IReportContext>();
            var viewModel = new FondsCapitalisationViewModel();
            var model = Auto.Create<SectionFondsCapitalisationModel>();
            var autoMapperFactory = new AutoMapperFactory(Substitute.For<IIllustrationReportDataFormatter>(), Substitute.For<IIllustrationResourcesAccessorFactory>(), _managerFactory);
            var mapper = new SectionFondsCapitalisationMapper(autoMapperFactory);
            
            mapper.Map(model, viewModel, context);

            viewModel.TitreSection.Should().Be(model.TitreSection);
            viewModel.Fonds.Should().HaveCount(model.Fonds.Count);
            viewModel.Avis.Should().HaveCount(model.Avis.Count);
        }
    }
}