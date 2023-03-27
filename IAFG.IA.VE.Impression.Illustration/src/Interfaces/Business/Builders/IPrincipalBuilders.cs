using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders
{
    public interface IPrincipalBuilders
    {
        IPageIntroductionBuilder PageIntroductionBuilder { get; }
        IPageSommaireProtectionsBuilder PageSommaireProtectionsBuilder { get; }
        IPagePrimesRenouvellementBuilder PagePrimesRenouvellementBuilder { get; }
        IPageHypotheseInvestissementBuilder PageHypotheseInvestissementBuilder { get; }
        IPageSignatureBuilder PageSignatureBuilder { get; }
        IPageDescriptionsProtectionsBuilder PageDescriptionsProtectionsBuilder { get; }
        IPageSommaireProtectionsIllustrationBuilder PageSommaireProtectionsIllustrationBuilder { get; }
        IPageApercuProtectionsBuilder PageApercuProtectionsBuilder { get; }
        IPageConditionsMedicalesBuilder PageConditionsMedicalesBuilder { get; }
        IPageNotesIllustrationBuilder PageNotesIllustrationBuilder { get; }
        IPageConceptVenteBuilder PageConceptVenteBuilder { get; }
        IPageModificationsDemandeesBuilder PageModificationsDemandeesBuilder { get; }
        IRelevantBuilder GetBuilderPrimesDeRenouvellement(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderHypothesesInvestissement(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderIntroduction(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderConditionsMedicales(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderModificationsDemandees(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderConceptVente(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderNotesIllustration(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderApercuProtections(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderSommaireProtections(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderDescriptionsProtections(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderTestDeSensibilite(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderSignature(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
        IRelevantBuilder GetBuilderProtections(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context);
    }
}