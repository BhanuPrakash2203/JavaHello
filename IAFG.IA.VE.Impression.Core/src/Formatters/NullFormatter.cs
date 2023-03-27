using IAFG.IA.VE.Impression.Core.Interface.Formatters;

namespace IAFG.IA.VE.Impression.Core.Formatters
{
    public class NullFormatter : IDefaultValueFormatter 
    {
        public string Format(string value)
        {
            return value;
        }
    }
}
