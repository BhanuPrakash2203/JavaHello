using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageModificationsDemandeesBuilder : IPageModificationsDemandeesBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageModificationsDemandeesMapper _mapper;
        private readonly ISectionContratBuilder _sectionContratBuilder;
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder;

        public PageModificationsDemandeesBuilder(IReportFactory reportFactory, IPageModificationsDemandeesMapper mapper,
            ISectionContratBuilder sectionContratBuilder, ISectionProtectionsBuilder sectionProtectionsBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionContratBuilder = sectionContratBuilder;
            _sectionProtectionsBuilder = sectionProtectionsBuilder;
        }

        public void Build(BuildParameters<SectionModificationsDemandeesModel> parameters)
        {
            var report = _reportFactory.Create<IPageModificationsDemandees>();
            ReportBuilderAssembler.Assemble(report, new PageModificationsDemandeesViewModel(), parameters, _mapper,
                vm => BuildSubParts(report, parameters.Data, parameters.ReportContext));
        }

        private void BuildSubParts(IReport report, SectionModificationsDemandeesModel model,
            IReportContext reportContext)
        {
            if (model.SectionContratModel?.Transactions != null && 
                model.SectionContratModel.Transactions.Any())
            {
                _sectionContratBuilder.Build(new BuildParameters<SectionContratModel>(model.SectionContratModel)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }

            if (model.SectionProtectionsModel?.Protections != null && 
                model.SectionProtectionsModel.Protections.Any(x => x.Transactions != null && x.Transactions.Any()))
            {
                _sectionProtectionsBuilder.Build(new BuildParameters<SectionProtectionsModel>(model.SectionProtectionsModel)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }           
        }
    }
}