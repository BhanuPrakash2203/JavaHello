using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Extensions
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void TestJoinStringLines()
        {
            using (new AssertionScope())
            {
                ((List<string>)null).JoinStringLines().Should().Be(string.Empty);
                new List<string>().JoinStringLines().Should().Be(string.Empty);
                new List<string> { "l1" }.JoinStringLines().Should().Be("l1\r\n");
                new List<string> { "l1", "l2" }.JoinStringLines().Should().Be("l1\r\nl2\r\n");
            }
        }
    }
}
