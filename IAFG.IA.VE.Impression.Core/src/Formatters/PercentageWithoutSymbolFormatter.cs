using System;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class PercentageWithoutSymbolFormatter : ValueFormatter, IPercentageWithoutSymbolFormatter
    {
        // ReSharper disable once InconsistentNaming
        private const string FORMAT_P0 = "P0";

        // ReSharper disable once InconsistentNaming
        private const string FORMAT_P2 = "P2";

        public PercentageWithoutSymbolFormatter(ICultureAccessor cultureAccessor, IDateBuilder dateBuilder)
            : base(cultureAccessor, dateBuilder)
        {
        }

        public override string Format(int value) => CalculatePercentage(value, FORMAT_P0, true).Replace("%", "").Trim();

        public override string Format(double value) => Format(value, false);

        public string Format(double value, bool baseEst100) => CalculatePercentage(value, FORMAT_P2, baseEst100).Replace("%", "").Trim();

        public override string Format(float value) => Format(value, false);

        public string Format(float value, bool baseEst100) => CalculatePercentage(value, FORMAT_P2, baseEst100).Replace("%", "").Trim();

        public override string Format(string value) => Format(value, true);

        public string Format(string value, bool baseEst100)
        {
            if (value == null) return string.Empty;

            double temp;
            return !double.TryParse(value, out temp) ? value : CalculatePercentage(value, FORMAT_P0, baseEst100).Replace("%", "").Trim();
        }

        public string FormatWithoutDecimals(float value) => FormatWithoutDecimals(value, false);

        public string FormatWithoutDecimals(float value, bool baseEst100) => CalculatePercentage(value, FORMAT_P0, baseEst100).Replace("%", "").Trim();

        public string FormatWithoutDecimals(double value) => FormatWithoutDecimals(value, false);

        public string FormatWithoutDecimals(double value, bool baseEst100) => CalculatePercentage(value, FORMAT_P0, baseEst100).Replace("%", "").Trim();

        private string CalculatePercentage(object value, string format, bool baseEst100)
        {
            if (value == null) return string.Empty;

            var valueAsDouble = Convert.ToDouble(value);
            return (baseEst100 ? valueAsDouble / 100 : valueAsDouble).ToString(format, CultureAccessor.GetCultureInfo());
        }
    }
}