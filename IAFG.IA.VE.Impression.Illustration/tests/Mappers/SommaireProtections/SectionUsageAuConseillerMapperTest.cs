using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtections
{
    [TestClass]
    public class SectionUsageAuConseillerMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestInitialize]
        public void Initialize()
        {
            _reportDataFormatter.FormatCurrency(Arg.Any<double?>()).ReturnsForAnyArgs("un montant");
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourceAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void Map_WhenSectionPrimeWrapper_ThenShouldBeAsExpected()
        {
            var section = Auto.Create<SectionUsageAuConseillerModel>();
            var context = Auto.Create<IReportContext>();
            var subject = new SectionUsageAuConseillerMapper(_autoMapperFactory);
            var viewModel = new UsageAuConseillerViewModel();
            subject.Map(section, viewModel, context);
            viewModel.MontantNetAuRisqueViewModel.Montant.Should().Be("un montant");
            viewModel.MontantNetAuRisqueViewModel.Annee.Should().Be(section.MontantNetAuRisque.Annee.ToString());
        }
    }
}
