using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Extensions
{
    [TestClass]
    public class VecteurIllustrationExtensionTest
    {
        [TestMethod]
        public void GIVEN_Projection_WHEN_ColumnsIsNull_THEN_False()
        {
            var projections = new Projections();
            var manager = new VecteurManager();
            using (new AssertionScope())
            {
                manager.ColonnePresente(projections, 999, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeFalse();
                projections.Projection = new Projection();
                manager.ColonnePresente(projections, 999, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeFalse();
                projections.Projection.Columns = new List<Column>();
                manager.ColonnePresente(projections, 999, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeFalse();
            }
        }

        [TestMethod]
        public void GIVEN_Projection_WHEN_ColumnExistsAndNotEmpty_THEN_True()
        {
            var projection = new Projection
            {
                Columns = new List<Column>()
            };

            projection.Columns.Add(new Column { Id = 998 });
            projection.Columns.Add(new Column { Id = 998, Value = new double[] { } });
            projection.Columns.Add(new Column { Id = 999, Value = new[] { 11.1 } });

            var projections = new Projections
            {
                Projection = projection
            };

            var manager = new VecteurManager();
            using (new AssertionScope())
            {
                manager.ColonnePresente(projections, 999, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeTrue();
                manager.ColonnePresente(projections, 998, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeFalse();
                manager.ColonnePresente(projections, 997, Types.Enums.TypeProjection.Normal, Types.Enums.TypeRendementProjection.Normal).Should().BeFalse();
            }
        }

        [TestMethod]
        public void SommeValeursToutLesGroupesAssures_Valid()
        {
            var projections = new Projections
            {
                Projection = new Projection
                {
                    Columns = new List<Column>
                    {
                        new Column {Id = 1, Value = new double[] {1, 2, 3}},
                        new Column {Id = 1, Insured = "i1", Value = new double[] {10, 20, 30}},
                        new Column {Id = 2, Insured = "i1", Value = new double[] {100, 200, 300}},
                        new Column {Id = 3, Insured = "i1", Value = new double[] {1000, 2000, 3000}},
                        new Column {Id = 1, Insured = "i2", Value = new[] {10.1, 20.1, 30.1}},
                        new Column {Id = 2, Insured = "i2", Value = new[] {100.1, 200.1, 300.1}},
                        new Column {Id = 3, Insured = "i2", Value = new[] {1000.1, 2000.1}},
                        new Column {Id = 999, Value = new[] {1.5, 2.5}}
                    }
                }
            };

            using (new AssertionScope())
            {
                projections.Projection.SommeValeursToutLesGroupesAssures(1).Should().BeEquivalentTo(new[] { 20.1, 40.1, 60.1 });
                projections.Projection.SommeValeursToutLesGroupesAssures(2).Should().BeEquivalentTo(new[] { 200.1, 400.1, 600.1 });
                projections.Projection.SommeValeursToutLesGroupesAssures(3).Should().BeEquivalentTo(new[] { 2000.1, 4000.1, 3000 });
                projections.Projection.SommeValeursToutLesGroupesAssures(998).Should().BeEmpty();
                projections.Projection.SommeValeursToutLesGroupesAssures(999).Should().BeEmpty();
            }
        }      
    }
}
