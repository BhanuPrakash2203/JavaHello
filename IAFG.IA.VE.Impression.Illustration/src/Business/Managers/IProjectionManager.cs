using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using ProjectionData = IAFG.IA.VI.Projection.Data;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IProjectionManager
    {
        ProjectionData.Projection GetDefaultProjection(ProjectionData.Projections projections);
        ProjectionData.Projection GetEstateBondProjection(ProjectionData.Projections projections);
        ProjectionData.Contract.Coverage.Coverage GetMainCoverage(ProjectionData.Projection projection);
        IGetPDFICoverageResponse GetPdfCoverage(ProjectionData.Projection projection, ProjectionData.Contract.Coverage.Coverage coverage);
        bool DeterminerPresenceCompteTerme(ProjectionData.Projection projection);
        bool DeterminerPresencePrimeRenouvellable(ProjectionData.Projection projection);
        bool DeterminerTabagismePreferentiel(ProjectionData.Projection projection);
        Etat EtatContrat(ProjectionData.Projection projection);
        Banniere Banniere(ProjectionData.Projection projection);
        ProvinceEtat ProvinceEtat(ProjectionData.Projection projection);
        bool ContratEstConjoint(ProjectionData.Projection projection);
        bool ContratEstEnDecheance(ProjectionData.Projection projection);
        bool ContractantEstCompagnie(ProjectionData.Projection projection);
    }
}
