using System.Diagnostics.CodeAnalysis;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    /// <summary>
    ///     Classe abstraite définissant une classe ayant la responsabilité d'effectuer la transformation ("Mapping") d'un
    ///     modèle vers un viewModel.
    ///     Cette classe effectue la transformation via <see cref="AutoMapperFactory" />.  Vous pouvez configurer la
    ///     transformation de vos entités en
    ///     implantant une classe héritant de "ReportProfileBase{TModel,TViewModel}" />.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class ReportMapperBase<TModel, TViewModel>
    {
        // ReSharper disable once InconsistentNaming
        protected readonly IAutoMapperFactory _autoMapperFactory;

        protected ReportMapperBase(IAutoMapperFactory autoMapperFactory)
        {
            _autoMapperFactory = autoMapperFactory;
        }

        public void Map(TModel model, TViewModel viewModel, IReportContext context)
        {
            _autoMapperFactory.InstanceFor(context.ReportAudience).Map(model, viewModel);
        }
    }

    /// <summary>
    ///     Classe abstraite définissant une classe ayant la responsabilité de configurer Automapper pour un sous-ensemble de
    ///     concepts
    ///     en considérent le type d'audience auquel les données transformées devront être présentés.
    ///     https://github.com/AutoMapper/AutoMapper/wiki/Configuration#profile-instances
    /// </summary>
    public abstract class ReportProfileWithAudienceBase : ReportAutoMapperProfileWithAudienceBase
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected ReportProfileWithAudienceBase(
            IIllustrationReportDataFormatter formatter, 
            IIllustrationResourcesAccessorFactory resourcesAccessor, 
            IManagerFactory managerFactory, 
            ReportAudienceTypes audience): base(formatter, resourcesAccessor, managerFactory, audience)
        {
            if (audience == ReportAudienceTypes.FullClearance)
                // ReSharper disable once VirtualMemberCallInContructor
                ConfigureMapping(formatter, resourcesAccessor, managerFactory);
            else
                // ReSharper disable once VirtualMemberCallInContructor
                ConfigureMappingForPrivacyAudience(formatter, resourcesAccessor, managerFactory);
        }

        protected abstract void ConfigureMapping(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory);
        protected abstract void ConfigureMappingForPrivacyAudience(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory);
    }

    /// <summary>
    ///     Classe abstraite définissant une classe ayant la responsabilité de configurer Automapper pour un sous-ensemble de
    ///     concepts.
    ///     https://github.com/AutoMapper/AutoMapper/wiki/Configuration#profile-instances
    /// </summary>
    public abstract class ReportProfileBase : ReportAutoMapperProfileBase
    {
        protected ReportProfileBase(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory)
            : base(formatter, resourcesAccessor, managerFactory)
        {
            // ReSharper disable once VirtualMemberCallInContructor
            ConfigureMapping(formatter, resourcesAccessor, managerFactory);
        }

        protected abstract void ConfigureMapping(IIllustrationReportDataFormatter formatter, IIllustrationResourcesAccessorFactory resourcesAccessor, IManagerFactory managerFactory);
    }
}