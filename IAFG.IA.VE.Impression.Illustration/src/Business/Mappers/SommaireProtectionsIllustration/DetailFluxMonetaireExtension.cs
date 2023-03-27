using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration
{
    internal static class DetailFluxMonetaireExtension
    {
        public static string FormatterDescription(this DetailFluxMonetaire source, IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatterEnum("FluxMonetaireTransaction", source.Type);
        }

        public static string FormatterPeriode(this DetailFluxMonetaire source, IIllustrationReportDataFormatter formatter)
        {
            return formatter.FormatterPeriodeAnnees(source.AnneeDebut, source.AnneeFin, true).PremiereLettreEnMajuscule();
        }

        public static string FormatterMontant(this DetailFluxMonetaire source, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            if (source.TypeMontant == TypeMontantFluxMonetaires.Maximum)
            {
                if (source.EstDepotRetraitMaximal && !source.EstDepotRetaitApresDecheance)
                {
                    return $"{formatter.FormatDecimal(source.Montant)} ({resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Maximal")})";
                }

                return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Maximal");
            }

            return formatter.FormatDecimal(source.Montant);
        }
    }
}