using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using ProjectionIllustration = IAFG.IA.VI.Projection.Data.Illustration;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnum = IAFG.IA.VI.Projection.Data.Enums;
using ProjectionMessages = IAFG.IA.VI.Projection.Messages;
using IAFG.IA.VI.Projection.DataExtensions;
using IAFG.IA.VI.Projection.Data.Extensions;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    public class ProjectionsMapper : IProjectionsMapper
    {
        public Projections Map(ProjectionData.Projections projections, 
            ParametreRapport parametreRapport, ConfigurationRapport configurationRapport)
        {
            var projection = projections.GetDefaultProjection();
            var result = new Projections
            {
                Projection = new Projection
                {
                    Columns = MapperColumns(projection?.Illustration?.Columns),
                    ColumnDescriptions = MapperColumnDescriptions(projection?.Illustration?.ColumnDescriptions),
                    Messages = MapperMessages(projection?.Messages, projection?.MessageDescriptions),
                    AnneeDecheance = GetLapseYear(projection?.Values),
                    Ages = MapColumnAges(projection?.Illustration),
                    AgesAssures = MapColumnAgesAssures(projection?.Illustration),
                    AnneesCalendrier = MapCalenderYears(projection?.Illustration),
                    AnneesContrat = MapContractYears(projection?.Illustration)
                },
                ProjectionDefavorable =
                    MapperSensitivityTest(
                        projection?.SensitivityTests?.Results?.FirstOrDefault(x => !x.Variances.IsFavourable)),
                ProjectionFavorable =
                    MapperSensitivityTest(
                        projection?.SensitivityTests?.Results?.FirstOrDefault(x => x.Variances.IsFavourable))
            };

            MapperBonSuccessoral(result, projections.GetEstateBondProjection());
            MapperReductionBaremeParticipation(result, projection, parametreRapport?.ReductionBaremeParticipation);

            var annees = result.Projection.AnneesContrat.Where(v => v > 0).ToArray();
            result.AnneeDebutProjection = annees.FirstOrDefault();
            result.AnneeFinProjection = annees.LastOrDefault();
            result.AgeFinProjection = (int)result.Projection.Ages.LastOrDefault(v => v > 0);
            result.AgeReferenceFinProjection = Math.Min(result.AgeFinProjection, configurationRapport.AgeReferenceProjection);
            result.IndexFinProjection = projection?.EndIndexForContract(result.AgeReferenceFinProjection) ?? -1;
            return result;
        }

        private void MapperBonSuccessoral(Projections projections, ProjectionData.Projection projection)
        {
            if (projection == null)
            {
                return;
            }

            var descriptions = projection.Illustration.ColumnDescriptions.Where(x => !projections.Projection.ColumnDescriptions.Any(y => y.Id == x.Id)).ToList();
            projections.Projection.ColumnDescriptions.AddRange(MapperColumnDescriptions(descriptions));

            projections.BonSuccessoral = new Projection
            {
                Columns = MapperColumns(projection?.Illustration?.Columns),
                ColumnDescriptions = MapperColumnDescriptions(projection?.Illustration?.ColumnDescriptions),
                Messages = MapperMessages(projection?.Messages, projection?.MessageDescriptions),
                AnneeDecheance = GetLapseYear(projection?.Values),
                Ages = MapColumnAges(projection?.Illustration),
                AnneesContrat = MapContractYears(projection?.Illustration)
            };
        }

        public Facturation MapFacturation(ProjectionData.Projection projection)
        {
            var frequence = projection?.Contract?.Billing?.Frequency.ConvertirFrequence() ??
                            TypeFrequenceFacturation.AucunMode;

            var montantOptionDepotSupplementaire =
                projection?.Contract?.Billing?.Premium?.Scenario ==
                ProjectionEnum.Billing.PremiumScenario.ModalPlusAdditionalDepositOption
                    ? projection?.Contract?.Billing?.Premium.Amount
                    : null;

            return new Facturation
            {
                FrequenceFacturation = frequence,
                MontantOptionDepotSupplementaire = montantOptionDepotSupplementaire
            };
        }

        public AvancesSurPolice MapAvancesSurPolice(ProjectionData.Projection projection)
        {
            var loans = projection.Contract?.TraditionalFinancial?.Loans;

            if (loans == null || loans.Balance == 0) return null;

            return new AvancesSurPolice
            {
                DateDerniereMiseAJour = loans.LastUpdate,
                Solde = loans.Balance
            };
        }

        private void MapperReductionBaremeParticipation(Projections projections,
                 ProjectionData.Projection projection, double? reduction)
        {
            if (reduction.HasValue)
            {
                var projectionAvecReduction = projection?.SensitivityTests?.Results?.FirstOrDefault(x =>
                    !x.Variances.IsFavourable &&
                    !x.Variances.VarianceIndexAccounts.HasValue &&
                    !x.Variances.VariancePortfolioAccounts.HasValue &&
                    x.Variances.VarianceInterestAccounts.HasValue &&
                    Math.Abs(x.Variances.VarianceInterestAccounts.Value - reduction.Value) < 0.0000001);

                if (projectionAvecReduction != null)
                {
                    projections.ProjectionDefavorable = MapperSensitivityTest(projectionAvecReduction);
                }
            }
        }

        private Projection MapperSensitivityTest(ProjectionData.Sensitivity.SensitivityTest sensitivityTest)
        {
            if (sensitivityTest == null)
            {
                return null;
            }

            var result = new Projection
            {
                Columns = MapperColumns(sensitivityTest.Illustration?.Columns),
                ColumnDescriptions = MapperColumnDescriptions(sensitivityTest.Illustration?.ColumnDescriptions),
                Messages = MapperMessages(sensitivityTest.Messages, sensitivityTest.MessageDescriptions),
                AnneeDecheance = GetLapseYear(sensitivityTest.Values),
                Ages = MapColumnAges(sensitivityTest.Illustration),
                AgesAssures = MapColumnAgesAssures(sensitivityTest.Illustration),
                AnneesCalendrier = MapCalenderYears(sensitivityTest.Illustration),
                AnneesContrat = MapContractYears(sensitivityTest.Illustration),
                Variances = MapVariances(sensitivityTest.Variances)
            };

            return result;
        }

        private Variances MapVariances(ProjectionData.Sensitivity.Variances variances)
        {
            if (variances == null) return null;
            return new Variances
            {
                EcartCompteIndiciel = variances.VarianceIndexAccounts,
                EcartCompteInteret = variances.VarianceInterestAccounts,
                EcartComptePortefeuille = variances.VariancePortfolioAccounts
            };
        }

        private double[] MapColumnAges(ProjectionIllustration.Illustration illustration)
        {
            if (illustration == null) return new double[] { };
            var colonneAge = illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:Age" })?.FirstOrDefault();
            return illustration.Columns.FirstOrDefault(x => x.Id == (colonneAge?.Id ?? -1))?.Value ?? new double[] { };
        }

        private Dictionary<string, double[]> MapColumnAgesAssures(
            ProjectionIllustration.Illustration illustration)
        {
            if (illustration == null)
            {
                return new Dictionary<string, double[]>();
            }

            var colonneAge = illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:AgeInsured" })?.FirstOrDefault();
            return illustration.Columns.Where(x => x.Id == (colonneAge?.Id ?? -1)).ToDictionary(x => x.Insured.Id, x => x.Value);
        }

        private int[] MapContractYears(ProjectionIllustration.Illustration illustration)
        {
            if (illustration == null)
            {
                return new int[] { };
            }

            var colonneAnnee = illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:Year" })?.FirstOrDefault();
            return illustration.Columns.FirstOrDefault(x => x.Id == (colonneAnnee?.Id ?? 0))?.Value.Select(x => (int)x).ToArray() ?? new int[] { };
        }

        private int[] MapCalenderYears(ProjectionIllustration.Illustration illustration)
        {
            if (illustration == null)
            {
                return new int[] { };
            }

            var colonneAnnee = illustration.GetColumnDescriptionsWithAttributes(new[] { "Type:CalendarYear" })?.FirstOrDefault();
            return illustration.Columns.FirstOrDefault(x => x.Id == (colonneAnnee?.Id ?? 0))?.Value.Select(x => (int)x)
                       .ToArray() ?? new int[] { };
        }

        private List<Column> MapperColumns(List<ProjectionIllustration.Data<double[]>> columns)
        {
            var result = new List<Column>();
            columns?.ForEach(cd => result.Add(ToColumn(cd)));
            return result;
        }

        private List<ColumnDescription> MapperColumnDescriptions(
            List<ProjectionIllustration.ColumnDescription> descriptions)
        {
            var result = new List<ColumnDescription>();
            descriptions?.ForEach(cd =>
                result.Add(
                    new ColumnDescription
                    {
                        Id = cd.Id,
                        TitreEn = cd.TitleEn,
                        TitreFr = cd.TitleFr
                    }));

            return result;
        }

        private List<MessageMoteur> MapperMessages(List<ProjectionData.Message> messages,
            ProjectionMessages.Types.MessageDescriptions descriptions)
        {
            var result = new List<MessageMoteur>();
            if (messages == null)
            {
                return result;
            }

            result.AddRange(messages.Select(item => new MessageMoteur
            {
                MessageId = item.Code.ToString(),
                FormattedMessageEn = item.FormatMessage(descriptions, ProjectionEnum.Language.English),
                FormattedMessageFr = item.FormatMessage(descriptions, ProjectionEnum.Language.French),
                Parametres = item.MessageParameters?.Select(x => new ParametreMessageMoteur
                {
                    DateValue = x.DateValue,
                    DoubleValue = x.DoubleValue,
                    IntegerValue = x.IntegerValue,
                    SequenceId = x.SequenceId,
                    StringValue = x.StringValue
                }).ToList()
            }));

            return result;
        }

        private Column ToColumn(ProjectionIllustration.Data<double[]> data)
        {
            return new Column
            {
                Id = data.Id,
                Coverage = data.Coverage?.Id,
                Individual = data.Individual?.Id,
                Insured = data.Insured?.Id,
                Value = data.Value
            };
        }

        private int? GetLapseYear(List<KeyValuePair<ProjectionData.Characteristics.Characteristic, double>> values)
        {
            if (values == null) return null;
            return (int?)values.Search(ProjectionEnum.ValueId.LapseYear);
        }
    }
}
