using Syncfusion.Drawing;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public static class PositionExtensions
    {
        public static RectangleF GetRectangleF(this Position position)
        {
            return new RectangleF(position.X, position.Y, position.Width, position.Height);
        }
    }
}