using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;

namespace IAFG.IA.VE.Impression.Core.Builders
{
    public class BuildParameters<TBuildData>
    {
        public BuildParameters(TBuildData data)
        {
            Data = data;
        }

        public TBuildData Data { get; private set; }
        public IReport ParentReport { get; set; }
        public IReportContext ReportContext { get; set; }
        public IStyleOverride StyleOverride { get; set; }
    }
}