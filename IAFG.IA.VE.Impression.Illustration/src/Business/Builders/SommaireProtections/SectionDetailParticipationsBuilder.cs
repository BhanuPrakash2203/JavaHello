﻿using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionDetailParticipationsBuilder : ISectionDetailParticipationsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionDetailParticipationsMapper _mapper;

        public SectionDetailParticipationsBuilder(IReportFactory reportFactory, ISectionDetailParticipationsMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionDetailParticipationsModel> parameters)
        {
            var report = _reportFactory.Create<ISectionDetailParticipations>();
            ReportBuilderAssembler.Assemble(report, new DetailParticipationsViewModel(), parameters, _mapper);
        }
    }
}