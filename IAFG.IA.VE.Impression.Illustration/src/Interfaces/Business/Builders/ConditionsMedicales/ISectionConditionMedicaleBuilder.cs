using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ConditionsMedicales
{
    public interface ISectionConditionMedicaleBuilder
    {
        void Build(BuildParameters<ConditionMedicaleViewModel> parameters);
    }
}