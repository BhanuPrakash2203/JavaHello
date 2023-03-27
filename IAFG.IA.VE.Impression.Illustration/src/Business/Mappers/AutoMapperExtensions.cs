using System;
using System.Linq.Expressions;
using AutoMapper;
using FormatterResources = IAFG.IA.VE.Impression.Illustration.Resources.Properties.Resources;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Permet de configurer la conversion d'une propriété en tenant compte du fait que sa valeur peut-être cachée.
        /// <example>
        /// CreateMap{UnitedStatesCitizen, ApplicantUsResidentForTaxViewModel}().ForMember(d => d.SNN, m => m.MapFrom(forPrivacy, s => s.AmericanSocialInsuranceNumber));
        /// </example>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="configForMember"></param>
        /// <param name="shouldBePrivate">Si actif, la valeur de la propriété sera cachée lors de la conversion.  Sinon, 
        /// la valeur de la propriété (<paramref name="sourceMember"/>) sera retournée</param>
        /// <param name="sourceMember"></param>
        public static void MapFrom<TModel, TViewModel, TMember>(this IMemberConfigurationExpression<TModel, TViewModel, TMember> configForMember,
                                                                bool shouldBePrivate,
                                                                Expression<Func<TModel, TMember>> sourceMember) where TMember : class
        {
            if (shouldBePrivate)
                configForMember.ResolveUsing((s, d) =>
                                             {
                                                 var value = sourceMember.Compile()(s);
                                                 return IsDefaultValue(value) ? string.Empty : FormatterResources.MaskForHiddenData; // On ne veut pas masquer des données absentes
                                             });
            else
                configForMember.MapFrom(sourceMember);
        }

        private static bool IsDefaultValue<TMember>(TMember value) where TMember : class
        {
            if (typeof(TMember) == typeof(string))
                return string.IsNullOrWhiteSpace(Convert.ToString(value));
            return value == default(TMember);
        }
    }
}