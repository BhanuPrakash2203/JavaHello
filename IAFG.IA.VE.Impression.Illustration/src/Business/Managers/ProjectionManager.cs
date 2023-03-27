using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnum = IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.Projection.Data.Extensions;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly IRegleAffaireAccessor _regleAffaireAccessor;

        public ProjectionManager(IRegleAffaireAccessor regleAffaireAccessor)
        {
            _regleAffaireAccessor = regleAffaireAccessor;
        }

        public ProjectionData.Projection GetDefaultProjection(ProjectionData.Projections projections)
        {
            return projections?.GetDefaultProjection();
        }

        public ProjectionData.Projection GetEstateBondProjection(ProjectionData.Projections projections)
        {
            return projections?.GetEstateBondProjection();
        }
        
        public ProjectionData.Contract.Coverage.Coverage GetMainCoverage(ProjectionData.Projection projection)
        {
            return projection?.GetMainCoverage(); 
        }

        public IGetPDFICoverageResponse GetPdfCoverage(ProjectionData.Projection projection, ProjectionData.Contract.Coverage.Coverage coverage)
        {
            return _regleAffaireAccessor.GetPdfCoverage(projection, coverage);
        }

        public bool DeterminerPresenceCompteTerme(ProjectionData.Projection projection)
        {
            if (projection?.Contract?.FinancialSection?.Funds == null)
            {
                return false;
            }

            var vehicules = new List<string>();
            foreach (var fund in projection.Contract.FinancialSection.Funds)
            {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                if (fund.Accounts != null)
                {
                    foreach (var account in fund.Accounts.Where(x => !string.IsNullOrEmpty(x.Vehicle)))
                    {
                        if (!vehicules.Contains(account.Vehicle)) vehicules.Add(account.Vehicle);
                    }
                }

                if (fund.Instructions?.Investments == null)
                {
                    continue;
                }

                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var instruction in fund.Instructions.Investments.Where(x => !string.IsNullOrEmpty(x.Vehicle)))
                {
                    if (!vehicules.Contains(instruction.Vehicle))
                    {
                        vehicules.Add(instruction.Vehicle);
                    }
                }
            }

            return vehicules.Any(_regleAffaireAccessor.EstCompteInteretGarantie);
        }

        public bool DeterminerPresencePrimeRenouvellable(ProjectionData.Projection projection)
        {
            var idColonnePrimeRenouvellement = projection.Illustration?
                                                   .GetColumnDescriptionsWithAttributes(
                                                       new[] { "Type:GuaranteedRenewal" })?.FirstOrDefault()?.Id;

            if (!idColonnePrimeRenouvellement.HasValue)
            {
                return false;
            }

            return projection.Illustration.Columns.Any(c =>
                c.Id == idColonnePrimeRenouvellement && c.Value.Any(v => v > 0));
        }

        public bool DeterminerTabagismePreferentiel(ProjectionData.Projection projection)
        {
            return projection?.Contract?.Insured != null && 
                projection.Contract.Insured.Any(
                    assure => assure.Coverages?.FirstOrDefault(c => c.IsMain)?.Insured?.SmokerType.IsStatusPreferentiel() ?? false);
        }

        public Etat EtatContrat(ProjectionData.Projection projection)
        {
            var protectionBase = GetMainCoverage(projection);
            if (protectionBase == null) throw new ArgumentNullException(nameof(protectionBase));
            return protectionBase.Status == ProjectionEnum.Coverage.CoverageStatus.New ? Etat.NouvelleVente : Etat.EnVigueur;
        }

        public Banniere Banniere(ProjectionData.Projection projection)
        {
            return projection?.Contract == null
                ? Types.Enums.Banniere.Defaut
                : (Banniere)projection.Contract.Banner;
        }

        public ProvinceEtat ProvinceEtat(ProjectionData.Projection projection)
        {
            return projection?.Contract == null
                ? Types.Enums.ProvinceEtat.Inconnu
                : (ProvinceEtat)projection.Contract.ProvinceState;
        }

        public bool ContratEstEnDecheance(ProjectionData.Projection projection)
        {
            return projection?.Values?.Search(ProjectionEnum.ValueId.LapseYear) != null;
        }

        public bool ContractantEstCompagnie(ProjectionData.Projection projection)
        {
            var result = projection?.Contract?.Individuals?.Any(x => x.IsApplicant && x.IsCorporation);
            return result ?? false;
        }

        public bool ContratEstConjoint(ProjectionData.Projection projection)
        {
            return GetMainCoverage(projection).InsuranceType != ProjectionEnum.Coverage.InsuranceType.Individual;
        }
    }
}
