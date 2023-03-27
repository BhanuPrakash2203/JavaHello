using IAFG.IA.VE.Impression.Core.Interface.Formatters;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class BaseFormatter
    {
        protected readonly IDefaultValueFormatter _defaultValueFormatter;

        public BaseFormatter(IDefaultValueFormatter defaultValueFormatter)
        {
            _defaultValueFormatter = defaultValueFormatter;
        }

        public BaseFormatter()
        {
            _defaultValueFormatter = new NullFormatter();
        }
    }
}
