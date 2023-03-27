using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using ISectionFluxMonetaireBuilder =
    IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration.
    ISectionFluxMonetaireBuilder;
using ISectionPrimesBuilder =
    IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration.
    ISectionPrimesBuilder;
using ISectionSurprimesBuilder =
    IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration.
    ISectionSurprimesBuilder;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageSommaireProtectionsIllustrationBuilder : IPageSommaireProtectionsIllustrationBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSommaireProtectionsIllustrationMapper _mapper;
        private readonly ISectionContractantsBuilder _sectionContractantsBuilder;
        private readonly ISectionAssuresBuilder _sectionAssuresBuilder;
        private readonly ISectionSurprimesBuilder _sectionSurprimesBuilder;
        private readonly ISectionConseillerBuilder _sectionConseillerBuilder;
        private readonly ISectionPrimesBuilder _sectionPrimesBuilder;
        private readonly ISectionPrimesVerseesBuilder _sectionPrimesVerseesBuilder;
        private readonly ISectionASLBuilder _sectionASLBuilder;
        private readonly ISectionUsageAuConseillerBuilder _sectionUsageAuConseillerBuilder;
        private readonly ISectionCaracteristiquesIllustrationBuilder _sectionCaracteristiquesIllustrationBuilder;
        private readonly ISectionFluxMonetaireBuilder _sectionFluxMonetaireBuilder;
        private readonly ISectionDetailParticipationsBuilder _sectionDetailParticipationsBuilder;
        private readonly ISectionChangementAffectationParticipationsBuilder _sectionChangementAffectationParticipationsBuilder;
        private readonly ISectionDetailEclipseDePrimeBuilder _sectionDetailEclipseDePrimeBuilder;
        private readonly ISectionScenarioParticipationsBuilder _sectionScenarioParticipationsBuilder;
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder;

        public PageSommaireProtectionsIllustrationBuilder(IReportFactory reportFactory,
            IPageSommaireProtectionsIllustrationMapper mapper,
            ISectionContractantsBuilder sectionContractantsBuilder, ISectionAssuresBuilder sectionAssuresBuilder,
            ISectionSurprimesBuilder sectionSurprimesBuilder,
            ISectionConseillerBuilder sectionConseillerBuilder, ISectionPrimesBuilder sectionPrimesBuilder,
            ISectionPrimesVerseesBuilder sectionPrimesVerseesBuilder,
            ISectionCaracteristiquesIllustrationBuilder sectionCaracteristiquesIllustrationBuilder,
            ISectionFluxMonetaireBuilder sectionFluxMonetaireBuilder,
            ISectionASLBuilder sectionAslBuilder,
            ISectionDetailParticipationsBuilder sectionDetailParticipationsBuilder,
            ISectionChangementAffectationParticipationsBuilder sectionChangementAffectationParticipationsBuilder,
            ISectionDetailEclipseDePrimeBuilder sectionDetailEclipseDePrimeBuilder,
            ISectionScenarioParticipationsBuilder sectionScenarioParticipationsBuilder,
            ISectionUsageAuConseillerBuilder sectionUsageAuConseillerBuilder, 
            ISectionProtectionsBuilder sectionProtectionsBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionContractantsBuilder = sectionContractantsBuilder;
            _sectionAssuresBuilder = sectionAssuresBuilder;
            _sectionSurprimesBuilder = sectionSurprimesBuilder;
            _sectionConseillerBuilder = sectionConseillerBuilder;
            _sectionPrimesBuilder = sectionPrimesBuilder;
            _sectionPrimesVerseesBuilder = sectionPrimesVerseesBuilder;
            _sectionFluxMonetaireBuilder = sectionFluxMonetaireBuilder;
            _sectionASLBuilder = sectionAslBuilder;
            _sectionDetailParticipationsBuilder = sectionDetailParticipationsBuilder;
            _sectionChangementAffectationParticipationsBuilder = sectionChangementAffectationParticipationsBuilder;
            _sectionCaracteristiquesIllustrationBuilder = sectionCaracteristiquesIllustrationBuilder;
            _sectionDetailEclipseDePrimeBuilder = sectionDetailEclipseDePrimeBuilder;
            _sectionScenarioParticipationsBuilder = sectionScenarioParticipationsBuilder;
            _sectionUsageAuConseillerBuilder = sectionUsageAuConseillerBuilder;
            _sectionProtectionsBuilder = sectionProtectionsBuilder;
        }

        public void Build(BuildParameters<SectionSommaireProtectionsIllustrationModel> parameters)
        {
            var pageSommaireViewModel = new PageSommaireProtectionsIllustrationViewModel();
            _mapper.Map(parameters.Data, pageSommaireViewModel, parameters.ReportContext);

            var param = new BuildParameters<PageSommaireProtectionsIllustrationViewModel>(pageSommaireViewModel)
            {
                ParentReport = parameters.ParentReport,
                ReportContext = parameters.ReportContext,
                StyleOverride = parameters.StyleOverride
            };

            var report = _reportFactory.Create<IPageSommaireProtectionsIllustration>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param,
                vm => BuildSubparts(report, param.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(IReport report, PageSommaireProtectionsIllustrationViewModel paramData,
            IReportContext reportContext, IStyleOverride styleOverride)
        {
            if (paramData.SectionConseiller != null &&
                paramData.SectionConseiller.Conseillers.Any(x => !string.IsNullOrWhiteSpace(x.NomComplet)))
            {
                _sectionConseillerBuilder.Build(
                    new BuildParameters<SectionConseillerViewModel>(paramData.SectionConseiller)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionContractants != null)
            {
                _sectionContractantsBuilder.Build(
                    new BuildParameters<SectionContractantsViewModel>(paramData.SectionContractants)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionCaracteristiquesIllustration != null)
            {
                _sectionCaracteristiquesIllustrationBuilder.Build(
                    new BuildParameters<SectionCaracteristiquesIllustrationViewModel>(paramData
                        .SectionCaracteristiquesIllustration)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionsAssures != null)
            {
                foreach (var sectionAssures in paramData.SectionsAssures)
                {
                    _sectionAssuresBuilder.Build(new BuildParameters<SectionAssuresViewModel>(sectionAssures)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });

                    if (sectionAssures.Protections?.Protections != null && sectionAssures.Protections.Protections.Any())
                    {
                        _sectionProtectionsBuilder.Build(new BuildParameters<ProtectionViewModel>(sectionAssures.Protections)
                        {
                            ReportContext = reportContext,
                            ParentReport = report,
                            StyleOverride = styleOverride
                        });
                    }
                }
            }

            if (paramData.SectionSurprimes != null &&
                paramData.SectionSurprimes.Protections.Any(p => p.Surprimes.Any()))
            {
                _sectionSurprimesBuilder.Build(
                    new BuildParameters<SectionSurprimesViewModel>(paramData.SectionSurprimes)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionPrimes != null && paramData.SectionPrimes.Primes.Any())
            {
                _sectionPrimesBuilder.Build(new BuildParameters<SectionPrimesViewModel>(paramData.SectionPrimes)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }

            if (paramData.SectionPrimesVersees != null && paramData.SectionPrimesVersees.PrimesVersees.Any())
            {
                _sectionPrimesVerseesBuilder.Build(
                    new BuildParameters<SectionPrimesVerseesViewModel>(paramData.SectionPrimesVersees)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionASLModel != null)
            {
                _sectionASLBuilder.Build(new BuildParameters<ASLViewModel>(paramData.SectionASLModel)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }

            if (paramData.SectionDetailParticipations != null)
            {
                _sectionDetailParticipationsBuilder.Build(
                    new BuildParameters<SectionDetailParticipationsViewModel>(paramData.SectionDetailParticipations)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }
                  
            if (paramData.SectionScenarioParticipations != null)
            {
                _sectionScenarioParticipationsBuilder.Build(
                    new BuildParameters<SectionScenarioParticipationsViewModel>(paramData.SectionScenarioParticipations)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionFluxMonetaire != null && paramData.SectionFluxMonetaire.Details.Any())
            {
                _sectionFluxMonetaireBuilder.Build(
                    new BuildParameters<SectionFluxMonetaireViewModel>(paramData.SectionFluxMonetaire)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionDetailEclipseDePrime != null)
            {
                _sectionDetailEclipseDePrimeBuilder.Build(
                    new BuildParameters<SectionDetailEclipseDePrimeViewModel>(paramData.SectionDetailEclipseDePrime)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionChangementAffectationParticipations != null)
            {
                _sectionChangementAffectationParticipationsBuilder.Build(
                    new BuildParameters<SectionChangementAffectationParticipationsViewModel>(paramData.SectionChangementAffectationParticipations)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }

            if (paramData.SectionUsageAuConseiller != null)
            {
                _sectionUsageAuConseillerBuilder.Build(
                    new BuildParameters<SectionUsageAuConseillerViewModel>(paramData.SectionUsageAuConseiller)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }            
        }
    }
}