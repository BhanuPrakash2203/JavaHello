using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class HypothesesInvestissementModelFactory : IHypothesesInvestissementModelFactory
    {
        private readonly IRegleAffaireAccessor _regleAffaireAccessor;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IDefinitionTexteManager _texteManager;

        public HypothesesInvestissementModelFactory(IRegleAffaireAccessor regleAffaireAccessor,
                                                    IConfigurationRepository configurationRepository,
                                                    IIllustrationReportDataFormatter formatter, 
                                                    ISectionModelMapper sectionModelMapper,
                                                    IDefinitionTexteManager texteManager)
        {
            _regleAffaireAccessor = regleAffaireAccessor;
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _sectionModelMapper = sectionModelMapper;
            _texteManager = texteManager;
        }

        public SectionHypothesesInvestissementModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionHypothesesInvestissementModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SectionFondsCapitalisation = CreerSectionFondsCapitalisation(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "FondsCapitalisation"), donnees, context);
            model.SectionAjustementValeurMarchande = CreerSectionAjustementValeurMarchande(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "AjustementValeurMarchande"), donnees, context);
            model.SectionFondsTransitoire = CreerSectionFondsTransition(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "FondsTransitoire"), donnees, context);
            model.SectionPrets = CreerSectionPrets(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Prets"), donnees, context);
            return model;
        }

        private SectionFondsCapitalisationModel CreerSectionFondsCapitalisation(
            DefinitionSection definition, 
            DonneesRapportIllustration donnees, 
            IReportContext context)
        {
            var model = new SectionFondsCapitalisationModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            var boni = donnees.Boni;
            model.BoniFidelite = boni?.BoniFidelite ?? BoniFidelite.NonApplicable;
            model.ChoixBoniInteret = boni?.ChoixBoniInteret ?? ChoixBoniInteret.Aucun;
            model.BoniInteret = boni?.BoniInteret ?? BoniInteret.Regle0_AucunBoni;
            model.DebutBoniInteret = boni?.DebutBoniInteret ?? 0;
            model.TauxBoni = boni?.TauxBoni;
            model.DebutBoniFidelite = boni?.DebutBoniFidelite ?? 0;

            var fondsCapitalisation = donnees.HypothesesInvestissement.FondsCapitalisation;
            model.RendementMoyenCompte = fondsCapitalisation.RendementMoyenCompte;
            model.Fonds = MapperComptesFonds(
                context.Language == Language.French ? "Fonds de capitalisation" : "Accumulation Fund",
                fondsCapitalisation.Fonds,
                fondsCapitalisation.Solde,
                donnees.DateDerniereMiseAJourInterets,
                donnees,
                context.Language);

            foreach (var item in model.Fonds)
            {
                item.RendementMoyen = model.RendementMoyenCompte;
            }

            return model;
        }

        private SectionAjustementValeurMarchandeModel CreerSectionAjustementValeurMarchande(
            DefinitionSection definition, 
            DonneesRapportIllustration donnees, 
            IReportContext context)
        {
            var model = new SectionAjustementValeurMarchandeModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);
            
            model.Textes = _texteManager.CreerDetailTextes(definition.Textes, donnees);
            model.Comptes = MapperTauxAVMComptes(donnees, donnees.HypothesesInvestissement?.FondsCapitalisation?.TauxAVMComptes, context.Language);

            return model;
        }

        // ReSharper disable once InconsistentNaming
        private IList<DetailTauxAVMCompte> MapperTauxAVMComptes(
            DonneesRapportIllustration donnees, 
            IList<Taux> taux, 
            Language langue)
        {
            if (taux == null || !taux.Any())
                return null;
            
            var result = new List<DetailTauxAVMCompte>();

            foreach (var t in taux)
            {
                var vehicule = donnees.Vehicules.FirstOrDefault(x => x.Vehicle == t.Vehicule);
                result.Add(new DetailTauxAVMCompte()
                {
                    Vehicule = t.Vehicule,
                    Description = langue == Language.French ? vehicule?.DescriptionFr : vehicule?.DescriptionEn,
                    Taux = t.ValeurTaux,
                    AnneeDebut = t.Annee,
                    MoisDebut = t.Mois
                });
            }

            return result;
        }


        private SectionFondsTransitoireModel CreerSectionFondsTransition(
            DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            var model = new SectionFondsTransitoireModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            model.Fonds = MapperComptesFonds(
                context.Language == Language.French ? "Fonds transitoire" : "Shuttle Fund",
                donnees.HypothesesInvestissement.FondsTransitoire.Fonds,
                donnees.HypothesesInvestissement.FondsTransitoire.Solde,
                donnees.DateDerniereMiseAJourInterets,
                donnees,
                context.Language);

            return model;
        }

        private SectionPretsModel CreerSectionPrets(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            var model = new SectionPretsModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            var prets = new List<DetailPret>();
            if (donnees?.HypothesesInvestissement?.Prets != null)
                prets.AddRange(donnees.HypothesesInvestissement.Prets.Where(p => p.Solde > 0).Select(pret => new DetailPret
                {
                    Pret = pret.TypePret,
                    Solde = pret.Solde
                }));

            model.Prets = prets;
            return model;
        }

        private IList<DetailCompte> MapperComptesFonds(
            string description, 
            IList<Fonds> fonds, 
            double? solde, 
            DateTime? dateDerniereMiseAJour,
            DonneesRapportIllustration donnees,
            Language langue)
        {
            var detailComptes = new List<DetailCompte>();
            foreach (var itemFonds in fonds)
            {
                var vehicule = donnees.Vehicules.FirstOrDefault(x => x.Vehicle == itemFonds.Vehicule);
                var estComptePortefeuille = _regleAffaireAccessor.EstComptePorteuille(itemFonds.Vehicule);
                var estCompteTauxGarantie = _regleAffaireAccessor.EstCompteInteretGarantie(itemFonds.Vehicule);
                var detailCompte = new DetailCompte
                {
                    Vehicule = itemFonds.Vehicule,
                    Description = langue == Language.French ? vehicule.DescriptionFr : vehicule.DescriptionEn,
                    Solde = itemFonds.Solde,
                    RepartitionInvestissement = itemFonds.RepartitionInvestissement,
                    RepartitionDeduction = itemFonds.RepartitionDeduction,
                    Taux = itemFonds.Taux,
                    AnneeDebut = itemFonds.AnneeDebut,
                    MoisDebut = itemFonds.MoisDebut > 1 ? itemFonds.MoisDebut : null,
                    OrdreTri = estCompteTauxGarantie ? 0 : estComptePortefeuille ? 2 : 1
                };

                detailComptes.Add(detailCompte);
            }

            detailComptes.AddRange(MapperSoldeFonds(description, solde, detailComptes, dateDerniereMiseAJour, langue));
            return detailComptes;
        }
            
        private IEnumerable<DetailCompte> MapperSoldeFonds(
            string description, 
            double? solde, 
            IEnumerable<DetailCompte> comptes, 
            DateTime? dateDerniereMiseAJour,
            Language language)
        {
            var details = new List<DetailCompte>();
            if (comptes.Any())
            {
                var dateSoldeTotal = _formatter.FormatDate(dateDerniereMiseAJour);
                details.Add(new DetailCompte
                {
                    Description = language == Language.French ? $"Solde total au {dateSoldeTotal}" : $"Total Balance as at {dateSoldeTotal}",
                    Solde = solde,
                    EstSoldeTotal = true,
                    OrdreTri = 99
                });
            }
            else
            {
                details.Add(new DetailCompte
                {
                    Description = description,
                    Solde = solde,
                    EstSoldeTotal = true,
                    OrdreTri = 99
                });
            }

            return details;
        }
    }
}