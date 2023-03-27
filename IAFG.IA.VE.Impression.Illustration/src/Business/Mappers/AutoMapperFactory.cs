using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class AutoMapperFactory : IAutoMapperFactory
    {
        private readonly IAutoMapperFactoryConfiguration _config;
        private readonly object _lockObject = new object();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter;
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor;
        private readonly IManagerFactory _managerFactory;
        private IMapper _fullClearanceAudienceMapper;
        private IMapper _privacyAudienceMapper;

        public AutoMapperFactory(
            IIllustrationReportDataFormatter reportDataFormatter, 
            IIllustrationResourcesAccessorFactory resourcesAccessor,
            IManagerFactory managerFactory)
            : this(reportDataFormatter, resourcesAccessor, managerFactory, new AutoMapperFactoryConfiguration())
        {
        }

        public AutoMapperFactory(
            IIllustrationReportDataFormatter reportDataFormatter, 
            IIllustrationResourcesAccessorFactory resourcesAccessor, 
            IManagerFactory managerFactory, 
            IAutoMapperFactoryConfiguration autoMapperFactoryConfiguration)
        {
            _reportDataFormatter = reportDataFormatter;
            _resourcesAccessor = resourcesAccessor;
            _managerFactory = managerFactory;
            _config = autoMapperFactoryConfiguration ?? new AutoMapperFactoryConfiguration();
        }

        public IMapper InstanceFor(ReportAudienceTypes audience)
        {
            return audience == ReportAudienceTypes.Privacy
                       ? GetInstance(ref _privacyAudienceMapper, audience)
                       : GetInstance(ref _fullClearanceAudienceMapper, audience);
        }

        private void AddAdditionalProfiles(MapperConfigurationExpression cfg, ReportAudienceTypes audience)
        {
            foreach (var type in _config.AdditionalProfileTypes)
            {
                if (typeof (ReportAutoMapperProfileWithAudienceBase).IsAssignableFrom(type))
                {
                    CreateProfile(cfg, type, audience);
                }
                else
                {
                    CreateProfile(cfg, type);
                }
            }
        }

        private void AddProfilesFromAssemblies(MapperConfigurationExpression cfg, ReportAudienceTypes audience)
        {
            foreach (var assembly in _config.ProfilesAssembliesToScan)
            {
                AddProfilesFromAssembly(cfg, assembly);
                AddProfilesWithAudienceFromAssembly(cfg, assembly, audience);
            }
        }

        private void AddProfilesFromAssembly(MapperConfigurationExpression cfg, Assembly assembly)
        {
            var profileTypes = assembly.GetTypes().Where(x => typeof(ReportAutoMapperProfileBase).IsAssignableFrom(x) && !x.IsAbstract);
            foreach (var profileType in profileTypes)
            {
                CreateProfile(cfg, profileType);
            }
        }

        private void AddProfilesWithAudienceFromAssembly(MapperConfigurationExpression cfg, Assembly assembly, ReportAudienceTypes audience)
        {
            var profileTypes = assembly.GetTypes().Where(x => typeof(ReportAutoMapperProfileWithAudienceBase).IsAssignableFrom(x) && !x.IsAbstract);
            foreach (var profileType in profileTypes)
            {
                CreateProfile(cfg, profileType, audience);
            }
        }

        private IMapper ConfigureAndCreateMapper(ReportAudienceTypes audience)
        {
            var cfg = new MapperConfigurationExpression();
            ConfigureDefaultTypeConverters(cfg);
            ConfigureProfiles(cfg, audience);
            return new Mapper(new MapperConfiguration(cfg));
        }

        private void ConfigureDefaultTypeConverters(MapperConfigurationExpression cfg)
        {
            AutoMapperDefaultTypesConfiguration.Configure(cfg, _reportDataFormatter);
        }

        private void ConfigureProfiles(MapperConfigurationExpression cfg, ReportAudienceTypes audience)
        {
            AddProfilesFromAssemblies(cfg, audience);
            AddAdditionalProfiles(cfg, audience);
        }

        private void CreateProfile(IMapperConfigurationExpression cfg, Type profileType, ReportAudienceTypes audience)
        {
            cfg.AddProfile(Activator.CreateInstance(profileType, _reportDataFormatter, _resourcesAccessor, _managerFactory, audience) as ReportAutoMapperProfileWithAudienceBase);
        }

        private void CreateProfile(IMapperConfigurationExpression cfg, Type type)
        {
            cfg.AddProfile(Activator.CreateInstance(type, _reportDataFormatter, _resourcesAccessor, _managerFactory) as ReportAutoMapperProfileBase);
        }

        private IMapper GetInstance(ref IMapper audienceMapper, ReportAudienceTypes audience)
        {
            if (audienceMapper == null)
            {
                lock (_lockObject)
                {
                    if (audienceMapper == null)
                    {
                        audienceMapper = ConfigureAndCreateMapper(audience);
                    }
                }
            }

            return audienceMapper;
        }
    }
}