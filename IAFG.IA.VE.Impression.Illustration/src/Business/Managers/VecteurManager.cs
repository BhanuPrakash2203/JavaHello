using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class VecteurManager : IVecteurManager
    {
        public bool ColonnePresente(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement)
        {
            var vecteur = ObtenirVecteur(projections, colonne, typeProjection, typeRendement);
            return vecteur != null && vecteur.Any();
        }

        public double[] ObtenirVecteurOuDefaut(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendementProjection)
        {
            return ObtenirVecteur(projections, colonne, typeProjection, typeRendementProjection) ?? new double[] { };
        }

        public double[] ObtenirVecteur(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendementProjection)
        {
            if (typeProjection == TypeProjection.Normal)
            {
                switch (typeRendementProjection)
                {
                    case TypeRendementProjection.Normal:
                        return ObtenirVecteur(projections?.Projection, colonne);
                    case TypeRendementProjection.RendementDefavorable:
                        return ObtenirVecteur(projections?.ProjectionDefavorable, colonne);
                    case TypeRendementProjection.RendementFavorable:
                        return ObtenirVecteur(projections?.ProjectionFavorable, colonne);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeRendementProjection));
                }
            }

            if (typeProjection == TypeProjection.BonSuccessoral)
            {
                return ObtenirVecteur(projections?.BonSuccessoral, colonne);
            }

            throw new ArgumentOutOfRangeException(nameof(typeProjection));
        }

        private double[] ObtenirVecteur(Projection projection, int colonne)
        {
            return ObtenirVecteur(projection?.Columns, colonne);
        }

        private double[] ObtenirVecteur(IEnumerable<Column> vecteurColumns, int colonne)
        {
            return vecteurColumns?.FirstOrDefault(
                x => x.Id == colonne &&
                string.IsNullOrWhiteSpace(x.Coverage) &&
                string.IsNullOrWhiteSpace(x.Individual) &&
                string.IsNullOrWhiteSpace(x.Insured))?.Value;
        }

        public double[] ObtenirVecteurOuDefautPourGroupeAssure(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection, string identifiantGroupeAssure)
        {
            return ObtenirVecteurPourGroupeAssure(projections, colonne, typeProjection, typeRendementProjection, identifiantGroupeAssure) ?? new double[] { };
        }

        public double[] ObtenirVecteurPourGroupeAssure(Projections projections, int colonne,
            TypeProjection typeProjection, TypeRendementProjection typeRendementProjection, string identifiantGroupeAssure)
        {
            if (typeProjection == TypeProjection.Normal)
            {
                switch (typeRendementProjection)
                {
                    case TypeRendementProjection.Normal:
                        return ObtenirVecteurPourGroupeAssure(projections?.Projection, colonne, identifiantGroupeAssure);
                    case TypeRendementProjection.RendementDefavorable:
                        return ObtenirVecteurPourGroupeAssure(projections?.ProjectionDefavorable, colonne, identifiantGroupeAssure);
                    case TypeRendementProjection.RendementFavorable:
                        return ObtenirVecteurPourGroupeAssure(projections?.ProjectionFavorable, colonne, identifiantGroupeAssure);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeRendementProjection));
                }
            }

            if (typeProjection == TypeProjection.BonSuccessoral)
            {
                return ObtenirVecteurPourGroupeAssure(projections?.BonSuccessoral, colonne, identifiantGroupeAssure);
            }

            throw new ArgumentOutOfRangeException(nameof(typeProjection));
        }

        private double[] ObtenirVecteurPourGroupeAssure(Projection projection, int colonne,
            string identifiantGroupeAssure)
        {
            return ObtenirVecteurPourGroupeAssure(projection?.Columns, colonne, identifiantGroupeAssure) ?? new double[] { };
        }

        private double[] ObtenirVecteurPourGroupeAssure(IList<Column> vecteurColumns, int colonne, 
            string identifiantGroupeAssure)
        {
            var result = vecteurColumns?.FirstOrDefault(x =>
                    x.Id == colonne &&
                    !string.IsNullOrEmpty(x.Insured) &&
                    x.Insured.Equals(identifiantGroupeAssure))
                ?.Value;

            return result ?? ObtenirVecteur(vecteurColumns, colonne);
        }
        public double[] ObtenirVecteurMontantNetAuRisque(Projections projections)
        {
            return ObtenirVecteurOuDefaut(projections, (int)ProjectionVectorId.MaximumNetAmountAtRisk, TypeProjection.Normal, TypeRendementProjection.Normal);
        }

        public int TrouverIndexPremiereValeurNonNull(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement)
        {
            var valeur = ObtenirVecteurOuDefaut(projections, colonne, typeProjection, typeRendement).DefaultIfEmpty(-1).FirstOrDefault(v => v > 0);
            if (valeur <= 0) return -1;
            return Array.IndexOf(ObtenirVecteurOuDefaut(projections, colonne, typeProjection, typeRendement), valeur);
        }

        public int TrouverIndexDerniereValeurNonNull(Projections projections, int colonne, TypeProjection typeProjection, TypeRendementProjection typeRendement)
        {
            var valeur = ObtenirVecteurOuDefaut(projections, colonne, typeProjection, typeRendement).DefaultIfEmpty(-1).LastOrDefault(v => v > 0);
            if (valeur <= 0) return -1;
            return Array.LastIndexOf(ObtenirVecteurOuDefaut(projections, colonne, typeProjection, typeRendement), valeur);
        }

        public int TrouverIndexSelonAge(Projection projection, int age)
        {
            if (projection?.Ages == null) return 0;
            var index = Array.FindIndex(projection.Ages, x => x.Equals(age));
            return index >= 0 ? index : -1;
        }

        public int TrouverIndexSelonAgeAssure(Projection projection, int age, string idAssure)
        {
            var indexResult = 0;
            if (projection?.AgesAssures == null) return indexResult;
            if (!projection.AgesAssures.ContainsKey(idAssure)) return indexResult;
            var vecteur = projection.AgesAssures.FirstOrDefault(c => c.Key == idAssure).Value;
            if (vecteur == null) return indexResult;
            var index = Array.FindIndex(vecteur, x => x.Equals(age));
            if (index >= 0) indexResult = index;
            return indexResult;
        }

        public int TrouverIndexSelonAnneeCalendrier(Projection projection, int annee)
        {
            return projection?.AnneesCalendrier == null
                ? 0
                : Array.IndexOf(projection.AnneesCalendrier,
                    projection.AnneesCalendrier.FirstOrDefault(v => v == annee));
        }

        public int TrouverAnneeSelonIndex(Projection projection, int index)
        {
            if (index >= 0 && projection.AnneesContrat.Length > index)
            {
                return projection.AnneesContrat[index];
            }

            return 0;
        }
    }

}
