using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Traces
{
    public static class TracesExtensions
    {
        public static bool HasNoTraces(this Traces traces)
        {
            return !traces.Document1.Pages.Any() &&
                   !traces.Document1.Unmatched.Any() &&
                   !traces.Document2.Pages.Any() &&
                   !traces.Document2.Unmatched.Any();
        }

        public static Traces ReadTraces(string fileName, string path)
        {
            var fullName = Path.Combine(path, fileName);
            var filedata = File.ReadAllText(fullName, Encoding.GetEncoding("UTF-8"));
            return JsonConvert.DeserializeObject<Traces>(filedata,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Converters = new JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
                });

        }
    }
}