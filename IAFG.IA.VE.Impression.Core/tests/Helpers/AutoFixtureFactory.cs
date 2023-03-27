using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace IAFG.IA.VE.Impression.Core.Tests.Helpers
{
    public static class AutoFixtureFactory
    {
        public static IFixture Create()
        {
            var random = new Random();
            var auto = new Fixture().Customize(new AutoNSubstituteCustomization());

            auto.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => auto.Behaviors.Remove(b));
            auto.Behaviors.Add(new OmitOnRecursionBehavior(2));
            return auto;
        }
    }
}
