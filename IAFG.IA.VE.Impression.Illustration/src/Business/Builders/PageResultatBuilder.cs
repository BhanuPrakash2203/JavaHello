﻿using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageResultatBuilder : IPageResultatBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageResultatMapper _mapper;
        private readonly ISectionTableauResultatBuilder _sectionTableauResultatBuilder;

        public PageResultatBuilder(IReportFactory reportFactory,
                                   IPageResultatMapper mapper,
                                   ISectionTableauResultatBuilder sectionTableauResultatBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionTableauResultatBuilder = sectionTableauResultatBuilder;
        }

        public void Build(BuildParameters<SectionResultatModel> parameters)
        {
            var report = _reportFactory.Create<IPageResultat>();       
            var viewModel = new PageResultatViewModel();
            if (parameters?.Data != null)
            {
                _mapper.Map(parameters.Data, viewModel, parameters.ReportContext);
            }

            if (IsRelevant(viewModel))
            {
                ReportBuilderAssembler.AssembleWithoutModelMapping(report, viewModel, parameters, vm => BuildSubparts(vm, report, parameters?.ReportContext));
            }
        }

        private static bool IsRelevant(PageResultatViewModel viewModel)
        {
            //On n'affiche pas la page si au moins une colonne dite normale (autre que celles affichant les années ou les âges) n'est pas présente.
            return viewModel.Tableaux?.Any(t => t.GroupeColonnes?.Any(x => x.Colonnes.Any(y => y.TypeColonne == TypeColonne.Normale)) ?? false) ?? false;
        }

        private void BuildSubparts(PageResultatViewModel viewModel, IReport report, IReportContext reportContext)
        {
            foreach (var tableau in viewModel.Tableaux)
            {
                _sectionTableauResultatBuilder.Build(new BuildParameters<TableauResultatViewModel>(tableau)
                                                     {
                                                         ReportContext = reportContext,
                                                         ParentReport = report
                                                     });
            }
        }
    }
}