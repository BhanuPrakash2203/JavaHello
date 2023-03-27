using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections
{
    public class SectionIdentificationMapper: ReportMapperBase<SectionIdendificationModel, IdentificationViewModel>, ISectionIdentificationMapper
    {
        public SectionIdentificationMapper(IAutoMapperFactory autoMapperFactory) : base(autoMapperFactory)
        {
        }

        public class ReportProfile : ReportProfileBase
        {
            public ReportProfile(
                IIllustrationReportDataFormatter formatter,
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory) : base(formatter, resourcesAccessor, managerFactory)
            {
            }

            protected override void ConfigureMapping(
                IIllustrationReportDataFormatter formatter,
                IIllustrationResourcesAccessorFactory resourcesAccessor,
                IManagerFactory managerFactory)
            {
                CreateMap<SectionIdendificationModel, IdentificationViewModel>().
                    ForMember(d => d.TitreSection, m => m.MapFrom(s => s.TitreSection)).
                    ForMember(d => d.Province, m => m.MapFrom(s => formatter.FormatterProvince(s.Province))).
                    ForMember(d => d.ImpotCorporation, m => m.MapFrom(s => s.ImpotCorporation.HasValue ? formatter.FormatPercentage(s.ImpotCorporation.Value) : string.Empty)).
                    ForMember(d => d.ImpotParticulier, m => m.MapFrom(s => s.ImpotParticulier.HasValue ? formatter.FormatPercentage(s.ImpotParticulier.Value) : string.Empty));

                CreateMap<Client, ClientViewModel>().
                    ForMember(d => d.Age, m => m.MapFrom(s => s.Age.HasValue ? formatter.FormatAge(s.Age.Value) : string.Empty)).
                    ForMember(d => d.DateNaissance, m => m.MapFrom(s => formatter.FormatDate(s.DateNaissance))).
                    ForMember(d => d.Sexe, m => m.MapFrom(s => formatter.FormatSexe(s.Sexe, TypeAffichageSexe.Sexe))).
                    ForMember(d => d.NomComplet, m => m.MapFrom(s => formatter.FormatFullName(s.Prenom, s.Nom, s.Initiale))).
                    ForMember(d => d.EstContractant, m => m.MapFrom(s => s.EstContractant)).
                    ForMember(d => d.RoleAssure, m => m.MapFrom(s => formatter.FormatterRoleAssure(s.EstContractant))).
                    ForMember(d => d.SequenceId, m => m.MapFrom(s => s.SequenceId));
            }
        }      
    }
}