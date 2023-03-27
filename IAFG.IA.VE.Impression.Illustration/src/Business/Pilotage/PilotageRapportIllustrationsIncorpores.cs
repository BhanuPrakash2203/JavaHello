using System.IO;
using System.Reflection;

namespace IAFG.IA.VE.Impression.Illustration.Business.Pilotage
{
    public class PilotageRapportIllustrationsIncorpores : PilotageRapportIllustrationsBase
    {
        private readonly Assembly _assembly;

        public PilotageRapportIllustrationsIncorpores()
        {
            var t = typeof(PilotageRapportIllustrationsIncorpores);
            _assembly = Assembly.GetAssembly(t);
            Initialize(t.Namespace + ".ConfigurationRapport");
        }

        protected override string LireFichier(string path, string filename)
        {
            var resourceName = path + "." + filename.Replace("\\", ".");
            using (var stream = _assembly.GetManifestResourceStream(resourceName))
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                        return result;
                    }
                }

            throw new FileNotFoundException($"La ressource {resourceName} est introuvable.", resourceName);
        }
    }
}