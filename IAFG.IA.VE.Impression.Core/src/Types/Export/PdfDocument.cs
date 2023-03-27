using IAFG.IA.VE.Impression.Core.Types.Enums;

namespace IAFG.IA.VE.Impression.Core.Types.Export
{
    public class PdfDocument
    {
        public byte[] Content { get; set; }
        public long ContentLength 
        {
            get { return Content.LongLength; }
        }

        public IsoPdfVersion PdfVersion { get; set; }

        public readonly string ContentExtension = ".pdf";
    }
}
