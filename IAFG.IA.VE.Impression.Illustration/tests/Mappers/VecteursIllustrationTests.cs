using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Illustration;
using IAFG.IA.VI.Projection.Data.Sensitivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class VecteursIllustrationTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        
        [TestMethod]
        public void MapperVecteursIllustration_AvecProjectionVide()
        {
            var projections = new Projections { List = new List<Projection>() };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Projection.ColumnDescriptions.Should().BeEmpty();
                result.Projection.Columns.Should().BeEmpty();
                result.ProjectionFavorable.Should().BeNull();
                result.ProjectionDefavorable.Should().BeNull();
                result.Projection.Ages.Should().BeEmpty();
                result.Projection.AgesAssures.Should().BeEmpty();
                result.Projection.AnneesContrat.Should().BeEmpty();
                result.Projection.AnneesCalendrier.Should().BeEmpty();
                result.Projection.AnneeDecheance.Should().BeNull();
                result.IndexFinProjection.Should().Be(-1);
            }
        }
        
        [TestMethod]
        public void MapperVecteursIllustration_AvecIllustrationDontLesColumnsSontNimporteQuoi()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = Auto.Create<List<Data<double[]>>>()
                }
            };

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.ColumnDescriptions.Should().NotBeNullOrEmpty();
                result.Projection.ColumnDescriptions.Select(x => x.Id + x.TitreEn + x.TitreFr).Should()
                    .BeEquivalentTo(
                        projection.Illustration.ColumnDescriptions.Select(x => x.Id + x.TitleEn + x.TitleFr));
                result.Projection.Columns.Should().NotBeNullOrEmpty();
                result.ProjectionFavorable.Should().BeNull();
                result.ProjectionDefavorable.Should().BeNull();
                result.Projection.Ages.Should().BeEmpty();
                result.Projection.AnneesContrat.Should().BeEmpty();
                result.Projection.AnneesCalendrier.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_AvecIllustrationFavorable()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = new List<Data<double[]>>
                    {
                        new Data<double[]> {Id = 111, Value = new[] {11.1, 11.2}, Coverage = new UniqueIdentifier {Id = "Coverage_C1"}},
                        new Data<double[]> {Id = 222, Value = new[] {22.1, 22.2}, Individual = new UniqueIdentifier {Id = "Individual_I1"}}
                    }
                },
                SensitivityTests = new SensitivityTests
                {
                    Results = new List<SensitivityTest>
                    {
                        new SensitivityTest
                        {
                            Variances = new Variances
                            {
                                IsFavourable = true,
                                VarianceIndexAccounts = .015
                            },
                            Illustration = new VI.Projection.Data.Illustration.Illustration
                            {
                                Columns = new List<Data<double[]>>
                                {
                                    new Data<double[]> {Id = 888, Value = new[] {88.1, 88.2}},
                                    new Data<double[]> {Id = 999, Value = new[] {99.1, 99.2}, Insured = new UniqueIdentifier {Id = "Insured_I2"}}
                                }
                            }
                        },
                    }
                }
            };

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.Columns.Should().NotBeNullOrEmpty();
                result.Projection.Columns.Select(x => x.Id).Should().BeEquivalentTo(new [] {111, 222});
                result.ProjectionFavorable.Columns.Should().NotBeEmpty();
                result.ProjectionFavorable.Columns.Select(x => x.Id).Should().BeEquivalentTo(new[] { 888, 999 });
                result.ProjectionDefavorable.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_AvecIllustrationDefavorable()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = new List<Data<double[]>>
                    {
                        new Data<double[]>
                        {
                            Id = 111,
                            Value = new[] {11.1, 11.2},
                            Coverage = new UniqueIdentifier {Id = "Coverage_C1"}
                        },
                        new Data<double[]>
                        {
                            Id = 222,
                            Value = new[] {22.1, 22.2},
                            Individual = new UniqueIdentifier {Id = "Individual_I1"}
                        }
                    }
                },
                SensitivityTests = new SensitivityTests
                {
                    Results = new List<SensitivityTest>
                    {
                        new SensitivityTest
                        {
                            Variances = new Variances
                            {
                                IsFavourable = false,
                                VarianceIndexAccounts = -.015
                            },
                            Illustration = new VI.Projection.Data.Illustration.Illustration
                            {
                                Columns = new List<Data<double[]>>
                                {
                                    new Data<double[]> {Id = 888, Value = new[] {88.1, 88.2}},
                                    new Data<double[]>
                                    {
                                        Id = 999,
                                        Value = new[] {99.1, 99.2},
                                        Insured = new UniqueIdentifier {Id = "Insured_I2"}
                                    }
                                }
                            }
                        },
                    }
                }
            };

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.Columns.Should().NotBeNullOrEmpty();
                result.Projection.Columns.Select(x => x.Id).Should().BeEquivalentTo(new[] { 111, 222 });
                result.Projection.Columns.Select(x => x.Value).Should().BeEquivalentTo(new List<double[]> {  new[] { 11.1, 11.2 }, new[] { 22.1, 22.2} });
                result.Projection.Columns.Select(x => x.Coverage).Should().BeEquivalentTo(new List<string> { "Coverage_C1", null });
                result.Projection.Columns.Select(x => x.Individual).Should().BeEquivalentTo(new List<string> { null, "Individual_I1"});
                result.ProjectionFavorable.Should().BeNull();
                result.ProjectionDefavorable.Columns.Should().NotBeEmpty();
                result.ProjectionDefavorable.Columns.Select(x => x.Id).Should().BeEquivalentTo(new[] { 888, 999 });
                result.ProjectionDefavorable.Columns.Select(x => x.Insured).Should().BeEquivalentTo(new List<string> { null, "Insured_I2" });
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_MapColumnYears()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = new List<Data<double[]>>
                    {
                        new Data<double[]> {Id = 111, Value = new[] {11.1, 11.2}},
                        new Data<double[]> {Id = 222, Value = new[] {22.1, 22.2}},
                        new Data<double[]> {Id = 333, Value = new[] {0.0, 1, 2, 99.0, 100.0}},
                        new Data<double[]> {Id = 444, Value = new[] {0.0, 2017, 2018, 2019, 2055}}
                    }
                }
            };

            projection.Illustration.ColumnDescriptions.Add(new ColumnDescription
            {
                Id = 333,
                Attributes = new List<string> {"Type:Year"}
            });

            projection.Illustration.ColumnDescriptions.Add(new ColumnDescription
            {
                Id = 444,
                Attributes = new List<string> { "Type:CalendarYear" }
            });

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.AnneesContrat.Should().BeEquivalentTo(new[] { 0, 1, 2, 99, 100 });
                result.Projection.AnneesCalendrier.Should().BeEquivalentTo(new[] { 0, 2017, 2018, 2019, 2055 });
                result.AnneeDebutProjection.Should().Be(1);
                result.AnneeFinProjection.Should().Be(100);
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_MapColumnAges()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = new List<Data<double[]>>
                    {
                        new Data<double[]> {Id = 111, Value = new[] {11.1, 11.2}},
                        new Data<double[]> {Id = 222, Value = new[] {22.1, 22.2}},
                        new Data<double[]> {Id = 333, Value = new[] {0.0, 44, 45, 46, 47.0}},
                        new Data<double[]> {Id = 444, Value = new[] {0.0, 45, 46, 47, 48.0}, Insured = new UniqueIdentifier {Id = "Insured_1"}},
                        new Data<double[]> {Id = 444, Value = new[] {0.0, 60, 61, 62, 63.0}, Insured = new UniqueIdentifier {Id = "Insured_2"}}
                    }
                }
            };

            projection.Illustration.ColumnDescriptions.Add(new ColumnDescription
            {
                Id = 333,
                Attributes = new List<string> { "Type:Age" }
            });

            projection.Illustration.ColumnDescriptions.Add(new ColumnDescription
            {
                Id = 444,
                Attributes = new List<string> { "Type:AgeInsured" }
            });

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.AgeFinProjection.Should().Be(47);
                result.Projection.Should().NotBeNull();
                result.Projection.Ages.Should().BeEquivalentTo(new[] { 0, 44, 45, 46, 47 });
                result.Projection.AgesAssures.Should().BeEquivalentTo(new Dictionary<string, double[]>
                {
                    {"Insured_1", new []{ 0.0, 45, 46, 47, 48 } },
                    {"Insured_2", new []{ 0.0, 60, 61, 62, 63.0 } }
                });
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_MapLapseYear()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Illustration = new VI.Projection.Data.Illustration.Illustration
                {
                    ColumnDescriptions = Auto.Create<List<ColumnDescription>>(),
                    Columns = new List<Data<double[]>>
                    {
                        new Data<double[]> {Id = 111, Value = new[] {11.1, 11.2}},
                        new Data<double[]> {Id = 222, Value = new[] {22.1, 22.2}}
                    }             
                },
                Values = new List<KeyValuePair<Characteristic, double>>
                {
                    new KeyValuePair<Characteristic, double>(new Characteristic {Id = (int) ValueId.AmountToBePaidAtIssue}, 88.55),
                    new KeyValuePair<Characteristic, double>(new Characteristic {Id = (int) ValueId.FaceAmount, Flag = CharacteristicFlag.Contract}, 99.7),
                    new KeyValuePair<Characteristic, double>(new Characteristic {Id = (int) ValueId.LapseYear, Flag = CharacteristicFlag.Contract}, 44)
                }
            };

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.AnneeDecheance.Should().Be(44);
                result.ProjectionFavorable.Should().BeNull();
                result.ProjectionDefavorable.Should().BeNull();
            }
        }

        [TestMethod]
        public void MapperVecteursIllustration_MapLapseYearFavorableUnfavorable()
        {
            var projection = new Projection
            {
                Contract = new Contract { ContractType = ContractType.Universal },
                Values = new List<KeyValuePair<Characteristic, double>>
                {
                    new KeyValuePair<Characteristic, double>(
                        new Characteristic {Id = (int) ValueId.AmountToBePaidAtIssue}, 88.55),
                    new KeyValuePair<Characteristic, double>(
                        new Characteristic {Id = (int) ValueId.FaceAmount, Flag = CharacteristicFlag.Contract}, 99.7)
                },
                SensitivityTests = new SensitivityTests
                {
                    Results = new List<SensitivityTest>
                    {
                        new SensitivityTest
                        {
                            Variances = new Variances {IsFavourable = true},
                            Values = new List<KeyValuePair<Characteristic, double>>
                            {
                                new KeyValuePair<Characteristic, double>(
                                    new Characteristic
                                    {
                                        Id = (int) ValueId.FaceAmount,
                                        Flag = CharacteristicFlag.Contract
                                    }, 101.7),
                                new KeyValuePair<Characteristic, double>(
                                    new Characteristic
                                    {
                                        Id = (int) ValueId.LapseYear,
                                        Flag = CharacteristicFlag.Contract
                                    }, 55)
                            }
                        },
                        new SensitivityTest
                        {
                            Variances = new Variances {IsFavourable = false},
                            Values = new List<KeyValuePair<Characteristic, double>>
                            {
                                new KeyValuePair<Characteristic, double>(
                                    new Characteristic
                                    {
                                        Id = (int) ValueId.FaceAmount,
                                        Flag = CharacteristicFlag.Contract
                                    }, -458.52),
                                new KeyValuePair<Characteristic, double>(
                                    new Characteristic
                                    {
                                        Id = (int) ValueId.LapseYear,
                                        Flag = CharacteristicFlag.Contract
                                    }, 22)
                            }
                        }
                    }
                }
            };

            var projections = new Projections { List = new List<Projection> { projection } };
            var mapper = new ProjectionsMapper();
            var result = mapper.Map(projections, null, new ConfigurationRapport());

            using (new AssertionScope())
            {
                result.Projection.Should().NotBeNull();
                result.Projection.AnneeDecheance.Should().BeNull();
                result.ProjectionFavorable.AnneeDecheance.Should().Be(55);
                result.ProjectionDefavorable.AnneeDecheance.Should().Be(22);
            }
        }
    }
}
