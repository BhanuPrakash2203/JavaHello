using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    public static class VecteurIllustrationExtension
    {
        public static double[] SommeValeursToutLesGroupesAssures(this Projection projection, int colonne)
        {
            var ar = projection.Columns?.Where(x => x.Id == colonne && !string.IsNullOrWhiteSpace(x.Insured)).Select(x => x.Value);
            return SumArrays(ar);
        }

        private static double[] SumArrays(this IEnumerable<double[]> arrayValues)
        {
            if (arrayValues == null) return null;
            var arraysDoubles = ManageNullValueArrays(arrayValues);
            var result =
                (from array in arraysDoubles
                 from valueIndex in array.Select((value, index) => new { Value = value, Index = index })
                 group valueIndex by valueIndex.Index
                 into indexGroups
                 select indexGroups.Select(indexGroup => indexGroup.Value).ToList().Sum()).ToArray();

            return result;
        }

        private static IEnumerable<double[]> ManageNullValueArrays(IEnumerable<double[]> arrayValues)
        {
            if (arrayValues == null) return new List<double[]>();
            var arraysDoubles = arrayValues.Select(x => x ?? new[] { 0.0 }).ToArray();
            var lengths = arraysDoubles.Select(x => x.Length).ToArray();
            var length = lengths.Any() ? lengths.Max() : 0;
            return arraysDoubles.Select(x => ManageLengthArray(x, length)).ToList();
        }

        private static double[] ManageLengthArray(double[] arrayValue, int length)
        {
            if (arrayValue.Length < length) Array.Resize(ref arrayValue, length);
            return arrayValue;
        }
    }
}