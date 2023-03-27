namespace IAFG.IA.VE.Impression.Core.Types.Export
{
    public class ApplicationPdfDocument : PdfDocument 
    {
        /// <summary>
        /// Contains the policy number specified on report generation or ""
        /// </summary>
        public string PropositionReferenceId { get; set; }
    }
}
