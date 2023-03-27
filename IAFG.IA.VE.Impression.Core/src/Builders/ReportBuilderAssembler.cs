using System;
using IAFG.IA.VE.Impression.Core.Mappers;
using IAFG.IA.VE.Impression.Core.Types.Reports;

namespace IAFG.IA.VE.Impression.Core.Builders
{
    /// <summary>
    ///     Classe permettant de réutiliser les étapes standards de construction de rapport et sous-sections.
    /// </summary>
    public static class ReportBuilderAssembler
    {
        /// <summary>
        ///     Effectue les étapes nécessaires à la construction d'un rapport
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TReport"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="report"></param>
        /// <param name="viewModel">Le view model attendue par le rapport.</param>
        /// <param name="parameters">Contient entre autre le rapport Parent et les données source du rapport (Model)</param>
        /// <param name="mapper">
        ///     Instance permettant d'effectuer la conversion du <paramref name="parameters" />.data vers le
        ///     <paramref name="viewModel" />
        /// </param>
        /// <param name="buildSubparts">
        ///     (Optionnel) Méthode anonyme qui sera appelée pour batir les sous sections du rapport.
        ///     Cette méthode permet de transmettre le <paramref name="viewModel" /> aux builders des sous sections.
        /// </param>
        /// <returns>Retourne le <paramref name="report" /> recue suite à l'assemblage</returns>
        public static TReport Assemble<TReport, TViewModel, TParameter>(TReport report,
                                                                        TViewModel viewModel,
                                                                        BuildParameters<TParameter> parameters,
                                                                        IReportMapperWithContext<TParameter, TViewModel> mapper,
                                                                        Action<TViewModel> buildSubparts = null) where TReport : IReportWithModel<TViewModel>
        {
            if ((parameters != null) && (parameters.Data != null))
                mapper.Map(parameters.Data, viewModel, parameters.ReportContext);
            return AssembleWithoutModelMapping(report, viewModel, parameters, buildSubparts);
        }

        /// <summary>
        ///     Effectue les étapes nécessaires à la construction d'un rapport qui ne possède pas de données.
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TReport"></typeparam>
        /// <param name="report"></param>
        /// <param name="parameters"></param>
        /// ///
        /// <param name="buildSubparts">(Optionnel) Méthode anonyme qui sera appelée pour batir les sous sections du rapport.</param>
        /// <returns>Retourne le <paramref name="report" /> recue suite à l'assemblage</returns>
        public static TReport AssembleWithoutData<TReport, TParameter>(TReport report,
                                                                       BuildParameters<TParameter> parameters,
                                                                       Action buildSubparts = null) where TReport : IReport
        {
            report.StyleOverride = parameters.StyleOverride;
            buildSubparts?.Invoke();
            parameters.ParentReport?.AddSubReport(report);
            return report;
        }

        /// <summary>
        ///     Effectue les étapes nécessaires à la construction d'un rapport qui ne nécessite pas de conversion de son model en
        ///     viewModel puisque ce dernier est déjà alimenté.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TReport"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="report"></param>
        /// <param name="viewModel">Le viewModel contenant les données du rapport</param>
        /// <param name="parameters"></param>
        /// <param name="buildSubparts">(Optionnel) Méthode anonyme qui sera appelée pour batir les sous sections du rapport.</param>
        /// <returns>Retourne le <paramref name="report" /> recue suite à l'assemblage</returns>
        public static TReport AssembleWithoutModelMapping<TReport, TViewModel, TParameter>(TReport report,
                                                                                           TViewModel viewModel,
                                                                                           BuildParameters<TParameter> parameters,
                                                                                           Action<TViewModel> buildSubparts = null) where TReport : IReportWithModel<TViewModel>
        {
            if (viewModel != null)
            {
                Action buildSubpartsOrDoNothing = () =>
                                                  {
                                                      buildSubparts?.Invoke(viewModel);
                                                  };

                report.Model = viewModel;
                AssembleWithoutData(report, parameters, buildSubpartsOrDoNothing);
            }
            return report;
        }
    }
}