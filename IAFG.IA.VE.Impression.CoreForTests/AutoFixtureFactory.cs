using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace IAFG.IA.VE.Impression.CoreForTests
{
    public static class AutoFixtureFactory
    {
        public static IFixture Create()
        {
            var random = new Random();
            var auto = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

            auto.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => auto.Behaviors.Remove(b));
            auto.Behaviors.Add(new OmitOnRecursionBehavior(2));
            return auto;
        }
    }
}
