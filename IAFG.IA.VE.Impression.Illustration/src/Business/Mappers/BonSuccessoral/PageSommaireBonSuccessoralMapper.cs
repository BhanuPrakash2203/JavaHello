using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral.Sommaire;
using System;
using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.BonSuccessoral
{
    public class PageSommaireBonSuccessoralMapper : ReportMapperBase<SommaireBonSuccessoralModel, PageSommaireBonSuccessoralViewModel>, IPageSommaireBonSuccessoralMapper
    {
        public PageSommaireBonSuccessoralMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SommaireBonSuccessoralModel, PageSommaireBonSuccessoralViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
                    .ForMember(d => d.Libelles, m => m.MapFrom(s => s.Libelles))
                    .ForMember(d => d.SectionContrat, m => m.MapFrom(s => s.Contrat))
                    .ForMember(d => d.SectionHypothesesInvestissement, m => m.MapFrom(s => s.HypothesesInvestissement))
                    .ForMember(d => d.SectionImposition, m => m.MapFrom(s => s.Imposition))
                    .ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images)))
                    .ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis))
                    .ForMember(d => d.Notes, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));

                CreateMap<ContratModel, SectionContratViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Libelles, m => m.MapFrom(s => s.Libelles))
                    .ForMember(d => d.Valeurs, m => m.MapFrom(s => MapContratValeurs(s, formatter)));

                CreateMap<HypothesesInvestissementModel, SectionHypothesesInvestissementViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.RepartitionInterets, m => m.MapFrom(s => formatter.FormatPercentage(s.Interets.Repartition)))
                    .ForMember(d => d.RepartitionInteretsValeur, m => m.MapFrom(s => s.Interets.Repartition))
                    .ForMember(d => d.RepartitionDividendes, m => m.MapFrom(s => formatter.FormatPercentage(s.Dividendes.Repartition)))
                    .ForMember(d => d.RepartitionDividendesValeur, m => m.MapFrom(s => s.Dividendes.Repartition))
                    .ForMember(d => d.RepartitionGainCapital, m => m.MapFrom(s => formatter.FormatPercentage(s.GainCapital.Repartition)))
                    .ForMember(d => d.RepartitionGainCapitalValeur, m => m.MapFrom(s => s.GainCapital.Repartition))
                    .ForMember(d => d.TauxRendementInterets, m => m.MapFrom(s => formatter.FormatPercentage(s.Interets.TauxRendement)))
                    .ForMember(d => d.TauxRendementDividendes, m => m.MapFrom(s => formatter.FormatPercentage(s.Dividendes.TauxRendement)))
                    .ForMember(d => d.TauxRendementGainCapital, m => m.MapFrom(s => formatter.FormatPercentage(s.GainCapital.TauxRendement)))
                    .ForMember(d => d.TauxRealisationGainCapital, m => m.MapFrom(s => formatter.FormatPercentage(s.GainCapital.TauxRealisation)))
                   .ForMember(d => d.Libelles, m => m.MapFrom(s => s.Libelles));

                CreateMap<ImpositionModel, SectionImpositionViewModel>()
                    .ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection))
                    .ForMember(d => d.Libelles, m => m.MapFrom(s => s.Libelles))
                    .ForMember(d => d.Valeurs, m => m.MapFrom(s => MapImpositionValeurs(s, formatter)));
            }

            private List<KeyValuePair<string, string>> MapImpositionValeurs(ImpositionModel model, IIllustrationReportDataFormatter formatter)
            {
                var result = new List<KeyValuePair<string, string>>();
                const string tauxImpositionPrefix = "TauxImposition.";

                if (model.EstCorporation && model.TauxMarginal.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxMarginal) + "Entreprise");
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxMarginal)  + "Entreprise", formatter.FormatPercentage(model.TauxMarginal)));
                }
                else if (model.TauxMarginal.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxMarginal));
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxMarginal), formatter.FormatPercentage(model.TauxMarginal)));
                }

                if (model.EstCorporation && model.TauxDividendes.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxDividendes) + "Entreprise");
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxDividendes) + "Entreprise", formatter.FormatPercentage(model.TauxDividendes)));
                }
                else if (model.TauxDividendes.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxDividendes));
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxDividendes), formatter.FormatPercentage(model.TauxDividendes)));
                }
                
                if (model.TauxDividendesActionnaires.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxDividendesActionnaires));
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxDividendesActionnaires), formatter.FormatPercentage(model.TauxDividendesActionnaires)));
                }
                
                if (model.EstCorporation && model.TauxGainCapital.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxGainCapital) + "Entreprise");
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxGainCapital) + "Entreprise", formatter.FormatPercentage(model.TauxGainCapital)));
                }
                else if (model.TauxGainCapital.HasValue)
                {
                    ValiderLibelle(model.Libelles, tauxImpositionPrefix + nameof(model.TauxGainCapital));
                    result.Add(new KeyValuePair<string, string>(tauxImpositionPrefix + nameof(model.TauxGainCapital), formatter.FormatPercentage(model.TauxGainCapital)));
                }

                return result;
            }

            private List<KeyValuePair<string, string>> MapContratValeurs(ContratModel model, IIllustrationReportDataFormatter formatter)
            {
                var result = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(nameof(model.DescriptionProtection), model.DescriptionProtection),
                    new KeyValuePair<string, string>(nameof(model.MontantProtectionInitial), formatter.FormatCurrencyWithoutDecimal(model.MontantProtectionInitial)),
                };

                ValiderLibelle(model.Libelles, nameof(model.DescriptionProtection));
                ValiderLibelle(model.Libelles, nameof(model.MontantProtectionInitial));

                if (model.TauxInvestissement.HasValue)
                {
                    ValiderLibelle(model.Libelles, nameof(model.TauxInvestissement));
                    result.Add(new KeyValuePair<string, string>(nameof(model.TauxInvestissement), formatter.FormatPercentage(model.TauxInvestissement)));
                }

                if (!string.IsNullOrWhiteSpace(model.BaremeParticipation))
                {
                    ValiderLibelle(model.Libelles, nameof(model.BaremeParticipation));
                    result.Add(new KeyValuePair<string, string>(nameof(model.BaremeParticipation), model.BaremeParticipation));
                }

                return result;
            }

            private static void ValiderLibelle(Dictionary<string, string> libelles, string name)
            {
                if (libelles == null || !libelles.ContainsKey(name))
                {
                    throw new ArgumentNullException(name, @"Aucun libellé trouvé");
                }
            }          
        }
    }
}
