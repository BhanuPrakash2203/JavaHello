using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtections
{
    [TestClass]
    public class PageSommaireProtectionsMapperTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
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
            var section = Auto.Create<SectionSommaireProtectionsModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();

            var subject = new PageSommaireProtectionsMapper(_autoMapperFactory);
            var viewModel = new PageSommaireProtectionsViewModel();

            subject.Map(section, viewModel, context);

            viewModel.TitreSection.Should().Be(section.TitreSection);
            viewModel.Description.Should().Be(section.Description);
            viewModel.Should().BeEquivalentTo(section,
                                           options => options.Excluding(o => o.Avis)
                                           .Excluding(o => o.Libelles)
                                           .Excluding(o => o.Images)
                                           .Excluding(o => o.SectionIdendification)
                                           .Excluding(o => o.SectionProtections)
                                           .Excluding(o => o.SectionPrimes)
                                           .Excluding(o => o.SectionAsl)
                                           .Excluding(o => o.SectionFluxMonetaire)
                                           .Excluding(o => o.SectionSurprimes)
                                           .Excluding(o => o.SectionDetailParticipations)
                                           .Excluding(o => o.SectionScenarioParticipations)
                                           .Excluding(o => o.SectionEclipseDePrime)
                                           .Excluding(o => o.SectionAvancesSurPolice)
                                           .Excluding(o => o.SectionUsageAuConseiller)
                                           .Excluding(o => o.Notes));

            for (var i = 0; i < section.Avis.Count; i++)
            {
                viewModel.Avis[i].Should().Be(section.Avis[i]);
            }

            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }
        }

        [TestMethod]
        public void ShouldMapDetailParticipations()
        {
            var section = Auto.Create<SectionDetailParticipationsModel>();
            section.OptionParticipation = TypeOptionParticipation.Comptant;
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionDetailParticipationsMapper(_autoMapperFactory);
            var viewModel = new DetailParticipationsViewModel();

            subject.Map(section, viewModel, context);
            using (new AssertionScope())
            {
                viewModel.OptionParticipation.Should().NotBeNullOrWhiteSpace();
            }
        }

        [TestMethod]
        public void ShouldMapDetailParticipations_AvecBaremeNull()
        {
            var section = Auto.Create<SectionDetailParticipationsModel>();
            section.OptionParticipation = TypeOptionParticipation.Comptant;
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionDetailParticipationsMapper(_autoMapperFactory);
            var viewModel = new DetailParticipationsViewModel();

            subject.Map(section, viewModel, context);
            using (new AssertionScope())
            {
                viewModel.OptionParticipation.Should().NotBeNullOrWhiteSpace();
            }
        }

        [TestMethod]
        public void ShouldMapAvancesSurPolice()
        {
            var dateDerniereMiseAJour = new DateTime(2021, 08, 01);
            const double solde = 123.45D;
            const string expectedFormattedSolde = "123.45$";
            const string expectedFormattedDate = "8 août 2021";

            ReportDataFormatter.FormatCurrency(Arg.Any<double>()).Returns(expectedFormattedSolde);
            ReportDataFormatter.FormatLongDate(Arg.Any<DateTime>()).Returns(expectedFormattedDate);

            var section = Auto.Create<SectionAvancesSurPoliceModel>();
            section.DateDerniereMiseAJour = dateDerniereMiseAJour;
            section.Solde = solde;

            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionAvancesSurPoliceMapper(_autoMapperFactory);
            var viewModel = new AvancesSurPoliceViewModel();

            subject.Map(section, viewModel, context);
            using (new AssertionScope())
            {
                viewModel.SoldeEnDateDu.Should().NotBeNullOrWhiteSpace();
                viewModel.SoldeEnDateDu.Should().Contain(expectedFormattedSolde);
                viewModel.SoldeEnDateDu.Should().Contain(expectedFormattedDate);
            }
        }

        [TestMethod]
        public void ShouldMapScenarioParticipations()
        {
            var section = Auto.Create<SectionScenarioParticipationsModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionScenarioParticipationsMapper(_autoMapperFactory);
            var viewModel = new ScenarioParticipationsViewModel();

            subject.Map(section, viewModel, context);
            using (new AssertionScope())
            {
                viewModel.BaremesParticipations.Should().NotBeNullOrWhiteSpace();
            }
        }

        [TestMethod]
        public void ShouldMapScenarioParticipations_AvecBaremeNull()
        {
            var section = Auto.Create<SectionScenarioParticipationsModel>();
            section.EcartBaremeParticipation = null;
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionScenarioParticipationsMapper(_autoMapperFactory);
            var viewModel = new ScenarioParticipationsViewModel();

            subject.Map(section, viewModel, context);
            using (new AssertionScope())
            {
                viewModel.BaremesParticipations.Should().BeNullOrWhiteSpace();
            }
        }

        [TestMethod]
        public void ShouldMapAssuranceSupplementaireLiberee()
        {
            var section = Auto.Create<SectionASLModel>();
            foreach (var item in section.Notes)
            {
                item.NumeroReference = null;
            }

            var context = Auto.Create<IReportContext>();
            var subject = new SectionASLMapper(_autoMapperFactory);
            var viewModel = new ASLViewModel();

            subject.Map(section, viewModel, context);
            viewModel.Should().BeEquivalentTo(section,
                                           options => options.Excluding(o => o.Notes)
                                                             .Excluding(o => o.Images)
                                                             .Excluding(o => o.Libelles)
                                                             .Excluding(o => o.Taux)
                                                             .Excluding(o => o.OptionVersementBoni)
                                                             .Excluding(o => o.CapitalAssureMaximal)
                                                             .Excluding(o => o.AucunMaximum)
                                                             .Excluding(o => o.AucunAchat)
                                                             .Excluding(o => o.Description)
                                                             .Excluding(o => o.Avis)
                                                             .Excluding(o => o.Allocations));

            viewModel.OptionVersementBoni.Should().BeEquivalentTo(ReportDataFormatter.FormatterEnum<TypeOptionVersementBoni>(section.OptionVersementBoni.ToString()));

            var notesTriees = section.Notes.OrderBy(x => x.SequenceId).ToList();
            for (var i = 0; i < notesTriees.Count; i++)
            {
                viewModel.Notes[i].Should().Be(notesTriees[i].Texte);
            }

            for (var i = 0; i < section.Taux.Count; i++)
            {
                var taux = viewModel.Taux[i];
                taux.AnneeDebut.Should().Be(section.Taux[i].AnneeDebut);
                taux.Periode.Should().BeEquivalentTo(ReportDataFormatter.FormatterPeriodeAnneeMois(section.Taux[i].AnneeDebut, null));
                taux.Taux.Should().BeEquivalentTo(ReportDataFormatter.FormatPercentageWithoutSymbol(section.Taux[i].Taux));
            }

            for (var i = 0; i < section.Allocations.Count; i++)
            {
                var allocation = viewModel.Allocations[i];
                allocation.AnneeDebut.Should().Be(section.Allocations[i].AnneeDebut);
                allocation.Periode.Should().BeEquivalentTo(ReportDataFormatter.FormatterPeriodeAnneeMois(section.Allocations[i].AnneeDebut, null));
                allocation.Montant.Should().BeEquivalentTo(ReportDataFormatter.FormatDecimal(section.Allocations[i].Montant));
            }
        }

        [TestMethod]
        public void ShouldMapAssuranceSupplementaireLiberee_CapitalAssuraMaximal()
        {
            ReportDataFormatter.FormatterEnum<TypeOptionVersementBoni>(Arg.Any<string>()).Returns("TypeOptionVersementBoni");
            var section = Auto.Create<SectionASLModel>();
            var context = Auto.Create<IReportContext>();
            var subject = new SectionASLMapper(_autoMapperFactory);
            var viewModel = new ASLViewModel();

            subject.Map(section, viewModel, context);
            viewModel.CapitalAssuraMaximal.Should().NotBeNullOrEmpty();
       }

        [TestMethod]
        public void ShouldMapAssuranceSupplementaireLiberee_OptionVersementBoni()
        {
            ReportDataFormatter.FormatterEnum<TypeOptionVersementBoni>(Arg.Any<string>()).Returns("TypeOptionVersementBoni");

            var section = Auto.Create<SectionASLModel>();
            var context = Auto.Create<IReportContext>();
            var subject = new SectionASLMapper(_autoMapperFactory);
            var viewModel = new ASLViewModel();

            subject.Map(section, viewModel, context);
            viewModel.OptionVersementBoni.Should().BeEquivalentTo("TypeOptionVersementBoni");
        }

        [TestMethod]
        public void GIVEN_Montant_THEN_ReturnFormatted()
        {
            var illustrationReportDataFormatter = Helpers.Helpers.CreateIllustrationReportDataFormatter();

            var detailflux = new DetailFluxMonetaire
            {
                TypeTransaction = TypeTransactionFluxMonetaire.Depot,
                TypeMontant = TypeMontantFluxMonetaires.Maximum,
                EstDepotRetraitMaximal = true,
                Montant = 123.45
            };

            using (new AssertionScope())
            {
                DetailFluxMonetaireExtension.FormatterMontant(detailflux, illustrationReportDataFormatter, _resourceAccessorFactory).Should().BeOneOf(new string[]{ "123.45 (Maximal)", "123.45 (Maximum)" } );
                detailflux.Montant = 0;
                DetailFluxMonetaireExtension.FormatterMontant(detailflux, illustrationReportDataFormatter, _resourceAccessorFactory).Should().BeOneOf(new string[] { "0.00 (Maximal)", "0.00 (Maximum)" });
                detailflux.Montant = 45.78;
                detailflux.TypeMontant = TypeMontantFluxMonetaires.Personnalise;
                DetailFluxMonetaireExtension.FormatterMontant(detailflux, illustrationReportDataFormatter, _resourceAccessorFactory).Should().Be("45.78");
            }
        }
    }
}