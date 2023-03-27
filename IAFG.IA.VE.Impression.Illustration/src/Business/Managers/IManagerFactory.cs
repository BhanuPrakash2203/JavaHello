using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IManagerFactory
    {
        ITableauResultatManager GetTableauResultatManager();
        IModelMapper GetModelMapper();
    }
}
