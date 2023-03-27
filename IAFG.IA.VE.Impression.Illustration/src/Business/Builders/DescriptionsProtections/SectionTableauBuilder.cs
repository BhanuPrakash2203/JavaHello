﻿using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.DescriptionsProtections
{
    public class SectionTableauBuilder : ISectionTableauBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionTableauBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }
        public void Build(BuildParameters<DescriptionViewModel> parameters)
        {
            var param = new BuildParameters<DescriptionViewModel>(parameters.Data)
                        {
                            ReportContext = parameters.ReportContext,
                            ParentReport = parameters.ParentReport,
                            StyleOverride = parameters.StyleOverride
                        };
            var report = _reportFactory.Create<ISectionTableau>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param);
        }
    }
}