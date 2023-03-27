using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using System;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections
{
    public class UsageAuConseillerModelBuilder : IUsageAuConseillerModelBuilder
    {
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IVecteurManager _vecteurManager;
        private readonly IProductRules _productRules;

        public UsageAuConseillerModelBuilder(
            ISectionModelMapper sectionModelMapper, 
            IVecteurManager vecteurManager,
            IProductRules productRules)
        {
            _sectionModelMapper = sectionModelMapper;
            _vecteurManager = vecteurManager;
            _productRules = productRules;
        }

        public SectionUsageAuConseillerModel Build(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (!_productRules.EstParmiFamilleAssuranceParticipants(donnees.Produit))
            {
                return null;
            }

            var vecteurMontantNetAuRisque = _vecteurManager.ObtenirVecteurMontantNetAuRisque(donnees.Projections);
            if (vecteurMontantNetAuRisque.Length == 0)
            {
                return null;
            }

            var montantMax = vecteurMontantNetAuRisque.Max();
            var index = Array.IndexOf(vecteurMontantNetAuRisque, montantMax);
            var annee = _vecteurManager.TrouverAnneeSelonIndex(donnees.Projections.Projection, index);
            var section = new SectionUsageAuConseillerModel
            {
                MontantNetAuRisque = new MontantNetAuRisqueModel() { Montant = montantMax, Annee = annee }
            };

            if (donnees.MontantMaximumAnnuelOdsPermis.HasValue)
            {
                section.MaximumOdsPermis = new MaximumOdsPermisModel() { Montant = donnees.MontantMaximumAnnuelOdsPermis.Value};
            }

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }
    }
}
