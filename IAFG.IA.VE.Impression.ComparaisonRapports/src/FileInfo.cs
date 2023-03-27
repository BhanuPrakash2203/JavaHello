namespace IAFG.IA.VE.Impression.ComparaisonRapports
{
    public class FileInfo
    {
        public string FileName { get; set; }
        public FileStatus Status { get; set; }
    }

    public enum FileStatus
    {
        None = 0,
        Done = 1,
        NotFound = 2,
        IsDifferent = 3,
        Error = 4
    }
}