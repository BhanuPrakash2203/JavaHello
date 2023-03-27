using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using DetailFluxMonetaire = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailFluxMonetaire;
using PrimeViewModel = IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration.PrimeViewModel;
using ProtectionViewModel = IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration.ProtectionViewModel;
using FluxMonetaireDetailViewModel = IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration.FluxMonetaireDetailViewModel;
using SectionDetailEclipseDePrimeModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailEclipseDePrimeModel;
using SectionDetailParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailParticipationsModel;
using SectionChangementAffectationParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionChangementAffectationParticipationsModel;
using SectionScenarioParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionScenarioParticipationsModel;
using SectionFluxMonetaireModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionFluxMonetaireModel;
using SectionPrimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionPrimesModel;
using SectionSurprimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionSurprimesModel;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageSommaireProtectionsIllustrationMapper : ReportMapperBase<SectionSommaireProtectionsIllustrationModel, PageSommaireProtectionsIllustrationViewModel>, IPageSommaireProtectionsIllustrationMapper
    {

        public PageSommaireProtectionsIllustrationMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileWithAudienceBase
        {
            public ReportProfile()
                : this(new IllustrationReportDataFormatter(null, null, null, null, null, null, null, null, null, null, null, null, null, null), null, null, ReportAudienceTypes.FullClearance)
            {
            }

            public ReportProfile(
                IIllustrationReportDataFormatter formatter,
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory, 
                ReportAudienceTypes audience) : base(formatter, resourcesAccessor, managerFactory, audience)
            {
            }

            protected override void ConfigureMappingForPrivacyAudience(
                IIllustrationReportDataFormatter formatter, 
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory)
            {
                CreateMapping(formatter, managerFactory, resourcesAccessor, true);
            }

            protected override void ConfigureMapping(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory)
            {
                CreateMapping(formatter, managerFactory, resourcesAccessor);
            }

            private void CreateMapping(
                IIllustrationReportDataFormatter formatter,
                IManagerFactory managerFactory,
                IIllustrationResourcesAccessorFactory resourcesAccessor, 
                bool forPrivacy = false)
            {
                CreateMap<SectionSommaireProtectionsIllustrationModel, PageSommaireProtectionsIllustrationViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images)))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.SectionContractants, m => m.MapFrom(s => s.SectionContractantsModel))
                    .ForMember(d => d.SectionsAssures, m => m.MapFrom(s => s.SectionsAssuresModel))
                    .ForMember(d => d.SectionSurprimes, m => m.MapFrom(s => s.SectionSurprimesModel))
                    .ForMember(d => d.SectionConseiller, m => m.MapFrom(s => s.SectionConseillerModel))
                    .ForMember(d => d.SectionPrimes, m => m.MapFrom(s => s.SectionPrimesModel))
                    .ForMember(d => d.SectionPrimesVersees, m => m.MapFrom(s => s.SectionPrimesVerseesModel))
                    .ForMember(d => d.SectionASLModel, m => m.MapFrom(s => s.SectionASLModel))
                    .ForMember(d => d.SectionUsageAuConseiller, m => m.MapFrom(s => s.SectionUsageAuConseillerModel))
                    .ForMember(d => d.SectionFluxMonetaire, m => m.MapFrom(s => s.SectionFluxMonetaireModel))
                    .ForMember(d => d.SectionDetailParticipations, m => m.MapFrom(s => s.SectionDetailParticipationsModel))
                    .ForMember(d => d.SectionChangementAffectationParticipations, m => m.MapFrom(s => s.SectionChangementAffectationParticipationsModel))
                    .ForMember(d => d.SectionScenarioParticipations, m => m.MapFrom(s => s.SectionScenarioParticipationsModel))
                    .ForMember(d => d.SectionCaracteristiquesIllustration, m => m.MapFrom(s => s.SectionCaracteristiquesIllustrationModel))
                    .ForMember(d => d.SectionDetailEclipseDePrime, m => m.MapFrom(s => s.SectionDetailEclipseDePrimeModel));

                CreateMap<SectionAssuresModel, SectionAssuresViewModel>()
                    .ForMember(d => d.TypeAssurance, m => m.MapFrom(s => formatter.FormatTypeAssurance(s.TypeAssurance)))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => new AvisViewModel { Texte = s.Avis }))
                    .ForMember(d => d.Assures, m => m.MapFrom(s => s.Assures))
                    .ForMember(d => d.Protections, m => m.MapFrom(s => s.Protections))
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.DateEffectiveText, m => m.MapFrom(s => formatter.FormatDateEffectiveText(s.DateEffective)));

                CreateMap<Assure, InfoAssureViewModel>().
                    ForMember(d => d.Prenom, m => m.MapFrom(s => s.Prenom)).
                    ForMember(d => d.Nom, m => m.MapFrom(s => s.Nom)).
                    ForMember(d => d.DateNaissance, m => m.MapFrom(s => formatter.FormatLongDate(s.DateNaissance, forPrivacy, "NonFournie"))).
                    ForMember(d => d.AgeAssurance, m => m.MapFrom(s => s.Age.HasValue ? formatter.FormatAge(s.Age.Value) : string.Empty)).
                    ForMember(d => d.Sexe, m => m.MapFrom(s => formatter.FormatSexe(s.Sexe, TypeAffichageSexe.Sexe))).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId)).
                    ForMember(d => d.EstNonAssurableDetail, m => m.MapFrom(s => formatter.FormatNonAssurable(s.EstNonAssurable))).
                    ForMember(d => d.CategorieFumeur, m => m.MapFrom(s => s.Age < ConstanteFonctionnelle.AGE_ASSURANCE_STATUT_TABAGISME ? string.Empty : formatter.FormatStatutTabagisme(s.StatutFumeur)));

                CreateMap<SectionContractantsModel, SectionContractantsViewModel>()
                    .ForMember(d => d.NumeroContrat, m => m.MapFrom(s => s.NumeroContrat))
                    .ForMember(d => d.Province, m => m.MapFrom(s => s.Province))
                    .ForMember(d => d.TauxMarginalCorporation,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxMarginaux.TauxCorporation)))
                    .ForMember(d => d.TauxMarginalParticulier,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxMarginaux.TauxParticulier)))
                    .ForMember(d => d.DividendesCorporation,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxDividendes.TauxCorporation)))
                    .ForMember(d => d.DividendesParticulier,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxDividendes.TauxParticulier)))
                    .ForMember(d => d.GainCapitalCorporation,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxGainCapital.TauxCorporation)))
                    .ForMember(d => d.GainCapitalParticulier,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotCorporation.TauxGainCapital.TauxParticulier)))
                    .ForMember(d => d.ImpotMarginal,
                        m => m.MapFrom(s => formatter.FormatPercentage(s.ImpotParticulier)))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => new AvisViewModel { Texte = s.Avis }))
                    .ForMember(d => d.Contractants, m => m.MapFrom(s => s.Contractants))
                    .ForMember(d => d.Protections, m => m.MapFrom(s => s.Protections))
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.EstCompagnie, m => m.MapFrom(s => s.EstCompagnie))
                    .ForMember(d => d.DateEffectiveText, m => m.MapFrom(s => s.EstCompagnie ? string.Empty : formatter.FormatDateEffectiveText(s.DateEffective)));

                CreateMap<Contractant, InfoContractantViewModel>().
                    ForMember(d => d.Prenom, m => m.MapFrom(s => s.Prenom)).
                    ForMember(d => d.Nom, m => m.MapFrom(s => s.Nom)).
                    ForMember(d => d.DateNaissance, m => m.MapFrom(s => s.EstCompagnie ? string.Empty : formatter.FormatLongDate(s.DateNaissance, forPrivacy, "NonFournie"))).
                    ForMember(d => d.AgeAssurance, m => m.MapFrom(s => s.Age.HasValue ? formatter.FormatAge(s.Age.Value) : string.Empty)).
                    ForMember(d => d.Sexe, m => m.MapFrom(s => formatter.FormatSexe(s.Sexe, TypeAffichageSexe.Sexe))).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId)).
                    ForMember(d => d.CategorieFumeur, m => m.MapFrom(s => s.Age < ConstanteFonctionnelle.AGE_ASSURANCE_STATUT_TABAGISME ? string.Empty : FormatterCategorieFumeur(s, formatter, resourcesAccessor)));

                CreateMap<SectionProtectionsSommaireModel, ProtectionViewModel>().
                    ForMember(d => d.EstAccesVie, m => m.MapFrom(s => s.EstAccesVie)).
                    ForMember(d => d.Noms, m => m.MapFrom(s => s.NomComplet)).
                    ForMember(d => d.MontantPrimeTotal, m => m.MapFrom(s => formatter.FormatCurrency(s.MontantPrimeTotal))).
                    ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation)).
                    ForMember(d => d.Protections, m => m.MapFrom(s => s.Protections)).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes))).
                    ForMember(d => d.NomAffichage, m => m.MapFrom(s => FormatterNomAffichageSectionDetailProtection(s.Protections, resourcesAccessor)));

                CreateMap<DetailProtection, DetailProtectionViewModel>()
                    .ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId))
                    .ForMember(d => d.Noms, m => m.MapFrom(s => s.Noms))
                    .ForMember(d => d.MontantCapitalAssureActuel, m => m.MapFrom(s => s.MontantCapitalAssureActuel > 0 && s.AfficherCapitalAssure ? formatter.FormatCurrency(s.MontantCapitalAssureActuel) : string.Empty))
                    .ForMember(d => d.MontantPrimeMinimale, m => m.MapFrom(s => FormatterPrimeEtNote(formatter.FormatCurrency(s.MontantPrimeMinimale), s.ReferenceNotes)))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.SousDescription, m => m.MapFrom(s => FormatterSousDescription(s.TypeProtectionComplementaire, s.AgeMaturitePlan, s.TypePrestationPlan, formatter)))
                    .ForMember(d => d.DureePaiement, m => m.MapFrom(s => s.DureePaiement))
                    .ForMember(d => d.DureeProtection, m => m.MapFrom(s => FormatterDureeProtection(s, resourcesAccessor)))
                    .ForMember(d => d.DureeRenouvellement, m => m.MapFrom(s => FormatterDureeRenouvellement(s, resourcesAccessor)))
                    .ForMember(d => d.ReferenceNotes, m => m.MapFrom(s => s.ReferenceNotes))
                    .ForMember(d => d.Taux, m => m.MapFrom(s => FormatterTaux(s.Age, s.Sexe, s.StatutTabagisme, formatter, resourcesAccessor)))
                    .ForMember(d => d.EstProtectionConjointe, m => m.MapFrom(s => s.EstProtectionConjointe))
                    .ForMember(d => d.EstProtectionContractant, m => m.MapFrom(s => s.EstProtectionContractant))
                    .ForMember(d => d.EstProtectionBase, m => m.MapFrom(s => s.EstProtectionBase))
                    .ForMember(d => d.DescriptionAvecReferences, m => m.MapFrom(s => FormatterDescriptionAvecReferences(s, formatter)))
                    .ForMember(d => d.ContientSurprime, m => m.MapFrom(s => s.EstSurprimee ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Surprime") : ""))
                    .ForMember(d => d.InclusSurprime, m => m.MapFrom(s => s.EstSurprimee ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("SurprimeIncluse"), "") : ""))
                    .ForMember(d => d.TypeCout, m => m.MapFrom(s => s.TypeCout))
                    .ForMember(d => d.DescriptionAsl, m => m.MapFrom(s => s.CapitalAssureAsl.HasValue && s.CapitalAssureAsl > 0 ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DescriptionAsl"), "") : ""))
                    .ForMember(d => d.DescriptionOaca, m => m.MapFrom(s => s.CapitalAssureOaca.HasValue && s.CapitalAssureOaca > 0 ? string.Format(resourcesAccessor.GetResourcesAccessor().GetStringResourceById("DescriptionOaca"), "") : ""))
                    .ForMember(d => d.CapitalAssureAsl, m => m.MapFrom(s => formatter.FormatCurrency(s.CapitalAssureAsl)))
                    .ForMember(d => d.CapitalAssureOaca, m => m.MapFrom(s => formatter.FormatCurrency(s.CapitalAssureOaca)))
                    .ForMember(d => d.TypeAssurance, m => m.MapFrom(s => formatter.FormatterEnum<TypeAssurance>(s.TypeAssurance.ToString())))
                    .ForMember(d => d.Surprimes, m => m.MapFrom(s => s.Surprimes))
                    .ForMember(d => d.SurprimeTotal, m => m.MapFrom(s => formatter.FormatCurrency(s.SurprimeTotal)));

                CreateMap<SectionSurprimesModel, SectionSurprimesViewModel>()
                    .ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation))
                    .ForMember(d => d.MontantSurprimeTotal, m => m.MapFrom(s => formatter.FormatCurrency(s.MontantSurprimeTotal)))
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Protections, m => m.MapFrom(s => s.Protections));

                CreateMap<DetailSurprime, DetailSurprimeViewModel>()
                    .ForMember(d => d.DateLiberation, m => m.MapFrom(s => s.DateLiberation))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.EstTypeTemporaire, m => m.MapFrom(s => s.EstTypeTemporaire))
                    .ForMember(d => d.TauxMontant, m => m.MapFrom(s => s.TauxMontant.HasValue ? formatter.FormatCurrency(s.TauxMontant) : string.Empty))
                    .ForMember(d => d.TauxPourcentage, m => m.MapFrom(s => s.TauxPourcentage.HasValue ? formatter.FormatPercentage(s.TauxPourcentage) : string.Empty))
                    .ForMember(d => d.Terme, m => m.MapFrom(s => s.Terme));

                CreateMap<SectionConseillerModel, SectionConseillerViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Conseillers, m => m.MapFrom(s => s.Conseillers));

                CreateMap<SectionPrimesModel, SectionPrimesViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation))
                    .ForMember(d => d.Primes, m => m.MapFrom(s => s.Primes));

                CreateMap<Types.SectionModels.SommaireProtectionsIllustration.DetailPrime, PrimeViewModel>()
                    .ForMember(d => d.Description, m => m.ResolveUsing(s => s.FormatterDescription(formatter)))
                    .ForMember(d => d.MontantAvecTaxe, m => m.ResolveUsing(s => s.FormatterMontantAvecTaxe(formatter)))
                    .ForMember(d => d.SequenceId, m => m.Ignore())
                    .ForMember(d => d.Montant, m => m.MapFrom(s => formatter.FormatCurrency(s.Montant)));

                CreateMap<SectionPrimesVerseesModel, SectionPrimesVerseesViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.TitreColonnePrimesVersees, m => m.MapFrom(s => resourcesAccessor.GetResourcesAccessor().GetStringResourceById(s.TitreColonnePrimesVersees)))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.FrequenceFacturation, m => m.MapFrom(s => s.FrequenceFacturation))
                    .ForMember(d => d.PrimesVersees, m => m.MapFrom(s => s.PrimesVersees));

                CreateMap<DetailPrimesVersees, PrimesVerseesViewModel>()
                    .ForMember(d => d.Description, m => m.ResolveUsing(s => s.FormatterDescription(formatter, resourcesAccessor)))
                    .ForMember(d => d.Periode, m => m.MapFrom(s => s.FormatterPeriode(formatter)))
                    .ForMember(d => d.Montant, m => m.ResolveUsing(s => s.FormatterMontant(formatter, resourcesAccessor)))
                    .ForMember(d => d.SequenceId, m => m.Ignore());

                CreateMap<SectionUsageAuConseillerModel, SectionUsageAuConseillerViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.MontantNetAuRisqueViewModel, m => m.MapFrom(s => s.MontantNetAuRisque))
                    .ForMember(d => d.MaximumOdsPermisViewModel, m => m.MapFrom(s => s.MaximumOdsPermis));

                CreateMap<MontantNetAuRisqueModel, MontantNetAuRisqueViewModel>()
                    .ForMember(d => d.Montant, m => m.ResolveUsing(s => formatter.FormatCurrency(s.Montant)));

                CreateMap<MaximumOdsPermisModel, MaximumOdsPermisViewModel>()
                    .ForMember(d => d.Montant, m => m.ResolveUsing(s => formatter.FormatCurrency(s.Montant)));

                CreateMap<SectionCaracteristiquesIllustrationModel, SectionCaracteristiquesIllustrationViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Libelles, m => m.MapFrom(s => s.Libelles))
                    .ForMember(d => d.Valeurs, m => m.MapFrom(s => s.Valeurs));

                CreateMap<SectionFluxMonetaireModel, SectionFluxMonetaireViewModel>()
                    .ForMember(d => d.Details, m => m.MapFrom(s => s.Details))
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection));

                CreateMap<DetailFluxMonetaire, FluxMonetaireDetailViewModel>().
                    ForMember(d => d.TypeTransaction, m => m.MapFrom(s => s.TypeTransaction)).
                    ForMember(d => d.Description, m => m.MapFrom(s => s.FormatterDescription(formatter))).
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => s.FormatterPeriode(formatter))).
                    ForMember(d => d.Montant, m => m.MapFrom(s => s.FormatterMontant(formatter, resourcesAccessor)));

                CreateMap<SectionDetailParticipationsModel, SectionDetailParticipationsViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.OptionParticipation, m => m.MapFrom(s => FormatterOptionParticipation(s.OptionParticipation, formatter, resourcesAccessor)))
                    .ForMember(d => d.SoldeParticipationsEnDepot, m => m.MapFrom(s => FormatterSoldeParticipationsEnDepot(s.SoldeParticipationsEnDepot, formatter, resourcesAccessor)));

                CreateMap<SectionChangementAffectationParticipationsModel, SectionChangementAffectationParticipationsViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => FormatterDescriptionChangementOptionParticipation(s.Description, s.Annee)))
                    .ForMember(d => d.Annee, m => m.MapFrom(s => s.Annee));

                CreateMap<SectionScenarioParticipationsModel, SectionScenarioParticipationsViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.BaremesParticipations, m => m.MapFrom(s => FormatterBaremeParticipation(s.EcartBaremeParticipation, formatter, resourcesAccessor)));

                CreateMap<SectionDetailEclipseDePrimeModel, SectionDetailEclipseDePrimeViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.DescriptionAnneeActivation, m => m.MapFrom(s => s.FormatterDescriptionActivationEclipseDePrime(formatter, resourcesAccessor)))
                    .ForMember(d => d.Baremes, m => m.MapFrom(s => s.FormatterBaremes(formatter, resourcesAccessor)));

                CreateMap<SectionASLModel, ASLViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.OptionVersementBoni, m => m.MapFrom(s => formatter.FormatterEnum<TypeOptionVersementBoni>(s.OptionVersementBoni.ToString()))).
                    ForMember(d => d.CapitalAssuraMaximal, m => m.MapFrom(s => FormatterCapitalAssure(s.AucunAchat, s.AucunMaximum, s.CapitalAssureMaximal, formatter, resourcesAccessor))).
                    ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<DetailTaux, TauxASLViewModel>().
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, null))).
                    ForMember(d => d.Taux, m => m.MapFrom(s => formatter.FormatPercentageWithoutSymbol(s.Taux)));

                CreateMap<DetailAllocationASL, AllocationASLViewModel>().
                    ForMember(d => d.AnneeDebut, m => m.MapFrom(s => s.AnneeDebut)).
                    ForMember(d => d.Periode, m => m.MapFrom(s => formatter.FormatterPeriodeAnneeMois(s.AnneeDebut, null))).
                    ForMember(d => d.Montant, m => m.MapFrom(s => formatter.FormatDecimal(s.Montant)));
            }

            private static string FormatterOptionParticipation(TypeOptionParticipation optionParticipation,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                var label =
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("OptionVersementParticipations")}{formatter.AddColon()}";
                var value = formatter.FormatterEnum<TypeOptionParticipation>(optionParticipation.ToString());
                return $"{label} {value}";
            }
            
            private static string FormatterSoldeParticipationsEnDepot(double? soldeParticipationsEnDepot,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                if (!soldeParticipationsEnDepot.HasValue || soldeParticipationsEnDepot.GetValueOrDefault() == 0) 
                {
                    return string.Empty;
                }

                var label = $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("InfoSoldeParticipationsEnDepot")}";
                var value = formatter.FormatCurrency(Math.Abs(soldeParticipationsEnDepot.GetValueOrDefault()));
                return string.Format(label, value);
            }

            private static string FormatterBaremeParticipation(double? ecartBaremeParticipation,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                if (!ecartBaremeParticipation.HasValue)
                {
                    return string.Empty;
                }

                var label =
                    $"{resourcesAccessor.GetResourcesAccessor().GetStringResourceById("BaremeAlternatif")}{formatter.AddColon()}";
                var value = string.Format(
                    resourcesAccessor.GetResourcesAccessor().GetStringResourceById("BaremeParticipationCourantMoins"),
                    formatter.FormatPercentage(Math.Abs(ecartBaremeParticipation.GetValueOrDefault())));
                return $"{label} {value}";
            }

            private static string FormatterCategorieFumeur(Contractant contractant,
                IIllustrationReportDataFormatter formatter, IResourcesAccessorFactory resourcesAccessor)
            {
                if (contractant.EstCompagnie)
                {
                    return string.Empty;
                }

                return contractant.StatutFumeur != StatutTabagisme.NonDefini
                    ? formatter.FormatStatutTabagisme(contractant.StatutFumeur)
                    : resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonFournie");
            }

            private static string FormatterPrimeEtNote(string prime, IList<int> notes)
            {
                var chaine = string.Empty;

                if (!string.IsNullOrEmpty(prime))
                {
                    chaine = $"{prime}";
                }

                if (notes != null && notes.Any())
                {
                    chaine = $"{chaine} ({string.Join(", ", notes)})";
                }

                return chaine;
            }

            private static string FormatterDureeProtection(DetailProtection detailProtection,
                IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                if (detailProtection.DateMaturite.HasValue)
                {
                    var lifeTimeReference = new DateTime(1001, 01, 01);
                    result.Append(detailProtection.DateMaturite.Value.CompareTo(lifeTimeReference) == 0
                        ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AVie")
                        : detailProtection.DureeProtection.ToString());
                }

                return result.ToString();
            }
            
            private static string FormatterDureeRenouvellement(DetailProtection detailProtection,
                IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var result = new StringBuilder();

                if (detailProtection.DureeRenouvellement.HasValue)
                {
                    if (detailProtection.DateMaturite.HasValue)
                    {
                        var lifeTimeReference = new DateTime(1001, 01, 01);
                        result.Append(detailProtection.DateMaturite.Value.CompareTo(lifeTimeReference) == 0 && detailProtection.DureeRenouvellement == detailProtection.DureeProtection
                            ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AVie")
                            : detailProtection.DureeRenouvellement.ToString());
                    }
                    else
                    {
                        result.Append(detailProtection.DureeRenouvellement.ToString());
                    }
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

            private static string FormatterDescriptionAvecReferences(DetailProtection detailProtection, IIllustrationReportDataFormatter formatter)
            {
                var typeCout = formatter.FormatterEnum<TypeCout>(detailProtection.TypeCout.ToString());
                var references = FormatterReferenceNotes(detailProtection.ReferenceNotes);
                if (string.IsNullOrEmpty(typeCout) && string.IsNullOrEmpty(references)) return detailProtection.Description;
                if (string.IsNullOrEmpty(typeCout)) return string.Join(" ", detailProtection.Description, references);
                var description = string.Join(", ", detailProtection.Description, typeCout);
                return string.Join(" ", description, references);
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

            private static string FormatterNomAffichageSectionDetailProtection(IList<DetailProtection> protections, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                var protection = protections.FirstOrDefault();

                if (protection == null)
                    return string.Empty;

                return protection.EstProtectionContractant
                           ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById(protection.Noms.Count == 1 ? "ProtectionsContractant" : "ProtectionsContractants")
                           : resourcesAccessor.GetResourcesAccessor().GetStringResourceById(protection.TypeAssurance == TypeAssurance.Individuelle ? "ProtectionsIndividuel" : "ProtectionsConjointe");
            }

            private static string FormatterDescriptionChangementOptionParticipation(string description, int annee)
            {
                return string.Format(description, annee.ToString());
            }

            private static string FormatterCapitalAssure(bool aucunAchat, bool aucunMaximum, double capitalAssureMaximal, IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor)
            {
                if (aucunAchat) return resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunAchatASL");
                return aucunMaximum ? resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AucunMaximum") : formatter.FormatCurrency(capitalAssureMaximal);
            }
        }
    }
}