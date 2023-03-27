using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.BonSuccessoral
{
    public class PageGraphiqueModelFactory : IPageGraphiqueModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IVecteurManager _vecteurManager;

        public PageGraphiqueModelFactory(IConfigurationRepository configurationRepository,
            ISectionModelMapper sectionModelMapper, IVecteurManager vecteurManager)
        {
            _configurationRepository = configurationRepository;
            _sectionModelMapper = sectionModelMapper;
            _vecteurManager = vecteurManager;
        }

        public PageGraphiqueModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definition = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new PageGraphiqueModel();
            _sectionModelMapper.MapperDefinition(model, definition, donnees, context);

            model.Annees = donnees.Projections.Projection.AnneesContrat.Skip(1).Take(donnees.Projections.IndexFinProjection).Where(x => x > 0).ToArray();
            model.Ages = donnees.Projections.Projection.Ages.Skip(1).Take(donnees.Projections.IndexFinProjection).Where(x => x > 0).Select(x => (int)x).ToArray();

            var valeurs = new List<double[]>();
            if (_vecteurManager.ColonnePresente(donnees.Projections, 138, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal))
            {
                valeurs.Add(_vecteurManager.ObtenirVecteurOuDefaut(
                    donnees.Projections, 138, 
                    Types.Enums.TypeProjection.Normal, 
                    Types.Enums.TypeRendementProjection.Normal).Skip(1).Take(model.Annees.Length).ToArray());
            }

            if (_vecteurManager.ColonnePresente(donnees.Projections, 2024, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal))
            {
                valeurs.Add(_vecteurManager.ObtenirVecteurOuDefaut(
                    donnees.Projections, 2024,
                    Types.Enums.TypeProjection.Normal,
                    Types.Enums.TypeRendementProjection.Normal).Skip(1).Take(model.Annees.Length).ToArray());
            }

            if (_vecteurManager.ColonnePresente(donnees.Projections, 811, Types.Enums.TypeProjection.BonSuccessoral, Types.Enums.TypeRendementProjection.Normal))
            {
                valeurs.Add(_vecteurManager.ObtenirVecteurOuDefaut(
                    donnees.Projections, 811,
                    Types.Enums.TypeProjection.BonSuccessoral,
                    Types.Enums.TypeRendementProjection.Normal).Skip(1).Take(model.Annees.Length).ToArray());
            }

            model.Valeurs = valeurs.Where(x => x.Any()).ToArray();
            
            if (definition.Libelles.ContainsKey("Graphique.Titre"))
            {
                model.TitreGraphique = donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Graphique.Titre"].Libelle
                    : definition.Libelles["Graphique.Titre"].LibelleEn;
            }

            if (definition.Libelles.ContainsKey("Axe.Annees"))
            {
                model.LibelleAnnees = donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Axe.Annees"].Libelle
                    : definition.Libelles["Axe.Annees"].LibelleEn;
            }

            if (definition.Libelles.ContainsKey("Axe.Age"))
            {
                model.LibelleAge = donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Axe.Age"].Libelle
                    : definition.Libelles["Axe.Age"].LibelleEn;
            }

            if (definition.Libelles.ContainsKey("Axe.Valeurs"))
            {
                model.LibelleValeur = donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Axe.Valeurs"].Libelle
                    : definition.Libelles["Axe.Valeurs"].LibelleEn;
            }

            var legendes = new List<string>();
            if (definition.Libelles.ContainsKey("Legendes.BonSuccessoral"))
            {
                legendes.Add(donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Legendes.BonSuccessoral"].Libelle
                    : definition.Libelles["Legendes.BonSuccessoral"].LibelleEn);
            }

            if (definition.Libelles.ContainsKey("Legendes.Placement"))
            {
                legendes.Add(donnees.Langue == Core.Types.Enums.Language.French
                    ? definition.Libelles["Legendes.Placement"].Libelle
                    : definition.Libelles["Legendes.Placement"].LibelleEn);
            }

            model.Legendes = legendes.ToArray();
            return model;
        }
    }
}
