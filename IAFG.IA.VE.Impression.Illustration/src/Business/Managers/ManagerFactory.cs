using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class ManagerFactory : IManagerFactory
    {
        private readonly IModelMapper _modelMapper;
        private readonly ITableauResultatManager _tableauResultatManager;

        public ManagerFactory(IModelMapper modelMapper, ITableauResultatManager tableauResultatManager)
        {
            _modelMapper = modelMapper;
            _tableauResultatManager = tableauResultatManager;
        }

        public ITableauResultatManager GetTableauResultatManager()
        {
            return _tableauResultatManager;
        }

        public IModelMapper GetModelMapper()
        {
            return _modelMapper;
        }
    }

}
