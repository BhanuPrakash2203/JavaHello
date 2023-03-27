using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class DecimalFormatter: ValueFormatter, IDecimalFormatter
    {
        public DecimalFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder) : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(int value) => Format((double)value);

        public override string Format(float value) => Format((double)value);

        public override string Format(double value) => value.ToString("N2", CultureAccessor.GetCultureInfo());
    }
}