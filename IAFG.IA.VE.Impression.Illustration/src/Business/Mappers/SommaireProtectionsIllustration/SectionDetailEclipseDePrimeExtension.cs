using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration
{
    internal static class SectionDetailEclipseDePrimeExtension
    {
        public static string FormatterDescriptionActivationEclipseDePrime(this SectionDetailEclipseDePrimeModel source, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            return $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("EclipseDePrimeActivation")}{formatter.AddColon()}";
        }

        public static IList<string[]> FormatterBaremes(this SectionDetailEclipseDePrimeModel source, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            var results = new List<string[]>();

            if (source.Baremes == null)
            {
                return results;
            }

            results.AddRange(source
                .Baremes
                .Select(b => new List<string>
                {
                    b.FormatterDiminutionBareme(resourcesAccessor),
                    formatter.FormatterPeriodeAnnees(b.Annee, null)
                }.ToArray()));
            
            return results;
        }

        public static string FormatterDiminutionBareme(this Bareme bareme, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            var resourceId = bareme.Diminution.HasValue ? "BaremeAlternatif" : "BaremeCourant";
            return resourcesAccessor.GetResourcesAccessor().GetStringResourceById(resourceId);
        }
    }
}
