using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class ConceptVenteModelFactoryTest
    {
        private static readonly IFixture Fixture = AutoFixtureFactory.Create();
        private readonly IConfigurationRepository _configurationRepository = Substitute.For<IConfigurationRepository>();
        private readonly IIllustrationReportDataFormatter _formatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();

        [TestMethod]
        public void Factory_WHEN_Build_Then_ReturnModel()
        {
            var pretCollateral = Fixture.Create<DefinitionSection>();
            pretCollateral.SectionId = "PretCollateral";
            var paiementInterets = Fixture.Create<DefinitionSection>();
            paiementInterets.SectionId = "PretCollateral-PaiementInterets";
            var remboursement = Fixture.Create<DefinitionSection>();
            remboursement.SectionId = "PretCollateral-Remboursement";

            var definition = Fixture.Create<DefinitionSection>();
            definition.ListSections.Clear();
            definition.ListSections.Add(pretCollateral);
            definition.ListSections.Add(paiementInterets);
            definition.ListSections.Add(remboursement);

            var donnees = Fixture.Create<DonneesRapportIllustration>();

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(Arg.Any<DefinitionTitreDescriptionSelonProduit>(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new ConceptVenteModelFactory(
                _configurationRepository, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, new DefinitionTitreManager(_formatter), new DefinitionImageManager()),
                _noteManager, _formatter);
            
            var model = factory.Build("1", donnees, Fixture.Create<IReportContext>());
            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(definition.Titres.First().Titre);
                model.SectionPretCollateralPaiementInteret.Should().NotBeNull();
                model.SectionPretCollateral.Should().NotBeNull();
                model.SectionPretCollateralRemboursement.Should().NotBeNull();
            }
        }
    }
}
