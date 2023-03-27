using System.Drawing;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor
{
    public interface IResourcesAccessor
    {
        string GetStringResourceById(string id);
        Image GetImageResourceById(string id);
        byte[] GetReportResourceById(ReportType reportType);
        void ResetCulture();
    }
}