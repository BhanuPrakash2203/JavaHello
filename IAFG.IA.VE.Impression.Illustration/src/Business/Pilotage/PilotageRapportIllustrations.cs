using System.IO;
using System.Text;

namespace IAFG.IA.VE.Impression.Illustration.Business.Pilotage
{
    public class PilotageRapportIllustrations : PilotageRapportIllustrationsBase
    {
        public PilotageRapportIllustrations(string path)
        {
            Initialize(path);
        }

        protected override string LireFichier(string path, string filename)
        {
            var pathFile = Path.Combine(path, filename);
            if (!File.Exists(pathFile)) throw new FileNotFoundException($"Le fichier {filename} est introuvable.", pathFile);
            return File.ReadAllText(pathFile, Encoding.GetEncoding("UTF-8"));
        }
    }
}