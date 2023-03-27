using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Pdf;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public class TextData
    {
        public TextData(int index, List<TextLine> textLines, string text)
        {
            Index = index;
            Text = text;
            Words = text.Split(new []{" "}, StringSplitOptions.RemoveEmptyEntries).ToList();

            TextLines = textLines;
            var textLine = textLines.FirstOrDefault();

            if (textLine != null)
            {
                Position = new Position
                {
                    X = textLine.Bounds.X,
                    Y = textLine.Bounds.Y,
                    Height = textLine.Bounds.Height,
                    Width = textLine.Bounds.Width
                };
            }
        }

        public int Index { get; }
        public string Text { get; }
        public List<string> Words { get; }
        public TextData Found { get; set; }
        public TextData Matched { get; set; }
        public List<TextLine> TextLines { get; }
        public Position Position { get; }
    }
}