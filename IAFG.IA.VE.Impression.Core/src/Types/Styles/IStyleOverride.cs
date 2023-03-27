using IAFG.IA.VE.Impression.Core.Types.Enums;
using Section = IAFG.IA.VE.Impression.Core.Types.Enums.Section;

namespace IAFG.IA.VE.Impression.Core.Types.Styles
{
    public interface IStyleOverride
    {
        Section SectionType { get; }
        MarginLevel MarginLevel { get; set; }
        bool MoveAllLabels { get; }
    }
}
