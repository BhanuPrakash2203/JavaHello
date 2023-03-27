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
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Builder
{
    [TestClass]
    public class PageApercuProtectionsBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionTableauResultatBuilder _sectionTableauResultatBuilder = Substitute.For<ISectionTableauResultatBuilder>();
        private readonly IPageResultat _report = Substitute.For<IPageResultat>();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private AutoMapperFactory _autoMapperFactory;
        private IPageSectionMapper _sectionMapper;
        private IPageResultatMapper _resultatMapper;
        private PageApercuProtectionsBuilder _builder;
        private readonly IModelMapper _modelMapper = new ModelMapper();

        [TestInitialize]
        public void Initialize()
        {
            _autoMapperFactory = new AutoMapperFactory(
                ReportDataFormatter, 
                _resourceAccessorFactory, 
                new ManagerFactory(_modelMapper, 
                    new TableauResultatManager(
                        new VecteurManager(), _modelMapper,
                        ReportDataFormatter, 
                        _resourceAccessorFactory)));
            
            _resultatMapper = new PageResultatMapper(_autoMapperFactory);
            _sectionMapper = new PageSectionMapper(_autoMapperFactory);
            _reportFactory.Create<IPageResultat>().Returns(_report);
            _builder = new PageApercuProtectionsBuilder(
                _reportFactory, 
                _sectionMapper, 
                _resultatMapper, 
                _sectionTableauResultatBuilder);
        }

        [TestMethod]
        public void ShouldAddItselfToParentReport()
        {
            var buildParameters = CreateBuildParameters(_parentReport);
            _builder.Build(buildParameters);
            _parentReport.Received(1).AddSubReport(_report);
            _sectionTableauResultatBuilder.Received().Build(Arg.Any<BuildParameters<TableauResultatViewModel>>());
        }

        private BuildParameters<SectionApercuProtectionsModel> CreateBuildParameters(
            IIllustrationMasterReport illustrationMasterReport)
        {
            var projections = new Projections
            {
                AnneeDebutProjection = 0,
                AnneeFinProjection = 0,
                IndexFinProjection = 0,
                Projection = new Projection
                {
                    Columns = new List<Column>
                    {
                        new Column {Id = 0, Value = new double[] {0}},
                        new Column {Id = 1, Value = new double[] {1}},
                        new Column {Id = 3, Value = new double[] {2}},
                    }
                }
            };

            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.Projections = projections;
            donnees.ChoixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.ToutesLesAnnees;

            var sectionModel = Auto.Create<SectionApercuProtectionsModel>();
            sectionModel.SectionResultats = new[] { 
                new SectionResultatModel { 
                    Tableau = new TableauResultat { 
                        TypeTableau = TypeTableau.Contrat, 
                        GroupeColonnes =  new List<GroupeColonne> {
                            new GroupeColonne {
                                DefinitionColonnes = new List<ColonneTableau> { 
                                    new ColonneTableau { TypeColonne = TypeColonne.Normale} 
                                } 
                            } 
                        } 
                    },
                    IndexFinProjection = 0,
                    DonneesIllustration = donnees,
                    SelectionAgesResultats = null,
                    SelectionAnneesResultats = null
                }};

            var styleOverride = new StyleOverride {MarginLevel = MarginLevel.Level1, MoveAllLabels = false};

            var result = new BuildParameters<SectionApercuProtectionsModel>(sectionModel)
                {
                    ParentReport = illustrationMasterReport,
                    ReportContext = _context,
                    StyleOverride = styleOverride
                };

            return result;
        }
    }
}