using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageResultatAssureBuilder : IPageResultatAssureBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageResultatParAssureMapper _mapper;
        private readonly ISectionTableauResultatBuilder _sectionTableauResultatBuilder;

        public PageResultatAssureBuilder(IReportFactory reportFactory, IPageResultatParAssureMapper mapper,
                                            ISectionTableauResultatBuilder sectionTableauResultatBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionTableauResultatBuilder = sectionTableauResultatBuilder;
        }

        public void Build(BuildParameters<SectionResultatParAssureModel> parameters)
        {
            var report = _reportFactory.Create<IPageResultat>();
            var viewModel = new PageResultatViewModel();
            _mapper.Map(parameters.Data, viewModel, parameters.ReportContext);

            if (!IsRelevant(viewModel)) return;
            ReportBuilderAssembler.AssembleWithoutModelMapping(report,
                                                               viewModel,
                                                               parameters,
                                                               vm => BuildSubParts(vm, report, parameters.ReportContext));
        }

        private void BuildSubParts(PageResultatViewModel viewModel, IReport report, IReportContext reportContext)
        {
            if (viewModel == null) return;
            var premierePage = true;
            foreach (var tableau in viewModel.Tableaux)
            {
                if (!IsRelevant(tableau)) continue;
                BuildTableau(report, reportContext, tableau, premierePage);
                premierePage = false;
            }
        }

        private static bool IsRelevant(PageResultatViewModel viewModel)
        {
            //On n'affiche pas la page si au moins un tableau comporte une colonne dite normale (autre que celles affichant les années ou les âges) n'est pas présente.
            return viewModel.Tableaux?.Any(IsRelevant) ?? false;
        }

        private static bool IsRelevant(TableauResultatViewModel tableau)
        {
            return (tableau?.GroupeColonnes?.Any(x => x.Colonnes.Any(y => y.TypeColonne == TypeColonne.Normale)) ?? false) && (tableau.Lignes?.Any() ?? false);
        }

        private void BuildTableau(IReport report, IReportContext reportContext, TableauResultatViewModel viewModel, bool premierePage)
        {
            if (!premierePage)
            {
                report.AddSubReport(_reportFactory.Create<IPageBreakSubReport>());
            }

            _sectionTableauResultatBuilder.Build(new BuildParameters<TableauResultatViewModel>(viewModel)
                                                 {
                                                     ReportContext = reportContext,
                                                     ParentReport = report
                                                 });
        }
    }
}