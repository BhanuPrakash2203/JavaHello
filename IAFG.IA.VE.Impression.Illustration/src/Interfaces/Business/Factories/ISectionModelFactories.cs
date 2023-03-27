namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories
{
    public interface ISectionModelFactories
    {
        ISectionModelFactory SectionModelFactory { get; }
        IGlossaireModelFactory GlossaireModelFactory { get; }
        IHypothesesInvestissementModelFactory HypothesesInvestissementModelFactory { get; }
        IPrimesRenouvellementModelFactory PrimesRenouvellementModelFactory { get; }
        IProjectionModelFactory ProjectionModelFactory { get; }
        IProjectionParAssureModelFactory ProjectionParAssureModelFactory { get; }
        ISignatureModelFactory SignatureModelFactory { get; }
        ISommaireProtectionsModelFactory SommaireProtectionsModelFactory { get; }
        ITestSensibiliteModelFactory TestSensibiliteModelFactory { get; }
        IDescriptionsProtectionsModelFactory DescriptionsProtectionsModelFactory { get; }
        ISommaireProtectionsIllustrationModelFactory SommaireProtectionsIllustrationModelFactory { get; }
        IApercuProtectionsModelFactory ApercuProtectionsModelFactory { get; }
        INotesIllustrationModelFactory NotesIllustrationModelFactory { get; }
        IConditionsMedicalesModelFactory ConditionsMedicalesModelFactory { get; }
        IConceptVenteModelFactory ConceptVenteModelFactory { get; }
        IModificationsDemandeesModelFactory ModificationsDemandeesModelFactory { get; }
    }
}