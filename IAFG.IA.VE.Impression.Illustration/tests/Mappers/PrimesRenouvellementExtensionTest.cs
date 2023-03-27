using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.Models.PrimesRenouvellement;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Extensions;
using IAFG.IA.VI.Projection.Data.Illustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class PrimesRenouvellementExtensionTest
    {
        [TestMethod]
        public void TestMapperPrimesRenouvellement_AvecNull_THEN_ProtectectionsEstNonNullEtVide()
        {
            var primes = new Primes();
            primes.MapperPrimesRenouvellement(null, null, null, null);          
            primes.Protections.Should().BeEmpty();
        }

        [TestMethod]
        public void TestMapperPrimesRenouvellement_AvecProtections_THEN_ProtectionsAvecPrimes()
        {
            var idProtection1 = new UniqueIdentifier().CreateIdentifier();
            var idProtection2 = new UniqueIdentifier().CreateIdentifier();

            var protections = new List<Protection>
                              {
                                  new Protection {Id = idProtection1.Id},
                                  new Protection {Id = idProtection2.Id}
                              };

            var projection = new Projection
                             {
                                 Illustration = new VI.Projection.Data.Illustration.Illustration
                                                {
                                                    Columns = new List<Data<double[]>>
                                                              {
                                                                  new Data<double[]> {Id = 10, Value = new[] {0, 1.0, 2.1, 3.2}},
                                                                  new Data<double[]> {Id = 10, Value = new[] {0, 11.0, 22.1, 33.2}, Coverage = idProtection1},
                                                                  new Data<double[]> {Id = 10, Value = new[] {0, 0, 200.1, 300.2}, Coverage = idProtection2},
                                                                  new Data<double[]> {Id = 11, Value = new[] {0, 71.0, 72.1, 73.2}, Coverage = idProtection1},
                                                                  new Data<double[]> {Id = 12, Value = new[] {0, 17.0, 27.1, 37.2}, Coverage = idProtection2}
                                                              },
                                                    ColumnDescriptions = new List<ColumnDescription>
                                                                         {
                                                                             new ColumnDescription
                                                                             {
                                                                                 Id = 10,
                                                                                 Attributes = new List<string> {"Type:GuaranteedRenewal"}
                                                                             },
                                                                             new ColumnDescription
                                                                             {
                                                                                 Id = 11,
                                                                                 Attributes = new List<string> {"Type:CoveragePremium"}
                                                                             },
                                                                             new ColumnDescription
                                                                             {
                                                                                 Id = 12,
                                                                                 Attributes = new List<string> {"Type:CoveragePremium"}
                                                                             }
                                                                         }
                                                }
                             };

            PrimesRenouvellementExtension.MapperPrimesRenouvellement(protections, projection);

            var p1 = protections.First(x => x.Id == idProtection1.Id);
            var p2 = protections.First(x => x.Id == idProtection2.Id);

            using (new AssertionScope())
            {
                p1.CapitalAssure.Should().Be(71);
                p1.Primes.Should().HaveCount(3);
                p1.Primes[0].Annee.Should().Be(1);
                p1.Primes[0].MontantGaranti.Should().Be(11);
                p1.Primes[1].Annee.Should().Be(2);
                p1.Primes[1].MontantGaranti.Should().Be(22.1);
                p1.Primes[2].Annee.Should().Be(3);
                p1.Primes[2].MontantGaranti.Should().Be(33.2);

                p2.CapitalAssure.Should().Be(27.1);
                p2.Primes.Should().HaveCount(2);
                p2.Primes[0].Annee.Should().Be(2);
                p2.Primes[0].MontantGaranti.Should().Be(200.1);
                p2.Primes[1].Annee.Should().Be(3);
                p2.Primes[1].MontantGaranti.Should().Be(300.2);
            }
        }
    }
}
