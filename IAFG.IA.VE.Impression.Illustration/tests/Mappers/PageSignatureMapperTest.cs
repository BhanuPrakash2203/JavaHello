using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers
{
    [TestClass]
    public class PageSignatureMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private AutoMapperFactory _autoMapperFactory;
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = new ManagerFactory(new ModelMapper(), null);

        [TestInitialize]
        public void Initialize()
        {
            _resourceAccessorFactory.Contexte = ResourcesContexte.Illustration;
            _resourceAccessorFactory.GetResourcesAccessor().Returns(new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(Helpers.Helpers.CreateCultureAccessor())));
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
        }

        [TestMethod]
        public void ShouldMap()
        {
            var section = Auto.Create<SectionSignatureModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();

            var subject = new PageSignatureMapper(_autoMapperFactory);
            var viewModel = new PageSignatureViewModel();

            subject.Map(section, viewModel, context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.EstNouveauContrat.Should().Be(section.EstNouveauContrat);
            viewModel.NumeroContrat.Should().Be(section.NumeroContrat);

            viewModel.Should().BeEquivalentTo(section,
                               options => options.Excluding(o => o.Avis)
                                                 .Excluding(o => o.Notes)
                                                 .Excluding(o => o.Images)
                                                 .Excluding(o => o.Libelles)
                                                 .Excluding(o => o.Details));
            
            Assert.AreEqual(section.Details.Count, viewModel.Details.Count);
            for (var i = 0; i < section.Details.Count; i++)
            {
                viewModel.Details[i].Should().BeEquivalentTo(section.Details[i]);
            }

            Assert.AreEqual(section.Signatures.Count, viewModel.Signatures.Count);
            for (var i = 0; i < section.Signatures.Count; i++)
            {
                viewModel.Signatures[i].Should().Be(section.Signatures[i]);
            }

            Assert.AreEqual(section.Avis.Count, viewModel.Avis.Count);
            for (var i = 0; i < section.Avis.Count; i++)
            {
                viewModel.Avis[i].Should().Be(section.Avis[i]);
                viewModel.Avis[i].Should().BeEquivalentTo(section.Avis[i]);
            }

            Assert.AreEqual(section.Notes.Count, viewModel.Notes.Count);
            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }
        }

    }
}