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
    public class PageResultatBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageResultat _report = Substitute.For<IPageResultat>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly ISectionTableauResultatBuilder _sectionTableauResultatBuilder = Substitute.For<ISectionTableauResultatBuilder>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IModelMapper _modelMapper = new ModelMapper();
        private AutoMapperFactory _autoMapperFactory;

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
            _sectionTableauResultatBuilder.Received(1).Build(Arg.Any<BuildParameters<TableauResultatViewModel>>());
        }

        private void CallReportBuilder()
        {
            _reportFactory.Create<IPageResultat>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(
                _reportDataFormatter, 
                _resourcesAccessor, 
                new ManagerFactory(_modelMapper, new TableauResultatManager(new VecteurManager(), _modelMapper, _reportDataFormatter, _resourcesAccessor)));
            
            var builder = new PageResultatBuilder(_reportFactory, new PageResultatMapper(_autoMapperFactory), _sectionTableauResultatBuilder);
            var buildParam = CreateBuildParameters(_parentReport);
            builder.Build(buildParam);
        }

        private BuildParameters<SectionResultatModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
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

            var sectionModel = Auto.Create<SectionResultatModel>();
            sectionModel.IndexFinProjection = 0;
            sectionModel.DonneesIllustration.Projections = projections;
            sectionModel.DonneesIllustration.ChoixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.ToutesLesAnnees;
            sectionModel.SelectionAgesResultats = null;
            sectionModel.SelectionAnneesResultats = null;

            sectionModel.Tableau.GroupeColonnes = new List<GroupeColonne>
            { 
                new GroupeColonne
                { 
                    DefinitionColonnes = new List<ColonneTableau>
                    {
                        new ColonneTableau
                        {
                            ColonneMoteur = 0,
                            Visible = false,
                            TypeColonne = TypeColonne.Annee
                        },
                        new ColonneTableau                    
                        {
                            ColonneMoteur = 1,
                            Visible = false,
                            TypeColonne = TypeColonne.Age
                        },
                        new ColonneTableau
                        {
                            ColonneMoteur = 3,
                            Visible = true,
                            TypeColonne = TypeColonne.Normale
                        }
                    }
                }
            };

            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };
            return new BuildParameters<SectionResultatModel>(sectionModel)
                   {
                       ParentReport = illustrationMasterReport,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}
