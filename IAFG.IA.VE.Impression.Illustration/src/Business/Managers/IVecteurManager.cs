using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public interface IVecteurManager
    {
        bool ColonnePresente(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement);

        double[] ObtenirVecteur(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection);

        double[] ObtenirVecteurOuDefaut(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection);

        double[] ObtenirVecteurPourGroupeAssure(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection, string identifiantGroupeAssure);

        double[] ObtenirVecteurOuDefautPourGroupeAssure(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection, string identifiantGroupeAssure);

        double[] ObtenirVecteurMontantNetAuRisque(Projections projections);

        int TrouverIndexPremiereValeurNonNull(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement);
        int TrouverIndexDerniereValeurNonNull(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement);
        int TrouverIndexSelonAge(Projection projection, int age);
        int TrouverIndexSelonAgeAssure(Projection projection, int age, string idAssure);
        int TrouverIndexSelonAnneeCalendrier(Projection projection, int annee);
        int TrouverAnneeSelonIndex(Projection projection, int index);
    }
}
