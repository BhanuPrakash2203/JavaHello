using System;
using System.Collections.Generic;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Extensions;
using EnumProjection = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    internal static class ProjectionsExtension
    {
        public static double GetMaxValue(this List<KeyValuePair<Characteristic, double>> values,
            EnumProjection.ValueId enum1, EnumProjection.ValueId enum2)
        {
            // Permet de recupérer la valeur peut importe que l'on soit en vigueur ou en nouvelle vente.
            var v1 = values.Search(enum1) ?? 0;
            var v2 = values.Search(enum2) ?? 0;
            return Math.Max(v1, v2);
        }

        public static double GetMaxValueByCoverage(this List<KeyValuePair<Characteristic, double>> values, string id,
            EnumProjection.ValueId enum1, EnumProjection.ValueId enum2)
        {
            // Permet de recupérer la valeur peut importe que l'on soit en vigueur ou en nouvelle vente.
            var v1 = values.SearchByCoverage(id, enum1) ?? 0;
            var v2 = values.SearchByCoverage(id, enum2) ?? 0;
            return Math.Max(v1, v2);
        }
    }
}