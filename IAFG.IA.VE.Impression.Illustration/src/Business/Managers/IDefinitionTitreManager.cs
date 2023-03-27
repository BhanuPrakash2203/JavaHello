using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionTitreManager
    {
        string ObtenirDescription(List<DefinitionTitreDescriptionSelonProduit> definitions,
            DonneesRapportIllustration donnees);

        string ObtenirTitre(List<DefinitionTitreDescriptionSelonProduit> definitions,
            DonneesRapportIllustration donnees, string[] parametres = null);
    }
}