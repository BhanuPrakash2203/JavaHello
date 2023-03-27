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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class PageGlossaireMapperTest
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
        public void ShouldMapGlossaire()
        {
            var section = Auto.Create<SectionGlossaireModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var detailGlossaire1 = Auto.Create<DetailGlossaire>();
            var detailGlossaire2 = Auto.Create<DetailGlossaire>();
            var detailGlossaire3 = Auto.Create<DetailGlossaire>();
            var context = Auto.Create<IReportContext>();

            IList<DetailGlossaire> details = new List<DetailGlossaire> { detailGlossaire1, detailGlossaire2, detailGlossaire3 };
            section.Details = details;

            var subject = new PageGlossaireMapper(_autoMapperFactory);
            var viewModel = new PageGlossaireViewModel();

            subject.Map(section, viewModel, context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                           options => options.Excluding(o => o.Avis)
                                                             .Excluding(o => o.Notes)
                                                             .Excluding(o => o.Images)
                                                             .Excluding(o => o.Libelles)
                                                             .Excluding(o => o.Details));

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
                var current = viewModel.Details[i];
                current.CodeGlossaire.Should().Be(details[i].CodeGlossaire);
                current.CodeSequenceId.Should().Be(details[i].CodeSequenceId);
                current.Titre.Should().Be(details[i].Titre);
                current.Libelle.Should().Be(details[i].Libelle);
                current.Texte.Should().Be(details[i].Texte);
                current.Html.Should().Be(string.Empty);
                current.SequenceId.Should().Be(details[i].SequenceId);
                current.Should().BeEquivalentTo(details[i]);
            }
        }

        [TestMethod] public void ShouldMapGlossaireHtml()
        {
            var section = Auto.Create<SectionGlossaireModel>();
            var detailGlossaire = Auto.Create<DetailGlossaire>();
            detailGlossaire.Texte = "<html>Test</html>";
            var context = Auto.Create<IReportContext>();

            IList<DetailGlossaire> details = new List<DetailGlossaire> { detailGlossaire };
            section.Details = details;

            var subject = new PageGlossaireMapper(_autoMapperFactory);
            var viewModel = new PageGlossaireViewModel();

            subject.Map(section, viewModel, context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                           options => options.Excluding(o => o.Avis)
                                                             .Excluding(o => o.Notes)
                                                             .Excluding(o => o.Images)
                                                             .Excluding(o => o.Libelles)
                                                             .Excluding(o => o.Details));

            for (var i = 0; i < details.Count; i++)
            {
                var current = viewModel.Details[i];
                current.Texte.Should().Be(string.Empty);
                current.Html.Should().Be(@"<html><font face=""Calibri"" size=""2pt"">Test</font></html>");
            }
        }
    }
}