using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Rules
{

    [TestClass]
    public class ProductRulesTests
    {
        [TestMethod]
        [DataRow(Produit.AssuranceParticipant, true)]
        [DataRow(Produit.AssuranceParticipantValeur, true)]
        [DataRow(Produit.AssuranceParticipantPatrimoine, true)]
        [DataRow(Produit.NonDefini, false)]
        [DataRow(Produit.AccesVie, false)]
        [DataRow(Produit.Traditionnel, false)]
        public void GIVEN_ProduitAndResult_THEN_ReturnsValidResult(Produit produit, bool valid)
        {
            var productRules = new ProductRules();
            var result = productRules.EstParmiFamilleAssuranceParticipants(produit);
            result.Should().Be(valid);
        }

        [TestMethod]
        public void GIVEN_ProduitsAndFamilleAssuranceParticipants_THEN_ReturnsValidResult()
        {
            var familleAssuranceParticipants = new List<Produit> 
                { 
                    Produit.AssuranceParticipant, 
                    Produit.AssuranceParticipantValeur, 
                    Produit.AssuranceParticipantPatrimoine
                };

            var enumVectors = ((Produit[])Enum.GetValues(typeof(Produit))).ToList();
            var productRules = new ProductRules();
            using (new AssertionScope())
            {
                foreach (var produit in enumVectors.Where(x => familleAssuranceParticipants.Contains(x)))
                {
                    productRules.EstParmiFamilleAssuranceParticipants(produit).Should().Be(familleAssuranceParticipants.Contains(produit));
                }
            }
        }
    }
}