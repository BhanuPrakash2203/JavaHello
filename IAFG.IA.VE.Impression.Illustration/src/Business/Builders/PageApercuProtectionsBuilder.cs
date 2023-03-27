using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageApercuProtectionsBuilder : IPageApercuProtectionsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSectionMapper _sectionMapper;
        private readonly IPageResultatMapper _resultatMapper;
        private readonly ISectionTableauResultatBuilder _tableauResultatBuilder;

        public PageApercuProtectionsBuilder(
            IReportFactory reportFactory, 
            IPageSectionMapper sectionMapper,
            IPageResultatMapper resultatMapper,
            ISectionTableauResultatBuilder tableauResultatBuilder)
        {
            _reportFactory = reportFactory;
            _sectionMapper = sectionMapper;
            _resultatMapper = resultatMapper;
            _tableauResultatBuilder = tableauResultatBuilder;
        }

        public void Build(BuildParameters<SectionApercuProtectionsModel> parameters)
        {
            var report = _reportFactory.Create<IPageResultat>();
            var viewModel = new PageResultatViewModel();
            _sectionMapper.Map(parameters.Data, viewModel, parameters.ReportContext);

            ReportBuilderAssembler.AssembleWithoutModelMapping(report, viewModel, parameters,
                vm => BuildSubparts(report, parameters.ReportContext, parameters.Data.SectionResultats));
        }

        private static bool IsRelevant(PageResultatViewModel viewModel)
        {
            //On n'affiche pas la page si au moins une colonne dite normale (autre que celles affichant les années ou les âges) n'est pas présente.
            return viewModel.Tableaux?.Any(t => t.GroupeColonnes?.Any(x => x.Colonnes.Any(y => y.TypeColonne == TypeColonne.Normale)) ?? false) ?? false;
        }

        private void BuildSubparts(IPageResultat report, IReportContext reportContext, SectionResultatModel[] models)
        {
            var premierePage = true;
            foreach (var model in models)
            {
                var viewModelTableau = new PageResultatViewModel();
                _resultatMapper.Map(model, viewModelTableau, reportContext);

                if (IsRelevant(viewModelTableau))
                {
                    if (!premierePage)
                    {
                        report.AddSubReport(_reportFactory.Create<IPageBreakSubReport>());
                    }
                    premierePage = false;

                    foreach (var tableau in viewModelTableau.Tableaux)
                    {
                        _tableauResultatBuilder.Build(new BuildParameters<TableauResultatViewModel>(tableau)
                        {
                            ReportContext = reportContext,
                            ParentReport = report
                        });
                    }
                }
            }
        }
    }
}