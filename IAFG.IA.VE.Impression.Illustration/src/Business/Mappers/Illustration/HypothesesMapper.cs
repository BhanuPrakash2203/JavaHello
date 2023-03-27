using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types.Models.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnums = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class HypothesesMapper : IHypothesesMapper
    {
        private readonly IProjectionManager _projectionManager;
        private readonly IVecteurManager _vecteurManager;

        public HypothesesMapper(IProjectionManager projectionManager, IVecteurManager vecteurManager)
        {
            _projectionManager = projectionManager;
            _vecteurManager = vecteurManager;
        }

        public Hypotheses MapHypothesesInvestissement(
            ProjectionData.Projection projection, 
            DateTime dateEmission, 
            Projections projections)
        {
            var result = new Hypotheses
            {
                FondsCapitalisation = MapperFondsCapitalisation(projection, dateEmission),
                FondsTransitoire = MapperFondsTransitoire(projection, dateEmission, projections),
                Prets = MapperPrets(projection.Contract?.FinancialSection?.Loans?.List)
            };

            return result;
        }

        private FondsCapitalisation MapperFondsCapitalisation(ProjectionData.Projection projection, DateTime dateEmission)
        {
            var result = new FondsCapitalisation();
            var listFonds = projection?.Contract?.FinancialSection?.Funds?.
                Where(f => f.FundType == ProjectionEnums.Financial.FundType.Accumulation).ToList()
                ?? new List<ProjectionData.Contract.Financial.Funds.Fund>();

            result.Directives = MapperDirectives(listFonds);
            result.TauxComptes = MapperTauxComptes(dateEmission, projection?.Parameters?.Assumptions?.AccumulationFunds);
            result.TauxAVMComptes = MapperTauxAVMComptes(dateEmission, projection?.Parameters?.Assumptions?.AccumulationFunds);
            result.Comptes = MapperComptes(listFonds);
            result.Solde = _projectionManager.EtatContrat(projection) == Types.Enums.Etat.NouvelleVente
                ? (double?)null
                : listFonds.FirstOrDefault()?.Balance ?? 0;

            result.Fonds = MapperComptesFonds(result.Comptes, result.TauxComptes, result.Directives, result.Solde);
            result.RendementMoyenCompte = CalculerRendementMoyen(result.Fonds);
            return result;
        }

        private double? CalculerRendementMoyen(IList<Fonds> fonds)
        {
            var listFonds = fonds.OrderBy(x => x.AnneeDebut.GetValueOrDefault(0))
                                  .Where(x => x.RepartitionInvestissement.HasValue && x.Taux.HasValue);

            if (!listFonds.Any()) return null;

            var sum = 0.0;
            return listFonds.TakeWhile(x =>
            {
                var temp = sum;
                sum += x.RepartitionInvestissement.GetValueOrDefault(0);
                return temp < 1;
            })
                .Select(x => new { Pourcentage = x.RepartitionInvestissement.GetValueOrDefault(0), Taux = x.Taux.GetValueOrDefault(0) })
                .Sum(x => x.Pourcentage * x.Taux);
        }

        private FondsTransitoire MapperFondsTransitoire(ProjectionData.Projection projection, DateTime dateEmission, Projections projections)
        {
            var result = new FondsTransitoire();
            var listFonds = projection?.Contract?.FinancialSection?.Funds?
                .Where(f => f.FundType == ProjectionEnums.Financial.FundType.Shuttle).ToList() ??
                new List<ProjectionData.Contract.Financial.Funds.Fund>();

            result.Solde = listFonds.FirstOrDefault()?.Balance ?? 0;
            result.Directives = MapperDirectives(listFonds);
            result.TauxComptes = MapperTauxComptes(dateEmission, projection?.Parameters?.Assumptions?.ShuttleFunds);
            result.Comptes = MapperComptes(listFonds);
            result.Solde = _projectionManager.EtatContrat(projection) == Types.Enums.Etat.NouvelleVente
                ? (double?)null
                : listFonds.FirstOrDefault()?.Balance ?? 0;

            result.Fonds = MapperComptesFonds(result.Comptes, result.TauxComptes, result.Directives, result.Solde);
            result.FondsUtilise = listFonds.FirstOrDefault()?.Balance > 0 ||
                                  ValeurExiste(projections, ProjectionVectorId.TransferToShuttleAccount) ||
                                  ValeurExiste(projections, ProjectionVectorId.ShuttleAccountBfTax);

            return result;
        }

        private bool ValeurExiste(Projections projections, ProjectionVectorId colonne)
        {
            return _vecteurManager.ObtenirVecteurOuDefaut(
                projections, (int)colonne,
                Types.Enums.TypeProjection.Normal, 
                Types.Enums.TypeRendementProjection.Normal).Any(v => v > 0);
        }

        private static List<Compte> MapperComptes(IEnumerable<ProjectionData.Contract.Financial.Funds.Fund> listFonds)
        {
            return (from compte in listFonds.Where(d => d.Accounts != null).SelectMany(i => i.Accounts)
                    where compte.AccountType == ProjectionEnums.Financial.AccountType.GuaranteedRate ||
                          compte.AccountType == ProjectionEnums.Financial.AccountType.Vehicle
                    select new Compte
                    {
                        Vehicule = compte.Vehicle,
                        Solde = compte.Balance
                    }).ToList();
        }

        // ReSharper disable once InconsistentNaming
        private static IList<Taux> MapperTauxAVMComptes(DateTime dateEmission, IEnumerable<ProjectionData.Parameters.Assumptions.Account> accounts)
        {
            var tauxComptes = new List<Taux>();
            if (accounts == null)
            {
                return tauxComptes;
            }

            foreach (var item in accounts.Where(x => x.MarketValueAdjustmentRates != null))
            {
                tauxComptes.AddRange(item.MarketValueAdjustmentRates.Select(itemTaux => new Taux
                {
                    Vehicule = item.Vehicle,
                    ValeurTaux = itemTaux.Value,
                    DateEffective = itemTaux.StartDate.ConvertirDateProjection(dateEmission).GetValueOrDefault(),
                    Annee = itemTaux.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Mois = itemTaux.StartDate.CalculerMoisContratProjection(dateEmission)
                }));
            }

            return tauxComptes;
        }

        private static IList<Taux> MapperTauxComptes(DateTime dateEmission, IEnumerable<ProjectionData.Parameters.Assumptions.Account> accounts)
        {
            var tauxComptes = new List<Taux>();
            if (accounts == null)
            {
                return tauxComptes;
            }

            foreach (var item in accounts)
            {
                tauxComptes.AddRange(item.InterestRates.Select(itemTaux => new Taux
                {
                    Vehicule = item.Vehicle,
                    ValeurTaux = itemTaux.Value,
                    DateEffective = itemTaux.StartDate.ConvertirDateProjection(dateEmission).GetValueOrDefault(),
                    Annee = itemTaux.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Mois = itemTaux.StartDate.CalculerMoisContratProjection(dateEmission)
                }));
            }

            return tauxComptes;
        }

        private static IList<Directive> MapperDirectives(IEnumerable<ProjectionData.Contract.Financial.Funds.Fund> listFonds)
        {
            var directives = new List<Directive>();
            foreach (var fonds in listFonds.Where(d => d.Instructions != null))
            {
                if (fonds.Instructions.Investments != null)
                {
                    directives.AddRange(fonds.Instructions.Investments.Select(i => new Directive
                    {
                        Vehicule = i.Vehicle,
                        Pourcentage = i.Percentage,
                        TypeDirective = TypeDirective.Investissement
                    }));
                }

                if (fonds.Instructions.Deductions != null)
                {
                    directives.AddRange(fonds.Instructions.Deductions.Select(d => new Directive
                    {
                        Vehicule = d.Vehicle,
                        Pourcentage = d.Percentage,
                        TypeDirective = TypeDirective.Deduction
                    }));
                }
            }

            return directives;
        }

        private static List<Prets> MapperPrets(IEnumerable<ProjectionData.Contract.Financial.Loans.Loan> prets)
        {
            var result = new List<Prets>();
            if (prets == null) return result;
            result.AddRange(prets.Select(pret => new Prets
            {
                TypePret = pret.LoanType.ConvertirTypePret(),
                Solde = pret.Balance
            }));

            return result;
        }
        
        private static IList<Fonds> MapperComptesFonds(
            IEnumerable<Compte> comptes, 
            IList<Taux> tauxComptes, 
            IList<Directive> directives, 
            double? solde)
        {
            var listFonds = new List<Fonds>();
            var directivesInvestissement = directives?.Where(d => d.TypeDirective == TypeDirective.Investissement).ToList() ?? new List<Directive>();
            var directivesDeduction = directives?.Where(d => d.TypeDirective == TypeDirective.Deduction).ToList() ?? new List<Directive>();
            
            var groupeComptes = (from c in comptes?.ToList() ?? new List<Compte>()
                                 group c by c.Vehicule
                                 into g
                                 select new
                                 {
                                     Vehicule = g.Key,
                                     Solde = g.Sum(x => x.Solde)
                                 }).ToList();

            foreach (var compte in groupeComptes.OrderBy(d => d.Vehicule))
            {
                var pourcentageInvestissement = directivesInvestissement.FirstOrDefault(c => c.Vehicule.Equals(compte.Vehicule))?.Pourcentage;
                var pourcentageDeduction = directivesDeduction.FirstOrDefault(d => d.Vehicule.Equals(compte.Vehicule))?.Pourcentage;
                var soldeCompte = solde.HasValue ? compte.Solde : (double?) null;
                listFonds.AddRange(MapperDetailComptes(tauxComptes, compte.Vehicule, 
                    soldeCompte, pourcentageInvestissement, pourcentageDeduction));
            }

            foreach (var directive in directivesInvestissement.Where(d => listFonds.All(f => f.Vehicule != d.Vehicule)).OrderBy(d => d.Vehicule))
            {
                var pourcentageDeduction = directivesDeduction.FirstOrDefault(d => d.Vehicule.Equals(directive.Vehicule))?.Pourcentage;
                var soldeDirective = solde.HasValue ? 0 : (double?)null;
                listFonds.AddRange(MapperDetailComptes(tauxComptes, directive.Vehicule, 
                    soldeDirective, directive.Pourcentage, pourcentageDeduction));
            }

            foreach (var directive in directivesDeduction.Where(d => listFonds.All(f => f.Vehicule != d.Vehicule)).OrderBy(d => d.Vehicule))
            {
                var pourcentageInvestissement = directivesInvestissement.FirstOrDefault(c => c.Vehicule.Equals(directive.Vehicule))?.Pourcentage;
                var soldeDirective = solde.HasValue ? 0 : (double?)null;
                listFonds.AddRange(MapperDetailComptes(tauxComptes, directive.Vehicule,
                    soldeDirective, pourcentageInvestissement, directive.Pourcentage));
            }

            return listFonds;
        }

        private static IEnumerable<Fonds> MapperDetailComptes(IEnumerable<Taux> tauxComptes, 
            string vehicule, double? solde, double? pourcentageInvestissement, double? pourcentageDeduction)
        {
            var listTaux = tauxComptes?.Where(t => t.Vehicule == vehicule).ToList() ?? new List<Taux>();
            var listFonds = new List<Fonds>();
            Fonds fondsPrecedent = null;
            foreach (var taux in listTaux.OrderBy(t => t.Annee))
            {
                if (fondsPrecedent == null || Math.Abs((fondsPrecedent.Taux ?? 0) - taux.ValeurTaux) > 0.00001)
                {
                    var fonds = new Fonds
                    {
                        Vehicule = vehicule,
                        Solde = solde,
                        RepartitionInvestissement = pourcentageInvestissement,
                        RepartitionDeduction = pourcentageDeduction,
                        Taux = taux.ValeurTaux,
                        AnneeDebut = taux.Annee,
                        MoisDebut = taux.Mois
                    };

                    listFonds.Add(fonds);
                    fondsPrecedent = fonds;
                }
            }

            //Si un seul taux présent alors c'est à vie (AnneeDebut sera donc à null).
            if (listFonds.Count == 1)
            {
                listFonds.First().AnneeDebut = null;
            }

            return listFonds;
        }
    }
}