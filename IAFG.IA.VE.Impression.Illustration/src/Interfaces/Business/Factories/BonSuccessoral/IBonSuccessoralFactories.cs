namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.BonSuccessoral
{
    public interface IBonSuccessoralFactories
    {
        ITitreRapportModelFactory TitreRapportModelFactory { get; }
        IPageGraphiqueModelFactory PageGraphiqueModelFactory { get; }
        ISommaireBonSuccessoralModelFactory SommaireBonSuccessoralModelFactory { get; }
    }
}