using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.Pilotage;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Parameters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Services;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Services;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.Factories;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.Factories.Initializer;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.Initializer;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.Resultats;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Tiers.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.Sommaire;
using Unity;
using Unity.Lifetime;

namespace IAFG.IA.VE.Impression.Illustration.DIContainer
{
    internal static class IllustrationsRegistry
    {
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<IReportFactory, ReportFactoryContainer>();
            container.RegisterType<ISystemInformation, SystemInformation>(new ContainerControlledLifetimeManager());

            InitializeCoreDependencies(container);
            InitializeResourcesDependencies(container);
            InitializeIllustrationsDependencies(container);
        }

        private static void InitializeCoreDependencies(IUnityContainer container)
        {
            container.RegisterType<IDecimalFormatter, DecimalFormatter>();
            container.RegisterType<INoDecimalFormatter, NoDecimalFormatter>();
            container.RegisterType<ICurrencyFormatter, CurrencyFormatter>();
            container.RegisterType<ICurrencyWithoutDecimalFormatter, CurrencyWithoutDecimalFormatter>();
            container.RegisterType<IPercentageFormatter, PercentageFormatter>();
            container.RegisterType<IPercentageWithoutSymbolFormatter, PercentageWithoutSymbolFormatter>();
            container.RegisterType<IDateBuilder, DateBuilder>();
            container.RegisterType<IDateFormatter, DateFormatter>();
            container.RegisterType<ILongDateFormatter, LongDateFormatter>();
            container.RegisterType<IDefaultValueFormatter, DefaultValueFormatter>();
        }

        private static void InitializeResourcesDependencies(IUnityContainer container)
        {
            container.RegisterType<IIllustrationResourcesAccessorFactory, ResourcesAccessorFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IArSectionInitializer, ArSectionInitializer>();
            container.RegisterType<ICultureAccessor, CultureAccessor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImpressionResourcesAccessor, ImpressionResourcesAccessor>(new ContainerControlledLifetimeManager());
        }

        private static void InitializeIllustrationsDependencies(IUnityContainer container)
        {
            container.RegisterType<IConfigurationRepository, ConfigurationRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPilotageRapportIllustrations, PilotageRapportIllustrationsIncorpores>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRegleAffaireAccessor, RegleAffaireAccessor>(new ContainerControlledLifetimeManager());

            container.RegisterType<IIllustrationReportDataFormatter, IllustrationReportDataFormatter>(new ContainerControlledLifetimeManager());

            container.RegisterType<IAutoMapperFactoryConfiguration, AutoMapperFactoryConfiguration>();
            container.RegisterType<IAutoMapperFactory, AutoMapperFactory>(new ContainerControlledLifetimeManager());

            container.RegisterType<IIllustrationsExportService, IllustrationsExportService>();
            container.RegisterType<IIllustrationMasterReport, IllustrationMasterReport>();
            container.RegisterType<IIllustrationMasterReportBuilder, IllustrationMasterReportBuilder>();
            container.RegisterType<IGeneriqueBuilders, GeneriqueBuilders>();
            container.RegisterType<IPrincipalBuilders, PrincipalBuilders>();
            container.RegisterType<IIllustrationMasterReportMapper, IllustrationMasterReportMapper>();
            container.RegisterType<IIllustrationModelMapper, IllustrationModelMapper>();
            container.RegisterType<IProtectionsMapper, ProtectionsMapper>();
            container.RegisterType<IProjectionsMapper, ProjectionsMapper>();
            container.RegisterType<IHypothesesMapper, HypothesesMapper>();
            container.RegisterType<IModificationsMapper, ModificationsMapper>();
            container.RegisterType<IConceptVenteMapper, ConceptVenteMapper>();
            container.RegisterType<IClientsMapper, ClientsMapper>();
            container.RegisterType<ISectionModelFactories, SectionModelFactories>();
            container.RegisterType<ISectionModelFactory, SectionModelFactory>();
            container.RegisterType<ISectionModelMapper, SectionModelMapper>();
            container.RegisterType<IModelMapper, ModelMapper>();
            container.RegisterType<IProjectionManager, ProjectionManager>();
            container.RegisterType<IDefinitionNoteManager, DefinitionNoteManager>();
            container.RegisterType<IDefinitionImageManager, DefinitionImageManager>();
            container.RegisterType<IDefinitionSectionManager, DefinitionSectionManager>();
            container.RegisterType<IDefinitionTexteManager, DefinitionTexteManager>();
            container.RegisterType<IDefinitionTitreManager, DefinitionTitreManager>();
            container.RegisterType<IDefinitionTableauManager, DefinitionTableauManager>();
            container.RegisterType<ITableauResultatManager, TableauResultatManager>();
            container.RegisterType<IVecteurManager, VecteurManager>();
            container.RegisterType<IProductRules, ProductRules>();
            container.RegisterType<IManagerFactory, ManagerFactory>();

            container.RegisterType<IBuilderRelevancyAnalyzer, BuilderRelevancyAnalyzer>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPageRelevancyAnalyzer, PageRelevancyAnalyzer>();
            container.RegisterType<IPageBreakSubReport, PageBreakSubReport>();
            container.RegisterType<IPageSectionBuilder, PageSectionBuilder>();
            container.RegisterType<IPageSectionMapper, PageSectionMapper>();
            container.RegisterType<IPageSection, PageSection>();

            RegisterPageApercuProtections(container);
            RegisterPageConditionsMedicales(container);
            RegisterPageConceptVente(container);
            RegisterPageDescriptionsProtections(container);
            RegisterPageGlossaire(container);
            RegisterPageHypotheseInvestissement(container);
            RegisterPageIntroduction(container);
            RegisterPageModificationsDemandees(container);
            RegisterPageNotesIllustration(container);
            RegisterPagePrimesRenouvellement(container);
            RegisterPageResultat(container);
            RegisterPageSignature(container);
            RegisterPageSommaire(container);
            RegisterPageSommaireProtections(container);
            RegisterPageSommaireProtectionsIllustration(container);

            container.RegisterType<IProjectionModelFactory, ProjectionModelFactory>();
            container.RegisterType<IProjectionParAssureModelFactory, ProjectionParAssureModelFactory>();
            container.RegisterType<ITestSensibiliteModelFactory, TestSensibiliteModelFactory>();

            RegisterBonSuccessoral(container);
        }

        private static void RegisterBonSuccessoral(IUnityContainer container)
        {
            container.RegisterType<IBonSuccessoralMapper, BonSuccessoralMapper>();
            container.RegisterType<Interfaces.Business.Factories.BonSuccessoral.IBonSuccessoralFactories,
                Business.Factories.BonSuccessoral.BonSuccessoralFactories>();
            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.IBonSuccessoralBuilders,
                Business.Builders.BonSuccessoral.BonSuccessoralBuilders>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.IPageTitreBuilder,
                Business.Builders.BonSuccessoral.PageTitreBuilder>();
            container.RegisterType<Interfaces.Business.Mappers.BonSuccessoral.IPageTitreMapper,
                Business.Mappers.BonSuccessoral.PageTitreMapper>();
            container.RegisterType<Interfaces.Business.Factories.BonSuccessoral.ITitreRapportModelFactory,
                Business.Factories.BonSuccessoral.TitreRapportModelFactory>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.IPageTitre,
                Tiers.Reports.SubReports.BonSuccessoral.PageTitre>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.IPageGraphiqueBuilder,
                Business.Builders.BonSuccessoral.PageGraphiqueBuilder>();
            container.RegisterType<Interfaces.Business.Mappers.BonSuccessoral.IPageGraphiqueMapper,
                Business.Mappers.BonSuccessoral.PageGraphiqueMapper>();
            container.RegisterType<Interfaces.Business.Factories.BonSuccessoral.IPageGraphiqueModelFactory,
                Business.Factories.BonSuccessoral.PageGraphiqueModelFactory>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.IPageGraphique,
                Tiers.Reports.SubReports.BonSuccessoral.PageGraphique>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.IPageSommaireBonSuccessoralBuilder,
                Business.Builders.BonSuccessoral.PageSommaireBonSuccessoralBuilder>();
            container.RegisterType<Interfaces.Business.Mappers.BonSuccessoral.IPageSommaireBonSuccessoralMapper,
                Business.Mappers.BonSuccessoral.PageSommaireBonSuccessoralMapper>();
            container.RegisterType<Interfaces.Business.Factories.BonSuccessoral.ISommaireBonSuccessoralModelFactory,
                Business.Factories.BonSuccessoral.SommaireBonSuccessoralModelFactory>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.IPageSommaireBonSuccessoral,
                Tiers.Reports.SubReports.BonSuccessoral.PageSommaireBonSuccessoral>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.Sommaire.ISectionContratBuilder,
                Business.Builders.BonSuccessoral.Sommaire.SectionContratBuilder>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.Sommaire.ISectionContrat,
                Tiers.Reports.SubReports.BonSuccessoral.Sommaire.SectionContrat>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.Sommaire.ISectionHypothesesInvestissementBuilder,
                Business.Builders.BonSuccessoral.Sommaire.SectionHypothesesInvestissementBuilder>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.Sommaire.ISectionHypothesesInvestissement,
                Tiers.Reports.SubReports.BonSuccessoral.Sommaire.SectionHypothesesInvestissement>();

            container.RegisterType<Interfaces.Business.Builders.BonSuccessoral.Sommaire.ISectionImpositionBuilder,
                Business.Builders.BonSuccessoral.Sommaire.SectionImpositionBuilder>();
            container.RegisterType<Types.Reports.SubReports.BonSuccessoral.Sommaire.ISectionImposition,
                Tiers.Reports.SubReports.BonSuccessoral.Sommaire.SectionImposition>();
        }

        private static void RegisterPageSommaireProtectionsIllustration(IUnityContainer container)
        {
            container.RegisterType<IPageSommaireProtectionsIllustration, PageSommaireProtectionsIllustration>();
            container.RegisterType<IPageSommaireProtectionsIllustrationBuilder, PageSommaireProtectionsIllustrationBuilder>();
            container.RegisterType<IPageSommaireProtectionsIllustrationMapper, PageSommaireProtectionsIllustrationMapper>();
            container.RegisterType<ISommaireProtectionsIllustrationModelFactory, Business.Factories.SommaireProtections.SommaireProtectionsIllustrationModelFactory>();

            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionContractants, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionContractants>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionAssures, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionAssures>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionConseiller, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionConseiller>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionCaracteristiquesIllustration, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionCaracteristiquesIllustration>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionProtections, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionProtections>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionProtectionsAccesVie, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionProtectionsAccesVie>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionPrimes, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionPrimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionPrimesVersees, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionPrimesVersees>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionASL, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionASL>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionUsageAuConseiller, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionUsageAuConseiller>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionSurprimes, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionSurprimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionTableauSurprimes, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionTableauSurprimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionDetailsSurprimes, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionDetailsSurprimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionFluxMonetaire, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionFluxMonetaire>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionDetailParticipations, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionDetailParticipations>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionChangementAffectationParticipations, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionChangementAffectationParticipations>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionDetailEclipseDePrime, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionDetailEclipseDePrime>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionScenarioParticipations, Tiers.Reports.SubReports.SommaireProtectionsIllustration.SectionScenarioParticipations>();

            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionContractantsBuilder, Business.Builders.SommaireProtectionsIllustration.SectionContractantsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionAssuresBuilder, Business.Builders.SommaireProtectionsIllustration.SectionAssuresBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionConseillerBuilder, Business.Builders.SommaireProtectionsIllustration.SectionConseillerBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionCaracteristiquesIllustrationBuilder, Business.Builders.SommaireProtectionsIllustration.SectionCaracteristiquesIllustrationBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionProtectionsBuilder, Business.Builders.SommaireProtectionsIllustration.SectionProtectionsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionPrimesBuilder, Business.Builders.SommaireProtectionsIllustration.SectionPrimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionPrimesVerseesBuilder, Business.Builders.SommaireProtectionsIllustration.SectionPrimesVerseesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionASLBuilder, Business.Builders.SommaireProtectionsIllustration.SectionASLBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionUsageAuConseillerBuilder, Business.Builders.SommaireProtectionsIllustration.SectionUsageAuConseillerBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionSurprimesBuilder, Business.Builders.SommaireProtectionsIllustration.SectionSurprimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionTableauSurprimesBuilder, Business.Builders.SommaireProtectionsIllustration.SectionTableauSurprimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionDetailsSurprimesBuilder, Business.Builders.SommaireProtectionsIllustration.SectionDetailsSurprimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionFluxMonetaireBuilder, Business.Builders.SommaireProtectionsIllustration.SectionFluxMonetaireBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionDetailParticipationsBuilder, Business.Builders.SommaireProtectionsIllustration.SectionDetailParticipationsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionChangementAffectationParticipationsBuilder, Business.Builders.SommaireProtectionsIllustration.SectionChangementAffectationParticipationsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionDetailEclipseDePrimeBuilder, Business.Builders.SommaireProtectionsIllustration.SectionDetailEclipseDePrimeBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtectionsIllustration.ISectionScenarioParticipationsBuilder, Business.Builders.SommaireProtectionsIllustration.SectionScenarioParticipationsBuilder>();
        }

        private static void RegisterPageConditionsMedicales(IUnityContainer container)
        {
            container.RegisterType<IPageConditionsMedicales, PageConditionsMedicales>();
            container.RegisterType<IPageConditionsMedicalesBuilder, PageConditionsMedicalesBuilder>();
            container.RegisterType<IPageConditionsMedicalesMapper, PageConditionsMedicalesMapper>();
            container.RegisterType<IConditionsMedicalesModelFactory, ConditionsMedicalesModelFactory>();

            container
                .RegisterType<Types.Reports.SubReports.ConditionsMedicales.ISectionTexteDescription,
                    Tiers.Reports.SubReports.ConditionsMedicales.SectionTexteDescription>();
            container
                .RegisterType<Types.Reports.SubReports.ConditionsMedicales.ISectionConditionMedicale,
                    Tiers.Reports.SubReports.ConditionsMedicales.SectionConditionMedicale>();
            container
                .RegisterType<Types.Reports.SubReports.ConditionsMedicales.ISectionTableauDescription,
                    Tiers.Reports.SubReports.ConditionsMedicales.SectionTableauDescription>();

            container
                .RegisterType<Interfaces.Business.Builders.ConditionsMedicales.ISectionTexteDescriptionBuilder,
                    Business.Builders.ConditionsMedicales.SectionTexteDescriptionBuilder>();
            container
                .RegisterType<Interfaces.Business.Builders.ConditionsMedicales.ISectionConditionMedicaleBuilder,
                    Business.Builders.ConditionsMedicales.SectionConditionMedicaleBuilder>();
            container
                .RegisterType<Interfaces.Business.Builders.ConditionsMedicales.ISectionTableauDescriptionBuilder,
                    Business.Builders.ConditionsMedicales.SectionTableauDescriptionBuilder>();
        }

        private static void RegisterPageSommaireProtections(IUnityContainer container)
        {
            container.RegisterType<IPageSommaireProtections, PageSommaireProtections>();
            container.RegisterType<IPageSommaireProtectionsBuilder, PageSommaireProtectionsBuilder>();
            container.RegisterType<IPageSommaireProtectionsMapper, PageSommaireProtectionsMapper>();
            container.RegisterType<ISommaireProtectionsModelFactory, Business.Factories.SommaireProtections.SommaireProtectionsModelFactory>();
            container.RegisterType<IUsageAuConseillerModelBuilder, Business.Factories.SommaireProtections.UsageAuConseillerModelBuilder>();
            container.RegisterType<IAssuranceSupplementaireLibereeModelBuilder, Business.Factories.SommaireProtections.AssuranceSupplementaireLibereeModelBuilder>();

            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionIdentificationBuilder, Business.Builders.SommaireProtections.SectionIdentificationBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionProtectionsBuilder, Business.Builders.SommaireProtections.SectionProtectionsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionPrimesBuilder, Business.Builders.SommaireProtections.SectionPrimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionSurprimesBuilder, Business.Builders.SommaireProtections.SectionSurprimesBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionASLBuilder, Business.Builders.SommaireProtections.SectionASLBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionFluxMonetaireBuilder, Business.Builders.SommaireProtections.SectionFluxMonetaireBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionDetailParticipationsBuilder, Business.Builders.SommaireProtections.SectionDetailParticipationsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionDetailEclipseDePrimeBuilder, Business.Builders.SommaireProtections.SectionDetailEclipseDePrimeBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionScenarioParticipationsBuilder, Business.Builders.SommaireProtections.SectionScenarioParticipationsBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionUsageAuConseillerBuilder, Business.Builders.SommaireProtections.SectionUsageAuConseillerBuilder>();
            container.RegisterType<Interfaces.Business.Builders.SommaireProtections.ISectionAvancesSurPoliceBuilder, Business.Builders.SommaireProtections.SectionAvancesSurPoliceBuilder>();
            
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionIdentificationMapper, Business.Mappers.SommaireProtections.SectionIdentificationMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionProtectionsMapper, Business.Mappers.SommaireProtections.SectionProtectionsMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionPrimesMapper, Business.Mappers.SommaireProtections.SectionPrimesMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionSurprimeMapper, Business.Mappers.SommaireProtections.SectionSurprimeMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionASLMapper, Business.Mappers.SommaireProtections.SectionASLMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionFluxMonetaireMapper, Business.Mappers.SommaireProtections.SectionFluxMonetaireMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionDetailParticipationsMapper, Business.Mappers.SommaireProtections.SectionDetailParticipationsMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionDetailEclipseDePrimeMapper, Business.Mappers.SommaireProtections.SectionDetailEclipseDePrimeMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionScenarioParticipationsMapper, Business.Mappers.SommaireProtections.SectionScenarioParticipationsMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionUsageAuConseillerMapper, Business.Mappers.SommaireProtections.SectionUsageAuConseillerMapper>();
            container.RegisterType<Interfaces.Business.Mappers.SommaireProtections.ISectionAvancesSurPoliceMapper, Business.Mappers.SommaireProtections.SectionAvancesSurPoliceMapper>();

            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionIdentification, Tiers.Reports.SubReports.SommaireProtections.SectionIdentification>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionProtections, Tiers.Reports.SubReports.SommaireProtections.SectionProtections>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionASL, Tiers.Reports.SubReports.SommaireProtections.SectionASL>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionPrimes, Tiers.Reports.SubReports.SommaireProtections.SectionPrimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionSurprimes, Tiers.Reports.SubReports.SommaireProtections.SectionSurprimes>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionFluxMonetaire, Tiers.Reports.SubReports.SommaireProtections.SectionFluxMonetaire>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionDetailParticipations, Tiers.Reports.SubReports.SommaireProtections.SectionDetailParticipations>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionDetailEclipseDePrime, Tiers.Reports.SubReports.SommaireProtections.SectionDetailEclipseDePrime>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionScenarioParticipations, Tiers.Reports.SubReports.SommaireProtections.SectionScenarioParticipations>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionUsageAuConseiller, Tiers.Reports.SubReports.SommaireProtections.SectionUsageAuConseiller>();
            container.RegisterType<Types.Reports.SubReports.SommaireProtections.ISectionAvancesSurPolice, Tiers.Reports.SubReports.SommaireProtections.SectionAvancesSurPolice>();
        }

        private static void RegisterPageApercuProtections(IUnityContainer container)
        {
            container.RegisterType<IPageApercuProtectionsBuilder, PageApercuProtectionsBuilder>();
            container.RegisterType<IApercuProtectionsModelFactory, ApercuProtectionsModelFactory>();
        }

        private static void RegisterPageNotesIllustration(IUnityContainer container)
        {
            container.RegisterType<IPageNotesIllustrationMapper, PageNotesIllustrationMapper>();
            container.RegisterType<IPageNotesIllustration, PageNotesIllustration>();
            container.RegisterType<IPageNotesIllustrationBuilder, PageNotesIllustrationBuilder>();
            container.RegisterType<INotesIllustrationModelFactory, NotesIllustrationModelFactory>();

            container.RegisterType<Interfaces.Business.Builders.NotesIllustration.ISectionTexteDescriptionBuilder, Business.Builders.NotesIllustration.SectionTexteDescriptionBuilder>();
            container.RegisterType<Interfaces.Business.Builders.NotesIllustration.ISectionListeDescriptionBuilder, Business.Builders.NotesIllustration.SectionListeDescriptionBuilder>();

            container.RegisterType<Types.Reports.SubReports.NotesIllustration.ISectionTexteDescription, Tiers.Reports.SubReports.NotesIllustration.SectionTexteDescription>();
            container.RegisterType<Types.Reports.SubReports.NotesIllustration.ISectionListeDescription, Tiers.Reports.SubReports.NotesIllustration.SectionListeDescription>();
        }

        private static void RegisterPagePrimesRenouvellement(IUnityContainer container)
        {
            container.RegisterType<IPagePrimesRenouvellement, PagePrimesRenouvellement>();
            container.RegisterType<IPagePrimesRenouvellementBuilder, PagePrimesRenouvellementBuilder>();
            container.RegisterType<IPagePrimesRenouvellementMapper, PagePrimesRenouvellementMapper>();
            container.RegisterType<IPrimesRenouvellementModelFactory, PrimesRenouvellementModelFactory>();

            container.RegisterType<ISectionPrimesRenouvellement, SectionPrimesRenouvellement>();
            container.RegisterType<ISectionPrimesRenouvellementBuilder, SectionPrimesRenouvellementBuilder>();
        }

        private static void RegisterPageSommaire(IUnityContainer container)
        {
            container.RegisterType<IPageSommaire, PageSommaire>();
            container.RegisterType<ISectionSommaire, SectionSommaire>();
            container.RegisterType<ISectionSommaireBuilder, SectionSommaireBuilder>();
        }

        private static void RegisterPageDescriptionsProtections(IUnityContainer container)
        {
            container.RegisterType<IPageDescriptionsProtections, PageDescriptionsProtections>();
            container.RegisterType<IPageDescriptionsProtectionsBuilder, PageDescriptionsProtectionsBuilder>();
            container.RegisterType<IPageDescriptionsProtectionsMapper, PageDescriptionsProtectionsMapper>();
            container.RegisterType<IDescriptionsProtectionsModelFactory, DescriptionsProtectionsModelFactory>();

            container
                .RegisterType<Types.Reports.SubReports.DescriptionsProtections.ISectionDescription,
                    Tiers.Reports.SubReports.DescriptionsProtections.SectionDescription>();
            container
                .RegisterType<Types.Reports.SubReports.DescriptionsProtections.ISectionTextes,
                    Tiers.Reports.SubReports.DescriptionsProtections.SectionTextes>();
            container
                .RegisterType<Types.Reports.SubReports.DescriptionsProtections.ISectionTableau,
                    Tiers.Reports.SubReports.DescriptionsProtections.SectionTableau>();

            container
                .RegisterType<Interfaces.Business.Builders.DescriptionsProtections.ISectionTextesBuilder,
                    Business.Builders.DescriptionsProtections.SectionTextesBuilder>();
            container
                .RegisterType<Interfaces.Business.Builders.DescriptionsProtections.ISectionDescriptionBuilder,
                    Business.Builders.DescriptionsProtections.SectionDescriptionBuilder>();
            container
                .RegisterType<Interfaces.Business.Builders.DescriptionsProtections.ISectionTableauBuilder,
                    Business.Builders.DescriptionsProtections.SectionTableauBuilder>();
        }

        private static void RegisterPageIntroduction(IUnityContainer container)
        {
            container.RegisterType<IPageIntroduction, PageIntroduction>();
            container.RegisterType<IPageIntroductionBuilder, PageIntroductionBuilder>();
        }

        private static void RegisterPageModificationsDemandees(IUnityContainer container)
        {
            container.RegisterType<IPageModificationsDemandees, PageModificationsDemandees>();
            container.RegisterType<IPageModificationsDemandeesBuilder, PageModificationsDemandeesBuilder>();
            container.RegisterType<IPageModificationsDemandeesMapper, PageModificationsDemandeesMapper>();
            container.RegisterType<IModificationsDemandeesModelFactory, ModificationsDemandeesModelFactory>();
            container
                .RegisterType<Types.Reports.SubReports.ModificationsDemandees.ISectionContrat,
                    Tiers.Reports.SubReports.ModificationsDemandees.SectionContrat>();
            container
                .RegisterType<Types.Reports.SubReports.ModificationsDemandees.ISectionProtections,
                    Tiers.Reports.SubReports.ModificationsDemandees.SectionProtections>();
            container
                .RegisterType<Interfaces.Business.Builders.ModificationsDemandees.ISectionContratBuilder,
                    Business.Builders.ModificationsDemandees.SectionContratBuilder>();
            container
                .RegisterType<Interfaces.Business.Builders.ModificationsDemandees.ISectionProtectionsBuilder,
                    Business.Builders.ModificationsDemandees.SectionProtectionsBuilder>();
            container
                .RegisterType<Interfaces.Business.Mappers.ModificationsDemandees.ISectionContratMapper,
                    Business.Mappers.ModificationsDemandees.SectionContratMapper>();
            container
                .RegisterType<Interfaces.Business.Mappers.ModificationsDemandees.ISectionProtectionsMapper,
                    Business.Mappers.ModificationsDemandees.SectionProtectionsMapper>();
        }

        private static void RegisterPageConceptVente(IUnityContainer container)
        {
            container.RegisterType<IPageConceptVenteBuilder, PageConceptVenteBuilder>();
            container.RegisterType<IPageConceptVenteMapper, PageConceptVenteMapper>();
            container.RegisterType<IConceptVenteModelFactory, ConceptVenteModelFactory>();
        }

        private static void RegisterPageResultat(IUnityContainer container)
        {
            container.RegisterType<IPageResultat, PageResultat>();
            container.RegisterType<IPageResultatBuilder, PageResultatBuilder>();
            container.RegisterType<IPageResultatAssureBuilder, PageResultatAssureBuilder>();
            container.RegisterType<IPageResultatMapper, PageResultatMapper>();
            container.RegisterType<IPageResultatParAssureMapper, PageResultatParAssureMapper>();
            container.RegisterType<ISectionTableauResultat, SectionTableauResultat>();
            container.RegisterType<ISectionTableauResultatBuilder, SectionTableauResultatBuilder>();
        }

        private static void RegisterPageGlossaire(IUnityContainer container)
        {
            container.RegisterType<IPageGlossaire, PageGlossaire>();
            container.RegisterType<IPageGlossaireBuilder, PageGlossaireBuilder>();
            container.RegisterType<IPageGlossaireMapper, PageGlossaireMapper>();
            container.RegisterType<IGlossaireModelFactory, GlossaireModelFactory>();
        }

        private static void RegisterPageSignature(IUnityContainer container)
        {
            container.RegisterType<IPageSignature, PageSignature>();
            container.RegisterType<IPageSignatureBuilder, PageSignatureBuilder>();
            container.RegisterType<IPageSignatureMapper, PageSignatureMapper>();
            container.RegisterType<ISignatureModelFactory, SignatureModelFactory>();
        }

        private static void RegisterPageHypotheseInvestissement(IUnityContainer container)
        {
            container.RegisterType<IPageHypotheseInvestissement, PageHypotheseInvestissement>();
            container.RegisterType<IPageHypotheseInvestissementBuilder, PageHypotheseInvestissementBuilder>();
            container.RegisterType<IPageHypotheseInvestissementMapper, PageHypotheseInvestissementMapper>();
            container.RegisterType<IHypothesesInvestissementModelFactory, HypothesesInvestissementModelFactory>();

            container.RegisterType<ISectionFondsCapitalisation, SectionFondsCapitalisation>();
            container.RegisterType<ISectionFondsCapitalisationMapper, SectionFondsCapitalisationMapper>();
            container.RegisterType<ISectionFondsCapitalisationBuilder, SectionFondsCapitalisationBuilder>();

            container.RegisterType<ISectionAjustementValeurMarchande, SectionAjustementValeurMarchande>();
            container.RegisterType<ISectionAjustementValeurMarchandeMapper, SectionAjustementValeurMarchandeMapper>();
            container.RegisterType<ISectionAjustementValeurMarchandeBuilder, SectionAjustementValeurMarchandeBuilder>();

            container.RegisterType<ISectionFondsTransitoire, SectionFondsTransitoire>();
            container.RegisterType<ISectionFondsTransitoireBuilder, SectionFondsTransitoireBuilder>();
            container.RegisterType<ISectionFondsTransitoireMapper, SectionFondsTransitoireMapper>();

            container.RegisterType<ISectionPrets, SectionPrets>();
            container.RegisterType<ISectionPretsBuilder, SectionPretsBuilder>();
            container.RegisterType<ISectionPretsMapper, SectionPretsMapper>();
        }
    }
}