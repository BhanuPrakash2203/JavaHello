namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IDecimalFormatter
    {
        string Format(int value);
        string Format(double value);
        string Format(float value);
    }
}