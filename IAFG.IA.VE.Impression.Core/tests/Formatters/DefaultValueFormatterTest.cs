using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class DefaultValueFormatterTest
    {
        const string STRING_VALUE = "TEST";

        [TestMethod]
        public void Format_WhenValueEmpty_ReturnStringEmpty()
        {
            var value = string.Empty;
            var subject = new DefaultValueFormatter();

            var formatted = subject.Format(value);

            formatted.Should().BeEmpty();
        }

        [TestMethod]
        public void Format_WhenValueNull_ReturnStringEmpty()
        {
            string value = null;
            var subject = new DefaultValueFormatter();

            var formatted = subject.Format(value);

            formatted.Should().BeEmpty();
        }

        [TestMethod]
        public void Format_WhenValueNotNullOrEmpty_ReturnValue()
        {
            var subject = new DefaultValueFormatter();

            var formatted = subject.Format(STRING_VALUE);

            formatted.Should().Be(STRING_VALUE);
        }

    }
}