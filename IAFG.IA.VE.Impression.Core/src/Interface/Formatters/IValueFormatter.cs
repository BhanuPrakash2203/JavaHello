using System;

namespace IAFG.IA.VE.Impression.Core.Interface.Formatters
{
    public interface IValueFormatter : IFormatter 
    {
        string Format(object value);
        string Format(DateTime value);
        string Format(double value);
        string Format(float value);
        string Format(int value);
        string Format(string value);
    }
}