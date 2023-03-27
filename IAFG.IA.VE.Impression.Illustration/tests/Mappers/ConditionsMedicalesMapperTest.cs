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
    public class ConditionsMedicalesMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestMethod]
        public void AutoMappingConfigurationShouldBeValid()
        {
            AutoMapperAssertions.AssertConfigurationIsValid<PageConditionsMedicalesMapper.ReportProfile>();
        }

        [TestMethod]
        public void Map_ShouldMapPageConditionsMedicalesViewModel()
        {
            var context = Auto.Create<IReportContext>();
            var viewModel = new PageConditionsMedicalesViewModel();
            var model = Auto.Create<SectionConditionsMedicalesModel>();
            var autoMapperFactory = new AutoMapperFactory(Substitute.For<IIllustrationReportDataFormatter>(), Substitute.For<IIllustrationResourcesAccessorFactory>(), _managerFactory);
            var mapper = new PageConditionsMedicalesMapper(autoMapperFactory);

            mapper.Map(model, viewModel, context);

            viewModel.TitreSection.Should().Be(model.TitreSection);
            viewModel.Sections.Count.Should().Be(model.Sections.Count);
            viewModel.Sections.First().Details.Count.Should().Be(model.Sections.First().Details.Count);
            viewModel.Sections.First().Details.First().Texte.Should().Be(model.Sections.First().Details.First().Texte);
            viewModel.Sections.First().Details.First().Textes.First().Texte.Should().NotBeEmpty();
        }
    }
}