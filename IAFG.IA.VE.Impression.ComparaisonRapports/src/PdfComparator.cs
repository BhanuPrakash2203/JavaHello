using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IAFG.IA.VE.Impression.ComparaisonRapports.Data;

namespace IAFG.IA.VE.Impression.ComparaisonRapports
{
    public class PdfComparator
    {
        private const string PDF_FILE_EXTENSION = "*.pdf";
        public static int GetTotalFilesCount(string folder1)
        {
           return Directory.GetFiles(folder1, PDF_FILE_EXTENSION, SearchOption.AllDirectories).Length;
        }

        public ResultData Compare(string folder1, string folder2, string outputFolder, IProgress<int> progres)
        {
            var filesFolder1 = Directory.GetFiles(folder1, PDF_FILE_EXTENSION, SearchOption.AllDirectories);
            return CompareFiles(filesFolder1.ToList(), folder2, outputFolder, progres);
        }

        private static ResultData CompareFiles(IReadOnlyCollection<string> files, string folder, string outputFolder, IProgress<int> progress)
        {
            var count = files.Count / 4;
            var l1 = files.Take(count);
            var l2 = files.Skip(count).Take(count);
            var l3 = files.Skip(count * 2).Take(count);
            var l4 = files.Skip(count * 3);

            var filesInfo = new List<FileInfo>();
            var task1 = Task.Factory.StartNew(() => filesInfo.AddRange(new ComparatorThread().CompareFiles(l1, folder, outputFolder, progress)));
            var task2 = Task.Factory.StartNew(() => filesInfo.AddRange(new ComparatorThread().CompareFiles(l2, folder, outputFolder, progress)));
            var task3 = Task.Factory.StartNew(() => filesInfo.AddRange(new ComparatorThread().CompareFiles(l3, folder, outputFolder, progress)));
            var task4 = Task.Factory.StartNew(() => filesInfo.AddRange(new ComparatorThread().CompareFiles(l4, folder, outputFolder, progress)));
            Task.WaitAll(task1, task2, task3, task4);

            var result = new ResultData
            {
                Files = filesInfo
            };

            var status = new List<string>{ $"{result.FilesCount - result.FilesIdentical}", $"Files count : {result.FilesCount}" };
            status.AddRange(filesInfo.GroupBy(x => x.Status).Select(x => $"{x.Key} : {x.Count()}"));
            status.Add($"Total : {result.FilesCount}");
            status.Add($"-------------- WithDifferences : {result.FilesWithDifferences}");
            status.AddRange(filesInfo.Where(x => x.Status == FileStatus.IsDifferent).Select(x => x.FileName).OrderBy(x => x));
            status.Add($"-------------- NotFoundFiles : {result.FilesNotFound}");
            status.AddRange(filesInfo.Where(x => x.Status == FileStatus.NotFound).Select(x => x.FileName).OrderBy(x => x));
            status.Add($"-------------- ErrorFiles : {result.FilesWithError}");
            status.AddRange(filesInfo.Where(x => x.Status == FileStatus.Error).Select(x => x.FileName).OrderBy(x => x));

            File.WriteAllLines(Path.Combine(outputFolder, "__StatusFiles.txt"), status);

            return result;
        }
    }

    public class ComparatorThread
    {
        private const string ERROR_REPORT_FILE_NAME = "_ERROR_ReportCompare_{0}.txt";

        public IEnumerable<FileInfo> CompareFiles(IEnumerable<string> files, string folder, string outputFolder, IProgress<int> progress)
        {
            var filesInfo = new List<FileInfo>();
            foreach (var currentFile in files)
            {
                var fileInfo = new FileInfo { FileName = Path.GetFileName(currentFile) };
                filesInfo.Add(fileInfo);

                var filename = Path.GetFileName(currentFile);
                var fileToCompareWith = Path.Combine(folder, filename);
                if (!File.Exists(fileToCompareWith))
                {
                    fileInfo.Status = FileStatus.NotFound;
                    continue;
                }

                try
                {
                    var result = SynchfusionComparator.CompareFiles(currentFile, fileToCompareWith, outputFolder);
                    fileInfo.Status = result ? FileStatus.Done : FileStatus.IsDifferent;
                }
                catch (Exception ex)
                {
                    fileInfo.Status = FileStatus.Error;
                    var filenameErr = Path.Combine(outputFolder,
                        string.Format(ERROR_REPORT_FILE_NAME, Path.GetFileNameWithoutExtension(currentFile)));
                    File.WriteAllLines(filenameErr, new[] { ex.Message, ex.StackTrace });
                }

                progress?.Report(1);
            }

            return filesInfo;
        }
    }
}