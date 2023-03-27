using System;

namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IDateFormatter
    {
        string Format(DateTime value);

        string Format(DateTime value, bool includeTime);
        string FormatXeAnnee(int annee);
    }
}