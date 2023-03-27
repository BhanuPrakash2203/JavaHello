using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder
{
    public interface IBuilderRelevancyAnalyzer
    {
        IList<IRelevantBuilder> GetRelevantBuilders(DonneesRapportIllustration donnees, IReportContext context);
    }
}