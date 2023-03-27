using IAFG.IA.VE.Impression.Illustration.Business.Constants;

namespace IAFG.IA.VE.Impression.Illustration.Business.Helper
{
    public static class CodePlanCategorie
    {
        public static bool EstAccesVieGarantie(string codePlan)
        {
            return codePlan.Substring(ConstanteAccesVie.PositionCaractereEtape, ConstanteAccesVie.LongueurCaractereEtape) == ConstanteAccesVie.AccesGarantie;
        }

        public static bool EstAccesVieDiffere(string codePlan)
        {
            return codePlan.Substring(ConstanteAccesVie.PositionCaractereEtape, ConstanteAccesVie.LongueurCaractereEtape) == ConstanteAccesVie.Differe;
        }

        public static bool EstAccesVieDifferePlus(string codePlan)
        {
            return codePlan.Substring(ConstanteAccesVie.PositionCaractereEtape, ConstanteAccesVie.LongueurCaractereEtape) == ConstanteAccesVie.DifferePlus;
        }

        public static bool EstAccesVieImmediatPLus(string codePlan)
        {
            return codePlan.Substring(ConstanteAccesVie.PositionCaractereEtape, ConstanteAccesVie.LongueurCaractereEtape) == ConstanteAccesVie.ImmediatPlus;
        }
    }
}