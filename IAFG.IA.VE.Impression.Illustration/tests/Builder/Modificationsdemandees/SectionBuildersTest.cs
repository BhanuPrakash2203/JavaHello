using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.ModificationsDemandees
{
    [TestClass]
    public class SectionBuildersTest
    {
        private static readonly IFixture AutoFixture = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IReportContext _context = AutoFixture.Create<IReportContext>();
        private readonly ISectionContrat _reportContrat = Substitute.For<ISectionContrat>();
        private readonly ISectionProtections _reportProtections = Substitute.For<ISectionProtections>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private AutoMapperFactory _autoMapperFactory;

        private void CallReportBuilderContrat()
        {
            _reportFactory.Create<ISectionContrat>().Returns(_reportContrat);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
            var builder = new SectionContratBuilder(
                _reportFactory,
                new SectionContratMapper(_autoMapperFactory));
            var buildParam = CreateBuildParametersContrat(_parentReport);

            builder.Build(buildParam);
        }

        private BuildParameters<SectionContratModel> CreateBuildParametersContrat(IReport illustrationMasterReport)
        {
            var sectionModel = AutoFixture.Create<SectionContratModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionContratModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }

        private void CallReportBuilderProtections()
        {
            _reportFactory.Create<ISectionProtections>().Returns(_reportProtections);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);

            var builder = new SectionProtectionsBuilder(
                _reportFactory,
                new SectionProtectionsMapper(_autoMapperFactory));

            var buildParam = CreateBuildParametersProtections(_parentReport);
            builder.Build(buildParam);
        }

        private BuildParameters<SectionProtectionsModel> CreateBuildParametersProtections(IReport illustrationMasterReport)
        {
            var sectionModel = AutoFixture.Create<SectionProtectionsModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionProtectionsModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }

        [TestMethod]
        public void Builder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilderContrat();
            CallReportBuilderProtections();
            _parentReport.Received(2).AddSubReport(Arg.Any<IReport>());
        }
    }
}
