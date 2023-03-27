using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PrincipalBuilders : IPrincipalBuilders
    {
        private readonly ISectionModelFactories _modelFactories;
        private readonly IGeneriqueBuilders _generiqueBuilders;

        public PrincipalBuilders(
            ISectionModelFactories modelFactories,
            IGeneriqueBuilders generiqueBuilders,
            IPageIntroductionBuilder pageIntroductionBuilder,
            IPageSommaireProtectionsBuilder pageSommaireProtectionsBuilder,
            IPagePrimesRenouvellementBuilder pagePrimesRenouvellementBuilder,
            IPageHypotheseInvestissementBuilder pageHypotheseInvestissementBuilder,
            IPageSignatureBuilder pageSignatureBuilder,
            IPageDescriptionsProtectionsBuilder pageDescriptionsProtectionsBuilder,
            IPageSommaireProtectionsIllustrationBuilder pageSommaireProtectionsIllustrationBuilder,
            IPageApercuProtectionsBuilder pageApercuProtectionBuilder,
            IPageNotesIllustrationBuilder pageNotesIllustrationBuilder,
            IPageConditionsMedicalesBuilder pageConditionsMedicalesBuilder,
            IPageConceptVenteBuilder pageConceptVenteBuilder, 
            IPageModificationsDemandeesBuilder pageModificationsDemandeesBuilder)
        {
            _modelFactories = modelFactories;
            _generiqueBuilders = generiqueBuilders;
            PageIntroductionBuilder = pageIntroductionBuilder;
            PageSommaireProtectionsBuilder = pageSommaireProtectionsBuilder;
            PagePrimesRenouvellementBuilder = pagePrimesRenouvellementBuilder;
            PageHypotheseInvestissementBuilder = pageHypotheseInvestissementBuilder;
            PageSignatureBuilder = pageSignatureBuilder;
            PageDescriptionsProtectionsBuilder = pageDescriptionsProtectionsBuilder;
            PageSommaireProtectionsIllustrationBuilder = pageSommaireProtectionsIllustrationBuilder;
            PageApercuProtectionsBuilder = pageApercuProtectionBuilder;
            PageNotesIllustrationBuilder = pageNotesIllustrationBuilder;
            PageConditionsMedicalesBuilder = pageConditionsMedicalesBuilder;
            PageConceptVenteBuilder = pageConceptVenteBuilder;
            PageModificationsDemandeesBuilder = pageModificationsDemandeesBuilder;
        }

        public IPageIntroductionBuilder PageIntroductionBuilder { get; }
        public IPageSommaireProtectionsBuilder PageSommaireProtectionsBuilder { get; }
        public IPagePrimesRenouvellementBuilder PagePrimesRenouvellementBuilder { get; }
        public IPageHypotheseInvestissementBuilder PageHypotheseInvestissementBuilder { get; }
        public IPageSignatureBuilder PageSignatureBuilder { get; }
        public IPageDescriptionsProtectionsBuilder PageDescriptionsProtectionsBuilder { get; }
        public IPageSommaireProtectionsIllustrationBuilder PageSommaireProtectionsIllustrationBuilder { get; }
        public IPageApercuProtectionsBuilder PageApercuProtectionsBuilder { get; }
        public IPageConditionsMedicalesBuilder PageConditionsMedicalesBuilder { get; }
        public IPageNotesIllustrationBuilder PageNotesIllustrationBuilder { get; }
        public IPageConceptVenteBuilder PageConceptVenteBuilder { get; }
        public IPageModificationsDemandeesBuilder PageModificationsDemandeesBuilder { get; }

        public IRelevantBuilder GetBuilderPrimesDeRenouvellement(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPagePrimesRenouvellementBuilder, PagePrimesRenouvellementModel>(
                PagePrimesRenouvellementBuilder,
                _modelFactories.PrimesRenouvellementModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderHypothesesInvestissement(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageHypotheseInvestissementBuilder, SectionHypothesesInvestissementModel>(
                PageHypotheseInvestissementBuilder,
                _modelFactories.HypothesesInvestissementModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderIntroduction(ConfigurationSection section, DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageIntroductionBuilder, DonneesRapportIllustration>(
                PageIntroductionBuilder, donnees, context);
        }

        public IRelevantBuilder GetBuilderConditionsMedicales(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            var model = _modelFactories.ConditionsMedicalesModelFactory.Build(section.SectionId, donnees, context);
            if (model.Sections == null || !model.Sections.Any()) return null;

            return new RelevantBuilder<IPageConditionsMedicalesBuilder, SectionConditionsMedicalesModel>(
                PageConditionsMedicalesBuilder, model, context);
        }


        public IRelevantBuilder GetBuilderModificationsDemandees(ConfigurationSection section, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageModificationsDemandeesBuilder, SectionModificationsDemandeesModel>(
                PageModificationsDemandeesBuilder,
                _modelFactories.ModificationsDemandeesModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderConceptVente(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageConceptVenteBuilder, SectionConceptVenteModel>(
                PageConceptVenteBuilder,
                _modelFactories.ConceptVenteModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderNotesIllustration(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageNotesIllustrationBuilder, SectionNotesIllustrationModel>(
                PageNotesIllustrationBuilder,
                _modelFactories.NotesIllustrationModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderApercuProtections(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageApercuProtectionsBuilder, SectionApercuProtectionsModel>(
                PageApercuProtectionsBuilder,
                _modelFactories.ApercuProtectionsModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderSommaireProtections(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageSommaireProtectionsIllustrationBuilder,
                SectionSommaireProtectionsIllustrationModel>(
                PageSommaireProtectionsIllustrationBuilder,
                _modelFactories.SommaireProtectionsIllustrationModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderDescriptionsProtections(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageDescriptionsProtectionsBuilder, SectionDescriptionsProtectionsModel>(
                PageDescriptionsProtectionsBuilder,
                _modelFactories.DescriptionsProtectionsModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderTestDeSensibilite(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageResultatBuilder, SectionResultatModel>(
                _generiqueBuilders.PageResultatBuilder,
                _modelFactories.TestSensibiliteModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderSignature(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageSignatureBuilder, SectionSignatureModel>(
                PageSignatureBuilder,
                _modelFactories.SignatureModelFactory.Build(section.SectionId, donnees, context),
                context);
        }

        public IRelevantBuilder GetBuilderProtections(ConfigurationSection section,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return new RelevantBuilder<IPageSommaireProtectionsBuilder, SectionSommaireProtectionsModel>(
                PageSommaireProtectionsBuilder,
                _modelFactories.SommaireProtectionsModelFactory.Build(section.SectionId, donnees, context),
                context);
        }
    }
}