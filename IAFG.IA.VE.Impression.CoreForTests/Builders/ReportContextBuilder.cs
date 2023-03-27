using AutoFixture;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Export;

namespace IAFG.IA.VE.Impression.CoreForTests.Builders
{
    public static class ReportContextBuilder
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();

        public static IReportContext Create()
        {
            var context = _auto.Create<IReportContext>();
            context.ReportAudience = ReportAudienceTypes.FullClearance;
            return context;
        }
    }
}