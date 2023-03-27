using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Builders.Base;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;


namespace IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder
{
    public class RelevantBuilder<TBuilder, TBuildData> : IRelevantBuilder where TBuilder : IReportBuilder<TBuildData>
    {
        private readonly TBuilder _builder;
        private readonly TBuildData _data;
        private readonly IReportContext _context;
        private readonly IStyleOverride _style;

        public RelevantBuilder(TBuilder builder, TBuildData data, IReportContext context)
        {
            _builder = builder;
            _data = data;
            _context = context;
            _style = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };
        }

        public void Build(IReport parentReport)
        {
            _builder.Build(new BuildParameters<TBuildData>(_data)
            {
                ReportContext = _context,
                ParentReport = parentReport,
                StyleOverride = _style
            });
        }
    }

}