using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class DefinitionNoteManagerTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IIllustrationReportDataFormatter _formatter;
        private IVecteurManager _vecteurManager;
        private const string TexteGenerique = "TexteGenerique";
        private const string TexteTauxPreferentiel = "TexteTauxPreferentiel";
        private const string TexteHospitalisationPresent = "TexteHospitalisationPresent";

        [TestInitialize]
        public void TestInitialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _vecteurManager = Substitute.For<IVecteurManager>();
        }

        [TestMethod]
        public void CreerNotes_WhenNotesNull_ThenListeVide()
        {
            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(null, null, new DonneesNote());
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void CreerNotes_WhenAucuneNotes_ThenListeVide()
        {
            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(new List<DefinitionNote>(), null, new DonneesNote());
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void CreerNotes_WhenNotesProjectIl_ThenRetourneNoteFormater()
        {
            var notes = CreerListeNotes();
            var donneesIllustration = CreerDonneesRapportIllustration();
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 0), donneesIllustration).Returns(TexteGenerique);

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(notes, donneesIllustration, new DonneesNote());
            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                result.ElementAt(0).SequenceId.Should().Be(0);
                result.ElementAt(0).NumeroReference.Should().BeNull();
                result.ElementAt(0).Texte.Should().Be(TexteGenerique);
                result.ElementAt(1).SequenceId.Should().Be(3);
                result.ElementAt(1).NumeroReference.Should().BeNull();
                result.ElementAt(1).Texte.Should().Be("MessageMoteurEn");
            }
        }

        [TestMethod]
        public void CreerNotes_WhenPresenceDeSurprimeRenouvellmentAvecPrimeDeRenouvellement_ThenRetourneLaBonneNote()
        {
            var notes = new List<DefinitionNote>
                        {
                            new DefinitionNote
                            {
                                SequenceId = 0,
                                TypeNote = TypeNote.Generique,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.PresenceDeSurprimeRenouvellement}}
                            }
                        };

            var donneesIllustration = CreerDonneesRapportIllustraitonPourAvecPrimesRenouvellementAvecSurprime(Auto.Create<IList<PrimeRenouvellement>>());
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 0), donneesIllustration).Returns(TexteGenerique);

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(notes, donneesIllustration, new DonneesNote());
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.ElementAt(0).SequenceId.Should().Be(0);
                result.ElementAt(0).NumeroReference.Should().BeNull();
                result.ElementAt(0).Texte.Should().Be(TexteGenerique);
            }
        }

        [TestMethod]
        public void CreerNotes_WhenPresenceDeSurprimeRenouvellmentSansPrimeDeRenouvellement_ThenRetourneLaBonneNote()
        {
            var notes = new List<DefinitionNote>
                        {
                            new DefinitionNote
                            {
                                SequenceId = 0,
                                TypeNote = TypeNote.Generique,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.PresenceDeSurprimeRenouvellement}}
                            }
                        };

            var donneesIllustration = CreerDonneesRapportIllustraitonPourAvecPrimesRenouvellementAvecSurprime(new List<PrimeRenouvellement>());
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 0), donneesIllustration).Returns(TexteGenerique);

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(notes, donneesIllustration, new DonneesNote());
            result.Should().BeEmpty();
        }


        [TestMethod]
        public void CreerNotes_WhenNotesIllustration_ThenRetourneNoteFormater()
        {
            var notes = CreerListeNotes();
            var donneesIllustration = CreerDonneesRapportIllustration();
            var protectionGroupe = CreerProtectionsGroupees();

            _formatter.FormatterNote(notes.First(n => n.SequenceId == 0), donneesIllustration).Returns(TexteGenerique);
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 1), donneesIllustration).Returns(TexteTauxPreferentiel);
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 2), donneesIllustration).Returns(TexteHospitalisationPresent);

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(notes, donneesIllustration, new DonneesNote(), protectionGroupe.ProtectionsAssures);

            using (new AssertionScope())
            {
                result.Should().HaveCount(4);
                result.ElementAt(0).SequenceId.Should().Be(0);
                result.ElementAt(0).NumeroReference.Should().BeNull();
                result.ElementAt(0).Texte.Should().Be(TexteGenerique);
                result.ElementAt(1).SequenceId.Should().Be(1);
                result.ElementAt(1).NumeroReference.Should().Be(1);
                result.ElementAt(1).Texte.Should().Be($"{TexteTauxPreferentiel}");
                result.ElementAt(2).SequenceId.Should().Be(2);
                result.ElementAt(2).NumeroReference.Should().Be(2);
                result.ElementAt(2).Texte.Should().Be($"{TexteHospitalisationPresent}");
                result.ElementAt(3).SequenceId.Should().Be(3);
                result.ElementAt(3).NumeroReference.Should().BeNull();
                result.ElementAt(3).Texte.Should().Be("MessageMoteurEn");
            }
        }

        [TestMethod]
        public void CreerNotes_WhenPrimeVersee_ThenRetourneLaBonneNote()
        {
            var notes = new List<DefinitionNote>
                        {
                            new DefinitionNote
                            {
                                SequenceId = 0,
                                TypeNote = TypeNote.Generique,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.PrimeVersee}}
                            }
                        };

            var donneesIllustration = new DonneesRapportIllustration { Primes = new Types.Models.SommaireProtections.Primes { PrimesVersees = new List<PrimeVersee> { new PrimeVersee { Montant = Auto.Create<double>() } } } };
            _formatter.FormatterNote(notes.First(n => n.SequenceId == 0), donneesIllustration).Returns(TexteGenerique);

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var result = manager.CreerNotes(notes, donneesIllustration, new DonneesNote());
            using (new AssertionScope())
            {
                result.Should().ContainSingle();
                result.ElementAt(0).SequenceId.Should().Be(0);
                result.ElementAt(0).NumeroReference.Should().BeNull();
                result.ElementAt(0).Texte.Should().Be(TexteGenerique);
            }
        }

        [TestMethod]
        public void EstVisible_WhenFraisChargeRachatPourcentageFondsEtAnneeDebutProjectionPlusGrandeQue10_ThenReturnsFalse()
        {
            var projections = new Projections { AnneeDebutProjection = 11 };
            var donneesRapport = new DonneesRapportIllustration { Projections = projections };
            var definitionNote = new DefinitionNote
            {
                Regles = new List<RegleNote[]>
                {
                    new[]
                    {
                        RegleNote.FraisChargeRachatPourcentageFonds
                    }
                }
            };

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(definitionNote, donneesRapport, new DonneesNote());
            estVisible.Should().BeFalse();
        }

        [TestMethod]
        public void EstVisible_WhenFraisChargeRachatPourcentagePrimesEtAnneeDebutProjectionPlusGrandeQue10_ThenReturnsFalse()
        {
            var projections = new Projections { AnneeDebutProjection = 11 };
            var donneesRapport = new DonneesRapportIllustration { Projections = projections };
            var definitionNote = new DefinitionNote
            {
                Regles = new List<RegleNote[]>
                {
                    new[]
                    {
                        RegleNote.FraisChargeRachatPourcentagePrimes
                    }
                }
            };

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(definitionNote, donneesRapport, new DonneesNote());
            estVisible.Should().BeFalse();
        }

        [TestMethod]
        public void EstVisible_WhenFraisChargeRachatPourcentageFondsEtAnneeDebutProjectionPlusPetitQue11_ThenReturnsProtectionEstAvecFraisRachatSelonFonds()
        {
            var projections = new Projections { AnneeDebutProjection = 5 };
            var donneesRapport = new DonneesRapportIllustration { Projections = projections };
            var estAvecFraisRachatSelonFonds = Auto.Create<bool>();
            var definitionNote = new DefinitionNote
            {
                Regles = new List<RegleNote[]>
                {
                    new[]
                    {
                        RegleNote.FraisChargeRachatPourcentageFonds
                    }
                }
            };

            var protection = new Types.Models.SommaireProtections.Protection { EstAvecFraisRachatSelonFonds = estAvecFraisRachatSelonFonds };
            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(definitionNote, donneesRapport, new DonneesNote(), protection);
            estVisible.Should().Be(estAvecFraisRachatSelonFonds);
        }

        [TestMethod]
        public void EstVisible_WhenFraisChargeRachatPourcentagePrimesEtAnneeDebutProjectionPlusPetitQue11_ThenReturnsProtectionEstAvecFraisRachatSelonPrimes()
        {
            var projections = new Projections { AnneeDebutProjection = 5 };
            var donneesRapport = new DonneesRapportIllustration { Projections = projections };
            var estAvecFraisRachatSelonPrimes = Auto.Create<bool>();
            var definitionNote = new DefinitionNote
            {
                Regles = new List<RegleNote[]>
                {
                    new[]
                    {
                        RegleNote.FraisChargeRachatPourcentagePrimes
                    }
                }
            };

            var protection = new Types.Models.SommaireProtections.Protection { EstAvecFraisRachatSelonPrimes = estAvecFraisRachatSelonPrimes };
            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(definitionNote, donneesRapport, new DonneesNote(), protection);
            estVisible.Should().Be(estAvecFraisRachatSelonPrimes);
        }

        private IList<DefinitionNote> CreerListeNotes()
        {
            var notes = new List<DefinitionNote>
                        {
                            new DefinitionNote
                            {
                                SequenceId = 0,
                                TypeNote = TypeNote.Generique,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.Aucune}}
                            },
                            new DefinitionNote
                            {
                                SequenceId = 1,
                                TypeNote = TypeNote.Protection,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.TauxPreferentielPresent}}
                            },
                            new DefinitionNote
                            {
                                SequenceId = 2,
                                TypeNote = TypeNote.Protection,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.ProtectionHospitalisationPresent}}
                            },
                            new DefinitionNote
                            {
                                SequenceId = 3,
                                TypeNote = TypeNote.MessageMoteur,
                                Regles = new List<RegleNote[]> {new[] {RegleNote.Aucune}}
                            }
                        };

            return notes;
        }

        [TestMethod]
        public void EstVisible_WhenClientIsNotAssurable_ThenReturnsPresenceClientNonAssurableTrue()
        {
            var projections = new Projections { AnneeDebutProjection = 5 };
            var donneesRapport = new DonneesRapportIllustration 
            { 
                Projections = projections, 
                Clients = new List<Client>() { new Client() { IsNotAssurable = true } } 
            };
            
            var notes = new List<DefinitionNote>
            {
                new DefinitionNote
                {
                    Regles = new List<RegleNote[]> {new[] { RegleNote.PresenceClientNonAssurable } }
                }
            };

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(notes[0], donneesRapport, new DonneesNote());

            using (new AssertionScope())
            {
                estVisible.Should().Be(donneesRapport.Clients.First().IsNotAssurable.GetValueOrDefault());
                estVisible.Should().BeTrue();
            }
        }

        [TestMethod]
        public void EstVisible_WhenClientIsAssurable_ThenReturnsPresenceClientNonAssurableFalse()
        {
            var projections = new Projections { AnneeDebutProjection = 11 };
            var donneesRapport = new DonneesRapportIllustration 
            { 
                Projections = projections, 
                Clients = new List<Client>() { new Client() { IsNotAssurable = false } } 
            };
            
            var notes = new List<DefinitionNote>
            {
                new DefinitionNote
                {
                    Regles = new List<RegleNote[]> {new[] { RegleNote.PresenceClientNonAssurable } }
                }
            };

            var manager = new DefinitionNoteManager(_vecteurManager, _formatter);
            var estVisible = manager.EstVisible(notes[0], donneesRapport, new DonneesNote());

            using (new AssertionScope())
            {
                estVisible.Should().Be(donneesRapport.Clients.First().IsNotAssurable.GetValueOrDefault());
                estVisible.Should().BeFalse();
            }
        }

        private static DonneesRapportIllustration CreerDonneesRapportIllustraitonPourAvecPrimesRenouvellementAvecSurprime(
            IList<PrimeRenouvellement> primes)
        {
            return new DonneesRapportIllustration
            {
                ProtectionsGroupees = new List<ProtectionsGroupees>
                {
                    new ProtectionsGroupees
                    {
                        PrimesRenouvellement = new Types.Models.PrimesRenouvellement.Primes
                        {
                            Protections =
                                new List<Types.Models.PrimesRenouvellement.Protection>
                                {
                                    new Types.Models.PrimesRenouvellement.Protection
                                    {
                                        Primes = primes,
                                        PresenceSurprime = true
                                    }
                                }
                        }
                    }
                }
            };
        }

        private static DonneesRapportIllustration CreerDonneesRapportIllustration()
        {
            var donneees = new DonneesRapportIllustration
            {
                Projections = new Projections
                {
                    Projection = new Projection
                    {
                        Messages = new List<MessageMoteur>
                        {
                            new MessageMoteur
                            {
                                MessageId = "1",
                                FormattedMessageFr = "MessageMoteurFr",
                                FormattedMessageEn = "MessageMoteurEn",
                                Parametres = new List<ParametreMessageMoteur>
                                {
                                    new ParametreMessageMoteur {IntegerValue = 2},
                                    new ParametreMessageMoteur {IntegerValue = 3}
                                }
                            },
                            new MessageMoteur {MessageId = "2", Parametres = null}
                        }
                    }
                },
                Protections = new Protections
                {
                    ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                    {
                        new Types.Models.SommaireProtections.Protection
                        {
                            Plan = new Types.Models.SommaireProtections.Plan {CodePlan = string.Empty},
                            TypeAssurance = TypeAssurance.Individuelle,
                            StatutTabagisme = StatutTabagisme.FumeurPrivilege,
                            ReferenceNotes = new List<int>()
                        }
                    }
                },
                ProtectionsPDF = new List<ProtectionPdf>
                {
                    new ProtectionPdf
                    {
                        CodePlan = "68AT14",
                        Specification = new SpecificationProtection {IsHospitalization = true}
                    },
                    new ProtectionPdf
                    {
                        CodePlan = "69AT14",
                        Specification = new SpecificationProtection {IsHospitalization = true}
                    }
                }

            };

            return donneees;
        }

        private static ProtectionsGroupees CreerProtectionsGroupees()
        {
            return new ProtectionsGroupees
            {
                ProtectionsAssures = new List<Types.Models.SommaireProtections.Protection>
                {
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = string.Empty},
                        StatutTabagisme = StatutTabagisme.FumeurElite,
                        ReferenceNotes = new List<int>(),
                        TypeAssurance = TypeAssurance.Individuelle
                    },
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = string.Empty},
                        StatutTabagisme = StatutTabagisme.FumeurPrivilege,
                        ReferenceNotes = new List<int>(),
                        TypeAssurance = TypeAssurance.Individuelle
                    },
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = string.Empty},
                        StatutTabagisme = StatutTabagisme.NonFumeurElite,
                        ReferenceNotes = new List<int>(),
                        TypeAssurance = TypeAssurance.Individuelle
                    },
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = string.Empty},
                        StatutTabagisme = StatutTabagisme.NonFumeurPrivilege,
                        ReferenceNotes = new List<int>(),
                        TypeAssurance = TypeAssurance.Conjointe1erDec,
                        Assures = new List<Types.Models.SommaireProtections.Assure>
                        {
                            new Types.Models.SommaireProtections.Assure {StatutTabagisme = StatutTabagisme.Fumeur},
                            new Types.Models.SommaireProtections.Assure {StatutTabagisme = StatutTabagisme.NonFumeurElite}
                        }
                    },
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = "68AT14"},
                        StatutTabagisme = StatutTabagisme.NonFumeur,
                        TypeAssurance = TypeAssurance.Individuelle,
                        ReferenceNotes = new List<int>()
                    },
                    new Types.Models.SommaireProtections.Protection
                    {
                        Plan = new Types.Models.SommaireProtections.Plan {CodePlan = "69AT14"},
                        StatutTabagisme = StatutTabagisme.NonFumeur,
                        TypeAssurance = TypeAssurance.Individuelle,
                        ReferenceNotes = new List<int>()
                    }
                }
            };
        }
    }
}