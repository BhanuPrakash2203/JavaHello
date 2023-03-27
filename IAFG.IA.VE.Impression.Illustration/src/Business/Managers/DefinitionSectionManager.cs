using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionSectionManager : IDefinitionSectionManager
    {
        public DefinitionSection Merge(DefinitionSection source, DefinitionSection destination)
        {
            if (source == null)
            {
                return destination;
            }

            destination.Avis = MergeAvis(source.Avis, destination.Avis);
            destination.Libelles = MergeLibelles(source.Libelles, destination.Libelles);
            destination.Notes = MergeNotes(source.Notes, destination.Notes);
            destination.ListSections = Merge(source.ListSections, destination.ListSections);
            destination.Titres = MergeTitres(source.Titres, destination.Titres);
            return destination;
        }

        public List<DefinitionSection> Merge(IEnumerable<DefinitionSection> source, List<DefinitionSection> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new List<DefinitionSection>();
            }

            foreach (var sousSectionBase in source)
            {
                var sousSection = destination.FirstOrDefault(x => x.SectionId == sousSectionBase.SectionId);
                if (sousSection == null)
                {
                    destination.Add(sousSectionBase);
                }
                else
                {
                    sousSection.Avis = MergeAvis(sousSectionBase.Avis, sousSection.Avis);
                    sousSection.Notes = MergeNotes(sousSectionBase.Notes, sousSection.Notes);
                    sousSection.Textes = MergeTextes(sousSectionBase.Textes, sousSection.Textes);
                    sousSection.Titres = MergeTitres(sousSectionBase.Titres, sousSection.Titres);
                    sousSection.Libelles = MergeLibelles(sousSectionBase.Libelles, sousSection.Libelles);
                }
            }

            return destination;
        }

        public List<DefinitionAvis> MergeAvis(IEnumerable<DefinitionAvis> source, List<DefinitionAvis> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new List<DefinitionAvis>();
            }

            destination.AddRange(source);
            return destination;
        }

        public List<DefinitionNote> MergeNotes(IEnumerable<DefinitionNote> source, List<DefinitionNote> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new List<DefinitionNote>();
            }

            destination.AddRange(source);
            return destination;
        }

        public Dictionary<string, DefinitionLibelle> MergeLibelles(Dictionary<string, DefinitionLibelle> source, Dictionary<string, DefinitionLibelle> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new Dictionary<string, DefinitionLibelle>();
            }

            foreach (var item in source)
            {
                destination.Add(item.Key, item.Value);
            }

            return destination;
        }

        public List<DefinitionTexte> MergeTextes(IEnumerable<DefinitionTexte> source,
            List<DefinitionTexte> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new List<DefinitionTexte>();
            }

            destination.AddRange(source);
            return destination;
        }

        public List<DefinitionTitreDescriptionSelonProduit> MergeTitres(IEnumerable<DefinitionTitreDescriptionSelonProduit> source,
            List<DefinitionTitreDescriptionSelonProduit> destination)
        {
            if (source == null)
            {
                return destination;
            }

            if (destination == null)
            {
                destination = new List<DefinitionTitreDescriptionSelonProduit>();
            }

            destination.AddRange(source);
            return destination;
        }
    }
}