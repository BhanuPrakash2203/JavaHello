using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class ModelMapper : IModelMapper
    {
        public IList<string> MapperNotes(IList<DetailNote> notes)
        {
            return notes?.OrderBy(n => n.SequenceId).ThenBy(n => n.NumeroReference.GetValueOrDefault())
                .Select(FormatterTexteNote).ToList();
        }

        public IList<string> MapperNotes(IList<DetailNote> notes, bool enEnteteSection)
        {
            return notes?.Where(x => x.EnEnteteDeSection == enEnteteSection).OrderBy(n => n.SequenceId)
                .ThenBy(n => n.NumeroReference.GetValueOrDefault()).Select(FormatterTexteNote).ToList();
        }

        private static string FormatterTexteNote(DetailNote note)
        {
            if (note == null)
            {
                return string.Empty;
            }

            return note.NumeroReference.HasValue ? $"({note.NumeroReference.Value}) {note.Texte}" : note.Texte;
        }

        public Dictionary<string, ImageViewModel> MapperImages(Dictionary<string, ImageModel> images)
        {
            var result = new Dictionary<string, ImageViewModel>();
            if (images == null) return result;
            foreach (var item in images)
            {
                var image = new ImageViewModel
                {
                    PrefixNom = item.Key,
                    ImageId = item.Value.ImageId,
                    Parametres = item.Value.Parametres
                };

                if (item.Value.Height.HasValue || item.Value.Width.HasValue)
                {
                    image.Size = new System.Drawing.SizeF(item.Value.Width.GetValueOrDefault(), item.Value.Height.GetValueOrDefault());
                }

                GererProprietePageBreak(image);
                result.Add(item.Key, image);
            }
            return result;
        }

        private static void GererProprietePageBreak(ImageViewModel image)
        {
            if (image?.Parametres == null) return;
            if (image.Parametres.ContainsKey("PageBreak"))
            {
                var prop = image.Parametres["PageBreak"];
                image.PageBreakAvant = !string.IsNullOrEmpty(prop) && prop.ToUpper().Contains("AVANT");
                image.PageBreakApres = string.IsNullOrEmpty(prop) || prop.ToUpper().Contains("APRES");
            }
        }
    }
}