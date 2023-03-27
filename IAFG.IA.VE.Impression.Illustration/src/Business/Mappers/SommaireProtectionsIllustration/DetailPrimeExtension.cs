using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration
{
    internal static class DetailPrimeExtension
    {
        public static string FormatterDescription(
            this Types.SectionModels.SommaireProtectionsIllustration.DetailPrime source,
            IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatterEnum<TypeDetailPrime>(source.TypeDetailPrime.ToString());
        }

        public static string FormatterMontantAvecTaxe(
            this Types.SectionModels.SommaireProtectionsIllustration.DetailPrime source,
            IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatDecimal(source.MontantAvecTaxe);
        }
    }
}