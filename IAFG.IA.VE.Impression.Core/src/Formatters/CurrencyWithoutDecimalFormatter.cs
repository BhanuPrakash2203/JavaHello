using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class CurrencyWithoutDecimalFormatter : ValueFormatter, ICurrencyWithoutDecimalFormatter
    {
        public CurrencyWithoutDecimalFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder) : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(int value)
        {
            return value.ToString("C0", CultureAccessor.GetCultureInfo());
        }

        public override string Format(double value)
        {
            return Format((int)value);
        }

        public override string Format(float value)
        {
            return Format((int)value);
        }
    }
}