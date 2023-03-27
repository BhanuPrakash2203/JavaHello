using System.Linq;
using AutoFixture;
using Castle.Core.Internal;
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
    public class PagePrimesRenouvellementMapperTest
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
            var section = Auto.Create<PagePrimesRenouvellementModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();

            var subject = new PagePrimesRenouvellementMapper(_autoMapperFactory);
            var viewModel = new PagePrimesRenouvellementViewModel();

            subject.Map(section, viewModel, context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                           options => options.Excluding(o => o.Avis)
                                                             .Excluding(o => o.Notes)
                                                             .Excluding(o => o.Images)
                                                             .Excluding(o => o.Libelles)
                                                             .Excluding(o => o.PresenceGarantie)
                                                             .Excluding(o => o.PresenceSurprime)
                                                             .Excluding(o => o.SectionPrimesRenouvellementModels));

            for (var i = 0; i < section.Avis.Count; i++)
            {
                viewModel.Avis[i].Should().Be(section.Avis[i]);
            }

            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }

            for (var i = 0; i < section.SectionPrimesRenouvellementModels.Count; i++)
            {
                var prime = viewModel.SectionPrimesRenouvellementViewModels[i];
                
                for (var j = 0; j < prime.DetailsPrimeRenouvellement.Count; j++)
                {
                    var detailModel = section.SectionPrimesRenouvellementModels[i].DetailsPrimeRenouvellement[j];
                    var detailViewModel = viewModel.SectionPrimesRenouvellementViewModels[i].DetailsPrimeRenouvellement[j];
                    if (detailModel.CapitalAssure.HasValue && detailModel.CapitalAssure > 0)
                    {
                        Assert.IsFalse(detailViewModel.CapitalAssure.IsNullOrEmpty());
                    }
                    else
                    {
                        Assert.IsTrue(detailViewModel.CapitalAssure.IsNullOrEmpty());
                    }

                    if (detailModel.Assures.Any(a => !a.IsNullOrEmpty()))
                    {
                        Assert.IsFalse(detailViewModel.Assures.IsNullOrEmpty());
                    }
                    else
                    {
                        Assert.IsTrue(detailViewModel.Assures.IsNullOrEmpty());
                    }

                    for (var p = 0; p < detailViewModel.Periodes.Count; p++)
                    {
                        var periode = detailViewModel.Periodes[p];
                        Assert.IsFalse(periode.Periode.IsNullOrEmpty());

                        if (detailModel.Periodes[p].PrimeGarantie > 0)
                            Assert.IsFalse(periode.PrimeGarantie.IsNullOrEmpty());
                        else
                            Assert.IsFalse(periode.PrimeGarantie.IsNullOrEmpty());
                    }
                }
            }
        }
    }
}