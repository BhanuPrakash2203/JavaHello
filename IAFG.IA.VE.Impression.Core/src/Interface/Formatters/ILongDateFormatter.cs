using System;

namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface ILongDateFormatter
    {
        string Format(DateTime value);

        string Format(DateTime value, bool includeTime);
    }
}