using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Data
{
    public class ResultData
    {
        public int FilesCount => Files?.Count() ?? 0;
        public int FilesIdentical => Files?.Count(x => x.Status == FileStatus.Done) ?? 0;
        public int FilesWithDifferences => Files?.Count(x => x.Status == FileStatus.IsDifferent) ?? 0;
        public int FilesWithError => Files?.Count(x => x.Status == FileStatus.Error) ?? 0;
        public int FilesNotFound => Files?.Count(x => x.Status == FileStatus.NotFound) ?? 0;
        public IEnumerable<FileInfo> Files { get; set; }
    }
}