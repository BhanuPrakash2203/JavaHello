using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VI.Projection.Data.Extensions;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnums = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.PrimesRenouvellement
{
    internal static class PrimesRenouvellementExtension
    {
        public static Primes MapperPrimesRenouvellement(this Primes primes,
                                                        ProjectionData.Contract.Insured insured,
                                                        ProjectionData.Projection projection,
                                                        IList<Client> clients,
                                                        IRegleAffaireAccessor regleAffaireAccessor)
        {
            var protections = new List<Protection>();
            primes.Protections = protections;
            protections.AddRange(MapperProtections(projection, insured, clients, regleAffaireAccessor));
            MapperPrimesRenouvellement(protections, projection);
            return primes;
        }

        internal static void MapperPrimesRenouvellement(IEnumerable<Protection> protections, ProjectionData.Projection projection)
        {
            if (projection == null) return;
            var idsColumnRenouvellememt = projection.Illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:GuaranteedRenewal" }).Select(c => c.Id).ToList();
            var idsColumnCapitalAssure = projection.Illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:CoveragePremium" }).Select(c => c.Id).ToList();

            //Toutes les colonnes utilisées doivent être par années de protections.
            var columnsRenouvellement = projection.Illustration.Columns.Where(c => idsColumnRenouvellememt.Contains(c.Id) && c.Coverage != null).ToList();
            var columnsCapitalAssure = projection.Illustration.Columns.Where(c => idsColumnCapitalAssure.Contains(c.Id) && c.Coverage != null).ToList();

            if (!columnsRenouvellement.Any() || !columnsCapitalAssure.Any())
            {
                return;
            }

            foreach (var protection in protections)
            {
                var capital = columnsCapitalAssure.FirstOrDefault(c => c.Coverage.Id == protection.Id)?.Value ?? new double[] { };
                var vGaranti = columnsRenouvellement.FirstOrDefault(c => c.Coverage.Id == protection.Id)?.Value ?? new double[] { };
                protection.CapitalAssure = DeterminerCapitalAssure(protection, capital, vGaranti);
                protection.Primes = CreerPrimesRenouvellement(vGaranti);
            }
        }

        private static IEnumerable<Protection> MapperProtections(ProjectionData.Projection projection,
                                                                  ProjectionData.Contract.Insured insured,
                                                                  IList<Client> clients,
                                                                  IRegleAffaireAccessor regleAffaireAccessor)
        {
            var result = new List<Protection>();
            if (projection == null || insured == null) return result;
            result.AddRange(MapperProtections(projection, insured.Coverages, clients, regleAffaireAccessor));           
            return result;
        }

        private static IEnumerable<Protection> MapperProtections(ProjectionData.Projection projection,
                                                                  IEnumerable<ProjectionData.Contract.Coverage.Coverage> coverages,
                                                                  IList<Client> clients,
                                                                  IRegleAffaireAccessor regleAffaireAccessor)
        {
            var result = new List<Protection>();
            if (coverages == null) return result;
            foreach (var coverage in coverages)
            {
                result.AddRange(MapperProtection(projection, coverage, clients, regleAffaireAccessor, false));
                result.AddRange(MapperProtections(projection, coverage.Coverages, clients, regleAffaireAccessor));
                result.AddRange(MapperGaranties(projection, coverage.AdditionalBenefits, clients, regleAffaireAccessor, false));
            }
            return result;
        }

        private static IEnumerable<Protection> MapperProtection(ProjectionData.Projection projection,
                                                                 ProjectionData.Contract.Coverage.Coverage coverage,
                                                                 IList<Client> clients,
                                                                 IRegleAffaireAccessor regleAffaireAccessor,
                                                                 bool estContractant)
        {
            var result = new List<Protection>();
            if (coverage == null) return result;

            var planInfo = regleAffaireAccessor.ObtenirPlan(coverage.PlanCode);

            var plan = new Plan
                       {
                           CodePlan = coverage.PlanCode,
                           DescriptionAn = planInfo.DescriptionAn,
                           DescriptionFr = planInfo.DescriptionFr,
                           InfoProtection = regleAffaireAccessor.DeterminerInfoProtection(projection, coverage)
                       };

            if (!EstRenouvelable(plan.InfoProtection)) return result;

            var typeAssurance = coverage.InsuranceType.ConvertirTypeAssurance();

            result.Add(new Protection
                       {
                           Id = coverage.Identifier.Id,
                           Plan = plan,
                           CapitalAssure = coverage.FaceAmount.Current,
                           TypeAssurance = typeAssurance,
                           PresenceSurprime = coverage.Insured?.ExtraPremiums?.Any() ?? false,
                           Assures = MapperAssures(coverage.Insured, projection.Contract?.Individuals, clients),
                           EstProtectionConjointe = typeAssurance != TypeAssurance.Individuelle,
                           EstProtectionContractant = estContractant,
                           EstProtectionBase = coverage.IsMain
                       });

            return result;
        }

        private static IEnumerable<Protection> MapperGaranties(ProjectionData.Projection projection,
                                                                IEnumerable<ProjectionData.Contract.Coverage.AdditionalBenefit> additionalBenefits,
                                                                IList<Client> clients,
                                                                IRegleAffaireAccessor regleAffaireAccessor,
                                                                bool estContractant)
        {
            var result = new List<Protection>();
            if (additionalBenefits == null) return result;

            foreach (var guarantee in additionalBenefits)
            {
                result.AddRange(MapperGarantie(projection, guarantee, clients, regleAffaireAccessor, estContractant));
            }

            return result;
        }

        private static IEnumerable<Protection> MapperGarantie(ProjectionData.Projection projection,
                                                               ProjectionData.Contract.Coverage.AdditionalBenefit additionalBenefit,
                                                               IList<Client> clients,
                                                               IRegleAffaireAccessor regleAffaireAccessor,
                                                               bool estContractant)
        {
            var result = new List<Protection>();
            var planInfo = regleAffaireAccessor.ObtenirPlan(additionalBenefit.PlanCode);
            var plan = new Plan
                       {
                           CodePlan = additionalBenefit.PlanCode,
                           DescriptionAn = planInfo.DescriptionAn,
                           DescriptionFr = planInfo.DescriptionFr,
                           InfoProtection = regleAffaireAccessor.DeterminerInfoProtection(projection, additionalBenefit)
                       };

            if (!EstRenouvelable(plan.InfoProtection)) return result;

            result.Add(new Protection
                       {
                           Id = additionalBenefit.Identifier.Id,
                           Plan = plan,
                           CapitalAssure = additionalBenefit.FaceAmount.Current,
                           TypeAssurance = additionalBenefit.InsuranceType.ConvertirTypeAssurance(),
                           PresenceSurprime = additionalBenefit.Insured?.ExtraPremiums?.Any() ?? false,
                           Assures = MapperAssures(additionalBenefit.Insured, projection.Contract?.Individuals, clients),
                           EstGarantie = true,
                           EstProtectionConjointe = additionalBenefit.InsuranceType != ProjectionEnums.Coverage.InsuranceType.Individual,
                           EstProtectionContractant = estContractant,
                           EstProtectionBase = false
                       });

            return result;
        }

        private static List<Assure> MapperAssures(ProjectionData.Contract.Coverage.Insured insured,
                                                   List<ProjectionData.Contract.Individual> individuals,
                                                   IList<Client> clients)
        {
            var result = new List<Assure>();
            if (insured?.InsuredIndividual != null)
                result.Add(CreerAssure(individuals.FirstOrDefault(x => x.Identifier.Id == insured.InsuredIndividual.Individual.Id),
                    clients.FirstOrDefault(x => x.ReferenceExterneId == insured.InsuredIndividual.Individual.Id)));

            if (insured?.Joints != null)
                result.AddRange(from conjoint in insured.Joints
                                where conjoint?.InsuredIndividual != null
                                select
                                    CreerAssure(individuals.FirstOrDefault(x => x.Identifier.Id == conjoint.InsuredIndividual.Individual.Id),
                                        clients.FirstOrDefault(x => x.ReferenceExterneId == conjoint.InsuredIndividual.Individual.Id)));

            return result;
        }

        private static Assure CreerAssure(ProjectionData.Contract.Individual individual, Client client)
        {
            var nom = client?.Nom;
            var prenom = client?.Prenom;
            var intiale = client?.Initiale;

            return new Assure
                   {
                       SequenceIndividu = individual.SequenceNumber,
                       ReferenceExterneId = individual.Identifier.Id,
                       Nom = string.IsNullOrEmpty(nom) ? "Client " + individual.SequenceNumber : nom,
                       Prenom = prenom,
                       Intitial = intiale
                   };
        }

        private static List<PrimeRenouvellement> CreerPrimesRenouvellement(double[] valeursGarantie)
        {
            var primes = new List<PrimeRenouvellement>();
            var derniereValeur = valeursGarantie.DefaultIfEmpty(-1).LastOrDefault(v => v > 0.0000001);
            if (derniereValeur < 0.0000001) return primes;
            var indexDerniereValeur = Array.LastIndexOf(valeursGarantie, derniereValeur);
            var indexPremiereValeur = Array.IndexOf(valeursGarantie, valeursGarantie.DefaultIfEmpty(-1).FirstOrDefault(v => v > 0.0000001));
            for (var index = indexPremiereValeur; index <= indexDerniereValeur; index++)
            {
                primes.Add(new PrimeRenouvellement
                           {
                               Annee = index,
                               MontantGaranti = valeursGarantie[index]
                           });
            }

            return primes;
        }

        private static bool EstRenouvelable(InfoProtection infoProtection)
        {
            return (infoProtection & InfoProtection.Renouvellable) == InfoProtection.Renouvellable;
        }

        private static double DeterminerCapitalAssure(Protection protection, IReadOnlyList<double> capitals, double[] valeursGarantie)
        {
            var indexPremiereValeur = Array.IndexOf(valeursGarantie, valeursGarantie.DefaultIfEmpty(-1).FirstOrDefault(v => v > 0.0000001));
            if (indexPremiereValeur < 0 || indexPremiereValeur > capitals.Count) return protection.CapitalAssure;
            return DeterminerCapital(capitals, indexPremiereValeur);
        }

        private static double DeterminerCapital(IReadOnlyList<double> capitals, int indexPremiereValeur)
        {
            double capital = 0;
            for (var i = indexPremiereValeur; i >= 0; i--)
            {
                capital = capitals[i];
                if (capital > 0)
                    break;
            }

            return capital;
        }
    }
}

