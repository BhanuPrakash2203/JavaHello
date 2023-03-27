using System;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Services;
using Unity;

namespace IAFG.IA.VE.Impression.Illustration.DIContainer
{
    public sealed class IllustrationExportServiceFactory
    {
        private readonly Lazy<IUnityContainer> _unityContainer;

        public IllustrationExportServiceFactory(VI.AF.IPDFVie.Factory.Interfaces.IFactory pdfVieFactory)
        {
            _unityContainer = new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                container.RegisterInstance(pdfVieFactory);
                IllustrationsRegistry.Configure(container);
                return container;
            });
        }

        public IIllustrationsExportService CreateIllustrationExportService()
        {
            return _unityContainer.Value.Resolve<IIllustrationsExportService>();
        }
    }
}
