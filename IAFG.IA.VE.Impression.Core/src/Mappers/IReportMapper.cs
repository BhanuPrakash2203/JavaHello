

using IAFG.IA.VE.Impression.Core.Interface.ReportContext;

namespace IAFG.IA.VE.Impression.Core.Mappers
{
    public interface IReportMapperWithContext<in TModel, in TViewModel>
    {
        void Map(TModel model, TViewModel viewModel, IReportContext context);
    }
}