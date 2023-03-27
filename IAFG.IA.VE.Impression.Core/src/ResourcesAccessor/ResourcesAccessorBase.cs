using System.Drawing;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;

namespace IAFG.IA.VE.Impression.Core.ResourcesAccessor
{
    public abstract class ResourcesAccessorBase
    {
        private readonly IEmbeddedResourcesSequence _embeddedResourcesSequence;

        protected ResourcesAccessorBase(IEmbeddedResourcesSequence embeddedResourcesSequence)
        {
            _embeddedResourcesSequence = embeddedResourcesSequence;
        }

        public string GetStringResourceById(string id)
        {
            foreach (var embeddedRessourcesReader in _embeddedResourcesSequence.GetReaders())
            {
                var result = embeddedRessourcesReader.GetString(id);
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return string.Empty;
        }

        public Image GetImageResourceById(string id)
        {
            foreach (var embeddedRessourcesReader in _embeddedResourcesSequence.GetReaders())
            {
                var result = embeddedRessourcesReader.GetImage(id);
                if (result != null)
                    return result;
            }

            return null;
        }

        public byte[] GetReportResourceById(ReportType reportType)
        {
            foreach (var embeddedRessourcesReader in _embeddedResourcesSequence.GetReaders())
            {
                var result = embeddedRessourcesReader.GetReport(reportType);
                if (result != null)
                    return result;
            }

            return null;
        }

        public void ResetCulture()
        {
            foreach (var embeddedRessourcesReader in _embeddedResourcesSequence.GetReaders())
            {
                var fileBasedReader = embeddedRessourcesReader as IFileBasedEmbeddedResourcesReader;
                fileBasedReader?.ResetCulture();
            }
        }
    }

    public enum ResourceFile
    {
        IllustrationsReportLabel,
        IllustrationsReportImage,
        ReportLabel,
        ReportImage,
        FormatterLabels
    }
}