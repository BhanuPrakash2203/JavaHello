using IAFG.IA.VE.Impression.Illustration.Types;

namespace IAFG.IA.VE.Impression.Illustration.Test.Fake
{
    public static class FakeAgent
    {
        public static Agent CreerAgent()
        {
            var agent = new Agent()
            {
                Agence = "Agence 123 inc.",
                Prenom = "Brian",
                Nom = "SuperAgent",
                Initiale = "K.",
                TelephoneBureau = "514 555-9997 1234",
                TelephonePrincipal = "418 999-1234",
                Courriel = "SuperAgent@ia.com",
            };

            return agent;
        }
    }
}