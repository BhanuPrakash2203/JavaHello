using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class GeneriqueBuilders : IGeneriqueBuilders
    {
        private readonly ISectionModelFactories _modelFactories;

        public GeneriqueBuilders(
            ISectionModelFactories modelFactories,
            IPageSectionBuilder pageSectionBuilder,
            IPageGlossaireBuilder pageGlossaireBuilder,
            IPageResultatBuilder pageResultatBuilder,
            IPageResultatAssureBuilder pageResultatAssureBuilder)
        {
            _modelFactories = modelFactories;
            PageSectionBuilder = pageSectionBuilder;
            PageGlossaireBuilder = pageGlossaireBuilder;
            PageResultatBuilder = pageResultatBuilder;
            PageResultatAssureBuilder = pageResultatAssureBuilder;
        }

        public IPageSectionBuilder PageSectionBuilder { get; }
        public IPageGlossaireBuilder PageGlossaireBuilder { get; }
        public IPageResultatBuilder PageResultatBuilder { get; }
        public IPageResultatAssureBuilder PageResultatAssureBuilder { get; }

        public IRelevantBuilder GetBuilderGlossaire(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageGlossaireBuilder, SectionGlossaireModel>(
                PageGlossaireBuilder,
                _modelFactories.GlossaireModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderSection(ConfigurationSection section, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageSectionBuilder, SectionModel>(
                PageSectionBuilder,
                _modelFactories.SectionModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderProjectionParGroupeAssure(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageResultatAssureBuilder, SectionResultatParAssureModel>(
                PageResultatAssureBuilder,
                _modelFactories.ProjectionParAssureModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderProjection(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageResultatBuilder, SectionResultatModel>(
                PageResultatBuilder,
                _modelFactories.ProjectionModelFactory.Build(section.SectionId, donnees, context),
                context);
        }
    }
}