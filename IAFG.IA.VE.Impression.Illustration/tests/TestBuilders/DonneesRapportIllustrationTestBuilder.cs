using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;

namespace IAFG.IA.VE.Impression.Illustration.Test.TestBuilders
{
    public class DonneesRapportIllustrationTestBuilder
    {
        private Produit _produit;
        private TypeFrequenceFacturation _frequenceFacturation;
        private int _anneeDebutProjection;
        private bool _estProduitAvecPrimeReference;

        public DonneesRapportIllustrationTestBuilder WithProduit(Produit produit)
        {
            _produit = produit;
            return this;
        }

        public DonneesRapportIllustrationTestBuilder WithFrequenceFacturation(TypeFrequenceFacturation frequenceFacturation)
        {
            _frequenceFacturation = frequenceFacturation;
            return this;
        }

        public DonneesRapportIllustrationTestBuilder WithAnneeDebutProjection(int anneeDebutProjection)
        {
            _anneeDebutProjection = anneeDebutProjection;
            return this;
        }

        public DonneesRapportIllustrationTestBuilder WithEstProduitAvecPrimeReference(bool estProduitAvecPrimeReference)
        {
            _estProduitAvecPrimeReference = estProduitAvecPrimeReference;
            return this;
        }

        public DonneesRapportIllustration Build()
        {
            return new DonneesRapportIllustration
            {
                Produit = _produit,
                Facturation = new Facturation { FrequenceFacturation = _frequenceFacturation },
                Projections = new Projections { AnneeDebutProjection = _anneeDebutProjection },
                EstProduitAvecPrimeReference = _estProduitAvecPrimeReference
            };
        }
    }
}