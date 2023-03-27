using System;

namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IDateBuilder
    {
        IDateBuilder WithShortDateFormat();
        IDateBuilder WithLongDateFormat();
        IDateBuilder WithTime();
        IDateBuilder WithInvariantCulture();
        string Build(DateTime value);
        IDateBuilder WithPrivacy();
    }
}
