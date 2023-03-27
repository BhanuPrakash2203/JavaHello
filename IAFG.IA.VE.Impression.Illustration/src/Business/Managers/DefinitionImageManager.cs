using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionImageManager : IDefinitionImageManager
    {
        public Dictionary<string, ImageModel> Mapper(Dictionary<string, List<DefinitionImageSelonProduit>> images, DonneesRapportIllustration donnees)
        {
            var result = new Dictionary<string, ImageModel>();
            if (images == null) return result;
            foreach (var item in images)
            {
                var value = item.Value?.FirstOrDefault(x => x.Produit == donnees.Produit) 
                            ?? item.Value?.FirstOrDefault(x => x.Produit == Produit.NonDefini);
              
                if (value != null)
                {
                    var image = new ImageModel
                    {
                        ImageId = value.Image.ImageId,
                        Parametres = value.Image.Parametres,
                        Height = value.Image.Height,
                        Width = value.Image.Width,
                    };

                    result.Add(item.Key, image);
                }
            }

            return result;
        }
    }
}