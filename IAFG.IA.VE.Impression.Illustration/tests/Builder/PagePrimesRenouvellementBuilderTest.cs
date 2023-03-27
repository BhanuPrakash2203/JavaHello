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
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.PrimesRenouvellement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{

    [TestClass]
    public class PagePrimesRenouvellementBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPagePrimesRenouvellement _report = Substitute.For<IPagePrimesRenouvellement>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportContext context = _auto.Create<IReportContext>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private AutoMapperFactory _autoMapperFactory;
        private readonly ISectionPrimesRenouvellementBuilder _sectionPrimesRenouvellementBuilder = Substitute.For<ISectionPrimesRenouvellementBuilder>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestMethod]
        public void PageResultatBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();
            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void PageResultatBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            CallReportBuilder();
            _sectionPrimesRenouvellementBuilder.Received(9).Build(Arg.Any<BuildParameters<DetailsPrimeRenouvellementViewModel>>());
        }

        
        [TestMethod]
        public void PageResultatBuilder_WhenBuildSansPrimes_ThenEmpty()
        {
            CallReportBuilder(true);
            _sectionPrimesRenouvellementBuilder.Received(1).Build(Arg.Any<BuildParameters<DetailsPrimeRenouvellementViewModel>>());
        }

        private void CallReportBuilder(bool sansPrime = false)
        {
            _reportFactory.Create<IPagePrimesRenouvellement>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
            var builder = new PagePrimesRenouvellementBuilder(_reportFactory, new PagePrimesRenouvellementMapper(_autoMapperFactory), _sectionPrimesRenouvellementBuilder);
            var buildParam = CreateBuildParameters(_parentReport, sansPrime);

            builder.Build(buildParam);
        }

        private BuildParameters<PagePrimesRenouvellementModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport, bool sansPrime)
        {
            PagePrimesRenouvellementModel sectionModel;

            if (sansPrime)
            {
                sectionModel = _auto.Build<PagePrimesRenouvellementModel>().With(p => p.SectionPrimesRenouvellementModels, new List<SectionPrimeRenouvellementModel>
                                                                                                                           {
                                                                                                                               new SectionPrimeRenouvellementModel
                                                                                                                               {
                                                                                                                                   DetailsPrimeRenouvellement =
                                                                                                                                       new List<DetailsPrimeRenouvellementModel>
                                                                                                                                       {
                                                                                                                                           new DetailsPrimeRenouvellementModel
                                                                                                                                           {Periodes = new List<PeriodePrimeModel>()},
                                                                                                                                           new DetailsPrimeRenouvellementModel
                                                                                                                                           {Periodes = _auto.Create<IList<PeriodePrimeModel>>()}
                                                                                                                                       }
                                                                                                                               }
                                                                                                                           }).Create();
            }
            else
            {
                sectionModel = _auto.Create<PagePrimesRenouvellementModel>();
            }
            
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<PagePrimesRenouvellementModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }

    }
}
