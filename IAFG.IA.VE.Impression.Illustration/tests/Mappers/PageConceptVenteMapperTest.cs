using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class PageConceptVenteMapperTest
    {
        private static readonly IFixture Fixture = AutoFixtureFactory.Create();
        private readonly IIllustrationReportDataFormatter _formatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportContext _context = Fixture.Create<IReportContext>();
        private readonly IManagerFactory _managerFactory = new ManagerFactory(new ModelMapper(), null);
        private AutoMapperFactory _autoMapperFactory;

        [TestInitialize]
        public void Initialize()
        {
            _resourcesAccessorFactory.Contexte = ResourcesContexte.Illustration;
            _autoMapperFactory = new AutoMapperFactory(_formatter, _resourcesAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void GIVEN_Mapper_WHEN_MapModel_THEN_ReturnPageViewModel()
        {
            var section = Fixture.Create<SectionConceptVenteModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var viewModel = new PageSommaireViewModel();

            var mapper = new PageConceptVenteMapper(_autoMapperFactory);
            mapper.Map(section, viewModel, _context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                              options => options.Excluding(o => o.Avis)
                                                                .Excluding(o => o.Notes)
                                                                .Excluding(o => o.Images)
                                                                .Excluding(o => o.Libelles)
                                                                .Excluding(o => o.SectionPretCollateral)
                                                                .Excluding(o => o.SectionPretCollateralPaiementInteret)
                                                                .Excluding(o => o.SectionPretCollateralRemboursement)
                                                                .Excluding(o => o.SectionAvancePret)
                                                                .Excluding(o => o.SectionAvancePretRemboursement));

            for (var i = 0; i < section.Avis.Count; i++)
            {
                viewModel.Avis[i].Should().Be(section.Avis[i]);
            }

            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }

            viewModel.Sections.Should().HaveCount(5);

        }
    }
}
