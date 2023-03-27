using System.Collections.Generic;
using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConditionsMedicales;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageConditionsMedicalesBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionTexteDescriptionBuilder _sectionTexteDescriptionBuilder = Substitute.For<ISectionTexteDescriptionBuilder>();
        private readonly IPageConditionsMedicales _report = Substitute.For<IPageConditionsMedicales>();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestMethod]
        public void ShouldAddItselfToParentReport()
        {
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
            var mapper = new PageConditionsMedicalesMapper(_autoMapperFactory);
            _reportFactory.Create<IPageConditionsMedicales>().Returns(_report);

            var builder = new PageConditionsMedicalesBuilder(_reportFactory, mapper, _sectionTexteDescriptionBuilder);
            var buildParameters = CreateBuildParameters(_parentReport);

            builder.Build(buildParameters);

            _parentReport.Received(1).AddSubReport(_report);
            _sectionTexteDescriptionBuilder.Received(2).Build(Arg.Any<BuildParameters<ConditionMedicaleViewModel>>());
        }

        private BuildParameters<SectionConditionsMedicalesModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var conditionsMedicalesModel = Auto.Create<SectionConditionsMedicalesModel>();
            var detail1 = Auto.Create<ConditionMedicale>();
            var detail2 = Auto.Create<ConditionMedicale>();
            conditionsMedicalesModel.Sections.Clear();

            var detailConditionsMedicalesModel = Auto.Create<ConditionsMedicalesSection>();
            detailConditionsMedicalesModel.Details = new List<ConditionMedicale> {detail1,detail2};
            conditionsMedicalesModel.Sections.Add(detailConditionsMedicalesModel);

            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionConditionsMedicalesModel>(conditionsMedicalesModel)
                   {
                       ParentReport = illustrationMasterReport,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}