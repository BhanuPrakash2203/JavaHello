using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionTexteManager
    {
        string ObtenirTexte(DefinitionTexte definition, DonneesRapportIllustration donnees);
        List<DetailTexte> CreerDetailTextes(List<DefinitionTexte> textes, DonneesRapportIllustration donnees);
        bool EstVisible(DefinitionTexte definitionTexte, DonneesRapportIllustration donnees);
    }
}