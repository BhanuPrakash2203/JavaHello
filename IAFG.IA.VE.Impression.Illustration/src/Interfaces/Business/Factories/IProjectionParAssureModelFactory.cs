using System.Collections.Generic;
using IAFG.IA.VE.Impression.Core.Interface.Factories;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories
{
    public interface IProjectionParAssureModelFactory : IFactoryBase<SectionResultatParAssureModel>
    {
        SectionResultatParAssureModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context);
        ChoixAnneesRapport DeterminerAnneesProjection(DonneesRapportIllustration donnees, TypeChoixAnneesRapport? choixAnnees);
        IList<ProtectionsGroupees> ObtenirProtectionsGroupees(DonneesRapportIllustration donnees, TypeTableau typeTableau);
    }
}