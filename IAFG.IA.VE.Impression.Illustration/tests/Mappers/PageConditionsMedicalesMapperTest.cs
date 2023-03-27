using System.Collections.Generic;
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
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConditionsMedicales;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class PageConditionsMedicalesMapperTest
    {
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IManagerFactory _managerFactory = new ManagerFactory(new ModelMapper(), null);
        private AutoMapperFactory _autoMapperFactory;

        [TestInitialize]
        public void Initialize()
        {
            _resourceAccessorFactory.Contexte = ResourcesContexte.Illustration;
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void GIVEN_ConditionsMedicalesMapper_WHEN_MapConditionsMedicalesModel_THEN_ReturnPageConditionsMedicalesViewModel()
        {
            var section = Auto.Create<SectionConditionsMedicalesModel>();
            section.Sections.Clear();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var detailDescription1 = Auto.Create<ConditionMedicale>();
            detailDescription1.Tableau = new List<TableauItem>();
            var detailDescription2 = Auto.Create<ConditionMedicale>();
            var detailDescription3 = Auto.Create<ConditionMedicale>();
            var detailDescription4 = Auto.Create<ConditionMedicale>();

            IList<ConditionMedicale> details = new List<ConditionMedicale> {detailDescription1,detailDescription2,detailDescription3,detailDescription4};
            
            var detailConditionsMedicalesModel = Auto.Create<ConditionsMedicalesSection>();
            detailConditionsMedicalesModel.Details = details;
            section.Sections.Add(detailConditionsMedicalesModel);
            
            var mapper = new PageConditionsMedicalesMapper(_autoMapperFactory);
            var viewModel = new PageConditionsMedicalesViewModel();

            mapper.Map(section,viewModel,context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                              options => options.Excluding(o => o.Avis)
                                                                .Excluding(o => o.Notes)
                                                                .Excluding(o => o.Images)
                                                                .Excluding(o => o.Libelles)
                                                                .Excluding(o => o.Sections));
            for (var i = 0; i < section.Avis.Count; i++)
            {
                viewModel.Avis[i].Should().Be(section.Avis[i]);
            }

            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }

            for (var i = 0; i < details.Count; i++)
            {
                var current = section.Sections.First().Details[i];
                current.SequenceId.Should().Be(details[i].SequenceId);
                current.Titre.Should().Be(details[i].Titre);
                current.Libelle.Should().Be(details[i].Libelle);
                current.Texte.Should().Be(details[i].Texte);
                current.Html.Should().Be(details[i].Html);
                current.Textes.Should().HaveCount(details[i].Textes.Count);
                if (details[i].Textes.Any())
                {
                    foreach (var listeDescriptionsItem in details[i].Textes)
                    {
                        current.Textes.Any(x => x.Texte == listeDescriptionsItem.Texte).Should().BeTrue();
                    }
                }
            }
        }
    }
}