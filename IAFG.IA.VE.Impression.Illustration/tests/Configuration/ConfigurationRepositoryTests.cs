using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Parameters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace IAFG.IA.VE.Impression.Illustration.Test.Configuration
{
    [TestClass]
    public class ConfigurationRepositoryTests
    {
        [TestMethod]
        public void ObtenirLibelleNoteEsperanceVie()
        {
            var pilotage = Substitute.For<IPilotageRapportIllustrations>();

            var ressources = Substitute.For<Ressources>();
            ressources.NotesEsperanceVie = null;

            var configRepo = new ConfigurationRepository(pilotage);

            using (new AssertionScope())
            {
                pilotage.Ressources.ReturnsNull();
                configRepo.ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie.Individuelle).Should().BeEmpty();

                pilotage.Ressources.Returns(ressources);
                configRepo.ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie.Individuelle).Should().BeEmpty();

                ressources.NotesEsperanceVie = new Dictionary<NoteEsperanceVie, DefinitionLibelle>
                {
                    {NoteEsperanceVie.Individuelle, new DefinitionLibelle {Libelle = "toto", LibelleEn = "tata"}}
                };
                configRepo.ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie.Individuelle).Should().Be("tata");

                configRepo.Language = Language.French;
                configRepo.ObtenirLibelleNoteEsperanceVie(NoteEsperanceVie.Individuelle).Should().Be("toto");

            }

        }
    }
}
