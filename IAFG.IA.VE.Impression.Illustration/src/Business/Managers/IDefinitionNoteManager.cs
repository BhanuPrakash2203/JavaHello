using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionNoteManager
    {
        IList<string> CreerAvis(IEnumerable<DefinitionAvis> definitions, DonneesRapportIllustration donnees);

        IList<DetailNote> CreerNotes(IEnumerable<DefinitionNote> definitions,
            DonneesRapportIllustration donnees, DonneesNote donneesNote);

        IList<DetailNote> CreerNotes(IEnumerable<DefinitionNote> definitions,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, 
            IList<Types.Models.SommaireProtections.Protection> protections);

        DetailNote CreerNote(IList<DetailNote> notes, DefinitionNote definition,
            DonneesRapportIllustration donnees);

        DetailNote CreerNote(IList<DetailNote> notes, DefinitionNote definition, string texte);

        bool EstVisible(DefinitionNote definitionNote, DonneesRapportIllustration donnees, DonneesNote donneesNote);
    }
}
