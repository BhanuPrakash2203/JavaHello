using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionTableauManager
    {
        TableauResultat CreerTableau(DefinitionTableau definitionTableau, DonneesRapportIllustration donnees);

        TableauResultat CreerTableauPourGroupeAssures(DefinitionTableau definitionTableau,
            string identifiantGroupeAssure, string[] noms, DonneesRapportIllustration donnees);

        List<GroupeColonne> CreerGroupeColonnes(
            IEnumerable<DefinitionGroupeColonne> definitionGroupes,
            DonneesRapportIllustration donnees,
            string identifiantGroupeAssure);
    }
}
