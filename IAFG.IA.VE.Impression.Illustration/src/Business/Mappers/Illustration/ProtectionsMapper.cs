using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.Participations;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VI.Projection.Data.Extensions;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.PDF.Plan.ENUMs;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnum = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class ProtectionsMapper : IProtectionsMapper
    {
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private readonly IRegleAffaireAccessor _regleAffaireAccessor;

        public ProtectionsMapper(IIllustrationResourcesAccessorFactory resourcesAccessorFactory, 
            IRegleAffaireAccessor regleAffaireAccessor)
        {
            _resourcesAccessorFactory = resourcesAccessorFactory;
            _regleAffaireAccessor = regleAffaireAccessor;
        }

        public IList<ProtectionsGroupees> MapperProtectionsGroupees(DonneesIllustration data,
            ProjectionData.Projection projection, IList<Client> clients)
        {
            ProtectionsGroupees resultContractant = null;
            var resultAssure = new List<ProtectionsGroupees>();

            if (projection?.Contract?.Insured != null)
            {
                var insured = projection.Contract.Insured;
                resultAssure.AddRange(insured.Select(assure =>
                {
                    var protectionsAssures = MapProtections(assure, projection, clients, data.DonneesPdf).ToList();
                    return new ProtectionsGroupees
                    {
                        IsApplicant = false,
                        Identifier = assure.Identifier,
                        ProtectionsAssures = protectionsAssures,
                        PrimesRenouvellement =
                            new Types.Models.PrimesRenouvellement.Primes().MapperPrimesRenouvellement(assure,
                                projection,
                                clients, _regleAffaireAccessor)
                    };
                }));
            }


            if (projection?.Contract?.Applicants != null)
            {
                var protection = MapGarantieContractant(projection, clients, data.DonneesPdf).ToList();
                resultContractant = new ProtectionsGroupees
                {
                    IsApplicant = true,
                    Identifier = projection.Contract.Applicants.Identifier,
                    ProtectionsAssures = protection
                };
            }

            var result = new List<ProtectionsGroupees>();
            if (resultContractant != null)
            {
                result.Add(resultContractant);
            }

            if (resultAssure.Any())
            {
                result.AddRange(resultAssure);
            }

            return result;
        }

        public Protections MapperProtections(DonneesIllustration data, 
            ProjectionData.Projection projection, IList<Client> clients)
        {
            if (projection == null) return null;
            var montantPrimeTotal = projection.Values.GetMaxValue(
                ProjectionEnum.ValueId.SuggestedBilling, ProjectionEnum.ValueId.TotalPremium);

            var assures = projection.Contract?.Insured;
            var protectionBase = assures?.FirstOrDefault(x => x.IsMain)?.Coverages?.FirstOrDefault(c => c.IsMain);
            var deathBenefit = protectionBase?.DeathBenefit;
            var prestationDeces = deathBenefit?.Current ?? ProjectionEnum.Coverage.DeathBenefitOption.NonApplicable;

            var statutOacaActif = deathBenefit == null ||
                                  deathBenefit.AutomaticFaceAmountOptimization == ProjectionEnum.BooleanType.Unspecified
                ? default(bool?)
                : deathBenefit.AutomaticFaceAmountOptimization == ProjectionEnum.BooleanType.True;

            var result = new Protections
            {
                ExisteProtectionTemporaireRenouvelable = DetermineSiExisteProtectionTemporaireRenouvelable(data.DonneesPdf),
                MontantPrimeTotal = montantPrimeTotal,
                StatutOacaActif = statutOacaActif,
                PrestationDeces = ((OptionPrestationDeces)prestationDeces)
            };

            if (deathBenefit?.Current == ProjectionEnum.Coverage.DeathBenefitOption.WealthMaximizer ||
                deathBenefit?.Current == ProjectionEnum.Coverage.DeathBenefitOption.FaceAmountPlusFundWealthMaximizer)
            {
                result.ValeurMaximisee = new ValeurMaximisee
                {
                    CapitalAssurePlafond = protectionBase.FaceAmount?.Current,
                    CapitalAssurePlancher = deathBenefit.MinimalFaceAmount,
                    DureeDebutMinimisation = deathBenefit.DurationBeforeMinimization
                };
            }

            var protections = new List<Protection>();
            protections.AddRange(MapGarantieContractant(projection, clients, data.DonneesPdf));

            if (projection?.Contract?.Insured != null)
            {
                foreach (var insured in projection.Contract.Insured)
                {
                    protections.AddRange(MapProtections(insured, projection, clients, data.DonneesPdf));
                }
            }

            result.ProtectionsAssures = protections
                .OrderByDescending(p => p.EstProtectionContractant)
                .ThenByDescending(p => p.EstProtectionConjointe).ToList();

            return result;
        }

        private IEnumerable<Protection> MapProtections(ProjectionData.Contract.Insured insured,
            ProjectionData.Projection projection, IList<Client> clients, DonneesPdf donneesPdf)
        {
            var result = new List<Protection>();
            if (insured?.Coverages == null)
            {
                return result;
            }

            foreach (var coverage in insured.Coverages.Where(c => !c.IsAddedAutomaticallyWhenProjected(projection)))
            {
                var protection = MapProtection(coverage,
                                               projection,
                                               clients,
                                               donneesPdf,
                                               insured.IsMain,
                                               false);

                if (protection == null)
                {
                    continue;
                }

                result.Add(protection);

                if (coverage.Coverages != null)
                {
                    result.AddRange(coverage.Coverages.Select(subCoverage => MapProtection(subCoverage,
                                                                                           projection,
                                                                                           clients,
                                                                                           donneesPdf,
                                                                                           insured.IsMain,
                                                                                           true)).Where(x => x != null));
                }

                if (coverage.AdditionalBenefits != null)
                {
                    result.AddRange(
                                    coverage.AdditionalBenefits.Select(guarantee =>
                                                               MapperGarantie(guarantee,
                                                                              projection,
                                                                              insured.IsMain,
                                                                              false,
                                                                              projection.Contract,
                                                                              clients,
                                                                              donneesPdf
                                                                              )).Where(x => x != null && !x.EstProtectionContractant));
                }
            }

            return result;
        }

        private TypeProtectionComplementaire DeterminerProtectionComplementaire(CodeGarantie? codeGarantie)
        {
            if (codeGarantie == null)
            {
                return TypeProtectionComplementaire.NonApplicable;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (codeGarantie)
            {
                case CodeGarantie.EP:
                    return TypeProtectionComplementaire.CIA;
                case CodeGarantie.EPI:
                    return TypeProtectionComplementaire.CIC;
                case CodeGarantie.EPD:
                    return TypeProtectionComplementaire.CDC;
                default:
                    return TypeProtectionComplementaire.NonApplicable;
            }
        }

        private Plan ObtenirPlan(IPlanInfo planInfo, IGetPDFICoverageResponse pdfCoverage)
        {
            var infoProtection = pdfCoverage.DeterminerInfoProtection();
            return new Plan
            {
                AgeMaturite = planInfo.AgeMaturite,
                CodeGlossaire = planInfo.CodeGlossaire,
                CodePlan = planInfo.CodePlan,
                DescriptionAn = planInfo.DescriptionAn,
                DescriptionFr = planInfo.DescriptionFr,
                InfoProtection = infoProtection,
                SansVolumeAssurance = pdfCoverage.Coverage.Plan.SansVolumeAssurance,
                TypePrestationPlan = planInfo.TypePrestationPlan,
                TypeProtectionComplementaire = DeterminerProtectionComplementaire(pdfCoverage.Coverage.AdditionalBenefit?.TypeAlis),
                CacherDetailTaux = DeterminerCacherDetailTaux(pdfCoverage)
            };
        }

        private bool DeterminerCacherDetailTaux(IGetPDFICoverageResponse pdfCoverage)
        {
            if (pdfCoverage?.Coverage?.Plan == null)
            {
                return false;
            }

            return pdfCoverage.Coverage.Plan.TypeProtection == TypeProtection.CouvertureAvenantVieIndivACjt ||
                   pdfCoverage.Coverage.Plan.TypeProtection.HasFlag(TypeProtection.ModuleEnfant);
        }

        private Protection MapperGarantie(ProjectionData.Contract.Coverage.AdditionalBenefit garantie,
            ProjectionData.Projection projection,
            bool estAssurePrincipal,
            bool estContractant,
            ProjectionData.Contract.Contract contract,
            IList<Client> clients,
            DonneesPdf donneesPdf)
        {
            if (garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Terminated))
            {
                return null;
            }

            var garantieValues = projection.Values.SearchByCoverage(garantie.Identifier.Id).ToList();
            var dureeProtection = garantie.Dates.Maturity.Year - garantie.Dates.Issuance.Year;
            var montantPrimeMinimale =
                garantieValues.SearchByCoverage(garantie.Identifier.Id, ProjectionEnum.ValueId.SuggestedBilling) ?? 0;
            var dureePaiement = ObtenirDureeDePaiment(garantieValues, garantie.Identifier.Id, donneesPdf);
            var planInfo = _regleAffaireAccessor.ObtenirPlan(garantie.PlanCode);
            var pdfCoverage = _regleAffaireAccessor.GetPdfCoverage(projection, garantie);
            var plan = ObtenirPlan(planInfo, pdfCoverage);
            var estSurprimee = garantie.Insured.ExtraPremiums?.Any() ?? false;
            if (garantie.Insured.Joints != null)
            {
                estSurprimee = estSurprimee || garantie.Insured.Joints.Any(x => x.ExtraPremiums?.Any() ?? false);
            }

            var surprimeTotal = garantieValues.SearchByCoverage(garantie.Identifier.Id,
                                    ProjectionEnum.ValueId.SuggestedBillingExtraPremium) ?? 0;

            var protectionAssure = new Protection
            {
                ReferenceExterneId = garantie.Identifier.Id,
                Plan = plan,
                EstNouvelleProtection = garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.New),
                EstProtectionAssurePrincipal = estAssurePrincipal,
                EstProtectionBase = false,
                EstProtectionContractant = garantie.AdditionalBenefitCode == ProjectionEnum.Coverage.AdditionalBenefitCode.EP_CIA || estContractant,
                EstProtectionConjointe = garantie.InsuranceType != ProjectionEnum.Coverage.InsuranceType.Individual,
                EstLiberee = garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Paidup),
                TypeAssurance = (TypeAssurance)garantie.InsuranceType,
                CapitalAssureActuel = garantie.FaceAmount?.Actual ?? 0,
                ReductionCapitalAssure = DeterminerMontantReductionCapitalAssure(projection.Transactions),
                DureeProtection = dureeProtection,
                DureePaiement = (int)dureePaiement,
                MontantPrime = montantPrimeMinimale,
                MontantPrimeSuggeree = montantPrimeMinimale,
                DateMaturite = garantie.Dates.Maturity,
                DateEmission = garantie.Dates.Issuance,
                DateVersion = garantie.Dates.ProductVersion,
                DateLiberation = garantie.Dates.PaidUp,
                StatutTabagisme = (StatutTabagisme)garantie.Insured.SmokerType,
                Sexe = (Sexe)garantie.Insured.Sex,
                AgeEmission = garantie.Insured.Age.Issuance,
                Assures = MapperAssures(garantie.Insured, contract, clients),
                ReferenceNotes = new List<int>(),
                EstAvecFraisRachatSelonPrimes = false,
                EstAvecFraisRachatSelonFonds = false,
                EstSurprimee = estSurprimee,
                Surprimes = ObtenirSurprimesGarantie(garantie, clients),
                SurprimeTotal = Math.Abs(surprimeTotal) > 0 ? surprimeTotal : (double?)null
            };

            return protectionAssure;
        }

        private Protection MapperGarantieContractante(ProjectionData.Contract.Coverage.AdditionalBenefit garantie,
            ProjectionData.Projection projection,
            ProjectionData.Contract.Contract contract,
            IList<Client> clients,
            DonneesPdf donneesPdf)
        {
            if (garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Terminated))
            {
                return null;
            }

            var garantieValues = projection.Values.SearchByCoverage(garantie.Identifier.Id).ToList();
            var dureeProtection = garantie.Dates.Maturity.Year - garantie.Dates.Issuance.Year;
            var montantPrimeMinimale =
                garantieValues.SearchByCoverage(garantie.Identifier.Id, ProjectionEnum.ValueId.SuggestedBilling) ?? 0;
            var dureePaiement = ObtenirDureeDePaiment(garantieValues, garantie.Identifier.Id, donneesPdf);
            var planInfo = _regleAffaireAccessor.ObtenirPlan(garantie.PlanCode);
            var pdfCoverage = _regleAffaireAccessor.GetPdfCoverage(projection, garantie);
            var plan = ObtenirPlan(planInfo, pdfCoverage);
            var contributionCalcule = contract.ContractType == ProjectionEnum.ContractType.Universal 
                ? projection.Values.SearchByCoverage(garantie.Identifier.Id, ProjectionEnum.ValueId.CalculatedMonthlyContribution) ?? 0
                : projection.Values.SearchByCoverage(garantie.Identifier.Id, ProjectionEnum.ValueId.CalculatedPeriodicContribution) ?? 0;

            var estSurprimee = garantie.Insured.ExtraPremiums?.Any() ?? false;
            if (garantie.Insured.Joints != null)
            {
                estSurprimee = estSurprimee || garantie.Insured.Joints.Any(x => x.ExtraPremiums?.Any() ?? false);
            }

            var surprimeTotal = garantieValues.SearchByCoverage(garantie.Identifier.Id,
                                    ProjectionEnum.ValueId.SuggestedBillingExtraPremium) ?? 0;

            var protectionAssure = new Protection
            {
                ReferenceExterneId = garantie.Identifier.Id,
                Plan = plan,
                EstNouvelleProtection = garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.New),
                EstProtectionAssurePrincipal = false,
                EstProtectionBase = false,
                EstProtectionContractant = true,
                EstProtectionConjointe = garantie.InsuranceType != ProjectionEnum.Coverage.InsuranceType.Individual,
                EstLiberee = garantie.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Paidup),
                TypeAssurance = (TypeAssurance)garantie.InsuranceType,
                CapitalAssureActuel = contributionCalcule,
                ReductionCapitalAssure = DeterminerMontantReductionCapitalAssure(projection.Transactions),
                DureeProtection = dureeProtection,
                DureePaiement = (int)dureePaiement,
                MontantPrime = montantPrimeMinimale,
                MontantPrimeSuggeree = montantPrimeMinimale,
                DateMaturite = garantie.Dates.Maturity,
                DateEmission = garantie.Dates.Issuance,
                DateVersion = garantie.Dates.ProductVersion,
                DateLiberation = garantie.Dates.PaidUp,
                StatutTabagisme = (StatutTabagisme)garantie.Insured.SmokerType,
                Sexe = (Sexe)garantie.Insured.Sex,
                AgeEmission = garantie.Insured.Age.Issuance,
                Assures = MapperAssures(garantie.Insured, contract, clients),
                ReferenceNotes = new List<int>(),
                EstAvecFraisRachatSelonPrimes = false,
                EstAvecFraisRachatSelonFonds = false,
                EstSurprimee = estSurprimee,
                Surprimes = ObtenirSurprimesGarantieContractante(garantie, clients),
                SurprimeTotal = Math.Abs(surprimeTotal) > 0 ? surprimeTotal : (double?)null
            };

            return protectionAssure;
        }

        private Protection MapProtection(ProjectionData.Contract.Coverage.Coverage coverage,
                                                 ProjectionData.Projection projection,
                                                 IList<Client> clients,
                                                 DonneesPdf donneesPdf,
                                                 bool estAssurePrincipal,
                                                 bool estAvenantLie)
        {
            if (coverage.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Terminated))
            {
                return null;
            }

            if (coverage.CoverageCode == ProjectionEnum.Coverage.CoverageCode.PaidUpAddition ||
                coverage.CoverageCode == ProjectionEnum.Coverage.CoverageCode.AutomaticOptimizationFaceAmount)
            {
                return null;
            }

            var coverageValues = projection.Values.SearchByCoverage(coverage.Identifier.Id).ToList();
            var montantPrimeMinimale = coverageValues.GetMaxValueByCoverage(coverage.Identifier.Id,
                ProjectionEnum.ValueId.SuggestedBillingWithoutFee, ProjectionEnum.ValueId.TotalPremiumWithoutFee);
            var montantPrimeMinimaleSuggeree = coverageValues.GetMaxValueByCoverage(coverage.Identifier.Id,
                ProjectionEnum.ValueId.SuggestedBilling, ProjectionEnum.ValueId.TotalPremium);
            var dureePaiement = ObtenirDureeDePaiment(coverageValues, coverage.Identifier.Id, donneesPdf);
            var dureeProtection = coverage.Dates.Maturity.Year - coverage.Dates.Issuance.Year;
            var dureeRenouvellement = ObtenirDureeDeRenouvellement(projection.Transactions?.Terminations, coverage.Identifier.Id, donneesPdf, dureeProtection);
            var surprimeTotal = coverageValues.GetMaxValueByCoverage(coverage.Identifier.Id,
                ProjectionEnum.ValueId.SuggestedBillingExtraPremium, ProjectionEnum.ValueId.TotalExtraPremium);

            var estSurprimee = coverage.Insured.ExtraPremiums?.Any() ?? false;
            if (coverage.Insured.Joints != null)
            {
                estSurprimee = estSurprimee || coverage.Insured.Joints.Any(x => x.ExtraPremiums?.Any() ?? false);
            }

            var planInfo = _regleAffaireAccessor.ObtenirPlan(coverage.PlanCode);
            var pdfCoverage = _regleAffaireAccessor.GetPdfCoverage(projection, coverage);
            var plan = ObtenirPlan(planInfo, pdfCoverage);

            var assures = MapperAssures(coverage.Insured, projection.Contract, clients);
            var ageMaturite = coverage.Insured.Age.Issuance + dureeProtection;
            var typeAssurance = coverage.InsuranceType.ConvertirTypeAssurance();
            var ageEmission = typeAssurance != TypeAssurance.Individuelle &&
                              plan.InfoProtection.HasFlag(InfoProtection.Temporaire)
                ? default(int?)
                : coverage.Insured.Age.Issuance;

            var protectionAssure = new Protection
            {
                ReferenceExterneId = coverage.Identifier.Id,
                Plan = plan,
                EstNouvelleProtection = coverage.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.New),
                EstProtectionAssurePrincipal = estAssurePrincipal,
                EstAvenantLie = estAvenantLie,
                EstProtectionBase = coverage.IsMain,
                EstProtectionConjointe = typeAssurance != TypeAssurance.Individuelle,
                EstLiberee = coverage.Status.HasFlag(ProjectionEnum.Coverage.CoverageStatus.Paidup),
                TypeAssurance = typeAssurance,
                TypeCout = (TypeCout)coverage.MortalityType,
                CapitalAssureActuel = coverage.FaceAmount?.Actual ?? 0,
                ReductionCapitalAssure = DeterminerMontantReductionCapitalAssure(projection.Transactions),
                DureeProtection = dureeProtection,
                DureePaiement = (int)dureePaiement,
                DureeRenouvellement = dureeRenouvellement,
                MontantPrime = montantPrimeMinimale,
                MontantPrimeSuggeree = montantPrimeMinimaleSuggeree,
                DateMaturite = ageMaturite > 99 ? new DateTime(1001, 01, 01) : coverage.Dates.Maturity,
                DateEmission = coverage.Dates.Issuance,
                DateVersion = coverage.Dates.ProductVersion,
                DateLiberation = coverage.Dates.PaidUp,
                StatutTabagisme = (StatutTabagisme)coverage.Insured.SmokerType,
                Sexe = (Sexe)coverage.Insured.Sex,
                AgeEmission = ageEmission,
                Assures = assures,
                ReferenceNotes = new List<int>(),
                EstSurprimee = estSurprimee,
                Surprimes = ObtenirSurprimes(coverage, clients),
                SurprimeTotal = Math.Abs(surprimeTotal) > 0 ? surprimeTotal : (double?)null,
                EstAvecFraisRachatSelonPrimes = EstAvecFraisRachatSelonPrimes(coverage, pdfCoverage),
                EstAvecFraisRachatSelonFonds = EstAvecFraisRachatSelonFonds(coverage, pdfCoverage)
            };

            if (!coverage.IsMain)
            {
                return protectionAssure;
            }

            protectionAssure.CapitalAssureASL = coverage.Coverages
                ?.FirstOrDefault(p => p.CoverageCode == ProjectionEnum.Coverage.CoverageCode.PaidUpAddition)?.FaceAmount?.Actual;
            protectionAssure.CapitalAssureOaca = coverage.Coverages
                ?.FirstOrDefault(p => p.CoverageCode == ProjectionEnum.Coverage.CoverageCode.AutomaticOptimizationFaceAmount)?.FaceAmount
                ?.Actual;

            return protectionAssure;
        }

        private static bool EstAvecFraisRachatSelonFonds(ProjectionData.Contract.Coverage.Coverage coverage, IGetPDFICoverageResponse pdfCoverage)
        {
            return pdfCoverage.Coverage.PDA.SurrenderChargeCalculationMethod ==
                   VI.AF.IPDFVie.PDF.PDA.ENUMs.SurrenderChargeCalculationMethod.RegleU &&
                   (coverage.MortalityType == ProjectionEnum.Coverage.MortalityType.YearlyRenewableTerm ||
                    coverage.Insured.Age.Leveling > coverage.Insured.Age.Issuance);
        }

        private static bool EstAvecFraisRachatSelonPrimes(ProjectionData.Contract.Coverage.Coverage coverage, IGetPDFICoverageResponse pdfCoverage)
        {
            return pdfCoverage.Coverage.PDA.SurrenderChargeCalculationMethod ==
                   VI.AF.IPDFVie.PDF.PDA.ENUMs.SurrenderChargeCalculationMethod.RegleR &&
                   (coverage.MortalityType == ProjectionEnum.Coverage.MortalityType.YearlyRenewableTerm ||
                    coverage.Insured.Age.Leveling > coverage.Insured.Age.Issuance);
        }

        private IList<Surprime> ObtenirSurprimes(ProjectionData.Contract.Coverage.Coverage coverage, IList<Client> clients)
        {
            var surprimes = new List<Surprime>();
            if (coverage.Insured?.Joints != null)
            {
                foreach (var joint in coverage.Insured.Joints.Where(j => j.ExtraPremiums != null))
                {
                    var nomAssure = MapperNomSurprime(clients, joint.InsuredIndividual?.Individual);
                    foreach (var extraPremium in joint.ExtraPremiums)
                    {
                        surprimes.Add(ConvertirSurprime(extraPremium, nomAssure, coverage));
                    }
                }
            }

            if (coverage.Insured?.ExtraPremiums != null)
            {
                var nomAssure = MapperNomSurprime(clients, coverage.Insured.InsuredIndividual?.Individual);
                foreach (var extraPremium in coverage.Insured.ExtraPremiums)
                {
                    surprimes.Add(ConvertirSurprime(extraPremium, nomAssure, coverage));
                }
            }

            return surprimes.Where(s => !s.EstV999).ToList();
        }

        private IList<Surprime> ObtenirSurprimesGarantie(ProjectionData.Contract.Coverage.AdditionalBenefit garantie, IList<Client> clients)
        {
            var surprimes = new List<Surprime>();
            string nomAssure;

            if (garantie.Insured?.Joints != null && garantie.Insured.Joints.Any(j => j.ExtraPremiums != null))
            {
                nomAssure = MapperNomSurprimeConjoint(clients, garantie.Insured.Joints);
                IEnumerable<ProjectionData.Contract.Coverage.ExtraPremium> extraPreniums = garantie.Insured.Joints.First(j => j.ExtraPremiums != null).ExtraPremiums;
                surprimes.AddRange(extraPreniums.Select(extraPremium => ConvertirSurprimeGarantie(extraPremium, nomAssure, garantie)));
            }

            if (garantie.Insured?.ExtraPremiums != null)
            {
                nomAssure = MapperNomSurprime(clients, garantie.Insured.InsuredIndividual?.Individual);
                surprimes.AddRange(garantie.Insured.ExtraPremiums.Select(extraPremium => ConvertirSurprimeGarantie(extraPremium, nomAssure, garantie)));
            }

            return surprimes.Where(s => !s.EstV999).ToList();
        }

        private IList<Surprime> ObtenirSurprimesGarantieContractante(ProjectionData.Contract.Coverage.AdditionalBenefit garantie,
            IList<Client> clients)
        {
            var surprimes = new List<Surprime>();
            if (garantie.Insured?.Joints != null)
            {
                foreach (var joint in garantie.Insured.Joints.Where(j => j.ExtraPremiums != null))
                {
                    var nomAssure = MapperNomSurprime(clients,
                        joint.InsuredIndividual?.Individual);
                    foreach (var extraPremium in joint.ExtraPremiums)
                    {
                        surprimes.Add(ConvertirSurprimeGarantie(extraPremium, nomAssure, garantie));
                    }
                }
            }

            if (garantie.Insured?.ExtraPremiums != null)
            {
                var nomAssure = MapperNomSurprime(clients, garantie.Insured.InsuredIndividual?.Individual);
                if (string.IsNullOrEmpty(nomAssure))
                {
                    nomAssure = _resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("Contractants");
                }

                foreach (var extraPremium in garantie.Insured.ExtraPremiums)
                {
                    surprimes.Add(ConvertirSurprimeGarantie(extraPremium, nomAssure, garantie));
                }
            }

            return surprimes.Where(s => !s.EstV999).ToList();
        }

        private string MapperNomSurprime(IList<Client> clients,
            ProjectionData.UniqueIdentifier individualIdentifier)
        {
            return string.IsNullOrEmpty(individualIdentifier?.Id)
                ? string.Empty
                : MapperNomConjoint(clients?.FirstOrDefault(i => i.ReferenceExterneId == individualIdentifier.Id));
        }

        private string MapperNomSurprimeConjoint(IList<Client> clients, IEnumerable<ProjectionData.Contract.Coverage.Joint> conjoints)
        {
            var listeNom = new List<string>();
            foreach (var conjoint in conjoints)
            {
                var id = conjoint.InsuredIndividual?.Individual.Id;
                listeNom.Add(MapperNomConjoint(clients?.FirstOrDefault(i => i.ReferenceExterneId == id)));
            }

            return string.Join("\n", listeNom);
        }

        private Surprime ConvertirSurprime(ProjectionData.Contract.Coverage.ExtraPremium extraPremium, 
            string nomAssure, ProjectionData.Contract.Coverage.Coverage coverage)
        {
            var surprime = new Surprime { Assure = nomAssure, Terme = extraPremium.Term };
            switch (extraPremium.ExtraPremiumType)
            {
                case ProjectionEnum.Coverage.ExtraPremiumType.Unspecified:
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentFlatExtra:
                    surprime.DateLiberation = coverage.Dates.PaidUp;
                    surprime.TauxMontant = Math.Abs(extraPremium.Amount) > 0 ? extraPremium.Amount : default(double?);
                    surprime.EstTypeTemporaire = false;
                    surprime.EstV999 = false;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentRate:
                    surprime.DateLiberation = coverage.Dates.PaidUp;
                    surprime.TauxPourcentage = extraPremium.Percentage < 999
                        ? (double)extraPremium.Percentage / 100
                        : default(double?);
                    surprime.EstV999 = extraPremium.Percentage >= 999;
                    surprime.EstTypeTemporaire = false;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.TemporaryFlatExtra:
                    surprime.DateLiberation = coverage.Dates.Issuance.AddYears(extraPremium.Term);
                    surprime.TauxMontant = Math.Abs(extraPremium.Amount) > 0 ? extraPremium.Amount : default(double?);
                    surprime.EstV999 = false;
                    surprime.EstTypeTemporaire = true;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentTable:
                    surprime.DateLiberation = coverage.Dates.PaidUp;
                    surprime.TauxPourcentage = extraPremium.Percentage < 999
                        ? (double)extraPremium.Percentage / 100
                        : default(double?);
                    surprime.EstV999 = extraPremium.Percentage >= 999;
                    surprime.EstTypeTemporaire = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return surprime;
        }

        private Surprime ConvertirSurprimeGarantie(ProjectionData.Contract.Coverage.ExtraPremium extraPremium, 
            string nomAssure, ProjectionData.Contract.Coverage.AdditionalBenefit garantie)
        {
            var surprime = new Surprime { Assure = nomAssure, Terme = extraPremium.Term };
            switch (extraPremium.ExtraPremiumType)
            {
                case ProjectionEnum.Coverage.ExtraPremiumType.Unspecified:
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentFlatExtra:
                    surprime.DateLiberation = garantie.Dates.PaidUp;
                    surprime.TauxMontant = Math.Abs(extraPremium.Amount) > 0 ? extraPremium.Amount : default(double?);
                    surprime.EstTypeTemporaire = false;
                    surprime.EstV999 = false;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentRate:
                    surprime.DateLiberation = garantie.Dates.PaidUp;
                    surprime.TauxPourcentage = extraPremium.Percentage < 999
                        ? (double)extraPremium.Percentage / 100
                        : default(double?);
                    surprime.EstV999 = extraPremium.Percentage >= 999;
                    surprime.EstTypeTemporaire = false;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.TemporaryFlatExtra:
                    surprime.DateLiberation = garantie.Dates.Issuance.AddYears(extraPremium.Term);
                    surprime.TauxMontant = Math.Abs(extraPremium.Amount) > 0 ? extraPremium.Amount : default(double?);
                    surprime.EstV999 = false;
                    surprime.EstTypeTemporaire = true;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentTable:
                    surprime.DateLiberation = garantie.Dates.PaidUp;
                    surprime.TauxPourcentage = extraPremium.Percentage < 999
                        ? (double)extraPremium.Percentage / 100
                        : default(double?);
                    surprime.EstV999 = extraPremium.Percentage >= 999;
                    surprime.EstTypeTemporaire = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return surprime;
        }

        private string MapperNomConjoint(Client client)
        {
            if (client != null)
            {
                return string.IsNullOrEmpty(client.Initiale)
                           ? $"{client.Prenom} {client.Nom}".Trim()
                           : $"{client.Prenom} {client.Initiale} {client.Nom}".Trim();
            }

            return string.Empty;
        }

        private IEnumerable<Protection> MapGarantieContractant(ProjectionData.Projection projection,
                                                                      IList<Client> clients,
                                                                      DonneesPdf donneesPdf)
        {
            var garanties = projection?.Contract?.Applicants?.AdditionalBenefits;
            var protectionContractante = garanties?.Select(
                                  garantie =>
                                      MapperGarantieContractante(garantie,
                                                                 projection,
                                                                 projection.Contract,
                                                                 clients,
                                                                 donneesPdf
                                                                 )).Where(x => x != null).ToList() ?? new List<Protection>();

            var assures = projection?.Contract?.Insured;
            if (assures == null)
            {
                return protectionContractante;
            }

            foreach (var insured in assures)
            {
                foreach (var coverage in insured.Coverages.Where(c => !c.IsAddedAutomaticallyWhenProjected(projection)))
                {
                    if (coverage.AdditionalBenefits != null)
                    {
                        protectionContractante.AddRange(
                                        coverage.AdditionalBenefits.Select(guarantee =>
                                                                               MapperGarantie(guarantee,
                                                                                  projection,
                                                                                  false,
                                                                                  false,
                                                                                  projection.Contract,
                                                                                  clients,
                                                                                  donneesPdf
                                                                                  )).Where(x => x != null && x.EstProtectionContractant));
                    }
                }
            }

            return protectionContractante;
        }

        private Assure MapperAssureConjoint(ProjectionData.Contract.Coverage.Joint joint,
            ProjectionData.Contract.Individual individual, Client client)
        {
            var nom = client?.Nom;
            return new Assure
            {
                SequenceIndividu = individual.SequenceNumber,
                ReferenceExterneId = individual.Identifier.Id,
                Nom = string.IsNullOrEmpty(nom) ? "Client " + individual.SequenceNumber : nom,
                Prenom = client?.Prenom,
                Initiale = client?.Initiale,
                DateNaissance = individual.Birthdate,
                AgeEsperanceVie = joint.AverageLifeExpectancy,
                AgeAssurance = joint.Age.Issuance,
                EstNonAssurable = joint.IsNotInsurable,
                StatutTabagisme = (StatutTabagisme)joint.SmokerType
            };
        }

        private Assure MapperAssure(ProjectionData.Contract.Coverage.Insured insured,
            ProjectionData.Contract.Individual individual, Client client)
        {
            var nom = client?.Nom;
            return new Assure
            {
                SequenceIndividu = individual.SequenceNumber,
                ReferenceExterneId = individual.Identifier.Id,
                Nom = string.IsNullOrEmpty(nom) ? "Client " + individual.SequenceNumber : nom,
                Prenom = client?.Prenom,
                Initiale = client?.Initiale,
                DateNaissance = individual.Birthdate,
                AgeEsperanceVie = insured.AverageLifeExpectancy,
                AgeAssurance = insured.Age.Issuance,
                StatutTabagisme = (StatutTabagisme)insured.SmokerType
            };
        }

        private IList<Assure> MapperAssures(ProjectionData.Contract.Coverage.Insured insured,
            ProjectionData.Contract.Contract contract, IEnumerable<Client> clients)
        {
            var result = new List<Assure>();
            if (insured?.Joints != null)
            {
                result.AddRange(
                                insured.Joints.Select(
                                                      conjoint =>
                                                      MapperAssureConjoint(conjoint,
                                                                           contract.Individuals?.FirstOrDefault(x => x.Identifier.Id == conjoint.InsuredIndividual?.Individual?.Id),
                                                          clients?.FirstOrDefault(x => x.ReferenceExterneId == conjoint.InsuredIndividual?.Individual?.Id))));
            }

            if (insured?.InsuredIndividual?.Individual != null && !result.Any())
            {
                result.Add(
                    MapperAssure(
                        insured,
                        contract.Individuals?.FirstOrDefault(x => x.Identifier.Id == insured.InsuredIndividual?.Individual?.Id),
                    clients?.FirstOrDefault(x => x.ReferenceExterneId == insured.InsuredIndividual?.Individual?.Id)));
            }

            return result;
        }

        private double? DeterminerMontantReductionCapitalAssure(ProjectionData.Transactions.Transactions transactions)
        {
            return
                transactions?.FaceAmountChanges?.FirstOrDefault(
                    x => x.FaceAmountChangeType == ProjectionEnum.Coverage.FaceAmountChangeType.Reduction)?.Amount;
        }

        private double ObtenirDureeDePaiment(
            IEnumerable<KeyValuePair<ProjectionData.Characteristics.Characteristic, double>> coveragesValue, 
            string coverageId, DonneesPdf donneesPdf)
        {
            var protectionPdf = donneesPdf?.ProtectionsPdf?.FirstOrDefault(p => p.IdProtection == coverageId);
            if (protectionPdf == null) throw new ArgumentNullException(nameof(protectionPdf));
            if (protectionPdf.Specification.IsTermCoverage && 
                protectionPdf.Specification.IsRenewable && 
                protectionPdf.TermesDetails?.CoverageTerm != null)
            {
                return protectionPdf.TermesDetails.CoverageTerm.Value;
            }

            return coveragesValue.SearchByCoverage(coverageId, ProjectionEnum.ValueId.PaymentDuration) ?? 0;
        }

        private int? ObtenirDureeDeRenouvellement(List<ProjectionData.Transactions.Coverage.Termination> terminations, 
            string coverageId, DonneesPdf donneesPdf, int dureeProtection)
        {
            var protectionPdf = donneesPdf?.ProtectionsPdf?.SingleOrDefault(p => p.IdProtection == coverageId);
            if (protectionPdf == null) throw new ArgumentNullException(nameof(protectionPdf));
            if (protectionPdf.Specification.IsRenewable && 
                protectionPdf.Specification.IsResiliable && 
                protectionPdf.TermesDetails.CoverageTerm.HasValue)
            {
                var termination = terminations?.FirstOrDefault(p => p.CoverageIdentifier.Id == coverageId);
                if (termination != null)
                {
                    return termination.StartDate.Year - 1;
                }
                else
                {
                    return dureeProtection;
                }
            }

            return null;
        }

        public Participations MapParticipations(ProjectionData.Projection projection, ParametreRapport parametreRapport)
        {
            if (projection.Contract?.TraditionalOptions == null ||
                projection.Contract.TraditionalOptions.DividendOption == ProjectionEnum.Traditional.DividendOption.Unspecified)
            {
                return null;
            }

            var result = new Participations
            {
                OptionParticipation = (TypeOptionParticipation)projection.Contract.TraditionalOptions.DividendOption,
                ReductionBaremeParticipation = parametreRapport?.ReductionBaremeParticipation,
                Baremes = MapperBaremes(projection, parametreRapport?.ReductionBaremeParticipation),
                EstEclipseDePrimeActivee = EstEclipseDePrimeActivee(projection),
                EstChangementOptionParticipationActivee = EstChangementOptionParticipationActivee(projection),
                AnneeChangementOptionParticipation = AnneeChangementOptionParicipation(projection),
                SoldeParticipationsEnDepot = projection.Contract.TraditionalOptions.Dividends?.BalanceOfDividendsOnDepositAccount
            };

            return result;
        }

        private static bool EstEclipseDePrimeActivee(ProjectionData.Projection projection)
        {
            var premiumOffsetYear = projection?.Values.Search(ProjectionEnum.ValueId.PremiumOffSetYear) ?? 0;
            if (premiumOffsetYear > 0)
            {
                var vecteur = projection?.Illustration?.GetColumnValuesByContract(2022);
                if (vecteur != null && premiumOffsetYear < vecteur.Length)
                {
                    return vecteur[(int)premiumOffsetYear] > 0;
                }
            }

            return false;
        }

        private static bool EstChangementOptionParticipationActivee(ProjectionData.Projection projection)
        {
            return AnneeChangementOptionParicipation(projection) > 0;
        }

        private static int AnneeChangementOptionParicipation(ProjectionData.Projection projection)
        {
            var annee = projection?.Values.Search(ProjectionEnum.ValueId.CashDividendOptionChangeYear) ?? 0;
            return (int)annee;
        }

        private IList<Bareme> MapperBaremes(ProjectionData.Projection projection, double? reduction)
        {
            var baremes = new List<Bareme>();
            MapperBareme(baremes, projection.Values.Search(ProjectionEnum.ValueId.PremiumOffSetYear));

            if (projection?.SensitivityTests?.Results != null && projection.SensitivityTests.Results.Any())
            {
                var defavourable = projection.SensitivityTests.Results.FirstOrDefault(
                    x => !x.Variances.IsFavourable && (reduction == null || x.Variances.VarianceInterestAccounts == reduction));

                if (defavourable != null)
                {
                    MapperBareme(baremes,
                        defavourable.Values.Search(ProjectionEnum.ValueId.PremiumOffSetYear),
                        defavourable.Variances.VarianceInterestAccounts);
                }
            }

            return baremes;
        }

        private void MapperBareme(IList<Bareme> baremes, double? premiumOffsetYear, double? diminution = null)
        {
            if (premiumOffsetYear.HasValue)
            {
                baremes.Add(new Bareme() { Annee = (int)premiumOffsetYear.Value, Diminution = diminution });
            }
        }

        private static bool DetermineSiExisteProtectionTemporaireRenouvelable(DonneesPdf donneesPdf)
        {
            if (donneesPdf.ProtectionsPdf == null || !donneesPdf.ProtectionsPdf.Any())
            {
                throw new ArgumentNullException(nameof(donneesPdf.ProtectionsPdf));
            }

            foreach (var item in donneesPdf.ProtectionsPdf)
            {
                if (item.Specification.IsRenewable && item.TermesDetails.CoverageTerm.HasValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
