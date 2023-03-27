using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    public static class StringExtension
    {
        public static string JoinStringLines(this IEnumerable<string> valeurs)
        {
            var sb = new StringBuilder();
            if (valeurs == null) return sb.ToString();
            foreach (var item in valeurs)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        public static string PremiereLettreEnMajuscule(this string valeur)
        {
            return !string.IsNullOrEmpty(valeur) ? valeur.First().ToString().ToUpper() + valeur.Substring(1) : valeur;
        }
    }
}