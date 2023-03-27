using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.PrimesRenouvellement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class PrimesRenouvellementModelFactory: IPrimesRenouvellementModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly ISectionModelMapper _sectionModelMapper;

        public PrimesRenouvellementModelFactory(IConfigurationRepository configurationRepository, 
            IIllustrationReportDataFormatter formatter, ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _sectionModelMapper = sectionModelMapper;
        }

        public PagePrimesRenouvellementModel Build(string sectionId, DonneesRapportIllustration donnees,
            IReportContext context)
        {
            var definitionSection =
                _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new PagePrimesRenouvellementModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SectionPrimesRenouvellementModels = CreerSectionsPrimesRenouvellement(donnees.ProtectionsGroupees,
                context.Language, donnees.Facturation.FrequenceFacturation);
            return model;
        }

        private List<SectionPrimeRenouvellementModel> CreerSectionsPrimesRenouvellement(IList<Types.Models.SommaireProtections.ProtectionsGroupees> protectionsGroupees, 
            Language language, TypeFrequenceFacturation frequenceFacturation)
        {
            var primesRenouvellement = new List<SectionPrimeRenouvellementModel>();
            if (!protectionsGroupees.Any(g => g.PrimesRenouvellement?.Protections?.Any() ?? false)) return primesRenouvellement;

            foreach (var groupe in protectionsGroupees.Where(g => g.PrimesRenouvellement != null))
            {
                var primeRenouvellementModel = new SectionPrimeRenouvellementModel()
                    {
                        DetailsPrimeRenouvellement = new List<DetailsPrimeRenouvellementModel>()
                    };
                foreach (var protection in groupe.PrimesRenouvellement.Protections)
                {

                    var prime = new DetailsPrimeRenouvellementModel()
                                {
                                    Id = protection.Id,
                                    CodePlan = protection.Plan.CodePlan,
                                    Description = language == Language.French ? protection.Plan.DescriptionFr : protection.Plan.DescriptionAn,
                                    CapitalAssure = protection.CapitalAssure,
                                    Periodes = CreerPeriodes(protection),
                                    EstProtectionContractant = protection.EstProtectionContractant,
                                    EstProtectionConjointe = protection.EstProtectionConjointe,
                                    EstProtectionBase = protection.EstProtectionBase,
                                    FrequenceFacturation = frequenceFacturation,
                                    Assures = protection.Assures.Select(a => _formatter.FormatFullName(a.Prenom, a.Nom, a.Intitial)).ToList()
                                };

                    primeRenouvellementModel.DetailsPrimeRenouvellement.Add(prime);
                }
                primesRenouvellement.Add(primeRenouvellementModel);
            }

            return primesRenouvellement;
        }

        private static List<PeriodePrimeModel> CreerPeriodes(Types.Models.PrimesRenouvellement.Protection protection)
        {
            var periodes = new List<PeriodePrimeModel>();
            if (protection?.Primes == null) return periodes;

            foreach (var prime in protection.Primes.OrderBy(p => p.Annee))
            {
                var periode = periodes.LastOrDefault();
                if (periode == null || Math.Abs(periode.PrimeGarantie - prime.MontantGaranti) > 0.009)
                {
                    periode = new PeriodePrimeModel
                              {
                                  AnneeDebut = prime.Annee,
                                  PrimeGarantie = prime.MontantGaranti
                              };

                    periodes.Add(periode);
                }
                else
                {
                    periode.AnneeFin = prime.Annee;
                }
            }

            return periodes;
        }
    }
}