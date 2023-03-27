using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    public static class DonneesRapportExtension
    {
        public static List<Plan> ObtenirPlanInfos(this DonneesRapportIllustration donnees)
        {
            var result = new List<Plan>();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var item in donnees.Protections.ProtectionsAssures)
            {
                if (result.Any(x => x.CodePlan == item.Plan.CodePlan)) continue;
                result.Add(item.Plan);
            }
            
            return result;
        }

        public static bool EstCapitalAssurePlusDe10Millions(this DonneesRapportIllustration donnees)
        {
            var capitalAssureParAssure = from protection in donnees.Protections.ProtectionsAssures
                                         from assure in protection.Assures
                                         select new Tuple<string, double>(assure.ReferenceExterneId, protection.CapitalAssureActuel);

            var capitalAssure = from item in capitalAssureParAssure
                                group item by item.Item1
                                into g
                                select g.Sum(_ => _.Item2);

            return capitalAssure.Any(montant => montant > 10000000);
        }

        public static int CalculerIndexAnneeSurbrillance(this DonneesRapportIllustration donnees, IVecteurManager vecteurManager)
        {
            var ageEsperanceVie = CalculerAgeEsperanceVie(donnees);
            return vecteurManager.TrouverIndexSelonAge(donnees.Projections.Projection, ageEsperanceVie);
        }

        public static int CalculerAgeEsperanceVie(this DonneesRapportIllustration donnees)
        {
            var protectionBase = donnees.Protections?.ProtectionsAssures?.FirstOrDefault(p => p.EstProtectionAssurePrincipal && p.EstProtectionBase);
            var assure = protectionBase?.Assures?.OrderByDescending(x => x.AgeAssurance).FirstOrDefault();
            if (donnees.TypeAssurance == TypeAssurance.ConjointeDernierDec || donnees.TypeAssurance == TypeAssurance.ConjointeDernierDecLib1er)
            {
                // on veut l'assure assurable le plus jeune
                assure = protectionBase?.Assures?.OrderBy(x => x.EstNonAssurable ? 1 : 0).ThenBy(x => x.AgeAssurance).FirstOrDefault();
            }

            if (assure?.AgeEsperanceVie != null) return assure.AgeEsperanceVie.Value;
            return -1;
        }

        public static int? CalculerAgeMaturiteProtection(this DonneesRapportIllustration donnees, IList<Assure> assures, DateTime? dateMaturite)
        {
            if (!dateMaturite.HasValue) return null; 

            if (donnees.TypeAssurance == TypeAssurance.Individuelle || donnees.TypeAssurance == TypeAssurance.Conjointe1erDec)
            {
                var ages = assures?.Select(x => x.CalculerAgeAssure(dateMaturite.Value)).OrderBy(x => x);
                return ages?.FirstOrDefault();
            }
            else
            {
                var ages = assures?.Select(x => x.CalculerAgeAssure(dateMaturite.Value)).OrderByDescending(x => x);
                return ages?.FirstOrDefault();
            }
        }

        public static int CalculerAgeAssure(this Assure assure, DateTime dateReference)
        {
            return assure.DateNaissance?.CalculerAge(dateReference) ?? assure.AgeAssurance;
        }

        public static string ObtenirIdentifiantGroupeAssurePrincipal(this DonneesRapportIllustration donnees)
        {
            return donnees.InformationTableauAssures.FirstOrDefault(x => x.EstPrincipal)?.IdAssure ?? string.Empty;
        }

        public static bool EstGroupeAssurePrincipal(this DonneesRapportIllustration donnees, string identifiantGroupeAssure)
        {
            return (identifiantGroupeAssure ?? string.Empty) == ObtenirIdentifiantGroupeAssurePrincipal(donnees);
        }

        public static bool HasPreferentialStatus(this DonneesRapportIllustration donnees)
        {
            if (donnees.Protections?.ProtectionsAssures == null || !donnees.Protections.ProtectionsAssures.Any())
            {
                return false;
            }

            return donnees.Protections.ProtectionsAssures.Any(donnees.HasPreferentialStatus);
        }

        public static bool HasPreferentialStatus(this DonneesRapportIllustration donnees, Protection protection)
        {
            return protection.TypeAssurance == TypeAssurance.Individuelle
                ? IsStatusPreferentiel(protection.StatutTabagisme)
                : protection.Assures.Any(assure => IsStatusPreferentiel(assure.StatutTabagisme));
        }

        // ReSharper disable once InconsistentNaming
        public static bool IsMarketValueAdjustmentApplied(this DonneesRapportIllustration donnees)
        {
            if (donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                string.Equals(f.Vehicule, v.Vehicle) &&
                string.Equals(f.AccountType, "Average5Years") &&
                string.Equals(f.SubType, "LisseAccountIris_Rule6"))))
            {
                return true;
            }

            if (donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                string.Equals(f.Vehicule, v.Vehicle) &&
                string.Equals(f.AccountType, "Average5Years") &&
                string.Equals(f.SubType, "SRIAAccount"))))
            {
                return true;
            }

            if (donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                string.Equals(f.Vehicule, v.Vehicle) &&
                string.Equals(f.AccountType, "Average5Years") &&
                string.Equals(f.SubType, "LisseAccount_Rule5"))))
            {
                return true;
            }

            return false;
        }

        private static bool IsStatusPreferentiel(StatutTabagisme statutTabagisme)
        {
            return new[]
            {
                StatutTabagisme.FumeurElite,
                StatutTabagisme.FumeurPrivilege,
                StatutTabagisme.NonFumeurElite,
                StatutTabagisme.NonFumeurPrivilege
            }.Contains(statutTabagisme);
        }
    }
}