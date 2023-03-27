
namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IBooleanFormatter : IFormatter
    {
        string ToYesOrNo(bool value);

        string ToXOrNothing(bool value);
    }
}
