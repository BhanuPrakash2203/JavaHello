using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    internal static class DetailPrimeVerseeExtension
    {
        public static string FormatterDescription(this DetailPrimeVersee source,
            IIllustrationReportDataFormatter illustrationReportDataFormatter,
            IIllustrationResourcesAccessorFactory illustrationResourcesAccessorFactory)
        {
            if (source.FacteurMultiplicateur > 0)
            {
                if (source.TypeScenarioPrime == TypeScenarioPrime.Variable_Minimale)
                {
                    return GetDescriptionWithMultiplicateur(illustrationReportDataFormatter,
                        illustrationResourcesAccessorFactory, "XMinimale", source.FacteurMultiplicateur);
                }

                if(source.TypeScenarioPrime == TypeScenarioPrime.Variable_Reference)
                {
                    return GetDescriptionWithMultiplicateur(illustrationReportDataFormatter,
                        illustrationResourcesAccessorFactory, "XReference", source.FacteurMultiplicateur);
                }

            }

            return illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(source.TypeScenarioPrime
                .ToString());
        }

        public static string FormatterMontant(this DetailPrimeVersee source,
            IIllustrationReportDataFormatter illustrationReportDataFormatter,
            IResourcesAccessorFactory resourcesAccessorFactory)
        {
            if (source.TypeScenarioPrime == TypeScenarioPrime.ModalePlusODE && source.Montant.GetValueOrDefault() > 0)
            {
                return resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("Prime") + 
                       " + " + illustrationReportDataFormatter.FormatCurrency(source.Montant);
            }

            return illustrationReportDataFormatter.FormatCurrency(source.Montant);
        }

        public static string FormatterPeriode(this DetailPrimeVersee source,
            IIllustrationReportDataFormatter illustrationReportDataFormatter)
        {
            if (source.Duree.HasValue)
            {
                return illustrationReportDataFormatter.FormatterDuree(TypeDuree.PendantNombreAnnees,
                    source.Duree.Value);
            }

            return illustrationReportDataFormatter.FormatterPeriodeAnneeMois(source.Annee, source.Mois);
        }

        public static string FormatterFrequenceFacturation(this DetailPrimeVersee source,
            IResourcesAccessorFactory resourcesAccessorFactory)
        {
            return resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById(source.FrequenceFacturation == TypeFrequenceFacturation.Annuelle ? "Annuelle" : "Mensuelle");
        }

        private static string GetDescriptionWithMultiplicateur(
            IIllustrationReportDataFormatter illustrationReportDataFormatter,
            IResourcesAccessorFactory resourcesAccessorFactory, string resourceId,
            double multiplicateur)
        {
            return string.Format(
                resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById(resourceId),
                illustrationReportDataFormatter.FormatDecimal(multiplicateur));
        }
    }
}
