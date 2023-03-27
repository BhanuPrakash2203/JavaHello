using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers
{
    public interface ISectionModelMapper
    {
        SectionResultatModel MapperDefinition(SectionResultatModel model, DefinitionSectionResultat definition, DonneesRapportIllustration donnees);
        ISectionModel MapperDefinition(ISectionModel model, IDefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context);
    }
}