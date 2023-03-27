using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections
{
    public interface IAssuranceSupplementaireLibereeModelBuilder
    {
        SectionASLModel Build(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context);
    }
}