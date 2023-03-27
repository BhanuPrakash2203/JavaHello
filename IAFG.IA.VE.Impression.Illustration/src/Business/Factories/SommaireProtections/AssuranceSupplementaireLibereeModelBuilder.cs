using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.ASL;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections
{
    public class AssuranceSupplementaireLibereeModelBuilder : IAssuranceSupplementaireLibereeModelBuilder
    {
        private readonly ISectionModelMapper _sectionModelMapper;

        public AssuranceSupplementaireLibereeModelBuilder(ISectionModelMapper sectionModelMapper)
        {
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionASLModel Build(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null || donnees.AssuranceSupplementaireLiberee == null ||
                (donnees.Produit != Produit.CapitalValeur && donnees.Produit != Produit.CapitalValeur3))
            {
                return null;
            }

            var section = new SectionASLModel
            {
                OptionVersementBoni = donnees.AssuranceSupplementaireLiberee.OptionVersementBoni,
                CapitalAssureMaximal = donnees.AssuranceSupplementaireLiberee.CapitalAssureMaximal,
                AucunAchat = Math.Abs(donnees.AssuranceSupplementaireLiberee.CapitalAssureMaximal) < .009,
                AucunMaximum = donnees.AssuranceSupplementaireLiberee.CapitalAssureMaximal >= 999999999,
                Taux = CreerTaux(donnees.AssuranceSupplementaireLiberee.TauxAnnees),
                Allocations = CreerAllocations(donnees.AssuranceSupplementaireLiberee, donnees.Projections.AnneeDebutProjection)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private static IList<DetailTaux> CreerTaux(IList<TauxAnnee> taux)
        {
            var result = new List<DetailTaux>();
            if (taux == null) return result;

            DetailTaux detailPrecedent = null;
            foreach (var item in taux.OrderBy(t => t.Annee))
            {
                if (detailPrecedent != null && !(Math.Abs(detailPrecedent.Taux - item.Taux) > .000009))
                {
                    continue;
                }

                var detail = new DetailTaux
                {
                    AnneeDebut = item.Annee,
                    Taux = item.Taux
                };

                result.Add(detail);
                detailPrecedent = detail;
            }

            return result;
        }

        private static List<DetailAllocationASL> CreerAllocations(AssuranceSupplementaireLiberee asl, int anneeDebut)
        {
            var result = new List<DetailAllocationASL>
            {
                new DetailAllocationASL
                {
                    AnneeDebut = anneeDebut,
                    Montant = asl.MontantAllocationInitial
                }
            };

            if (asl.Allocations == null) return result;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var item in asl.Allocations.OrderBy(a => a.Annee))
            {
                var detailAllocationPrecedente = result.Last();
                if (Math.Abs(detailAllocationPrecedente.Montant - item.Montant) > .009)
                {
                    result.Add(new DetailAllocationASL
                    {
                        AnneeDebut = item.Annee,
                        Montant = item.Montant
                    });
                }
            }

            return result;
        }
    }
}