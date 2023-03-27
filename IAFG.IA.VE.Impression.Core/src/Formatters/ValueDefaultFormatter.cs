using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class ValueDefaultFormatter : ValueFormatter
    {
        public ValueDefaultFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder) : base(cultureAccessor, dateBuilder)
        {
        }
    }
}
