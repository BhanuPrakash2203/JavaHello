using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders
{
    public interface IReportBuilder
    {

    }

    public interface IReportBuilder<in TSource, out TTarget> : IReportBuilder where TTarget : IEnumerable<IReport>
    {
        TTarget Build(TSource sourceObject, IReportContext reportContext);
    }
}