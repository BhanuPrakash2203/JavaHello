using System.Collections.Generic;
using System.Threading.Tasks;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Services;
using IAFG.IA.VE.Impression.Illustration.Types;

namespace IAFG.IA.VE.Impression.Illustration.Services
{
    public static class IllustrationsExportServiceExtension
    {
        public static async Task<IList<PageRapport>> TaskObtenirInformationPages(this IIllustrationsExportService service,
            DonneesIllustration illustration)
        {
            return await Task.Run(() =>
            {
                return service.ObtenirInformationPagesRapport(illustration);
            });
        }

        public static async Task<IList<PageRapport>> TaskObtenirInformationPages(this IIllustrationsExportService service,
            DonneesIllustration illustration, ExportOptions options)
        {
            return await Task.Run(() =>
            {
                return service.ObtenirInformationPagesRapport(illustration, options);
            });
        }

        public static async Task<PdfDocument> TaskExportToDocument(this IIllustrationsExportService service,
            DonneesIllustration illustration)
        {
            return await Task.Run(() =>
            {
                return service.ExportToPDF(illustration);
            });
        }

        public static async Task<PdfDocument> TaskExportToDocument(this IIllustrationsExportService service,
            DonneesIllustration illustration, ExportOptions options)
        {
            return await Task.Run(() =>
            {
                return service.ExportToPDF(illustration, options);
            });
        }
    }
}