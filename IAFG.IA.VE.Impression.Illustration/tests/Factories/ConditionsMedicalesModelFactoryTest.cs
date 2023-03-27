using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class ConditionsMedicalesModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private IDefinitionTitreManager _titreManager;
        private IDefinitionImageManager _imageManager;
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _titreManager = new DefinitionTitreManager(_formatter);
            _imageManager = new DefinitionImageManager();
        }

        [TestMethod]
        public void GIVEN_ConditionsMedicalesModelFactory_WHEN_Build_Then_ReturnSectionConditionsMedicalesModel()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = Auto.Create<DefinitionSectionConditionsMedicales>();
            definition.SectionId = "ConditionsMedicales";

            definition.Sections[0].SectionId = "SectionConditionsMedicales_4";
            definition.Sections[0].Regles = new List<RegleConditionsMedicales>();

            definition.Sections[1].SectionId = "SectionConditionsMedicales_25";
            definition.Sections[1].Regles = new List<RegleConditionsMedicales>();

            definition.Sections[2].SectionId = "SectionConditionsMedicales_Juvenile";
            definition.Sections[2].Regles = new List<RegleConditionsMedicales>();

            donnees.Protections.ProtectionsAssures.Clear();

            _configurationRepository.ObtenirDefinitionSection<DefinitionSectionConditionsMedicales>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);

            var sousDefinition = Auto.Create<DefinitionConditionsMedicalesSousSection>();
            sousDefinition.SectionId = "SectionConditionsMedicales_4";
            sousDefinition.ConditionMedicaleTextes = new List<DefinitionConditionMedicale>();

            _configurationRepository.ObtenirDefinitionSection<DefinitionConditionsMedicalesSousSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(sousDefinition);
            _formatter.FormatterTitre(definition.Titres.First(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new ConditionsMedicalesModelFactory(_configurationRepository, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());
            model.Sections.Should().NotBeNullOrEmpty();
            model.Sections.Count.Should().Be(3);
        }

        [TestMethod]
        public void
            GIVEN_DefinitionSectionConditionsMedicales_WHEN_CreerDetailConditionMedicalesWithDescriptiveCode_THEN_ReturnDetailsFilterByDescriptiveCode()
        {
            var definitionSection = Auto.Create<DefinitionSectionConditionsMedicales>();
            definitionSection.Sections.First().ConditionMedicaleTextes[0].CodesRegle = new List<string>();
            definitionSection.Sections.First().ConditionMedicaleTextes[0].Regles = new List<RegleConditionsMedicales[]>();
            definitionSection.Sections.First().ConditionMedicaleTextes[1].CodesRegle = new List<string>();
            definitionSection.Sections.First().ConditionMedicaleTextes[1].Regles = new List<RegleConditionsMedicales[]>();
            definitionSection.Sections.First().ConditionMedicaleTextes[2].CodesRegle = new List<string>();
            definitionSection.Sections.First().ConditionMedicaleTextes[2].Regles = new List<RegleConditionsMedicales[]>();
            definitionSection.Sections.First().Regles = new List<RegleConditionsMedicales>();

            var donnes = Auto.Create<DonneesRapportIllustration>();
            for (var x = 0; x < donnes.ProtectionsGroupees.Count; x++)
            {
                donnes.ProtectionsGroupees[0].ProtectionsAssures[x].ReferenceExterneId =
                    donnes.ProtectionsPDF[x].IdProtection;
                donnes.ProtectionsGroupees[0].ProtectionsAssures[x].Assures[0].AgeAssurance = 24;
                donnes.ProtectionsPDF[x].Specification.IsCriticalIllnessChildModule = true;
            }

            var result =
                ConditionsMedicalesModelFactory.CreerDetailConditionsMedicales(definitionSection.Sections.First(),
                    donnes);

            result.Should().HaveCount(3);
        }
    }
}
