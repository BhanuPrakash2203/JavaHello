using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageModificationsDemandeesBuilderTest
    {
        private static readonly IFixture AutoFixture = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IReportContext _context = AutoFixture.Create<IReportContext>();
        private readonly IPageModificationsDemandees _report = Substitute.For<IPageModificationsDemandees>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly ISectionContratBuilder _sectionContratBuilder = Substitute.For<ISectionContratBuilder>();
        private readonly ISectionProtectionsBuilder _sectionProtectionsLBuilder = Substitute.For<ISectionProtectionsBuilder>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;
        
        private void CallReportBuilder()
        {
            _reportFactory.Create<IPageModificationsDemandees>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
            var builder = new PageModificationsDemandeesBuilder(
                _reportFactory,
                new PageModificationsDemandeesMapper(_autoMapperFactory),
                _sectionContratBuilder,
                _sectionProtectionsLBuilder);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
        }

        private BuildParameters<SectionModificationsDemandeesModel> CreateBuildParameters(IReport illustrationMasterReport)
        {
            var sectionModel = AutoFixture.Create<SectionModificationsDemandeesModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionModificationsDemandeesModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }


        [TestMethod]
        public void PageResultatBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();
            _parentReport.Received(1).AddSubReport(_report);
        }
    }
}
