using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class DescriptionsProtectionsMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestMethod]
        public void AutoMappingConfigurationShouldBeValid()
        {
            AutoMapperAssertions.AssertConfigurationIsValid<PageDescriptionsProtectionsMapper.ReportProfile>();
        }

        [TestMethod]
        public void Map_ShouldMapPageDescriptionsProtectionsViewModel()
        {
            var context = Auto.Create<IReportContext>();
            var viewModel = new PageDescriptionsProtectionsViewModel();
            var model = Auto.Create<SectionDescriptionsProtectionsModel>();
            var autoMapperFactory = new AutoMapperFactory(Substitute.For<IIllustrationReportDataFormatter>(), Substitute.For<IIllustrationResourcesAccessorFactory>(), _managerFactory);
            var mapper = new PageDescriptionsProtectionsMapper(autoMapperFactory);

            mapper.Map(model, viewModel, context);

            viewModel.TitreSection.Should().Be(model.TitreSection);
            viewModel.Details.Count.Should().Be(model.Details.Count);
            viewModel.Details.First().Texte.Should().Be(model.Details.First().Texte);
            viewModel.Details.First().Textes.First().Texte.Should().NotBeEmpty();
        }
    }
}