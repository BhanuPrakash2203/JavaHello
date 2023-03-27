using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class DefinitionTexteManagerTests
    {
        private const int NoRegle1Id = 0;
        private const int NoRegle2Id = 1;
        private const int TabagismePreferentielId = 2;
        private const int CapitalAssurePlusDe10MId = 3;
        private const int CapitalValeurId = 4;
        private const int SaufCapitalValeurId = 5;
        private const int EnVigueurId = 6;
        private const int CapitalValeurAndEnVigueurId = 7;
        private const int ExclureProduitCapitalValeurId = 8;
        private const int ProduitCapitalValeurAndEnVigueurId = 9;
        private const double DefaultRate = 0.0025;

        private DonneesRapportIllustration _donnees;
        private IIllustrationReportDataFormatter _formatter;
        private readonly IProductRules _productRules = Substitute.For<IProductRules>();
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Initialize()
        {
            _donnees = Substitute.For<DonneesRapportIllustration>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatterTexte(Arg.Any<DefinitionTexte>(), Arg.Any<DonneesRapportIllustration>())
                .Returns(x => ((DefinitionTexte)x[0]).Texte);
        }

        [TestMethod]
        public void CreerDetailTextes_WhenTexteWithBullet_ThenFormattedTexteWithBullet()
        {
            var definition = new DefinitionTexte
            {
                Texte = "<html>Test</html>",
                Bullets = new List<DefinitionTexte>
                {
                    new DefinitionTexte
                    {
                        Texte = "<li>bullet-1</li>",
                        Bullets = new List<DefinitionTexte>
                        {
                            new DefinitionTexte
                            {
                                Texte = "<li>Sub-a</li>"
                            },
                            new DefinitionTexte
                            {
                                Texte = "<li>Sub-b</li>"
                            }
                        }
                    },
                    new DefinitionTexte
                    {
                        Texte = "<li>bullet-2</li>"
                    }
                }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var result = manager.ObtenirTexte(definition, _donnees);
            result.Should().Be("<html>Test</html><html><ul><li>bullet-1</li><ul><li>Sub-a</li><li>Sub-b</li></ul><li>bullet-2</li></ul></html>");
        }

        [TestMethod]
        public void CreerDetailTextes_WhenEmptyTextes_ThenEmptyList()
        {
            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var result = manager.CreerDetailTextes(new List<DefinitionTexte>(), _donnees);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void CreerDetailTextes_WhenNullTextes_ThenEmptyList()
        {
            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var result = manager.CreerDetailTextes(null, _donnees);
            result.Should().BeEmpty();
        }

        [DataRow(false, false, false, Produit.AccesVie, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(true, false, false, Produit.AccesVie, TabagismePreferentielId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(false, true, false, Produit.AccesVie, CapitalAssurePlusDe10MId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(false, false, true, Produit.AccesVie, EnVigueurId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(false, false, false, Produit.CapitalValeur, CapitalValeurId)]
        [DataRow(false, false, false, Produit.CapitalValeur3, CapitalValeurId)]
        [DataRow(true, true, false, Produit.AccesVie, TabagismePreferentielId, CapitalAssurePlusDe10MId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(true, false, true, Produit.AccesVie, TabagismePreferentielId, EnVigueurId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(true, false, false, Produit.CapitalValeur, TabagismePreferentielId, CapitalValeurId)]
        [DataRow(true, false, false, Produit.CapitalValeur3, TabagismePreferentielId, CapitalValeurId)]
        [DataRow(false, true, true, Produit.AccesVie, CapitalAssurePlusDe10MId, EnVigueurId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(false, true, false, Produit.CapitalValeur, CapitalAssurePlusDe10MId, CapitalValeurId)]
        [DataRow(false, true, false, Produit.CapitalValeur3, CapitalAssurePlusDe10MId, CapitalValeurId)]
        [DataRow(false, false, true, Produit.CapitalValeur, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataRow(false, false, true, Produit.CapitalValeur3, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataRow(true, true, true, Produit.AccesVie, TabagismePreferentielId, CapitalAssurePlusDe10MId, EnVigueurId, SaufCapitalValeurId, ExclureProduitCapitalValeurId)]
        [DataRow(false, true, true, Produit.CapitalValeur, CapitalAssurePlusDe10MId, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataRow(false, true, true, Produit.CapitalValeur3, CapitalAssurePlusDe10MId, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataRow(true, true, true, Produit.CapitalValeur, TabagismePreferentielId, CapitalAssurePlusDe10MId, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataRow(true, true, true, Produit.CapitalValeur3, TabagismePreferentielId, CapitalAssurePlusDe10MId, EnVigueurId, CapitalValeurId, CapitalValeurAndEnVigueurId, ProduitCapitalValeurAndEnVigueurId)]
        [DataTestMethod]
        public void CreerDetailTextes_ThenGoodRegles(bool tabagismePreferentiel,
                                             bool estCapitalAssurePlusDe10Millions,
                                             bool enVigueur,
                                             Produit produit,
                                             params int[] expecteResults)
        {
            var results = new List<int> { NoRegle1Id, NoRegle2Id };
            results.AddRange(expecteResults);
            _donnees.Produit = produit;
            _donnees.Etat = enVigueur ? Etat.EnVigueur : Etat.NouvelleVente;
            _donnees.TabagismePreferentiel = tabagismePreferentiel;
            _donnees.Protections = CreateProtections(estCapitalAssurePlusDe10Millions, tabagismePreferentiel);

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var result = manager.CreerDetailTextes(CreateDefinitionTextes(), _donnees);
            result.Select(x => x.SequenceId).Should().BeEquivalentTo(results);
        }

        [TestMethod]
        public void EstVisible_ReglesNull_ThenTrue()
        {
            var manager = new DefinitionTexteManager(_formatter, _productRules);
            manager.EstVisible(new DefinitionTexte(), new DonneesRapportIllustration()).Should().BeTrue();
        }

        [TestMethod]
        public void EstVisible_RegleNonGere_ThenArgumentOutOfRangeException()
        {
            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { (RegleTexte)(-8888) } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            Action action = () => manager.EstVisible(definition, new DonneesRapportIllustration());
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void EstVisible_WhenRenouvellementIllustree_ThenBonAffichageProtectionTemporaireRenouvelable(bool existeProtectionTemporaireRenouvelable, bool afficheRegle)
        {
            var donnees = new DonneesRapportIllustration
            { Protections = new Protections { ExisteProtectionTemporaireRenouvelable = existeProtectionTemporaireRenouvelable } };

            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { (RegleTexte.ProtectionTemporaireRenouvelable) } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var resultat = manager.EstVisible(definition, donnees);
            resultat.Should().Be(afficheRegle);
        }

        [DataTestMethod]
        [DataRow(true, 0, false)]
        [DataRow(false, 0, false)]
        [DataRow(false, DefaultRate, true)]

        public void EstVisible_WhenRenouvellementIllustree_ThenBonAffichageBoniPARPresent(bool fondsNull, double rate, bool expectedResult)
        {
            var donnees = new DonneesRapportIllustration
            {
                FondsProtectionPrincipale = fondsNull ?
                    null :
                    new Fonds
                    {
                        DefaultRate = rate,
                        Vehicule = string.Empty,
                        Id = string.Empty
                    }
            };

            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { (RegleTexte.BoniPARPresent) } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var resultat = manager.EstVisible(definition, donnees);
            resultat.Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow(Etat.NouvelleVente)]
        [DataRow(Etat.EnVigueur)]
        public void EstVisible_WhenFondsInvestissementAvecCompteSRDIris_ThenAfficheTexteInformationnelCdrl(Etat etatVente)
        {
            const string COMPTE_SRD_IRIS = "SRD080";
            var donnees = new DonneesRapportIllustration
            {
                Etat = etatVente,
                Vehicules = new List<Vehicule>
                {
                    new Vehicule { Vehicle = COMPTE_SRD_IRIS }
                },
                FondsInvestissement = new List<Fonds>
                {
                    new Fonds { Vehicule = COMPTE_SRD_IRIS }
                }
            };

            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { RegleTexte.PresenceCompteSRDIris } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var resultat = manager.EstVisible(definition, donnees);

            resultat.Should().BeTrue();
        }


        [DataTestMethod]
        [DataRow(Etat.NouvelleVente, true)]
        [DataRow(Etat.EnVigueur, false)]
        public void EstVisible_WhenFondsInvestissementAvecCompteSRDLisse_ThenBonAffichageTexteInformationnelCdrl(Etat etatVente, bool expectedResult)
        {
            const string COMPTE_SRD_LISSE = "SRD081";
            var donnees = new DonneesRapportIllustration
            {
                Etat = etatVente,
                Vehicules = new List<Vehicule>
                {
                    new Vehicule { Vehicle = COMPTE_SRD_LISSE }
                },
                FondsInvestissement = new List<Fonds>
                {
                    new Fonds { Vehicule = COMPTE_SRD_LISSE }
                }
            };

            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { RegleTexte.PresenceCompteSRDLisse, RegleTexte.NouvelleVente } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var resultat = manager.EstVisible(definition, donnees);

            resultat.Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow(Etat.NouvelleVente)]
        [DataRow(Etat.EnVigueur)]
        public void EstVisible_WhenFondsInvestissementSansCompteSRDIrisEtLisse_ThenAffichePasTextesInformationnelsCdrl(Etat etatVente)
        {
            const string AUTRE_COMPTE = "ABC001";
            var donnees = new DonneesRapportIllustration
            {
                Etat = etatVente,
                Vehicules = new List<Vehicule>
                {
                    new Vehicule { Vehicle = AUTRE_COMPTE }
                },
                FondsInvestissement = new List<Fonds>
                {
                    new Fonds { Vehicule = AUTRE_COMPTE }
                }
            };

            var definition = new DefinitionTexte
            {
                Regles = new List<RegleTexte[]> { new[] { RegleTexte.PresenceCompteSRDIris, RegleTexte.PresenceCompteSRDLisse, RegleTexte.NouvelleVente } }
            };

            var manager = new DefinitionTexteManager(_formatter, _productRules);
            var resultat = manager.EstVisible(definition, donnees);

            resultat.Should().BeFalse();
        }

        private static Protections CreateProtections(bool estCapitalAssurePlusDe10Millions, bool isPreferential)
        {
            return new Protections
            {
                ProtectionsAssures = new List<Protection>
                {
                    new Protection
                    {
                        TypeAssurance = TypeAssurance.Individuelle,
                        StatutTabagisme = isPreferential ? StatutTabagisme.NonFumeurElite : StatutTabagisme.NonFumeur,
                        CapitalAssureActuel = estCapitalAssurePlusDe10Millions ? 10000001 : 10000000,
                        Assures = new List<Assure>
                        {
                            new Assure { ReferenceExterneId = "1" }
                        }
                    }
                }
            };
        }

        private static List<DefinitionTexte> CreateDefinitionTextes()
        {
            var textes = new List<DefinitionTexte>
            {
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, NoRegle1Id)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .Without(x => x.Regles)
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, NoRegle2Id)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .Without(x => x.Regles)
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, TabagismePreferentielId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.TauxPreferentielPresent}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, CapitalAssurePlusDe10MId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.CapitalAssurePlusDe10M}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, CapitalValeurId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.CapitalValeur}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, SaufCapitalValeurId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.SaufCapitalValeur}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, EnVigueurId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.EnVigueur}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, CapitalValeurAndEnVigueurId)
                    .Without(x => x.Bullets)
                    .Without(x => x.RegleProduits)
                    .With(x => x.Regles,
                        new List<RegleTexte[]> {new[] {RegleTexte.EnVigueur, RegleTexte.CapitalValeur}})
                    .Without(x => x.TexteParams)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, ExclureProduitCapitalValeurId)
                    .With(x => x.RegleProduits,
                        new RegleProduits {Produits = new[] {Produit.CapitalValeur, Produit.CapitalValeur3}, Exclusion = true})
                    .With(x => x.Regles, new List<RegleTexte[]>())
                    .Without(x => x.TexteParams)
                    .Without(x => x.Bullets)
                    .Create(),
                Auto.Build<DefinitionTexte>()
                    .With(x => x.SequenceId, ProduitCapitalValeurAndEnVigueurId)
                    .With(x => x.RegleProduits,
                        new RegleProduits {Produits = new[] {Produit.CapitalValeur, Produit.CapitalValeur3}, Exclusion = false})
                    .With(x => x.Regles, new List<RegleTexte[]> {new[] {RegleTexte.EnVigueur}})
                    .Without(x => x.TexteParams)
                    .Without(x => x.Bullets)
                    .Create()
            };











            return textes;
        }
    }
}