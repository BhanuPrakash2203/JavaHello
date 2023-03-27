using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageSommaireProtectionsBuilder: IPageSommaireProtectionsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSommaireProtectionsMapper _mapper;
        private readonly ISectionIdentificationBuilder _sectionIdentificationBuilder;
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder;
        private readonly ISectionPrimesBuilder _sectionPrimesBuilder;
        private readonly ISectionASLBuilder _sectionAssuranceSupplementaireLibereeBuilder;
        private readonly ISectionFluxMonetaireBuilder _sectionFluxMonetaireBuilder;
        private readonly ISectionSurprimesBuilder _sectionSurprimesBuilder;
        private readonly ISectionDetailParticipationsBuilder _sectionDetailParticipationsBuilder;
        private readonly ISectionDetailEclipseDePrimeBuilder _sectionDetailEclipseDePrimeBuilder;
        private readonly ISectionScenarioParticipationsBuilder _sectionScenarioParticipationsBuilder;
        private readonly ISectionUsageAuConseillerBuilder _sectionUsageAuConseillerBuilder;
        private readonly ISectionAvancesSurPoliceBuilder _sectionAvancesSurPoliceBuilder;

        public PageSommaireProtectionsBuilder(IReportFactory reportFactory, 
            IPageSommaireProtectionsMapper  mapper,
            ISectionIdentificationBuilder sectionIdentificationBuilder, 
            ISectionPrimesBuilder sectionPrimesBuilder, 
            ISectionProtectionsBuilder sectionProtectionsBuilder, 
            ISectionFluxMonetaireBuilder sectionFluxMonetaireBuilder, 
            ISectionASLBuilder sectionAssuranceSupplementaireLibereeBuilder,
            ISectionSurprimesBuilder sectionSurprimesBuilder, 
            ISectionDetailParticipationsBuilder sectionDetailParticipationsBuilder,
            ISectionDetailEclipseDePrimeBuilder sectionDetailEclipseDePrimeBuilder,
            ISectionScenarioParticipationsBuilder sectionScenarioParticipationsBuilder,
            ISectionUsageAuConseillerBuilder sectionUsageAuConseillerBuilder,
            ISectionAvancesSurPoliceBuilder sectionAvancesSurPoliceBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionIdentificationBuilder = sectionIdentificationBuilder;
            _sectionProtectionsBuilder = sectionProtectionsBuilder;
            _sectionFluxMonetaireBuilder = sectionFluxMonetaireBuilder;
            _sectionAssuranceSupplementaireLibereeBuilder = sectionAssuranceSupplementaireLibereeBuilder;
            _sectionPrimesBuilder = sectionPrimesBuilder;
            _sectionSurprimesBuilder = sectionSurprimesBuilder;
            _sectionDetailParticipationsBuilder = sectionDetailParticipationsBuilder;
            _sectionDetailEclipseDePrimeBuilder = sectionDetailEclipseDePrimeBuilder;
            _sectionScenarioParticipationsBuilder = sectionScenarioParticipationsBuilder;
            _sectionUsageAuConseillerBuilder = sectionUsageAuConseillerBuilder;
            _sectionAvancesSurPoliceBuilder = sectionAvancesSurPoliceBuilder;
        }

        public void Build(BuildParameters<SectionSommaireProtectionsModel> parameters)
        {
            var report = _reportFactory.Create<IPageSommaireProtections>();
            ReportBuilderAssembler.Assemble(report, new PageSommaireProtectionsViewModel(), parameters, _mapper, vm => BuildSubParts(report, parameters.Data, parameters.ReportContext));
        }

        private void BuildSubParts(IReport report, SectionSommaireProtectionsModel sourceObject, IReportContext reportContext)
        {
            if (sourceObject.SectionIdendification != null)
            {
                _sectionIdentificationBuilder.Build(new BuildParameters<SectionIdendificationModel>(sourceObject.SectionIdendification)
                                                    {
                                                        ReportContext = reportContext,
                                                        ParentReport = report
                                                    });
            }

            if (sourceObject.SectionProtections != null)
            {
                _sectionProtectionsBuilder.Build(new BuildParameters<SectionProtectionsModel>(sourceObject.SectionProtections)
                                                 {
                                                     ReportContext = reportContext,
                                                     ParentReport = report
                                                 });
            }

            if (sourceObject.SectionSurprimes != null)
            {
                _sectionSurprimesBuilder.Build(new BuildParameters<SectionSurprimesModel>(sourceObject.SectionSurprimes)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionPrimes != null)
            {
                _sectionPrimesBuilder.Build(new BuildParameters<SectionPrimesModel>(sourceObject.SectionPrimes)
                                            {
                                                ReportContext = reportContext,
                                                ParentReport = report
                                            });
            }

            if (sourceObject.SectionAsl != null)
            {
                _sectionAssuranceSupplementaireLibereeBuilder.Build(new BuildParameters<SectionASLModel>(sourceObject.SectionAsl)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionDetailParticipations != null)
            {
                _sectionDetailParticipationsBuilder.Build(new BuildParameters<SectionDetailParticipationsModel>(sourceObject.SectionDetailParticipations)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionScenarioParticipations != null)
            {
                _sectionScenarioParticipationsBuilder.Build(new BuildParameters<SectionScenarioParticipationsModel>(sourceObject.SectionScenarioParticipations)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionFluxMonetaire != null)
            {
                _sectionFluxMonetaireBuilder.Build(new BuildParameters<SectionFluxMonetaireModel>(sourceObject.SectionFluxMonetaire)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionEclipseDePrime != null)
            {
                _sectionDetailEclipseDePrimeBuilder.Build(new BuildParameters<SectionDetailEclipseDePrimeModel>(sourceObject.SectionEclipseDePrime)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (sourceObject.SectionAvancesSurPolice != null)
            {
                _sectionAvancesSurPoliceBuilder.Build(
                    new BuildParameters<SectionAvancesSurPoliceModel>(sourceObject.SectionAvancesSurPolice)
                    {
                        ReportContext = reportContext,
                        ParentReport = report
                    });
            }

            if (sourceObject.SectionUsageAuConseiller != null)
            {
                _sectionUsageAuConseillerBuilder.Build(
                    new BuildParameters<SectionUsageAuConseillerModel>(sourceObject.SectionUsageAuConseiller)
                    {
                        ReportContext = reportContext,
                        ParentReport = report
                    });
            }
        }
    }
}