using AutoMapper;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    /// <summary>
    /// Classe que doivent hérité les classes devant configuré automapper https://github.com/AutoMapper/AutoMapper/wiki/Configuration#profile-instances
    /// <remarks>Cette classe a comme seule objectif de forcer un constructeur avec les bons parametres</remarks>
    /// </summary>
    public abstract class ReportAutoMapperProfileBase : Profile
    {
        protected ReportAutoMapperProfileBase()
        {
        }

        protected ReportAutoMapperProfileBase(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory)
        {
        }
    }

    /// <summary>
    /// Classe que doivent hérité les classes devant configuré automapper en considérant le type d'audience auquel les données transformées devront être présentés 
    /// (https://github.com/AutoMapper/AutoMapper/wiki/Configuration#profile-instances)
    /// <remarks>Cette classe a comme seule objectif de forcer un constructeur avec les bons parametres</remarks>
    /// </summary>
    public abstract class ReportAutoMapperProfileWithAudienceBase : Profile
    {
        protected ReportAutoMapperProfileWithAudienceBase(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory, ReportAudienceTypes audience)
        {
        }
    }
}