using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders
{
    public interface IGeneriqueBuilders
    {
        IPageSectionBuilder PageSectionBuilder { get; }
        IPageGlossaireBuilder PageGlossaireBuilder { get; }
        IPageResultatBuilder PageResultatBuilder { get; }
        IPageResultatAssureBuilder PageResultatAssureBuilder { get; }

        IRelevantBuilder GetBuilderGlossaire(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderSection(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderProjectionParGroupeAssure(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderProjection(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
    }
}