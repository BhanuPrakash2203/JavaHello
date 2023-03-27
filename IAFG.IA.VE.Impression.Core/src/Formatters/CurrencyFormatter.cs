using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class CurrencyFormatter : ValueFormatter, ICurrencyFormatter
    {
        public CurrencyFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder) : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(int value) => Format((double) value);

        public override string Format(float value) => Format((double) value);

        public override string Format(double value) => value.ToString("C2", CultureAccessor.GetCultureInfo());
    }
}