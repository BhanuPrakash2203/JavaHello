using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral
{
    public interface IBonSuccessoralBuilders
    {
        IPageTitreBuilder PageTitreBuilder { get; }
        IPageGraphiqueBuilder PageGraphiqueBuilder { get; }
        IPageSommaireBonSuccessoralBuilder PageSommaireBonSuccessoralBuilder { get; }

        IRelevantBuilder GetBuilderPageTitre(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderSommaireBonSuccessoral(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderGraphique(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
    }
}
