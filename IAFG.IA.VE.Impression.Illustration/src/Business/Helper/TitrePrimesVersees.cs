using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Helper
{
    public static class TitrePrimesVersees
    {
        public static string ObtenirNomLibelleTitreColonneSelonFrequence(
            TypeFrequenceFacturation frequencefacturation,
            Produit produit, 
            Etat etat)
        {
            if (etat != Etat.EnVigueur)
            {
                return ObtenirNomLibelleTitreColonneSelonFrequence(frequencefacturation, produit);
            }

            var estAssuranceParticipant = ProductRules.ObtenirFamilleAssuranceParticipants().Any(x => x == produit);
            return estAssuranceParticipant ? LibellesPrimeVersee.PrimesVerseesSelectionneesPAR : LibellesPrimeVersee.PrimesVerseesSelectionnees;
        }


        public static string ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation frequencefacturation, Produit produit)
        {
            var estAssuranceParticipant = ProductRules.ObtenirFamilleAssuranceParticipants().Any(x => x == produit);
            switch (frequencefacturation)
            {
                case TypeFrequenceFacturation.Mensuelle:
                    return estAssuranceParticipant ? LibellesPrimeVersee.PrimesMensuellesVerseesSelectionneesPAR : LibellesPrimeVersee.PrimesMensuellesVerseesSelectionnees;
                case TypeFrequenceFacturation.Annuelle:
                    return estAssuranceParticipant ? LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionneesPAR : LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionnees;
                case TypeFrequenceFacturation.AucunMode:
                    return estAssuranceParticipant ? LibellesPrimeVersee.PrimesVerseesSelectionneesPAR : LibellesPrimeVersee.PrimesVerseesSelectionnees;
                case TypeFrequenceFacturation.Autre:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
