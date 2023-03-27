using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionProtectionsMapper : ReportMapperBase<SectionProtectionsModel, ProtectionViewModel>, ISectionProtectionsMapper
    {
        public SectionProtectionsMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {
            public ReportProfile(
                IIllustrationReportDataFormatter formatter,
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory) : base(formatter, resourcesAccessor, managerFactory)
            {
            }

            protected override void ConfigureMapping(
                IIllustrationReportDataFormatter formatter,
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory)
            {
                CreateMap<SectionProtectionsModel, ProtectionViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation)).
                    ForMember(d => d.PrestationDeces, m => m.MapFrom(s => formatter.FormatterEnum<OptionPrestationDeces>(s.PrestationDeces.ToString()))).
                    ForMember(d => d.CapitalAssurePlancher, m => m.MapFrom(s => FormatCapitalAssurePlancher(formatter, resourcesAccessor, s))).
                    ForMember(d => d.DureeDebutMinimisation, m => m.MapFrom(s => FormatDureeMinimisation(formatter, resourcesAccessor, s))).
                    ForMember(d => d.CapitalAssurePlafond, m => m.MapFrom(s => FormatCapitalAssurePlafond(formatter, s))).
                    ForMember(d => d.StatutOacaActif, m => m.MapFrom(s => formatter.FormatOuiNon(s.StatutOacaActif))).
                    ForMember(d => d.PrimeTotale, m => m.MapFrom(s => formatter.FormatCurrency(s.MontantPrimeTotal))).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailProtection, ProtectionAssureViewModel>().
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId)).
                    ForMember(d => d.Noms, m => m.MapFrom(s => s.Noms.JoinStringLines())).
                    ForMember(d => d.TypeAssurance, m => m.MapFrom(s => formatter.FormatterEnum<TypeAssurance>(s.TypeAssurance.ToString()))).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.Description)).
                    ForMember(d => d.DescriptionAvecReferences, m => m.MapFrom(s => FormatterDescriptionAvecReferences(s, formatter))).
                    ForMember(d => d.ReferenceNotes, m => m.MapFrom(s => FormatterReferenceNotes(s.ReferenceNotes))).
                    ForMember(d => d.TypeCout, m => m.MapFrom(s => formatter.FormatterEnum<TypeCout>(s.TypeCout.ToString()))).
                    ForMember(d => d.SousDescription,
                              m =>
                              m.MapFrom(
                                        s =>
                                        FormatterSousDescription(s.TypeProtectionComplementaire, s.AgeMaturitePlan,s.TypePrestationPlan, formatter))).
                    ForMember(d => d.MontantCapitalAssureActuel,
                              m => m.MapFrom(s => ActionSiSansVolumeAssurance(s.SansVolumeAssurance, () => string.Empty, () => formatter.FormatCurrency(s.MontantCapitalAssureActuel)))).
                    ForMember(d => d.MontantPrimeMinimale,
                              m =>
                              m.MapFrom(
                                        s =>
                                        ActionSiLibereeOuNon(s.EstLiberee, () => resourcesAccessor.GetResourcesAccessor().GetStringResourceById("ProtectionLiberee"), () => formatter.FormatCurrency(s.MontantPrimeMinimale)))).
                    ForMember(d => d.DureeProtection, m => m.MapFrom(s => !s.EstNouvelleProtection ? FormatterDureeProtection(s, formatter, resourcesAccessor) : string.Empty)).
                    ForMember(d => d.DureeNouvelleProtection, m => m.MapFrom(s => s.EstNouvelleProtection ? FormatterDureeNouvelleProtection(s, resourcesAccessor) : string.Empty)).
                    ForMember(d => d.DureePaiement,
                              m => m.MapFrom(s => !s.EstNouvelleProtection ? ActionSiLibereeOuNon(s.EstLiberee, () => string.Empty, () => FormatterDureePaiement(s, formatter, resourcesAccessor)) : string.Empty)).
                    ForMember(d => d.DureePaiementNouvelleProtection,
                              m => m.MapFrom(s => s.EstNouvelleProtection ? FormatterDureePaiementNouvelleProtection(s, resourcesAccessor) : string.Empty)).
                    ForMember(d => d.Taux, 
                        m => 
                            m.MapFrom(s => 
                                FormatterTaux(s.Age, s.Sexe, s.Age < ConstanteFonctionnelle.AGE_ASSURANCE_STATUT_TABAGISME ? StatutTabagisme.NonDefini : s.StatutTabagisme, formatter, resourcesAccessor))).
                    ForMember(d => d.EstProtectionContractant, m => m.MapFrom(s => s.EstProtectionContractant)).
                    ForMember(d => d.EstProtectionConjointe, m => m.MapFrom(s => s.EstProtectionConjointe)).
                    ForMember(d => d.EstProtectionBase, m => m.MapFrom(s => s.EstProtectionBase)).
                    ForMember(d => d.ContientSurprime, m => m.MapFrom(s => s.EstSurprimee ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Surprime") : "")).
                    ForMember(d => d.InclusSurprime, m => m.MapFrom(s => s.EstSurprimee ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("SurprimeIncluse"), "") : "")).
                    ForMember(d => d.DescriptionAsl, m => m.MapFrom(s => s.CapitalAssureAsl.HasValue && s.CapitalAssureAsl > 0 ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DescriptionAsl"), "") : "")).
                    ForMember(d => d.DescriptionOaca, m => m.MapFrom(s => s.CapitalAssureOaca.HasValue && s.CapitalAssureOaca > 0 ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DescriptionOaca"), "") : "")).
                    ForMember(d => d.CapitalAssureAsl, m => m.MapFrom(s => formatter.FormatCurrency(s.CapitalAssureAsl))).
                    ForMember(d => d.CapitalAssureOaca, m => m.MapFrom(s => formatter.FormatCurrency(s.CapitalAssureOaca)));
            }

            internal static string FormatCapitalAssurePlafond(IIllustrationReportDataFormatter formatter,
                SectionProtectionsModel s)
            {
                if (s.ValeurMaximisee == null) return string.Empty;
                return s.ValeurMaximisee.CapitalAssurePlafond.HasValue
                    ? formatter.FormatCurrency(s.ValeurMaximisee.CapitalAssurePlafond)
                    : string.Empty;
            }

            internal static string FormatCapitalAssurePlancher(IIllustrationReportDataFormatter formatter,
                IResourcesAccessorFactory resourcesAccessor, 
                SectionProtectionsModel s)
            {
                if (s.ValeurMaximisee == null) return string.Empty;
                return s.ValeurMaximisee.CapitalAssurePlancher.HasValue
                    ? formatter.FormatCurrency(s.ValeurMaximisee.CapitalAssurePlancher.Value)
                    : resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_nonActive");
            }

            internal static string FormatDureeMinimisation(IIllustrationReportDataFormatter formatter,
                IResourcesAccessorFactory resourcesAccessor,
                SectionProtectionsModel s)
            {
                if (s.ValeurMaximisee == null) return string.Empty;
                if (s.ValeurMaximisee.CapitalAssurePlafond.HasValue) return string.Empty;
                return s.ValeurMaximisee.DureeDebutMinimisation.HasValue
                    ? formatter.FormatterDuree(TypeDuree.NombreAnnees, s.ValeurMaximisee.DureeDebutMinimisation.Value)
                    : resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_nonActivee");
            }

            private static string FormatterDescriptionAvecReferences(DetailProtection detailProtection, IIllustrationReportDataFormatter formatter)
            {
                var typeCout = formatter.FormatterEnum<TypeCout>(detailProtection.TypeCout.ToString());
                var references = FormatterReferenceNotes(detailProtection.ReferenceNotes);
                if (string.IsNullOrEmpty(typeCout) && string.IsNullOrEmpty(references)) return detailProtection.Description;
                if (string.IsNullOrEmpty(typeCout)) return string.Join(" ", detailProtection.Description, references);
                var description = string.Join(", ", detailProtection.Description, typeCout);
                return string.Join(" ", description, references);
            }

            private static string ActionSiSansVolumeAssurance(bool sansVolumeAssurance, Func<string> sansVolumeAssuranceFunc, Func<string> avecVolumeAssuranceFunc)
            {
                return sansVolumeAssurance ? sansVolumeAssuranceFunc.Invoke() : avecVolumeAssuranceFunc.Invoke();
            }

            private static string ActionSiLibereeOuNon(bool estLiberee, Func<string> libereeFunc, Func<string> nonLibereeFunc)
            {
                return estLiberee ? libereeFunc.Invoke() : nonLibereeFunc.Invoke();
            }

            private static string FormatterReferenceNotes(IEnumerable<int> referenceNotes)
            {
                var result = new StringBuilder();
                if (referenceNotes == null) return result.ToString();
                foreach (var item in referenceNotes)
                {
                    result.Append($"({item})");
                }

                return result.ToString();
            }

            private static string FormatterSousDescription(TypeProtectionComplementaire complementaire, int? ageMaturite, TypePrestationPlan prestation, IIllustrationReportDataFormatter formatter)
            {
                var libelleComplementaire = formatter.FormatterEnum<TypeProtectionComplementaire>(complementaire.ToString());
                var libellePrestation = formatter.FormatterEnum<TypePrestationPlan>(prestation.ToString());

                var result = new StringBuilder();
                if (!string.IsNullOrEmpty(libelleComplementaire))
                {
                    result.AppendLine(string.Format(libelleComplementaire, ageMaturite));
                }

                if (!string.IsNullOrEmpty(libellePrestation))
                {
                    result.AppendLine(string.Format(libellePrestation));
                }

                return result.ToString();
            }

            private static string FormatterTaux(int? age, Sexe? sexe, StatutTabagisme? statutTabagisme,
                IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var libelles = new List<string>
                {
                    sexe.HasValue ? formatter.FormatSexe(sexe.Value, TypeAffichageSexe.Genre) : string.Empty,
                    age.HasValue ? formatter.FormatAge(age.Value) : string.Empty,
                    formatter.FormatUsageTabac(statutTabagisme)
                };

                return
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Taux")}{formatter.AddColon()} {string.Join(" - ", libelles.Where(x => !string.IsNullOrEmpty(x)))}";
            }

            private static string FormatterDureePaiement(DetailProtection detailProtection, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                if (detailProtection.DateLiberation.HasValue)
                    result.AppendLine($"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("PrimesPayables")} {formatter.FormatterDuree(TypeDuree.DateTerminaison, formatter.FormatLongDate(detailProtection.DateLiberation))}");

                return result.ToString();
            }

            private static string FormatterDureePaiementNouvelleProtection(DetailProtection detailProtection, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                var labelDureePaiement = detailProtection.DureeRenouvellement.HasValue ?
                    resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DureePaiementProtectionRenouvelable") :
                    resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DureePaiement");
                var anneesFormatees = string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_X_ans"), detailProtection.DureePaiement);
                result.AppendLine(string.Format(labelDureePaiement, anneesFormatees));

                return result.ToString();
            }

            private static string FormatterDureeProtection(DetailProtection detailProtection, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                result.Append($"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Emission")}{formatter.AddColon()} {formatter.FormatLongDate(detailProtection.DateEmission)}");
                result.Append(" - ");
                var lifeTimeReference = new DateTime(1001, 01, 01);
                result.Append(detailProtection.DateMaturite.Value.CompareTo(lifeTimeReference) == 0
                        ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("ProtectionAVie")
                        : $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Expiration")}{formatter.AddColon()} {formatter.FormatLongDate(detailProtection.DateMaturite)}");

                return result.ToString();
            }

            private static string FormatterDureeNouvelleProtection(DetailProtection detailProtection, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                var lifeTimeReference = new DateTime(1001, 01, 01);
                var durationFormatees = detailProtection.DateMaturite.Value.CompareTo(lifeTimeReference) == 0
                                ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AVie") : string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_X_ans"), detailProtection.DureeProtection);
                if (detailProtection.DureeRenouvellement.HasValue)
                    result.AppendLine(string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DureeProtectionRenouvelable"), durationFormatees));
                else
                    result.AppendLine(string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DureeProtection"), durationFormatees));                

                return result.ToString();
            }
        }

    }
}