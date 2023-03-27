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
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.DescriptionsProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageDescriptionsProtectionsBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionDescriptionBuilder _sectionDescriptionBuilder = Substitute.For<ISectionDescriptionBuilder>();
        private readonly IPageDescriptionsProtections  _report = Substitute.For<IPageDescriptionsProtections>();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestMethod]
        public void ShouldAddItselfToParentReport()
        {
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
            var mapper = new PageDescriptionsProtectionsMapper(_autoMapperFactory);
            _reportFactory.Create<IPageDescriptionsProtections>().Returns(_report);

            var builder = new PageDescriptionsProtectionsBuilder(_reportFactory, mapper, _sectionDescriptionBuilder);
            var buildParameters = CreateBuildParameters(_parentReport);

            builder.Build(buildParameters);

            _parentReport.Received(1).AddSubReport(_report);
            _sectionDescriptionBuilder.Received(2).Build(Arg.Any<BuildParameters<DescriptionViewModel>>());
        }

        private BuildParameters<SectionDescriptionsProtectionsModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var descriptionProtectionModel = Auto.Create<SectionDescriptionsProtectionsModel>();
            var detail1 = Auto.Create<DescriptionProtection>();
            var detail2 = Auto.Create<DescriptionProtection>();
            descriptionProtectionModel.Details = new List<DescriptionProtection> {detail1,detail2};
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionDescriptionsProtectionsModel>(descriptionProtectionModel)
                   {
                       ParentReport = illustrationMasterReport,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}