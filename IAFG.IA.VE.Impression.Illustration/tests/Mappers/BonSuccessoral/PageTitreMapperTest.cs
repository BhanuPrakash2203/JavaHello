using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers.BonSuccessoral
{
    [TestClass]
    public class PageTitreMapperTest
    {
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestInitialize]
        public void Initialize()
        {
            _resourceAccessorFactory.Contexte = ResourcesContexte.Illustration;
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void ShouldMapGlossaire()
        {
            var section = Auto.Create<TitreRapportModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();

            var subject = new PageTitreMapper(_autoMapperFactory);
            var viewModel = new PageTitreViewModel();

            subject.Map(section, viewModel, context);
            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
        }
    }
}