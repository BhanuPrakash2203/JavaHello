using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface ITableauResultatManager
    {
        List<TableauResultatViewModel> MapperTableaux(TableauResultat source, SectionResultatModel model);
        List<TableauResultatViewModel> MapperTableaux(SectionResultatParAssureModel model);
    }
}
