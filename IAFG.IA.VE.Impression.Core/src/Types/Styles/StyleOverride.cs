using IAFG.IA.VE.Impression.Core.Types.Enums;
using Section = IAFG.IA.VE.Impression.Core.Types.Enums.Section;

namespace IAFG.IA.VE.Impression.Core.Types.Styles
{
    public class StyleOverride : IStyleOverride
    {
        public MarginLevel MarginLevel { get; set; }
        public bool MoveAllLabels { get; set; }

        public Section SectionType { get; set; }

    }
}
