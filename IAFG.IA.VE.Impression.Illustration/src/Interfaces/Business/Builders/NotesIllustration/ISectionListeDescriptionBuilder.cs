using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.NotesIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.NotesIllustration
{
    public interface ISectionListeDescriptionBuilder
    {
        void Build(BuildParameters<SectionListeViewModel> parameters);
    }
}