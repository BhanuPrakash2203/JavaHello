using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.BonSuccessoral
{
    public class BonSuccessoralFactories: IBonSuccessoralFactories
    {
        public BonSuccessoralFactories(
            ITitreRapportModelFactory pageTitreModelFactory, 
            IPageGraphiqueModelFactory pageGraphiqueModelFactory,
            ISommaireBonSuccessoralModelFactory pageSommaireBonSuccessoralModelFactory)
        {
            TitreRapportModelFactory = pageTitreModelFactory;
            PageGraphiqueModelFactory = pageGraphiqueModelFactory;
            SommaireBonSuccessoralModelFactory = pageSommaireBonSuccessoralModelFactory;
        }

        public ITitreRapportModelFactory TitreRapportModelFactory { get; }

        public IPageGraphiqueModelFactory PageGraphiqueModelFactory { get; }

        public ISommaireBonSuccessoralModelFactory SommaireBonSuccessoralModelFactory { get; }
    }
}