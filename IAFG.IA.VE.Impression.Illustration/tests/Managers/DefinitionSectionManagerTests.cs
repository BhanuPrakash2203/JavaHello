using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class DefinitionSectionManagerTests
    {
        private IDefinitionSectionManager _manager;

        [TestInitialize]
        public void Initialize()
        {
            _manager = new DefinitionSectionManager();
        }

        [TestMethod]
        public void MergeSousSection_WhenSourceIsNull_ReturnsDestination()
        {
            var destination = new List<DefinitionSection>
            {
                new DefinitionSection
                {
                    SectionId = "SousSection_001"
                }
            };

            var result = _manager.Merge(null, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeSousSection_WhenDestinationIsNull_ReturnsSource()
        {
            var source = new List<DefinitionSection>()
            {
                new DefinitionSection()
                {
                    SectionId = "SousSection_001"
                }
            };

            var result = _manager.Merge(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeSousSection_WhenSourceAndDestinationContainsElements_ReturnsMergedList()
        {
            var source = new List<DefinitionSection>()
            {
                new DefinitionSection()
                    .Init("soussection_001")
                    .WithTitre(new DefinitionTitreDescriptionSelonProduit().Build(Produit.AssuranceParticipant))
                    .WithTexte(new DefinitionTexte().Build(0))
                    .WithTexte(new DefinitionTexte().Build(1))
                    .WithNote(new DefinitionNote())
                    .WithAvis(new DefinitionAvis())
                    .WithLibelle("libelle_001", new DefinitionLibelle()),
            };

            var destination = new List<DefinitionSection>()
            {
                new DefinitionSection()
                    .Init("soussection_001")
                    .WithTexte(new DefinitionTexte())
                    .WithAvis(new DefinitionAvis())
                    .WithAvis(new DefinitionAvis())
                    .WithTitre(new DefinitionTitreDescriptionSelonProduit().Build(Produit.CapitalValeur))
                    .WithLibelle("libelle_002", new DefinitionLibelle()),
                new DefinitionSection()
                    .Init("soussection_002")
                    .WithAvis(new DefinitionAvis())
                    .WithNote(new DefinitionNote())
                    .WithNote(new DefinitionNote())
                    .WithLibelle("libelle_001", new DefinitionLibelle())
            };

            var result = _manager.Merge(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);

                var sousSectionUn = result.Find(ss => string.Equals(ss.SectionId, "soussection_001"));
                sousSectionUn.Libelles.Should().HaveCount(2);
                sousSectionUn.Notes.Should().HaveCount(1);
                sousSectionUn.Avis.Should().HaveCount(3);
                sousSectionUn.Textes.Should().HaveCount(3);
                sousSectionUn.Titres.Should().HaveCount(2);

                var sousSectionDeux = result.Find(ss => string.Equals(ss.SectionId, "soussection_002"));
                sousSectionDeux.Libelles.Should().HaveCount(1);
                sousSectionDeux.Notes.Should().HaveCount(2);
                sousSectionDeux.Avis.Should().HaveCount(1);
                sousSectionDeux.Textes.Should().BeEmpty();
                sousSectionDeux.Titres.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void MergeAvis_WhenSourceIsNull_ReturnsDestination()
        {
            IEnumerable<DefinitionAvis> source = null;
            var destination = new List<DefinitionAvis>()
            {
                new DefinitionAvis(){Id = "definition_avis_001"},
                new DefinitionAvis(){Id = "definition_avis_002"}
            };

            // ReSharper disable once ExpressionIsAlwaysNull
            var result = _manager.MergeAvis(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeAvis_WhenDestinationIsNull_ReturnsSource()
        {
            IEnumerable<DefinitionAvis> source = new List<DefinitionAvis>()
            {
                new DefinitionAvis(){Id = "definition_avis_001"},
                new DefinitionAvis(){Id = "definition_avis_002"}
            };

            var result = _manager.MergeAvis(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeAvis_WhenSourceAndDestinationContainsElements_ReturnsMergedList()
        {
            IEnumerable<DefinitionAvis> source = new List<DefinitionAvis>()
            {
                new DefinitionAvis(){Id = "definition_avis_001"},
                new DefinitionAvis(){Id = "definition_avis_003"}
            };

            var destination = new List<DefinitionAvis>()
            {
                new DefinitionAvis(){Id = "definition_avis_002"},
                new DefinitionAvis(){Id = "definition_avis_004"}
            };

            var result = _manager.MergeAvis(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(4);
                result.Any(x => string.Equals(x.Id, "definition_avis_001")).Should().BeTrue();
                result.Any(x => string.Equals(x.Id, "definition_avis_002")).Should().BeTrue();
                result.Any(x => string.Equals(x.Id, "definition_avis_003")).Should().BeTrue();
                result.Any(x => string.Equals(x.Id, "definition_avis_004")).Should().BeTrue();
            }
        }

        [TestMethod]
        public void MergeNotes_WhenDestinationIsNull_ReturnsDestination()
        {
            var source = new List<DefinitionNote>()
            {
                new DefinitionNote(){Id = "definition_note_001"}
            };

            // ReSharper disable once ExpressionIsAlwaysNull
            var result = _manager.MergeNotes(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeNotes_WhenSourceAndDestinationContainsElements_ReturnsMergedList()
        {
            var source = new List<DefinitionNote>()
            {
                new DefinitionNote(){Id = "definition_note_001"}
            };

            var destination = new List<DefinitionNote>()
            {
                new DefinitionNote(){Id = "definition_note_002"}
            };

            // ReSharper disable once ExpressionIsAlwaysNull
            var result = _manager.MergeNotes(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeNotes_WhenSourceIsNull_ReturnsDestination()
        {
            IEnumerable<DefinitionNote> source = null;
            var destination = new List<DefinitionNote>()
            {
                new DefinitionNote{Id = "definition_note_001"}
            };

            // ReSharper disable once ExpressionIsAlwaysNull
            var result = _manager.MergeNotes(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeLibelle_WhenSourceIsNull_ReturnsDestination()
        {
            var destination = new Dictionary<string, DefinitionLibelle>()
            {
                ["libelle_001"] = new DefinitionLibelle { Libelle = "libelle_001_fr", LibelleEn = "libelle_001_en" },
                ["libelle_002"] = new DefinitionLibelle { Libelle = "libelle_002_fr", LibelleEn = "libelle_002_en" },
            };

            var result = _manager.MergeLibelles(null, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeLibelle_WhenDestinationIsNull_ReturnsDestination()
        {
            var source = new Dictionary<string, DefinitionLibelle>
            {
                ["libelle_001"] = new DefinitionLibelle { Libelle = "libelle_001_fr", LibelleEn = "libelle_001_en" },
                ["libelle_002"] = new DefinitionLibelle { Libelle = "libelle_002_fr", LibelleEn = "libelle_002_en" },
            };

            var result = _manager.MergeLibelles(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeLibelle_WhenSourceAndDestinationContainsElements_ReturnsMergedDictionary()
        {
            var source = new Dictionary<string, DefinitionLibelle>
            {
                ["libelle_001"] = new DefinitionLibelle { Libelle = "libelle_001_fr", LibelleEn = "libelle_001_en" },
                ["libelle_002"] = new DefinitionLibelle { Libelle = "libelle_002_fr", LibelleEn = "libelle_002_en" },
            };

            var destination = new Dictionary<string, DefinitionLibelle>
            {
                ["libelle_003"] = new DefinitionLibelle { Libelle = "libelle_003_fr", LibelleEn = "libelle_003_en" },
                ["libelle_004"] = new DefinitionLibelle { Libelle = "libelle_004_fr", LibelleEn = "libelle_004_en" },
            };

            var result = _manager.MergeLibelles(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().HaveCount(4);
                result.Any(kvp => string.Equals(kvp.Key, "libelle_001")).Should().BeTrue();
                result.Any(kvp => string.Equals(kvp.Key, "libelle_002")).Should().BeTrue();
                result.Any(kvp => string.Equals(kvp.Key, "libelle_003")).Should().BeTrue();
                result.Any(kvp => string.Equals(kvp.Key, "libelle_004")).Should().BeTrue();
            }
        }

        [TestMethod]
        public void MergeTextes_WhenSourceIsNull_ReturnsDestination()
        {
            var destination = new List<DefinitionTexte>()
            {
                new DefinitionTexte() {SequenceId = 0},
                new DefinitionTexte() {SequenceId = 1},
            };

            var result = _manager.MergeTextes(null, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeTextes_WhenDestinationIsNull_ReturnsSource()
        {
            IEnumerable<DefinitionTexte> source = new List<DefinitionTexte>()
            {
                new DefinitionTexte() {SequenceId = 0},
                new DefinitionTexte() {SequenceId = 1}
            };

            var result = _manager.MergeTextes(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void MergeTextes_WhenSourceAndDestinationContainsElements_ReturnsMergedList()
        {
            IEnumerable<DefinitionTexte> source = new List<DefinitionTexte>()
            {
                new DefinitionTexte() {SequenceId = 0},
                new DefinitionTexte() {SequenceId = 1}
            };

            var destination = new List<DefinitionTexte>()
            {
                new DefinitionTexte() {SequenceId = 2},
                new DefinitionTexte() {SequenceId = 3},
            };

            var result = _manager.MergeTextes(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(4);
            }
        }

        [TestMethod]
        public void MergeTitres_WhenSourceIsNull_ReturnsDestination()
        {
            var destination = new List<DefinitionTitreDescriptionSelonProduit>()
            {
                new DefinitionTitreDescriptionSelonProduit{Produit = Produit.AssuranceParticipant}
            };

            var result = _manager.MergeTitres(null, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeTitres_WhenDestinationIsNull_ReturnsDestination()
        {
            var source = new List<DefinitionTitreDescriptionSelonProduit>
            {
                new DefinitionTitreDescriptionSelonProduit{Produit = Produit.AssuranceParticipant}
            };

            var result = _manager.MergeTitres(source, null);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MergeTitres_WhenSourceAndDestinationContainsElements_ReturnsMergedList()
        {
            var source = new List<DefinitionTitreDescriptionSelonProduit>
            {
                new DefinitionTitreDescriptionSelonProduit{Produit = Produit.AssuranceParticipant}
            };

            var destination = new List<DefinitionTitreDescriptionSelonProduit>
            {
                new DefinitionTitreDescriptionSelonProduit{Produit = Produit.Transition}
            };

            var result = _manager.MergeTitres(source, destination);

            using (new AssertionScope())
            {
                result.Should().NotBeNullOrEmpty();
                result.Should().HaveCount(2);
            }
        }
    }

    internal static class DefinitionSectionManagerTestsExtensions
    {
        public static DefinitionSection Init(this DefinitionSection definitionSousSection, string sousSection)
        {
            definitionSousSection.Avis = new List<DefinitionAvis>();
            definitionSousSection.Libelles = new Dictionary<string, DefinitionLibelle>();
            definitionSousSection.Notes = new List<DefinitionNote>();
            definitionSousSection.Textes = new List<DefinitionTexte>();
            definitionSousSection.Titres = new List<DefinitionTitreDescriptionSelonProduit>();
            definitionSousSection.SectionId = sousSection;

            return definitionSousSection;
        }

        public static DefinitionSection WithTitre(this DefinitionSection definitionSousSection, DefinitionTitreDescriptionSelonProduit titre)
        {
            definitionSousSection.Titres.Add(titre);
            return definitionSousSection;
        }

        public static DefinitionSection WithTexte(this DefinitionSection definitionSousSection, DefinitionTexte texte)
        {
            definitionSousSection.Textes.Add(texte);
            return definitionSousSection;
        }

        public static DefinitionSection WithNote(this DefinitionSection definitionSousSection, DefinitionNote note)
        {
            definitionSousSection.Notes.Add(note);
            return definitionSousSection;
        }

        public static DefinitionSection WithAvis(this DefinitionSection definitionSousSection, DefinitionAvis avis)
        {
            definitionSousSection.Avis.Add(avis);
            return definitionSousSection;
        }

        public static DefinitionSection WithLibelle(this DefinitionSection definitionSousSection, string key, DefinitionLibelle libelle)
        {
            definitionSousSection.Libelles.Add(key, libelle);
            return definitionSousSection;
        }

        public static DefinitionTitreDescriptionSelonProduit Build(this DefinitionTitreDescriptionSelonProduit titre, Produit produit)
        {
            titre.Produit = produit;
            return titre;
        }

        public static DefinitionTexte Build(this DefinitionTexte texte, int sequence)
        {
            texte.SequenceId = sequence;
            return texte;
        }
    }
}