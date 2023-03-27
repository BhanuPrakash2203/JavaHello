using System.Collections.Generic;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers
{
    public interface IModelMapper
    {
        IList<string> MapperNotes(IList<DetailNote> notes);
        IList<string> MapperNotes(IList<DetailNote> notes, bool enEnteteSection);

        Dictionary<string, ImageViewModel> MapperImages(Dictionary<string, ImageModel> images);

    }
}