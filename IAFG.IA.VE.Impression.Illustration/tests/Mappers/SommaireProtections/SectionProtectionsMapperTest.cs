using System;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers.SommaireProtections
{
    [TestClass]
    public class SectionProtectionsMapperTest
    {

        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private IResourcesAccessor _resourcesAccessor;

        [TestInitialize]
        public void Initialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatCurrency(Arg.Any<double?>()).Returns("10000.00 $");
            _formatter.FormatCurrencyWithoutDecimal(Arg.Any<double>()).Returns("10000 $");
            _formatter.FormatterDuree(TypeDuree.NombreAnnees, Arg.Any<int>()).Returns("8 ans");

            _resourcesAccessor = Substitute.For<IResourcesAccessor>();
            _resourcesAccessor.GetStringResourceById("_nonActive").Returns("Non Active");
            _resourcesAccessor.GetStringResourceById("_nonActivee").Returns("Non Activee");

            _resourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
            _resourcesAccessorFactory.GetResourcesAccessor().Returns(_resourcesAccessor);
            
        }

        [TestMethod]
        public void FormatDureeMinimisation_WhenValeurMaximiseeIsNull_shouldReturnStringEmpty()
        {
            var model = new SectionProtectionsModel();
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatDureeMinimisation(_formatter, _resourcesAccessorFactory, model);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FormatDureeMinimisation_WhenDureeMinimisationIsNull_shouldReturnResourceString()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    DureeDebutMinimisation = null
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatDureeMinimisation(_formatter, _resourcesAccessorFactory, model);

            result.Should().Be("Non Activee");
        }

        [TestMethod]
        public void FormatDureeMinimisation_WhenDureeMinimisationIsNull_shouldReturnString()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    DureeDebutMinimisation = 8
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatDureeMinimisation(_formatter, _resourcesAccessorFactory, model);

            result.Should().Be("8 ans");
        }

        [TestMethod]
        public void FormatDureeMinimisation_WhenCapitalPlafondHasValue_shouldReturnStringEmpty()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    CapitalAssurePlafond = 100000.00,
                    DureeDebutMinimisation = null
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatDureeMinimisation(_formatter, _resourcesAccessorFactory, model);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FormatCapitalAssurePlancher_WhenValeurMaximiseeIsNull_ShouldReturnStringEmpty()
        {
            var model = new SectionProtectionsModel();
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlancher(_formatter, _resourcesAccessorFactory, model);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FormatCapitalAssurePlancher_WhenValeurCapitalPlancherIsNull_ShouldReturnResourceString()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    CapitalAssurePlancher = null
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlancher(_formatter, _resourcesAccessorFactory, model);

            result.Should().Be("Non Active");
        }

        [TestMethod]
        public void FormatCapitalAssurePlancher_WhenValeurCapitalPlancherHasValue_ShouldReturnString()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    CapitalAssurePlancher = 10000.00
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlancher(_formatter, _resourcesAccessorFactory, model);

            result.Should().Be("10000.00 $");
        }

        [TestMethod]
        public void FormatCapitalAssurePlafond_WhenValeurMaximiseeIsNull_ShouldReturnStringEmpty()
        {
            var model = new SectionProtectionsModel();
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlafond(_formatter, model);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FormatCapitalAssurePlafond_WhenCapitalPlafondIsNull_ShouldReturnStringEmpty()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    CapitalAssurePlancher = null
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlafond(_formatter, model);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void FormatCapitalAssurePlafond_WhenCapitalPlafondHasValue_ShouldReturnString()
        {
            var model = new SectionProtectionsModel()
            {
                ValeurMaximisee = new ValeurMaximisee()
                {
                    CapitalAssurePlancher = 10000.00
                }
            };
            var result = IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
                .SectionProtectionsMapper
                .ReportProfile
                .FormatCapitalAssurePlafond(_formatter, model);

            result.Should().BeEmpty("10000.00 $");
        }
    }
}
