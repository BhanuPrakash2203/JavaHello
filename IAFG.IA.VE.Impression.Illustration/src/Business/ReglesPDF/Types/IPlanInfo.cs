using IAFG.IA.VE.Impression.Illustration.Types.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF.Types
{
    public interface IPlanInfo
    {
        string CodePlan { get; set; }
        string DescriptionFr { get; set; }
        string DescriptionAn { get; set; }
        int AgeMaturite { get; set; }
        string CodeGlossaire { get; set; }
        TypePrestationPlan TypePrestationPlan { get; set; }
    }

    public class PlanInfo : IPlanInfo
    {
        public string CodePlan { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionAn { get; set; }
        public int AgeMaturite { get; set; }
        public string CodeGlossaire { get; set; }
        public TypePrestationPlan TypePrestationPlan { get; set; }
    }

}