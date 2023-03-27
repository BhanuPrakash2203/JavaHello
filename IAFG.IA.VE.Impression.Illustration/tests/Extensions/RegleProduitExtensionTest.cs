using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Extensions
{
    [TestClass]
    public class RegleProduitExtensionTest
    {

        [DataRow(new[] {Produit.CapitalValeur, Produit.AccesVie}, false, Produit.CapitalValeur, true)]
        [DataRow(new[] { Produit.Genesis, Produit.AccesVie }, false, Produit.CapitalValeur, false)]
        [DataRow(new Produit[]{}, false, Produit.CapitalValeur, true)]
        [DataRow(null, false, Produit.CapitalValeur, true)]
        [DataRow(new[] {Produit.CapitalValeur, Produit.AccesVie}, true, Produit.CapitalValeur, false)]
        [DataRow(new[] { Produit.Genesis, Produit.AccesVie }, true, Produit.CapitalValeur, true)]
        [DataRow(new Produit[] { }, true, Produit.CapitalValeur, true)]
        [DataRow(null, true, Produit.CapitalValeur, true)]
        [DataTestMethod]
        public void CreerRegles_ThenGoodResult(Produit[] produits, bool exclusion, Produit produit, bool expecteResult)
        {
            var regle = new RegleProduits {Produits = produits, Exclusion = exclusion};
            regle.EstProduitValide(produit).Should().Be(expecteResult);
        }

        [DataRow(new[] { Produit.CapitalValeur, Produit.AccesVie }, Produit.CapitalValeur, true)]
        [DataRow(new[] { Produit.Genesis, Produit.AccesVie }, Produit.CapitalValeur, false)]
        [DataRow(new Produit[] { }, Produit.CapitalValeur, true)]
        [DataRow(null, Produit.CapitalValeur, true)]
        [DataTestMethod]
        public void EstProduitValide_ThenGoodResult(Produit[] produits, Produit produit, bool expecteResult)
        {
            produits.EstProduitValide(produit).Should().Be(expecteResult);
        }

        [DataRow(new[] { Produit.CapitalValeur, Produit.AccesVie }, Produit.CapitalValeur, false)]
        [DataRow(new[] { Produit.Genesis, Produit.AccesVie }, Produit.CapitalValeur, true)]
        [DataRow(new Produit[] { }, Produit.CapitalValeur, true)]
        [DataRow(null, Produit.CapitalValeur, true)]
        [DataTestMethod]
        public void EstProduitNonExclus_ThenGoodResult(Produit[] produits, Produit produit, bool expecteResult)
        {
            produits.EstProduitNonExclus(produit).Should().Be(expecteResult);
        }
    }
}
