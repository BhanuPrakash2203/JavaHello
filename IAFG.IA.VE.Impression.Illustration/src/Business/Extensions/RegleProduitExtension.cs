using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    public static class RegleProduitExtension
    {
        public static bool EstProduitValide(this RegleProduits regle, Produit produit)
        {
            if (regle?.Produits == null) return true;
            if (!regle.Produits.Any()) return true;

            return regle.Exclusion
                ? regle.Produits.EstProduitNonExclus(produit)
                : regle.Produits.EstProduitValide(produit);
        }

        internal static bool EstProduitValide(this Produit[] produits, Produit produit)
        {
            return produits == null || !produits.Any() || produits.Any(r => r == produit);
        }

        internal static bool EstProduitNonExclus(this Produit[] produitsExclus, Produit produit)
        {
            return produitsExclus == null || !produitsExclus.Any() || produitsExclus.All(r => r != produit);
        }
    }
}