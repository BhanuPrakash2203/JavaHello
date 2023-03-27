using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ConceptVentes;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnum = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class ConceptVenteMapper : IConceptVenteMapper
    {
        private readonly IProjectionManager _projectionManager;
        private readonly IBonSuccessoralMapper _bonSuccessoralMapper;

        public ConceptVenteMapper(IProjectionManager projectionManager, IBonSuccessoralMapper bonSuccessoralMapper)
        {
            _projectionManager = projectionManager;
            _bonSuccessoralMapper = bonSuccessoralMapper;
        }

        public ConceptVente Map(ProjectionData.Projections projections, DonneesRapportIllustration model, DateTime dateEmission)
        {
            return MapperConcept(new ConceptVente(), projections, model, dateEmission);
        }

        internal ConceptVente MapperConcept(ConceptVente conceptVente, ProjectionData.Projections projections, DonneesRapportIllustration model, DateTime dateEmission)
        {
            var projection = _projectionManager.GetDefaultProjection(projections);
            if (projection == null) return null;
            conceptVente.AvancePret = MapperAvancePret(projection, dateEmission);
            conceptVente.PretEnCollateral = MapperPretEnCollateral(projection, dateEmission);
            conceptVente.ProprietePartagee = MapperProprietePartagee(projection);
            conceptVente.BonSuccessoral = _bonSuccessoralMapper.Map(projections, model.HypothesesInvestissement);
            return conceptVente;
        }

        internal bool MapperProprietePartagee(ProjectionData.Projection projection)
        {
            if (projection.Concept?.Concepts != null)
            {
                return projection.Concept.Concepts.Any(x => x == ProjectionEnum.Concept.ConceptType.SharedOwnership);
            }

            return false;
        }

        internal static AvancePret MapperAvancePret(ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection.Concept?.PolicyLoan == null)
            {
                return null;
            }

            var avancePret = new AvancePret { Data = projection.Concept.PolicyLoan, Remboursements = new List<TransactionRemboursement>() };
            var pret = projection.Contract?.FinancialSection?.Loans?.List?.FirstOrDefault(x => x.LoanType == ProjectionEnum.Financial.LoanType.InvestmentPolicyLoan);
            if (pret?.DirectivesPersonnaliseesPourGarantirPret != null)
            {
                avancePret.Compte = pret.DirectivesPersonnaliseesPourGarantirPret.FirstOrDefault()?.Vehicle ?? string.Empty;
            }

            var depots = projection.Transactions?.Deposits?.Where(x => x.DepositType == ProjectionEnum.CashFlow.DepositType.IrisPolicyLoanRefund);
            if (depots == null) return avancePret;
            foreach (var item in depots)
            {
                avancePret.Remboursements.Add(new TransactionRemboursement
                {
                    Annee = item.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Montant = item.Amount?.Value,
                    TypeMontant = Convertir(item.Amount?.AmountType ?? ProjectionEnum.CashFlow.AmountType.Unspecified),
                    EstMontantMaximal = (item.Amount?.AmountType ?? ProjectionEnum.CashFlow.AmountType.Unspecified) == ProjectionEnum.CashFlow.AmountType.Maximum,
                    ProvenanceFonds = Convertir(item.RefundSource)
                });
            }

            return avancePret;
        }

        internal static PretEnCollateral MapperPretEnCollateral(ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection.Concept?.CollateralLoan == null)
            {
                return null;
            }

            var pretEnCollateral = new PretEnCollateral { Data = projection.Concept.CollateralLoan, Remboursements = new List<TransactionRemboursement>() };
            var pret = projection.Contract?.FinancialSection?.Loans?.List?.FirstOrDefault(x => x.LoanType == ProjectionEnum.Financial.LoanType.InvestmentLoan);
            if (pret?.DirectivesPersonnaliseesPourGarantirPret != null)
            {
                pretEnCollateral.Compte = pret.DirectivesPersonnaliseesPourGarantirPret.FirstOrDefault()?.Vehicle ?? string.Empty;
            }

            var depots = projection.Transactions?.Deposits?.Where(x => x.DepositType == ProjectionEnum.CashFlow.DepositType.IrisCollateralLoanRefund);
            if (depots == null) return pretEnCollateral;
            foreach (var item in depots)
            {
                pretEnCollateral.Remboursements.Add(new TransactionRemboursement
                {
                    Annee = item.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Montant = item.Amount?.Value,
                    TypeMontant = Convertir(item.Amount?.AmountType ?? ProjectionEnum.CashFlow.AmountType.Unspecified),
                    EstMontantMaximal = (item.Amount?.AmountType ?? ProjectionEnum.CashFlow.AmountType.Unspecified) == ProjectionEnum.CashFlow.AmountType.Maximum,
                    ProvenanceFonds = Convertir(item.RefundSource)
                });
            }

            return pretEnCollateral;
        }

        private static string Convertir(ProjectionEnum.CashFlow.AmountType amountType)
        {
            switch (amountType)
            {
                case ProjectionEnum.CashFlow.AmountType.Unspecified:
                    return string.Empty;
                case ProjectionEnum.CashFlow.AmountType.Maximum:
                    return "Maximum";
                case ProjectionEnum.CashFlow.AmountType.Customized:
                    return "Personnalise";
                case ProjectionEnum.CashFlow.AmountType.Forced:
                    return "Forcee";
                default:
                    throw new ArgumentOutOfRangeException(nameof(amountType), amountType, null);
            }
        }

        private static string Convertir(ProjectionEnum.CashFlow.RefundSource source)
        {
            switch (source)
            {
                case ProjectionEnum.CashFlow.RefundSource.Unspecified:
                    return string.Empty;
                case ProjectionEnum.CashFlow.RefundSource.Intern:
                    return "RetraitsPolice";
                case ProjectionEnum.CashFlow.RefundSource.AdditionnalDeposits:
                    return "DepotsAdditionnels";
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}
