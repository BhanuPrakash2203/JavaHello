using System.Drawing;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor
{
    public interface IEmbeddedResourcesReader
    {
        string GetString(string resourceId);
        Image GetImage(string resourceId);
        byte[] GetReport(ReportType reportType);
    }
}