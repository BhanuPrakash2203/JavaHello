using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Types;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Services
{
    public interface IIllustrationsExportService
    {
        // ReSharper disable once InconsistentNaming
        PdfDocument ExportToPDF(DonneesIllustration illustration);
        // ReSharper disable once InconsistentNaming
        PdfDocument ExportToPDF(DonneesIllustration illustration, ExportOptions options);
        IList<PageRapport> ObtenirInformationPagesRapport(DonneesIllustration illustration);
        IList<PageRapport> ObtenirInformationPagesRapport(DonneesIllustration illustration, ExportOptions options);
        IList<PageRapport> ObtenirInformationPagesRapport(DonneesRapport donneesRapport);
    }
}