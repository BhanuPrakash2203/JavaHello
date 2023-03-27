using System;
using IAFG.IA.VE.Impression.Core.Mappers;
using IAFG.IA.VE.Impression.Core.Types.Reports;

namespace IAFG.IA.VE.Impression.Core.Builders
{
    public interface IReportBuilderAssembler
    {
        TReport Assemble<TReport, TViewModel, TParameter>(TReport report,
            TViewModel viewModel,
            BuildParameters<TParameter> parameters,
            IReportMapperWithContext<TParameter, TViewModel> mapper,
            Action<TViewModel> buildSubparts = null) where TReport : IReportWithModel<TViewModel>;

        TReport AssembleWithoutData<TReport, TParameter>(TReport report,
            BuildParameters<TParameter> parameters,
            Action buildSubparts = null) where TReport : IReport;

        TReport AssembleWithoutModelMapping<TReport, TViewModel, TParameter>(TReport report,
            TViewModel viewModel,
            BuildParameters<TParameter> parameters,
            Action<TViewModel> buildSubparts = null) where TReport : IReportWithModel<TViewModel>;
    }
}