using System;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using ENUMs_ProjectionData = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    internal static class EnumsExtension
    {
        public static string GetDescription(this TypeCout typeCout, IResourcesAccessor ressourcesAcessor)
        {
            if (typeCout == TypeCout.Nivele || typeCout == TypeCout.NiveleEpargne)
                return ressourcesAcessor.GetStringResourceById("_coûts_niveles");

            if (typeCout == TypeCout.TRA)
                return ressourcesAcessor.GetStringResourceById("_coûts_tra");

            return string.Empty;
        }
        public static Sexe ConvertirSex(this ENUMs_ProjectionData.Sex sex)
        {
            return (Sexe) sex;
        }

        public static TypeAssurance ConvertirTypeAssurance(this ENUMs_ProjectionData.Coverage.InsuranceType insuranceType)
        {
            return (TypeAssurance) insuranceType;
        }

        public static string ObtenirLibelle(this TypeAssurance valeur, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            return resourcesAccessor.GetResourcesAccessor().GetStringResourceById(valeur.ToString());
        }

        public static TypeFrequenceFacturation ConvertirFrequence(this ENUMs_ProjectionData.Billing.Frequency frequency)
        {
            return (TypeFrequenceFacturation) frequency;
        }

        public static TypePret ConvertirTypePret(this ENUMs_ProjectionData.Financial.LoanType loanType)
        {
            return (TypePret) loanType;
        }

        public static bool IsStatusPreferentiel(this ENUMs_ProjectionData.SmokerType smokerClass)
        {
            switch (smokerClass)
            {
                case ENUMs_ProjectionData.SmokerType.SmokerElite:
                case ENUMs_ProjectionData.SmokerType.SmokerPreferred:
                case ENUMs_ProjectionData.SmokerType.NonSmokerElite:
                case ENUMs_ProjectionData.SmokerType.NonSmokerPreferred:
                    return true;
                case ENUMs_ProjectionData.SmokerType.NonApplicable:
                case ENUMs_ProjectionData.SmokerType.Unspecified:
                case ENUMs_ProjectionData.SmokerType.Smoker:
                case ENUMs_ProjectionData.SmokerType.NonSmoker:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(smokerClass), smokerClass, null);
            }
        }

        public static string ObtenirLibelle(this Sexe valeur, TypeAffichageSexe typeAffichage, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            return resourcesAccessor.GetResourcesAccessor().GetStringResourceById(typeAffichage == TypeAffichageSexe.Genre ? $"genre_{valeur}" : $"sexe_{valeur}");
        }

        public static string ObtenirLibelle(this TypeDuree typeDuree, IIllustrationResourcesAccessorFactory resourcesAccessor)
        {
            switch (typeDuree)
            {
                case TypeDuree.Vie:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_vie");
                case TypeDuree.AgeMaximum:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_jusqu_a_X_ans");
                case TypeDuree.DateTerminaison:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_jusqu_au");
                case TypeDuree.DurantNombreAnnees:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_durant_X_ans");
                case TypeDuree.NombreAnnees:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_X_ans");
                case TypeDuree.ParMois:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_par_mois");
                case TypeDuree.ParMoisDurantNombreAnnees:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_par_mois_durant_X_ans");
                case TypeDuree.PendantNombreAnnees:
                    return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_pendant_X_ans");
                case TypeDuree.NonDefini:
                    return "{0}";
                default:
                    return string.Empty;
            }
        }

        public static bool EstScenarioPrimeCalculee(this ENUMs_ProjectionData.Billing.PremiumScenario scenario)
        {
            return scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_QuickPayment ||
                   scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_TargetedFund_Duration ||
                   scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_TargetedFund_Premium ||
                   scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_KeepInForce ||
                   scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_LeveledMaximum ||
                   scenario == ENUMs_ProjectionData.Billing.PremiumScenario.Calculated_ReturnPremiumBased_Period;
        }
    }
}