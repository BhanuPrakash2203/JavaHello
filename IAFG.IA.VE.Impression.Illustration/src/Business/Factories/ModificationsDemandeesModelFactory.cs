using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionProtection;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories
{   
    public class ModificationsDemandeesModelFactory : IModificationsDemandeesModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private readonly ISectionModelMapper _sectionModelMapper;
        private bool _languageIsFrench;

        public ModificationsDemandeesModelFactory(IConfigurationRepository configurationRepository,
           IIllustrationResourcesAccessorFactory resourcesAccessorFactory, ISectionModelMapper sectionModelMapper)
        {
            _configurationRepository = configurationRepository;
            _resourcesAccessorFactory = resourcesAccessorFactory;
            _sectionModelMapper = sectionModelMapper;
        }

        public SectionModificationsDemandeesModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            _languageIsFrench = context.Language == Language.French;
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionModificationsDemandeesModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SectionContratModel = MapperModificationContrat(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Contrat"), donnees, context);
            model.SectionProtectionsModel = MapperModificationProtections(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Protections"), donnees, context);
            return model;
        }

        internal SectionContratModel MapperModificationContrat(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionContratModel
            {
                Transactions = MapperTransactionsContrat(donnees.ModificationsDemandees?.Contrat?.Transactions)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }
        
        internal SectionProtectionsModel MapperModificationProtections(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionProtectionsModel
            {
                Protections = new List<ProtectionModel>()
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            MapperProtections(donnees.ModificationsDemandees?.Protections?.Values.ToList(), section.Protections);
            return section;        
        }

        private void MapperProtections(IEnumerable<ModificationsProtection> modifications, ICollection<ProtectionModel> protections)
        {
            if (modifications == null) return;
            foreach (var item in modifications)
            {
                var p = new ProtectionModel
                {
                    Id = item.Protection.ReferenceExterneId,
                    Libelle = _languageIsFrench
                        ? item.Protection.Plan.DescriptionFr
                        : item.Protection.Plan.DescriptionAn,
                    Assures = item.Protection.Assures?.Individus?.Select(x =>
                            new AssureModel
                            {
                                Nom = x.Nom,
                                Prenom = x.Prenom,
                                Initiale = x.Initiale,
                            })
                        .ToList(),
                    Capital = !item.Protection.Plan.SansVolumeAssurance ? item.Protection.CapitalAssureActuel : null,
                    Transactions = MapperTransactionsProtection(item.Transactions)
                };
                protections.Add(p);
            }
        }

        private List<TransactionModel> MapperTransactionsContrat(List<Transaction> transactions)
        {
            var result = new List<TransactionModel>();
            if (transactions == null) return result;
            MapperTransactionDesactivationOptimisation(result,
                transactions.Select(x => x as DesactivationOptimisationCapitalAssure)
                    .Where(x => x != null).ToList());
            MapperTransactionChangementPrestationDeces(result,
                transactions.Select(x => x as ChangementPrestationDeces)
                    .Where(x => x != null).ToList());
            MapperTransactionChangementOptionAssuranceSupplementaire(result,
                transactions.Select(x => x as ChangementOptionAssuranceSupplementaireLiberee)
                    .Where(x => x != null).ToList());
            MapperTransactionChangementOptionParticipation(result,
                transactions.Select(x => x as ChangementOptionParticipation)
                    .Where(x => x != null).ToList());
            return result;
        }

        private List<TransactionModel> MapperTransactionsProtection(List<Transaction> transactions)
        {
            var result = new List<TransactionModel>();
            if (transactions == null) return result;
            MapperTransactionNivellements(result,
                transactions.Select(x => x as Nivellement).Where(x => x != null).ToList());
            MapperTransactionReductionCapital(result,
                transactions.Select(x => x as ReductionCapital).Where(x => x != null).ToList());
            MapperTransactionChangementUsageTabac(result,
                transactions.Select(x => x as ChangementUsageTabac).Where(x => x != null).ToList());
            MapperTransactionTerminaisons(result,
                transactions.Select(x => x as Terminaison).Where(x => x != null).ToList());
            MapperTransactionAjoutOption(result,
                transactions.Select(x => x as AjoutOption).Where(x => x != null).ToList());
            MapperTransactionTransformationConjointDernierDeces(result,
                transactions.Select(x => x as TransformationConjointDernierDeces).Where(x => x != null)
                    .ToList());
            MapperTransactionAjoutProtection(result,
                transactions.Select(x => x as AjoutProtection)
                    .Where(x => x != null).ToList());
            return result;
        }

        private void MapperTransactionAjoutProtection(List<TransactionModel> result, IEnumerable<AjoutProtection> transactions)
        {
            result.AddRange(transactions.Select(transaction =>
                new TransactionAjoutProtectionModel
                {
                    Annee = transaction.Annee,
                    Descpription = _languageIsFrench
                        ? "Ajout de protection à l'année {0}"
                        : "Addition of coverage in year {0}",
                    DescpriptionProtection = _languageIsFrench
                        ? transaction.Plan.DescriptionFr
                        : transaction.Plan.DescriptionAn,
                    DescpriptionMontantPrime = _languageIsFrench
                        ? "Prime de : {0}"
                        : "Premium of: {0}",
                    MontantPrime = transaction.MontantPrime
                })
            );
        }

        private void MapperTransactionChangementOptionAssuranceSupplementaire(List<TransactionModel> result,
            IEnumerable<ChangementOptionAssuranceSupplementaireLiberee> transactions)
        {
            result.AddRange(transactions.Select(transaction =>
                new TransactionChangementOptionAssuranceSupplementaireLibereeModel
                {
                    Annee = transaction.Annee,
                    CapitalAssurePlafond = transaction.CapitalAssurePlafond,
                    AucunAchat = transaction.AucunAchat,
                    AucunMaximum = transaction.AucunMaximum,
                    MontantAllocation = transaction.MontantAllocation,
                    OptionVersementBoni = transaction.OptionVersementBoni,
                    Descpription = _languageIsFrench
                        ? "Changement de l'option ASL à l'année {0}"
                        : "Change of the PUA Option in year {0}",
                    DescpriptionOptionAchat = _languageIsFrench
                        ? "Option de versement du Boni Valeur : {0}"
                        : "Bonus Payment Option: {0}",
                    DescpriptionMontantAllocation = _languageIsFrench
                        ? "Montant allocation : {0}"
                        : "Allocation amount: {0}"
                })
            );
        }

        private void MapperTransactionChangementOptionParticipation(List<TransactionModel> result,
            IEnumerable<ChangementOptionParticipation> transactions)
        {
            result.AddRange(transactions.Select(transaction =>
                new TransactionChangementOptionParticipantModel
                {
                    Annee = transaction.Annee,
                    Option = transaction.Option,
                    Descpription = _languageIsFrench
                        ? "Changement de l'option d'affectation des participations à l'année {0}"
                        : "Change of the dividend option in year {0}",
                    DescpriptionOption = _languageIsFrench
                        ? "Option d'affectation des participations : {0}"
                        : "Dividend option: {0}"
                })
            );
        }

        private void MapperTransactionChangementPrestationDeces(List<TransactionModel> result,
            IEnumerable<ChangementPrestationDeces> transactions)
        {
            result.AddRange(transactions.Select(transaction =>
                new TransactionChangementPrestationDecesModel
                {
                    Annee = transaction.Annee,
                    Descpription = _languageIsFrench
                        ? "Changement de prestation de décès à l'année {0}"
                        : "Death Benefit Option Change in year {0}",
                    DescpriptionOption = _languageIsFrench
                        ? "Nouvelle prestation de décès : {0}"
                        : "New Death Benefit: {0}",
                    OptionPrestationDeces = transaction.OptionPrestationDeces
                })
            );
        }
        
        private void MapperTransactionDesactivationOptimisation(List<TransactionModel> result,
            IEnumerable<DesactivationOptimisationCapitalAssure> transactions)
        {
            result.AddRange(transactions.Select(transaction =>
                new TransactionDesactivationOptimisationAutomatiqueCapitalAssureModel
                {
                    Annee = transaction.Annee,
                    Descpription = _languageIsFrench
                        ? "Désactivation de l'optimisation automatique du capital assuré (OACA) à l'année {0}"
                        : "Disabling of the Automatic optimization of the face amount (AOFA) in year {0}"
                })
            );
        }

        private void MapperTransactionTransformationConjointDernierDeces(List<TransactionModel> result,
            IEnumerable<TransformationConjointDernierDeces> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionTransformationConjointDernierDecesModel
                {
                    Annee = transaction.Annee,
                    Descpription = _languageIsFrench
                        ? "Transformation d’une protection individuelle en protection conjointe dernier décès à l'année {0}"
                        : "Modification of an Individual coverage to a Joint last-to-die coverage in year {0}",
                    Surprimes = MapperSuprimes(transaction.Surprimes)
                })
            );
        }

        private List<SurprimeModel> MapperSuprimes(List<Surprime> surprimes)
        {
            var result = new List<SurprimeModel>();
            if (surprimes == null) return result;
            var ressourceAccessor = _resourcesAccessorFactory.GetResourcesAccessor();
            result.AddRange(surprimes.Select(item => new SurprimeModel
            {
                Descpription = item.EstTypeTemporaire
                    ? ressourceAccessor.GetStringResourceById("_SurprimeTemporaire")
                    : ressourceAccessor.GetStringResourceById("_SurprimePermanente"),
                Terme = item.EstTypeTemporaire ? item.Terme : default(int?),
                TauxMontant = item.TauxMontant,
                TauxPourcentage = item.TauxPourcentage
            }));
            return result;
        }

        private void MapperTransactionTerminaisons(List<TransactionModel> result,
            IEnumerable<Terminaison> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionTerminaisonModel
                {
                    Annee = transaction.Annee,
                    Descpription = _languageIsFrench
                        ? "Terminaison de la protection demandée à l'année {0}"
                        : "Termination of the coverage in year {0}"
                })
            );
        }

        private void MapperTransactionChangementUsageTabac(List<TransactionModel> result,
            IEnumerable<ChangementUsageTabac> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionChangementUsageTabacModel
                {
                    Annee = transaction.Annee,
                    Nom = transaction.Nom,
                    Prenom = transaction.Prenom,
                    Initiale = transaction.Initiale,
                    StatutTabagisme = transaction.StatutTabagisme,
                    Descpription = _languageIsFrench
                        ? "Changement d'usage de tabac à l'année {0}"
                        : "Change of use of tobacco in year {0}"
                })
            );
        }

        private void MapperTransactionReductionCapital(List<TransactionModel> result,
            IEnumerable<ReductionCapital> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionReductionCapitalModel
                {
                    Annee = transaction.Annee,
                    Montant = transaction.Montant,
                    Descpription = _languageIsFrench
                        ? "Réduction du capital assuré à l'année {0}"
                        : "Face amount reduction in year {0}",
                    DescpriptionMontant = _languageIsFrench
                        ? "Nouveau capital assuré : {0}"
                        : "New face amount: {0}"
                })
            );
        }

        private void MapperTransactionAjoutOption(List<TransactionModel> result,
            IEnumerable<AjoutOption> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionAjoutOptionModel
                {
                    Annee = transaction.Annee,
                    MontantPrime = transaction.MontantPrime,
                    Descpription = _languageIsFrench
                        ? "Ajout d'option à l'année {0}"
                        : "Option added in year {0}",
                    DescpriptionOption = _languageIsFrench
                        ? transaction.Plan.DescriptionFr
                        : transaction.Plan.DescriptionAn,
                    DescpriptionMontantPrime = !transaction.MontantPrime.HasValue ? string.Empty : _languageIsFrench ? "Prime de : {0}" : "Premium of: {0}",
            })
            );
        }

        private void MapperTransactionNivellements(List<TransactionModel> result,
            IEnumerable<Nivellement> transactions)
        {
            result.AddRange(transactions.Select(transaction => new TransactionNivellementModel
                {
                    Annee = transaction.Annee,
                    Age = transaction.Age,
                    AgeSurprime = transaction.AgeSurprime,
                    Descpription = _languageIsFrench
                        ? "Nivellement à l'année {0}"
                        : "Leveling in year {0}",
                    DescpriptionAge = _languageIsFrench
                        ? "Âge au nivellement : {0}"
                        : "Leveling age: {0}",
                    DescpriptionAgeSurprime = _languageIsFrench
                        ? "Âge équivalent surprimé : {0}"
                        : "Rated Equivalent age: {0}"
                })
            );
        }
    }
}