using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration
{
    public static class DetailPrimeVerseeExtension
    {
        public static string FormatterDescription(this DetailPrimesVersees source,
            IIllustrationReportDataFormatter illustrationReportDataFormatter,
            IIllustrationResourcesAccessorFactory illustrationResourcesAccessorFactory)
        {
            if (source.FacteurMultiplicateur > 0)
            {
                if (source.TypeScenarioPrime == TypeScenarioPrime.Variable_Minimale)
                    return GetDescriptionWithMultiplicateur(illustrationReportDataFormatter,
                        illustrationResourcesAccessorFactory, "XMinimale", source.FacteurMultiplicateur);

                if (source.TypeScenarioPrime == TypeScenarioPrime.Variable_Reference)
                    return GetDescriptionWithMultiplicateur(illustrationReportDataFormatter,
                        illustrationResourcesAccessorFactory, "XReference", source.FacteurMultiplicateur);
            }

            return illustrationReportDataFormatter.FormatterEnum<TypeScenarioPrime>(source.TypeScenarioPrime
                .ToString());
        }

        public static string FormatterMontant(this DetailPrimesVersees source,
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

        public static string FormatterPeriode(this DetailPrimesVersees source,
            IIllustrationReportDataFormatter illustrationReportDataFormatter)
        {
            if (source.Duree.HasValue)
            {
                return illustrationReportDataFormatter.FormatterPeriodeAnneesDebutFin(source.Annee, source.Duree.Value)
                    .FirstCharToUpper();
            }

            return illustrationReportDataFormatter.FormatterPeriodeAnneeMois(source.Annee, source.Mois)
                .FirstCharToUpper();
        }

        public static string FirstCharToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
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
