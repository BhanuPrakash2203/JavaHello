using System;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class LongDateFormatter: ValueFormatter, ILongDateFormatter
    {
        public LongDateFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder)
            : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(DateTime value) => DateBuilder.WithLongDateFormat().Build(value);

        public string Format(DateTime value, bool includeTime) => !includeTime ? Format(value) : DateBuilder.WithLongDateFormat().WithTime().Build(value);
    }
}