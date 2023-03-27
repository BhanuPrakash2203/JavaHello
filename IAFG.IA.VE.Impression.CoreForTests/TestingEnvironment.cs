using System.IO;

namespace IAFG.IA.VE.Impression.CoreForTests
{
    public static class TestingEnvironment
    {
        public static string CurrentDirectory
        {
            get { return Path.GetDirectoryName(typeof(TestingEnvironment).Assembly.Location); }
        }

        public static string PathFromCurrentDirectory(string path)
        {
            return Path.Combine(CurrentDirectory, path);
        }
    }
}