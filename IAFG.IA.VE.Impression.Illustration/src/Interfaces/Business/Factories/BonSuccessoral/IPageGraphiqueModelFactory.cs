using IAFG.IA.VE.Impression.Core.Interface.Factories;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral
{
    public interface IPageGraphiqueModelFactory : IFactoryBase<PageGraphiqueModel>
    {
        PageGraphiqueModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context);
    }
}