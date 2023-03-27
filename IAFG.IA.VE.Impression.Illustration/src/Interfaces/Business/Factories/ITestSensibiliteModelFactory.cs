﻿using IAFG.IA.VE.Impression.Core.Interface.Factories;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories
{
    public interface ITestSensibiliteModelFactory : IFactoryBase<SectionResultatModel>
    {
        SectionResultatModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context);
    }
}