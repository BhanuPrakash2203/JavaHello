namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IPercentageFormatter
    {
        string Format(int value);
        string Format(double value, bool baseEst100);
        string Format(float value, bool baseEst100);
        string Format(string value, bool baseEst100);
        string FormatWithoutDecimals(float value, bool baseEst100);
        string FormatWithoutDecimals(double value, bool baseEst100);
    }
}