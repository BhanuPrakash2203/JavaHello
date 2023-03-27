using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral
{
    public class BonSuccessoralBuilders : IBonSuccessoralBuilders
    {
        private readonly IBonSuccessoralFactories _modelFactories;

        public BonSuccessoralBuilders(
            IBonSuccessoralFactories modelFactories, 
            IPageTitreBuilder pageTitreBuilder, 
            IPageGraphiqueBuilder pageGraphiqueBuilder,
            IPageSommaireBonSuccessoralBuilder pageSommaireBonSuccessoralBuilder)
        {
            _modelFactories = modelFactories;
            PageTitreBuilder = pageTitreBuilder;
            PageGraphiqueBuilder = pageGraphiqueBuilder;
            PageSommaireBonSuccessoralBuilder = pageSommaireBonSuccessoralBuilder;
        }

        public IPageTitreBuilder PageTitreBuilder { get; }

        public IPageGraphiqueBuilder PageGraphiqueBuilder { get; }

        public IPageSommaireBonSuccessoralBuilder PageSommaireBonSuccessoralBuilder { get; }

        public IRelevantBuilder GetBuilderGraphique(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageGraphiqueBuilder, Types.SectionModels.BonSuccessoral.PageGraphiqueModel>(
                PageGraphiqueBuilder,
                _modelFactories.PageGraphiqueModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderSommaireBonSuccessoral(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageSommaireBonSuccessoralBuilder, Types.SectionModels.BonSuccessoral.SommaireBonSuccessoralModel>(
                PageSommaireBonSuccessoralBuilder,
                _modelFactories.SommaireBonSuccessoralModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderPageTitre(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageTitreBuilder, Types.SectionModels.BonSuccessoral.TitreRapportModel>(
                PageTitreBuilder,
                _modelFactories.TitreRapportModelFactory.Build(section.SectionId, donnees, context),
                context);
        }
    }
}
