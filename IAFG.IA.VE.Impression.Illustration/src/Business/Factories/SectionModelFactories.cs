using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{
    public class SectionModelFactories: ISectionModelFactories
    {
        public SectionModelFactories(ISectionModelFactory sectionModelFactory, 
            IProjectionModelFactory projectionModelFactory,
            IProjectionParAssureModelFactory projectionParAssureModelFactory,
            ITestSensibiliteModelFactory testSensibiliteModelFactory,
            IHypothesesInvestissementModelFactory hypothesesInvestissementModelFactory,
            ISommaireProtectionsModelFactory sommaireProtectionsModelFactory,
            IPrimesRenouvellementModelFactory primesRenouvellementModelFactory,
            IGlossaireModelFactory glossaireModelFactory,
            ISignatureModelFactory signatureModelFactory,
            IDescriptionsProtectionsModelFactory descriptionsProtectionsModelFactory,
            ISommaireProtectionsIllustrationModelFactory sommaireProtectionsIllustrationModelFactory,
            IApercuProtectionsModelFactory apercuProtectionsModelFactory,
            IConditionsMedicalesModelFactory conditionsMedicalesModelFactory,
            INotesIllustrationModelFactory notesIllustrationModelFactory,
            IConceptVenteModelFactory conceptVenteModelFactory,
            IModificationsDemandeesModelFactory modificationsDemandeesModelFactory                                     )
        {
            SectionModelFactory = sectionModelFactory;
            GlossaireModelFactory = glossaireModelFactory;
            HypothesesInvestissementModelFactory = hypothesesInvestissementModelFactory;
            PrimesRenouvellementModelFactory = primesRenouvellementModelFactory;
            ProjectionModelFactory = projectionModelFactory;
            ProjectionParAssureModelFactory = projectionParAssureModelFactory;
            SignatureModelFactory = signatureModelFactory;
            DescriptionsProtectionsModelFactory = descriptionsProtectionsModelFactory;
            SommaireProtectionsIllustrationModelFactory = sommaireProtectionsIllustrationModelFactory;
            SommaireProtectionsModelFactory = sommaireProtectionsModelFactory;
            TestSensibiliteModelFactory = testSensibiliteModelFactory;
            ApercuProtectionsModelFactory = apercuProtectionsModelFactory;
            ConditionsMedicalesModelFactory = conditionsMedicalesModelFactory;
            NotesIllustrationModelFactory = notesIllustrationModelFactory;
            ConceptVenteModelFactory = conceptVenteModelFactory;
            ModificationsDemandeesModelFactory = modificationsDemandeesModelFactory;
        }

        public ISectionModelFactory SectionModelFactory { get; }
        public IGlossaireModelFactory GlossaireModelFactory { get; }
        public IHypothesesInvestissementModelFactory HypothesesInvestissementModelFactory { get; }
        public IProjectionModelFactory ProjectionModelFactory { get; }
        public IProjectionParAssureModelFactory ProjectionParAssureModelFactory { get; }
        public IPrimesRenouvellementModelFactory PrimesRenouvellementModelFactory { get; }
        public ISignatureModelFactory SignatureModelFactory { get; }
        public ISommaireProtectionsModelFactory SommaireProtectionsModelFactory { get; }
        public ITestSensibiliteModelFactory TestSensibiliteModelFactory { get; }
        public IDescriptionsProtectionsModelFactory DescriptionsProtectionsModelFactory { get; }
        public ISommaireProtectionsIllustrationModelFactory SommaireProtectionsIllustrationModelFactory { get; }
        public IApercuProtectionsModelFactory ApercuProtectionsModelFactory { get; }
        public INotesIllustrationModelFactory NotesIllustrationModelFactory { get; }
        public IConditionsMedicalesModelFactory ConditionsMedicalesModelFactory { get; }
        public IConceptVenteModelFactory ConceptVenteModelFactory { get; }
        public IModificationsDemandeesModelFactory ModificationsDemandeesModelFactory { get; }
    }
}