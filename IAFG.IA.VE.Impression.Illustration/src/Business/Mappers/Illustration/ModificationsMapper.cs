using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionProtection;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnum = IAFG.IA.VI.Projection.Data.Enums;
using ProjectionTransactions = IAFG.IA.VI.Projection.Data.Transactions;
using IAFG.IA.VI.Projection.Data.Extensions;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class ModificationsMapper : IModificationsMapper
    {
        private readonly IRegleAffaireAccessor _regleAffaireAccessor;
        
        public ModificationsMapper(IRegleAffaireAccessor regleAffaireAccessor)
        {
            _regleAffaireAccessor = regleAffaireAccessor;
        }

        public Types.Models.ModificationsDemandees.ModificationsDemandees MapModificationsDemandees(
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            var result = new Types.Models.ModificationsDemandees.ModificationsDemandees
            {
                Contrat = new ModificationsContrat(),
                Protections = new Dictionary<string, ModificationsProtection>()
            };

            if (result.Contrat == null)
            {
                result.Contrat = new ModificationsContrat();
            }

            if (result.Contrat.Transactions == null)
            {
                result.Contrat.Transactions = new List<Transaction>();
            }

            if (result.Protections == null)
            {
                result.Protections = new Dictionary<string, ModificationsProtection>();
            }

            MapperAjoutProtection(result, projection, clients, dateEmission);
            MapperChangementOptionAssuranceSupplementaire(result, projection, dateEmission);
            MapperChangementOptionParticipation(result, projection, dateEmission);
            MapperChangementPrestationDeces(result, projection, dateEmission);
            MapperDesactivationOptimisationAutomatiqueCapitalAssure(result, projection, dateEmission);
            MapperNivellements(result, projection, clients, dateEmission);
            MapperTerminaisons(result, projection, clients, dateEmission);
            MapperChangementCapitalAssure(result, projection, clients, dateEmission);
            MapperChangementUsageTabac(result, projection, clients, dateEmission);
            MapperAjoutOption(result, projection, clients, dateEmission);
            MapperChangementTypeAssurance(result, projection, clients, dateEmission);

            return result;
        }

        private void MapperAjoutProtection(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.CoverageAdditions == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.CoverageAdditions)
            {
                var mappedProtection = MapperProtection(projection, clients, transaction.CoverageIdentifier.Id);
                if (mappedProtection == null)
                {
                    continue;
                }

                var montantPrime = projection.Values?.SearchByCoverage(mappedProtection.ReferenceExterneId,
                    ProjectionEnum.ValueId.TotalPremium);

                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                modificationProtection.Transactions.Add(new AjoutProtection
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Plan = mappedProtection.Plan,
                    CapitalAssureActuel = mappedProtection.CapitalAssureActuel,
                    TypeAssurance = mappedProtection.TypeAssurance,
                    TypeCout = mappedProtection.TypeCout,
                    Assures = mappedProtection.Assures,
                    MontantPrime = montantPrime
                });
            }
        }

        private void MapperChangementPrestationDeces(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection?.Transactions?.DeathBenefitOptionChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.DeathBenefitOptionChanges)
            {
                modifications.Contrat.Transactions.Add(new ChangementPrestationDeces
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    OptionPrestationDeces = (OptionPrestationDeces)transaction.DeathBenefitOption
                });
            }
        }

        private void MapperChangementOptionAssuranceSupplementaire(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection?.Transactions?.PaidUpAdditionalOptionChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.PaidUpAdditionalOptionChanges)
            {
                var aucunAchat = Math.Abs(transaction.PaidUpAdditionalOption.MaximalFaceAmount) < .009;
                var aucunMaximum = transaction.PaidUpAdditionalOption.MaximalFaceAmount >= 999999999;

                modifications.Contrat.Transactions.Add(new ChangementOptionAssuranceSupplementaireLiberee
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    OptionVersementBoni = (TypeOptionVersementBoni)transaction.PaidUpAdditionalOption.PurchaseOption,
                    MontantAllocation = transaction.PaidUpAdditionalOption.AllocationAmount > 0 ? transaction.PaidUpAdditionalOption.AllocationAmount : default(double?),
                    CapitalAssurePlafond = !aucunAchat && !aucunMaximum ? transaction.PaidUpAdditionalOption.MaximalFaceAmount : default(double?),
                    AucunAchat = aucunAchat,
                    AucunMaximum = aucunMaximum
                });
            }
        }

        private void MapperChangementOptionParticipation(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications, 
            ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection?.Transactions?.Participating?.OptionChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.Participating.OptionChanges)
            {
                modifications.Contrat.Transactions.Add(new ChangementOptionParticipation
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Option = (TypeOptionParticipation) transaction.Option,
                });
            }
        }

        private void MapperDesactivationOptimisationAutomatiqueCapitalAssure(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, DateTime dateEmission)
        {
            if (projection?.Transactions?.DisableAutomaticFaceAmountOptimization == null)
            {
                return;
            }

            modifications.Contrat.Transactions.Add(new DesactivationOptimisationCapitalAssure
            {
                Annee = projection.Transactions.DisableAutomaticFaceAmountOptimization.StartDate
                    .CalculerAnneeContratProjection(dateEmission)
            });
        }

        private void MapperChangementTypeAssurance(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.InsuranceTypeChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.InsuranceTypeChanges)
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                modificationProtection.Transactions.Add(new TransformationConjointDernierDeces
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Surprimes = MapperSurprimes(transaction.ExtraPremiums)
                });
            }
        }

        private List<Surprime> MapperSurprimes(List<ProjectionTransactions.Coverage.ExtraPremium> extraPremiums)
        {
            var result = new List<Surprime>();
            if (extraPremiums == null) return result;
            result.AddRange(extraPremiums.Select(MapperSurprime));
            return result;
        }

        private Surprime MapperSurprime(ProjectionTransactions.Coverage.ExtraPremium extraPremium)
        {
            return MapperSurprime(extraPremium.ExtraPremiumType, extraPremium.Term, extraPremium.Amount,
                extraPremium.Percentage);
        }

        private List<Surprime> MapperSurprimes(List<ProjectionData.Contract.Coverage.ExtraPremium> extraPremiums)
        {
            var result = new List<Surprime>();
            if (extraPremiums == null) return result;
            result.AddRange(extraPremiums.Select(MapperSurprime));
            return result;
        }

        private Surprime MapperSurprime(ProjectionData.Contract.Coverage.ExtraPremium extraPremium)
        {
            return MapperSurprime(extraPremium.ExtraPremiumType, extraPremium.Term, extraPremium.Amount,
                extraPremium.Percentage);
        }

        private Surprime MapperSurprime(ProjectionEnum.Coverage.ExtraPremiumType extraPremiumType, int term,
            double amount, int percentage)
        {
            var surprime = new Surprime
            {
                Terme = term,
                EstTypeTemporaire = false,
                EstV999 = false
            };

            switch (extraPremiumType)
            {
                case ProjectionEnum.Coverage.ExtraPremiumType.Unspecified:
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentFlatExtra:
                    surprime.Terme = term;
                    surprime.TauxMontant = amount > 0 ? amount : default(double?);
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentRate:
                case ProjectionEnum.Coverage.ExtraPremiumType.PermanentTable:
                    surprime.TauxPourcentage = percentage < 999
                        ? (double)percentage / 100
                        : default(double?);
                    surprime.EstV999 = percentage >= 999;
                    break;
                case ProjectionEnum.Coverage.ExtraPremiumType.TemporaryFlatExtra:
                    surprime.TauxMontant = amount > 0 ? amount : default(double?);
                    surprime.EstTypeTemporaire = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return surprime;
        }

        private  void MapperAjoutOption(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.ExtendedCoverageAdditions == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.ExtendedCoverageAdditions)
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                var option = modificationProtection.Protection.Coverage.Coverages?.FirstOrDefault(x =>
                    x.Identifier.Id == transaction.ExtendedCoverageIdentifier.Id);

                if (option == null)
                {
                    option = modificationProtection.Protection.Coverage.Coverages?.FirstOrDefault(x =>
                        x.Identifier.Id.StartsWith(transaction.ExtendedCoverageIdentifier.Id));
                }

                if (option == null)
                {
                    var plan = _regleAffaireAccessor.ObtenirPlan(transaction.PlanCode);
                    modificationProtection.Transactions.Add(new AjoutOption
                    {
                        Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                        Plan = ObtenirPlan(plan, null)
                    });
                    continue;
                }

                var mappedOption = MapperProtection(option, projection, clients);
                var montantPrime = projection.Values?.SearchByCoverage(option.Identifier.Id, ProjectionEnum.ValueId.TotalPremium);

                modificationProtection.Transactions.Add(new AjoutOption
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Plan = mappedOption.Plan,
                    MontantPrime = montantPrime
                });
            }
        }

        private void MapperChangementUsageTabac(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.TobaccoUsageChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.TobaccoUsageChanges)
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                var client = clients.FirstOrDefault(x => x.ReferenceExterneId == transaction.IndividualIdentifier.Id);
                modificationProtection.Transactions.Add(new ChangementUsageTabac
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    StatutTabagisme = (StatutTabagisme)transaction.SmokerType,
                    Nom = client?.Nom,
                    Prenom = client?.Prenom,
                    Initiale = client?.Initiale
                });
            }
        }

        private void MapperChangementCapitalAssure(
            Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.FaceAmountChanges == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.FaceAmountChanges.Where(x =>
                x.FaceAmountChangeType == ProjectionEnum.Coverage.FaceAmountChangeType.Reduction))
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                modificationProtection.Transactions.Add(new ReductionCapital
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission),
                    Montant = transaction.Amount
                });
            }
        }

        private void MapperTerminaisons(Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.Terminations == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.Terminations)
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                modificationProtection.Transactions.Add(new Terminaison
                {
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission)
                });
            }
        }

        private void MapperNivellements(Types.Models.ModificationsDemandees.ModificationsDemandees modifications,
            ProjectionData.Projection projection, List<Client> clients, DateTime dateEmission)
        {
            if (projection?.Transactions?.Levelings == null)
            {
                return;
            }

            foreach (var transaction in projection.Transactions.Levelings)
            {
                var modificationProtection = TrouverModificationProtection(modifications.Protections,
                    transaction.CoverageIdentifier.Id, projection, clients);

                modificationProtection.Transactions.Add(new Nivellement
                {
                    Age = transaction.Age,
                    AgeSurprime = transaction.ExtraPremiumAge,
                    Annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission)
                });
            }
        }

        private ModificationsProtection TrouverModificationProtection(
            IDictionary<string, ModificationsProtection> modifications, string id, ProjectionData.Projection projection,
            IList<Client> clients)
        {
            if (modifications.ContainsKey(id))
            {
                return modifications.FirstOrDefault(x => x.Key == id).Value;
            }

            var protection = MapperProtection(projection, clients, id);
            if (protection == null)
            {
                throw new NullReferenceException(
                    $"La protection spécifiée dans la transaction est introuvable: {id}");
            }

            var modification = new ModificationsProtection
            {
                Protection = protection,
                Transactions = new List<Transaction>()
            };

            modifications.Add(protection.ReferenceExterneId, modification);
            return modification;
        }

        private Protection MapperProtection(ProjectionData.Projection projection,
            IList<Client> clients, string id)
        {
            var result = MapperAdditionalBenefits(projection?.Contract?.Applicants?.AdditionalBenefits, projection,
                clients, id);

            if (result != null)
            {
                return result;
            }

            if (projection?.Contract?.Insured == null)
            {
                return null;
            }

            foreach (var insured in projection.Contract.Insured)
            {
                result = MapperProtection(insured.Coverages, projection, clients, id);
                if (result != null) return result;
            }

            return null;
        }

        private Protection MapperProtection(IEnumerable<ProjectionData.Contract.Coverage.Coverage> coverages,
            ProjectionData.Projection projection, IList<Client> clients, string id)
        {
            if (coverages == null)
            {
                return null;
            }

            foreach (var coverage in coverages)
            {
                if (coverage.Identifier.Id == id)
                {
                    return MapperProtection(coverage, projection, clients);
                }

                var result = MapperAdditionalBenefits(coverage.AdditionalBenefits, projection, clients, id);

                if (result != null)
                {
                    return result;
                }

                result = MapperProtection(coverage.Coverages, projection, clients, id);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private Protection MapperAdditionalBenefits(
            List<ProjectionData.Contract.Coverage.AdditionalBenefit> additionalBenefits,
            ProjectionData.Projection projection, IList<Client> clients, string id)
        {
            if (additionalBenefits == null)
            {
                return null;
            }

            foreach (var additionalBenefit in additionalBenefits)
            {
                if (additionalBenefit.Identifier.Id == id)
                {
                    return MapperProtection(additionalBenefit, projection, clients);
                }
            }

            return null;
        }

        private Protection MapperProtection(ProjectionData.Contract.Coverage.Coverage coverage,
            ProjectionData.Projection projection, IList<Client> clients)
        {
            var planInfo = _regleAffaireAccessor.ObtenirPlan(coverage.PlanCode);
            var pdfCoverage = _regleAffaireAccessor.GetPdfCoverage(projection, coverage);
            var plan = ObtenirPlan(planInfo, pdfCoverage);
            var assures = MapperAssures(coverage.Insured, projection.Contract, clients);

            return new Protection
            {
                ReferenceExterneId = coverage.Identifier.Id,
                TypeAssurance = coverage.InsuranceType.ConvertirTypeAssurance(),
                TypeCout = (TypeCout)coverage.MortalityType,
                CapitalAssureActuel = coverage.FaceAmount?.Actual,
                Plan = plan,
                Assures = assures,
                Coverage = coverage
            };
        }

        private Assures MapperAssures(ProjectionData.Contract.Coverage.Insured insured,
            ProjectionData.Contract.Contract contract, IList<Client> clients)
        {
            var result = new Assures
            {
                Individus = new List<Individu>()
            };

            if (insured?.Joints != null)
            {
                result.Individus.AddRange(
                    insured.Joints.Select(
                        conjoint =>
                            MapperAssure(
                                contract.Individuals?.FirstOrDefault(x =>
                                    x.Identifier.Id == conjoint.InsuredIndividual?.Individual?.Id),
                                clients?.FirstOrDefault(x =>
                                    x.ReferenceExterneId == conjoint.InsuredIndividual?.Individual?.Id),
                                conjoint.SmokerType, conjoint.IsNotInsurable, conjoint.ExtraPremiums)));
            }

            if (insured?.InsuredIndividual?.Individual != null && !result.Individus.Any())
            {
                result.Individus.Add(MapperAssure(
                    contract.Individuals?.FirstOrDefault(x =>
                        x.Identifier.Id == insured.InsuredIndividual?.Individual?.Id),
                    clients?.FirstOrDefault(x => x.ReferenceExterneId == insured.InsuredIndividual?.Individual?.Id),
                    insured.SmokerType, null, insured.ExtraPremiums));
            }

            return result;
        }

        private Individu MapperAssure(ProjectionData.Contract.Individual individual, Client client,
            ProjectionEnum.SmokerType smokerType, bool? isNotInsurable,
            List<ProjectionData.Contract.Coverage.ExtraPremium> extraPremiums)
        {
            var surprimes = MapperSurprimes(extraPremiums);
            var sexe = client?.Sexe ?? (Sexe?)individual.Sex;
            if (sexe == Sexe.NonDefini || sexe == Sexe.Inconnu)
            {
                sexe = null;
            }

            var statutTabagisme = (StatutTabagisme?)smokerType;
            if (statutTabagisme.Value == StatutTabagisme.NonDefini)
            {
                statutTabagisme = client?.StatutFumeur;
            }

            return new Individu
            {
                Nom = string.IsNullOrEmpty(client?.Nom) ? "Client " + individual.SequenceNumber : client.Nom,
                Prenom = client?.Prenom,
                Initiale = client?.Initiale,
                EstNonAssurable = isNotInsurable ?? client?.IsNotAssurable,
                Sexe = sexe,
                StatutTabagisme = statutTabagisme,
                Surprimes = surprimes
            };
        }

        private Plan ObtenirPlan(IPlanInfo planInfo, IGetPDFICoverageResponse pdfCoverage)
        {
            return new Plan
            {
                AgeMaturite = planInfo.AgeMaturite,
                CodePlan = planInfo.CodePlan,
                DescriptionAn = planInfo.DescriptionAn,
                DescriptionFr = planInfo.DescriptionFr,
                SansVolumeAssurance = pdfCoverage?.Coverage?.Plan?.SansVolumeAssurance ?? false
            };
        }

        private Protection MapperProtection(ProjectionData.Contract.Coverage.AdditionalBenefit additionalBenefit,
            ProjectionData.Projection projection, IList<Client> clients)
        {
            var planInfo = _regleAffaireAccessor.ObtenirPlan(additionalBenefit.PlanCode);
            var pdfCoverage = _regleAffaireAccessor.GetPdfCoverage(projection, additionalBenefit);
            var plan = ObtenirPlan(planInfo, pdfCoverage);
            var assures = MapperAssures(additionalBenefit.Insured, projection.Contract, clients);

            return new Protection
            {
                ReferenceExterneId = additionalBenefit.Identifier.Id,
                TypeAssurance = additionalBenefit.InsuranceType.ConvertirTypeAssurance(),
                CapitalAssureActuel = additionalBenefit.FaceAmount?.Actual,
                Plan = plan,
                Assures = assures
            };
        }
    }
}