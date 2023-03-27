using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IDefinitionSectionManager
    {
        DefinitionSection Merge(DefinitionSection source, DefinitionSection destination);
        List<DefinitionSection> Merge(IEnumerable<DefinitionSection> source, List<DefinitionSection> destination);
        List<DefinitionAvis> MergeAvis(IEnumerable<DefinitionAvis> source, List<DefinitionAvis> destination);
        List<DefinitionNote> MergeNotes(IEnumerable<DefinitionNote> source, List<DefinitionNote> destination);
        Dictionary<string, DefinitionLibelle> MergeLibelles(Dictionary<string, DefinitionLibelle> source,
            Dictionary<string, DefinitionLibelle> destination);
        List<DefinitionTexte> MergeTextes(IEnumerable<DefinitionTexte> source,
            List<DefinitionTexte> destination);
        List<DefinitionTitreDescriptionSelonProduit> MergeTitres(
            IEnumerable<DefinitionTitreDescriptionSelonProduit> source,
            List<DefinitionTitreDescriptionSelonProduit> destination);
    }
}