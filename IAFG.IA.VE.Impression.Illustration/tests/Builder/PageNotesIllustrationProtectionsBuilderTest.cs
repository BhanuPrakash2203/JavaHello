using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageNotesIllustrationProtectionsBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionTexteDescriptionBuilder _sectionTexteDescriptionBuilder = Substitute.For<ISectionTexteDescriptionBuilder>();
        private readonly IPageNotesIllustration _report = Substitute.For<IPageNotesIllustration>();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;
        private IConfigurationRepository _configurationRepository;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            var definition = Auto.Create<DefinitionSection>();
            definition.SectionId = "NotesIllustration";
            definition.ListSections[0].SectionId = "Resultats";
            definition.ListSections[1].SectionId = "Garanties";
            definition.ListSections.RemoveAt(2);

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
        }

        [TestMethod]
        public void ShouldAddItselfToParentReport()
        {
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
            var mapper = new PageNotesIllustrationMapper(_autoMapperFactory);
            _reportFactory.Create<IPageNotesIllustration>().Returns(_report);

            var builder = new PageNotesIllustrationBuilder(_reportFactory, mapper, _sectionTexteDescriptionBuilder);
            var buildParameters = CreateBuildParameters(_parentReport);

            builder.Build(buildParameters);

            _parentReport.Received(1).AddSubReport(_report);
        }
        
        private BuildParameters<SectionNotesIllustrationModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionResultatModel = Auto.Create<SectionNotesIllustrationModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            BuildParameters<SectionNotesIllustrationModel> result = new BuildParameters<SectionNotesIllustrationModel>(sectionResultatModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };

            return result;
        }
    }
}