﻿using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.ConditionsMedicales
{
    public class SectionConditionMedicaleBuilder : ISectionConditionMedicaleBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionConditionMedicaleBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<ConditionMedicaleViewModel> parameters)
        {
            var param = new BuildParameters<ConditionMedicaleViewModel>(parameters.Data)
            {
                ReportContext = parameters.ReportContext,
                ParentReport = parameters.ParentReport,
                StyleOverride = parameters.StyleOverride
            };
            var report = _reportFactory.Create<ISectionConditionMedicale>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param); 
        }
    }
}