using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ConceptVente;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class PageConceptVenteMapper : ReportMapperBase<SectionConceptVenteModel, PageSommaireViewModel>, IPageConceptVenteMapper
    {
        public PageConceptVenteMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
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
                CreateMap<SectionConceptVenteModel, PageSommaireViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Sections, m => m.ResolveUsing(s => MapperConceptVente(s, formatter, managerFactory.GetModelMapper()))).
                    ForMember(d => d.Images, m => m.MapFrom(s => managerFactory.GetModelMapper().MapperImages(s.Images))).
                    ForMember(d => d.Avis, m => m.MapFrom(s => s.Avis)).
                    ForMember(d => d.Notes, m => m.ResolveUsing(s => managerFactory.GetModelMapper().MapperNotes(s.Notes)));               
            }

            private static IList<SectionSommaireViewModel> MapperConceptVente(SectionConceptVenteModel sectionConceptVente, IIllustrationReportDataFormatter formatter, IModelMapper modelMapper)
            {
                var result = new List<SectionSommaireViewModel>
                             {
                                 MapPretCollateral(sectionConceptVente.SectionPretCollateral, formatter, modelMapper),
                                 MapPaiementInteret(sectionConceptVente.SectionPretCollateralPaiementInteret, formatter, modelMapper),
                                 MapRemboursement(sectionConceptVente.SectionPretCollateralRemboursement, formatter, modelMapper, true),
                                 MapAvancePret(sectionConceptVente.SectionAvancePret, formatter, modelMapper),
                                 MapRemboursement(sectionConceptVente.SectionAvancePretRemboursement, formatter, modelMapper, false)
                             };

                return result.Where(x => x != null).ToList();
            }

            private static SectionSommaireViewModel MapAvancePret(SectionAvancePretModel section, IIllustrationReportDataFormatter formatter, IModelMapper modelMapper)
            {
                if (section == null) return null;
                var valeurs = new List<KeyValuePair<string, string[][]>>
                              {
                                  new KeyValuePair<string, string[][]>(nameof(section.CompteEnCollateral), new[] {new[] {section.CompteEnCollateral}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.ProvenanceFondsMaintienEnVigueur), new[] {new[] {section.ProvenanceFondsMaintienEnVigueur}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.PourcentageInteretsDeductibles), new[] {new[] {formatter.FormatPercentage(section.PourcentageInteretsDeductibles)}})
                              };

                var result = new SectionSommaireViewModel
                {
                    TitreSection = section.TitreSection,
                    Description = section.Description,
                    Valeurs = valeurs,
                    Libelles = section.Libelles ?? new Dictionary<string, string>(),
                    Avis = section.Avis,
                    Notes = modelMapper.MapperNotes(section.Notes)
                };
                
                return result;
            }

            private static SectionSommaireViewModel MapPretCollateral(SectionPretCollateralModel section, IIllustrationReportDataFormatter formatter, IModelMapper modelMapper)
            {
                if (section == null) return null;
                var valeurs = new List<KeyValuePair<string, string[][]>>
                              {
                                  new KeyValuePair<string, string[][]>(nameof(section.CompteEnCollateral), new[] {new[] {section.CompteEnCollateral}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.TermeEntente), new[] {new[] {section.TermeEntente}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.ProvenanceFondsMaintienEnVigueur), new[] {new[] {section.ProvenanceFondsMaintienEnVigueur}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.PourcentageInteretsDeductibles), new[] {new[] {formatter.FormatPercentage(section.PourcentageInteretsDeductibles)}}),
                                  new KeyValuePair<string, string[][]>(nameof(section.DeductionCoutNetAssurancePure), new[] {new[] {formatter.FormatOuiNon(section.DeductionCoutNetAssurancePure)}})
                              };

                //new KeyValuePair<string, string[][]>(nameof(section.CalculerPrimesAdditionnellesMaintienEnVigueur), new[] {new[] {formatter.FormatOuiNon(section.CalculerPrimesAdditionnellesMaintienEnVigueur)}}),
                
                if (section.FraisDeGarantie.HasValue)
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>(nameof(section.FraisDeGarantie),
                                                                   new[] { new[] { formatter.FormatPercentage(section.FraisDeGarantie) }}));
                }

                if (!string.IsNullOrEmpty(section.TitulaireEntente))
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>(nameof(section.TitulaireEntente), new[] { new[] { section.TitulaireEntente }}));
                }

                if (!string.IsNullOrEmpty(section.Caution))
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>(nameof(section.Caution), new[] { new[] { section.Caution }}));
                }

                var result = new SectionSommaireViewModel
                             {
                                 TitreSection = section.TitreSection,
                                 Description = section.Description,
                                 Valeurs = valeurs,
                                 Libelles = section.Libelles ?? new Dictionary<string, string>(),
                                 Avis = section.Avis,
                                 Notes = modelMapper.MapperNotes(section.Notes)
                };


                return result;
            }

            private static SectionSommaireViewModel MapPaiementInteret(SectionPaiementInteretModel section, IIllustrationReportDataFormatter formatter, IModelMapper modelMapper)
            {
                if (section == null) return null;
                var pretsAdditionnels = section.Libelles?.FirstOrDefault(x => x.Key == "MontantMaximal").Value;
                if (section.PourcentageMontantMaximal.HasValue && section.PourcentageMontantMaximal > 0.000001 && section.PourcentageMontantMaximal < 1)
                {
                    var libelle = section.Libelles?.FirstOrDefault(x => x.Key == "DuMontantMaximal").Value ?? "{0}";
                    pretsAdditionnels = string.Format(libelle, formatter.FormatPercentage(section.PourcentageMontantMaximal));
                }

                if (section.EstPersonnalise)
                {
                    pretsAdditionnels = section.Libelles?.FirstOrDefault(x => x.Key == "MontantPersonnalise").Value;
                }

                var valeurs = new List<KeyValuePair<string, string[][]>>
                              {
                                  new KeyValuePair<string, string[][]>("PretsAdditionnels", new[] {new[] { pretsAdditionnels }})
                              };

                if (section.SoldePourFigerPret.HasValue)
                {
                    valeurs.Add(new KeyValuePair<string, string[][]> ("FigerPret", new[] { new[] { formatter.FormatCurrency(section.SoldePourFigerPret) }}));
                }
               
                var result = new SectionSommaireViewModel
                             {
                                 TitreSection = section.TitreSection,
                                 Description = section.Description,
                                 Libelles = section.Libelles ?? new Dictionary<string, string>(),
                                 Valeurs = valeurs,
                                 Avis = section.Avis,
                                 Notes = modelMapper.MapperNotes(section.Notes)
                };

                return result;
            }

            private static SectionSommaireViewModel MapRemboursement(SectionRemboursementModel section, IIllustrationReportDataFormatter formatter, IModelMapper modelMapper, bool gererPersonnalise)
            {
                if (section?.Remboursements == null || !section.Remboursements.Any())
                {
                    return null;
                }

                var valeurs = new List<KeyValuePair<string, string[][]>>();
                if (!string.IsNullOrWhiteSpace(section.ProvenanceFonds))
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>("ProvenanceFonds", new[] { new[] { section.ProvenanceFonds } }));
                }

                if (gererPersonnalise && section.EstPersonnalise)
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>("RemboursementPret", new[] { new[] { section.LibellePersonnalise } }));
                }
                else
                {
                    valeurs.Add(new KeyValuePair<string, string[][]>("RemboursementPret",
                                                                     (from item in section.Remboursements
                                                                      let libelleMontant = item.EstMontantMaximal ? section.LibelleMontantMaximal : formatter.FormatCurrency(item.Montant)
                                                                      select new[] { formatter.FormatterPeriodeAnnees(item.AnneeDebut, item.AnneeFin), libelleMontant }).ToArray()));
                }

                var result = new SectionSommaireViewModel
                             {
                                 TitreSection = section.TitreSection,
                                 Description = section.Description,
                                 Libelles = section.Libelles ?? new Dictionary<string, string>(),
                                 Valeurs = valeurs,
                                 Avis = section.Avis,
                                 Notes = modelMapper.MapperNotes(section.Notes)
                };

                return result;
            }
        }
    }
}