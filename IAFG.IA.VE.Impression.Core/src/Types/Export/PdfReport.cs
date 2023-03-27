using System;

namespace IAFG.IA.VE.Impression.Core.Types.Export
{
    public class PdfReport
    {
        public Guid IndividualId { get; set; }
        public ApplicationPdfDocument Pdf { get; set; }
    }
}