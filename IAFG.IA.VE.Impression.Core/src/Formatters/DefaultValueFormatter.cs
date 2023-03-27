using IAFG.IA.VE.Impression.Core.Interface.Formatters;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class DefaultValueFormatter: BaseFormatter, IDefaultValueFormatter
    {
        public static readonly string EmptyValueReplacement = string.Empty;

        public string Format(string value)
        {
            return string.IsNullOrEmpty(value) ? EmptyValueReplacement : value;
        }
    }
}
