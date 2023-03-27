using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers
{
    [TestClass]
    public class PageModificationsDemandeesMapperTest
    {
        private static readonly IFixture AutoFixture = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = AutoFixture.Create<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = new ManagerFactory(new ModelMapper(), null);
        private AutoMapperFactory _autoMapperFactory;

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
            var model = new SectionModificationsDemandeesModel
            {
                TitreSection = AutoFixture.Create<string>(),
                Description = AutoFixture.Create<string>(),
                Avis = AutoFixture.Create<IList<string>>(),
                Notes = new List<DetailNote>
                {
                    new DetailNote {Texte = "Premiere note", SequenceId = 1},
                    new DetailNote {Texte = "Deuxieme note", SequenceId = 2}
                }
            };

            var context = AutoFixture.Create<IReportContext>();
            var subject = new PageModificationsDemandeesMapper(_autoMapperFactory);
            var viewModel = new PageModificationsDemandeesViewModel();

            subject.Map(model, viewModel, context);

            using (new AssertionScope())
            {
                viewModel.TitreSection.Should().Be(model.TitreSection);
                viewModel.Description.Should().Be(model.Description);
                viewModel.Avis.Should().BeEquivalentTo(model.Avis);
                viewModel.Notes.Should().BeEquivalentTo("Premiere note", "Deuxieme note");
            }
        }

        [TestMethod]
        public void ShouldMapContrat()
        {
            var model = new SectionContratModel
            {
                TitreSection = AutoFixture.Create<string>(),
                Description = AutoFixture.Create<string>(),
                Avis = AutoFixture.Create<IList<string>>(),
                Notes = new List<DetailNote>
                {
                    new DetailNote {Texte = "Premiere note", SequenceId = 1},
                    new DetailNote {Texte = "Deuxieme note", SequenceId = 2}
                }
            };

            var context = AutoFixture.Create<IReportContext>();
            var subject = new SectionContratMapper(_autoMapperFactory);
            var viewModel = new ContratViewModel();

            subject.Map(model, viewModel, context);

            using (new AssertionScope())
            {
                viewModel.TitreSection.Should().Be(model.TitreSection);
            }
        }

        [TestMethod]
        public void ShouldMapProtections()
        {
            var model = new SectionProtectionsModel
            {
                TitreSection = AutoFixture.Create<string>(),
                Description = AutoFixture.Create<string>(),
                Avis = AutoFixture.Create<IList<string>>(),
                Notes = new List<DetailNote>
                {
                    new DetailNote {Texte = "Premiere note", SequenceId = 1},
                    new DetailNote {Texte = "Deuxieme note", SequenceId = 2}
                }
            };

            var context = AutoFixture.Create<IReportContext>();
            var subject = new SectionProtectionsMapper(_autoMapperFactory);
            var viewModel = new ProtectionsViewModel();

            subject.Map(model, viewModel, context);

            using (new AssertionScope())
            {
                viewModel.TitreSection.Should().Be(model.TitreSection);
            }
        }
    }
}
