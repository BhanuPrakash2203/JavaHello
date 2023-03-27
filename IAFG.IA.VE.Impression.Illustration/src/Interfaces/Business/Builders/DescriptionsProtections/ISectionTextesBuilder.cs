using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections
{
    public interface ISectionTextesBuilder
    {
        void Build(BuildParameters<DescriptionViewModel> parameters);
    }
}