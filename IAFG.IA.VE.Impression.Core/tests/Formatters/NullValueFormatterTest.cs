using IAFG.IA.VE.Impression.Core.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Core.Tests.Formatters
{
    [TestClass]
    public class NullValueFormatterTest
    {
        private const string TEXT_TO_FORMAT = "TextToFormat";

        [TestMethod]
        public void TestMethod1()
        {
            string value = new NullFormatter().Format(TEXT_TO_FORMAT);

            Assert.AreEqual(TEXT_TO_FORMAT, value);
        }
    }
}
