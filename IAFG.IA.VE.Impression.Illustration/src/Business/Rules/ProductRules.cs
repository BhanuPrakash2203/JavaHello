using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Rules
{
    public class ProductRules : IProductRules
    {
        public bool EstParmiFamilleAssuranceParticipants(Produit produit)
        {
            return ObtenirFamilleAssuranceParticipants().Any(x => x == produit);
        }

        public static Produit[] ObtenirFamilleAssuranceParticipants()
        {
            return new []{ Produit.AssuranceParticipant, Produit.AssuranceParticipantPatrimoine, Produit.AssuranceParticipantValeur };
        }
    }
}