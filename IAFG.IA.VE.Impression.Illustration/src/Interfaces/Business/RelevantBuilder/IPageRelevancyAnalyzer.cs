using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder
{
    public interface IPageRelevancyAnalyzer
    {
        IList<PageRapportInfo> GetRelevantPages(DonneesRapportIllustration donneesIllustration);
        bool IsValid(string sectionId, DonneesRapportIllustration donneesIllustration);
    }
}