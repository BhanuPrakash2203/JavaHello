using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ConceptVentes;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConceptVente;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VI.Projection.Data.Enums.Concept;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class ConceptVenteModelFactory: IConceptVenteModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IDefinitionNoteManager _definitionNoteManager;
        private readonly IIllustrationReportDataFormatter _dataFormatter;

        public ConceptVenteModelFactory(
            IConfigurationRepository configurationRepository, 
            ISectionModelMapper sectionModelMapper,
            IDefinitionNoteManager definitionNoteManager,
            IIllustrationReportDataFormatter dataFormatter)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
            _definitionNoteManager = definitionNoteManager;
            _dataFormatter = dataFormatter;
        }

        public SectionConceptVenteModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionConceptVenteModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);

            if (donnees.ConceptVente?.PretEnCollateral != null)
            {
                model.SectionPretCollateral = MapperPretCollateral(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "PretCollateral"), donnees, context);
                model.SectionPretCollateralPaiementInteret = MapperPretCollateralPaiementInterets(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "PretCollateral-PaiementInterets"), donnees, context);
                model.SectionPretCollateralRemboursement = MapperPretCollateralRemboursement(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "PretCollateral-Remboursement"), donnees, context);
            }

            if (donnees.ConceptVente?.AvancePret != null)
            {
                model.SectionAvancePret = MapperAvancePret(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "AvancePret"), donnees, context);
                model.SectionAvancePretRemboursement = MapperAvancePretRemboursement(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "AvancePret-Remboursement"), donnees, context);
            }

            return model;
        }

        private SectionAvancePretModel MapperAvancePret(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null || donnees.ConceptVente?.AvancePret == null) return null;
            var model = new SectionAvancePretModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);
            MapperAvancePret(model, donnees, donnees.ConceptVente.AvancePret, definition.Libelles, context);
            return model;
        }

        private void MapperAvancePret(SectionAvancePretModel model, DonneesRapportIllustration donnees, AvancePret avancePret, Dictionary<string, DefinitionLibelle> libelles, IReportContext context)
        {
            if (avancePret == null) return;
            var vehicule = !string.IsNullOrEmpty(avancePret.Compte) ? donnees.Vehicules.FirstOrDefault(x => x.Vehicle == avancePret.Compte) : null;
            model.CompteEnCollateral = (context.Language == Language.French ? vehicule?.DescriptionFr : vehicule?.DescriptionEn) ?? string.Empty;
            model.PourcentageInteretsDeductibles = avancePret.Data?.Taxation?.InterestDeductiblePercentage;

            var provenanceFondsMaintienEnVigueur = avancePret.Data?.MaintainInforceType ?? MaintainInforceType.None;
            if (provenanceFondsMaintienEnVigueur != MaintainInforceType.None)
            {
                model.ProvenanceFondsMaintienEnVigueur = _dataFormatter.FormatterLibellees(libelles, $"{nameof(MaintainInforceType)}.{provenanceFondsMaintienEnVigueur}", context);
            }
        }

        private SectionRemboursementModel MapperAvancePretRemboursement(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var avance = donnees.ConceptVente?.AvancePret;
            if (avance?.Remboursements == null) return null;

            var model = new SectionRemboursementModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            var provenanceFonds = avance.Remboursements.FirstOrDefault()?.ProvenanceFonds;
            if (!string.IsNullOrEmpty(provenanceFonds))
            {
                model.ProvenanceFonds = _dataFormatter.FormatterLibellees(definition.Libelles,$"ProvenanceFonds.{provenanceFonds}", context);
            }

            model.LibellePersonnalise = _dataFormatter.FormatterLibellees(definition.Libelles,"MontantPersonnalise", context);
            model.LibelleMontantMaximal = _dataFormatter.FormatterLibellees(definition.Libelles,"MontantMaximal", context);

            var remboursements = new List<Remboursement>();
            Remboursement remboursement = null;
            foreach (var item in avance.Remboursements.OrderBy(x => x.Annee))
            {
                if (remboursement == null || remboursement.TypeMontant != item.TypeMontant || Math.Abs((remboursement.Montant ?? 0) - (item.Montant ?? 0)) > 0.009)
                {
                    remboursement = new Remboursement
                    {
                        AnneeDebut = item.Annee,
                        Montant = item.Montant,
                        TypeMontant = item.TypeMontant,
                        EstMontantMaximal = item.EstMontantMaximal
                    };
                    remboursements.Add(remboursement);
                }
                else
                {
                    remboursement.AnneeFin = item.Annee;
                }
            }

            model.Remboursements = remboursements;
            return model;
        }

        private SectionPretCollateralModel MapperPretCollateral(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null || donnees.ConceptVente?.PretEnCollateral == null) return null;
            var model = new SectionPretCollateralModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);
            MapperPretCollateral(model, donnees, donnees.ConceptVente.PretEnCollateral, definition.Libelles, context);
            return model;
        }

        private void MapperPretCollateral(SectionPretCollateralModel model, DonneesRapportIllustration donnees, 
            PretEnCollateral pret, Dictionary<string, DefinitionLibelle> libelles, IReportContext context)
        {
            if (pret == null) return;
            var vehicule = !string.IsNullOrEmpty(pret.Compte) ? donnees.Vehicules.FirstOrDefault(x => x.Vehicle == pret.Compte) : null;
            model.CompteEnCollateral = (context.Language == Language.French ? vehicule?.DescriptionFr : vehicule?.DescriptionEn) ?? string.Empty;
            model.TermeEntente = context.Language == Language.French ? "Taux 1 an renouvelable" : "1-year renewable rate";

            var borrowerType = pret.Data?.Taxation?.Borrower?.BorrowerType ?? BorrowerType.None;
            if (borrowerType != BorrowerType.None && donnees.ContractantEstCompagnie)
            {
                model.TitulaireEntente = _dataFormatter.FormatterLibellees(libelles,$"{nameof(BorrowerType)}.{borrowerType}", context);
            }

            var provenanceFondsMaintienEnVigueur = pret.Data?.MaintainInforceType ?? MaintainInforceType.None;
            if (provenanceFondsMaintienEnVigueur != MaintainInforceType.None)
            {
                model.ProvenanceFondsMaintienEnVigueur = _dataFormatter.FormatterLibellees(libelles,$"{nameof(MaintainInforceType)}.{provenanceFondsMaintienEnVigueur}", context);
            }

            model.FraisDeGarantie = pret.Data?.Taxation?.Borrower?.GuarantorFee;
            model.DeductionCoutNetAssurancePure = pret.Data?.Taxation?.Borrower?.NetCostPureInsuranceDeduction ?? false;
            model.PourcentageInteretsDeductibles = pret.Data?.Taxation?.InterestDeductiblePercentage;
            model.Caution = pret.Data?.Taxation?.Borrower != null && pret.Data.Taxation.Borrower.BorrowerType == BorrowerType.ThirdParty
                                ? donnees.Clients.FirstOrDefault(x => x.EstContractant)?.Nom
                                : string.Empty;
        }

        private SectionPaiementInteretModel MapperPretCollateralPaiementInterets(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var model = new SectionPaiementInteretModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            var pret = donnees.ConceptVente?.PretEnCollateral;
            if (pret == null) return model;
            model.PourcentageMontantMaximal = pret.Data?.AdditionalLoansEndYear?.FirstOrDefault(x => x.Value > 0.000001).Value;
            model.EstPersonnalise = pret.Data?.AdditionalLoansEndYear?.Where(x => x.Value > 0.000001).Select(x => x.Value).Distinct().Count() > 1;
            model.SoldePourFigerPret = pret.Data?.BorrowingLimits?.MaximumBalance;
            return model;
        }

        private SectionRemboursementModel MapperPretCollateralRemboursement(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var pret = donnees.ConceptVente?.PretEnCollateral;
            if (pret?.Remboursements == null) return null;

            var model = new SectionRemboursementModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            var provenanceFonds = pret.Remboursements.FirstOrDefault()?.ProvenanceFonds;
            if (!string.IsNullOrEmpty(provenanceFonds))
            {
                model.ProvenanceFonds = _dataFormatter.FormatterLibellees(definition.Libelles,$"ProvenanceFonds.{provenanceFonds}", context);
            }

            model.LibellePersonnalise = _dataFormatter.FormatterLibellees(definition.Libelles,"MontantPersonnalise", context);
            model.LibelleMontantMaximal = _dataFormatter.FormatterLibellees(definition.Libelles,"MontantMaximal", context);

            var remboursements = new List<Remboursement>();
            Remboursement remboursement = null;
            foreach (var item in pret.Remboursements.OrderBy(x => x.Annee))
            {
                if (remboursement == null || remboursement.TypeMontant != item.TypeMontant || Math.Abs((remboursement.Montant ?? 0) - (item.Montant ?? 0)) > 0.009)
                {
                    remboursement = new Remboursement
                                    {
                                        AnneeDebut = item.Annee,
                                        Montant = item.Montant,
                                        TypeMontant = item.TypeMontant,
                                        EstMontantMaximal = item.EstMontantMaximal
                                    };
                    remboursements.Add(remboursement);
                }
                else
                {
                    remboursement.AnneeFin = item.Annee;
                }
            }

            if (remboursements.Count > 3)
            {
                var note = definition.Notes.FirstOrDefault(x => x.Id == "Personnalisation");
                if (note != null)
                {
                    _definitionNoteManager.CreerNote(model.Notes, note, donnees);
                }

                model.EstPersonnalise = true;
            }

            model.Remboursements = remboursements;
            return model;
        }
    }
}