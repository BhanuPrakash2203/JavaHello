using System;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class DateFormatter : ValueFormatter, IDateFormatter
    {
        public DateFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder)
            : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(DateTime value) => DateBuilder.WithShortDateFormat().WithInvariantCulture().Build(value);

        public string Format(DateTime value, bool includeTime) => !includeTime ? Format(value) : DateBuilder.WithShortDateFormat().WithTime().WithInvariantCulture().Build(value);

        public string FormatXeAnnee(int annee)
        {
            string s = string.Empty;
            if (Equals(CultureAccessor.GetCultureInfo(), CultureHelper.FrenchCulture))
            {
                s = "e";
            }
            else
            {
                switch (annee)
                {
                    case 1:
                        s = "st";
                        break;

                    case 2:
                        s = "nd";
                        break;

                    case 3:
                        s = "rd";
                        break;

                    default:
                        s = "th";
                        break;
                }
            }
     
            return string.Concat(annee.ToString(), s);
        }
    }
}