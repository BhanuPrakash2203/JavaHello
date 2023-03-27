using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Export;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.DIContainer;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Services;
using IAFG.IA.VE.Impression.Illustration.Test.Fake;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.PDF;
using IAFG.IA.VI.PDF.Services.Dto;
using IAFG.IA.VI.PDF.Services.Dto.Interfaces;
using IAFG.IA.VI.PDF.Services.Interfaces;
using IAFG.IA.VI.PDF.Services.Octopus;
using IAFG.IA.VI.PDF.Services.Rabbit;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IAFG.IA.VE.Impression.Illustration.Test
{
    [TestClass]
    [DeploymentItem("IAFG.IA.VI.AF.PDFVie.dll")]
    [DeploymentItem("IAFG.IA.VE.Impression.Illustration.Tiers.Reports.dll.GrapeCity.Licenses.dll")]
    [DeploymentItem("feature-toggles.json")]
    public class IllustrationsExportServiceTest
    {
        private readonly bool _mustShowOnConsole = Environment.UserInteractive;
        private IProductDefinitionFile _servicePdf;
        private IIllustrationsExportService _exportService;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            if (!Environment.UserInteractive) return;
            var path = Path.GetTempPath();
            var files = Directory.GetFiles(path, "*.illustration.pdf");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    //ne rien faire
                }
            }
        }

        [TestInitialize]
        public void Setup()
        {
            var productDefinitionFileServiceFactory = new ProductDefinitionFileServiceFactory(new ProductDefinitionFileOptions(PDFData.Base));
            _servicePdf = productDefinitionFileServiceFactory.CreateProductDefinitionFileService();

            var illustrationExportServiceFactory = new IllustrationExportServiceFactory(_servicePdf.Get().IPDFVieFactory());
            _exportService = illustrationExportServiceFactory.CreateIllustrationExportService();
        }

        private static DonneesIllustration CreerDonneesIllustration(Projection projection)
        {
            var projections = new Projections { List = new List<Projection>() };
            projections.List.Add(projection);

            var donnees = new DonneesIllustration
            {
                ParametreRapport = new ParametreRapport {Language = Language.French, VersionProduitFormattee = "AssuranceTraditionnelle2", VersionEVO = "1.3"},
                Agents = new List<Agent> {FakeAgent.CreerAgent()},
                DateVersionProduit = DateTime.Now,
                Projections = projections,
                DonneesClients = CreerDonnesClients(projection),
                DonneesPdf = new DonneesPdf { ProtectionsPdf = new List<ProtectionPdf>() }
            };

            return donnees;
        }

        private static List<DonneesClient> CreerDonnesClients(Projection projection)
        {
            return projection.Contract.Individuals.Where(i => i.IsApplicant)
                             .Select(individual => new DonneesClient
                             {
                                 Id = individual.Identifier.Id,
                                 Sexe = (Sexe)individual.Sex,
                                 StatutFumeur = StatutTabagisme.NonFumeur,
                                 AgeAssurance = individual.Birthdate?.CalculerAge(projection.GetMainCoverage().Dates.Issuance)
                             }).ToList();
        }

        private void CreerDonneePdf(DonneesIllustration donnees, Projection projection)
        {
            donnees.DonneesPdf = new DonneesPdf { ProtectionsPdf = new List<ProtectionPdf>() };

            foreach (var insured in projection.Contract.Insured)
            {
                foreach (var insuredCoverage in insured.Coverages)
                {
                    var pdfCoverage = _servicePdf.Get().Coverage(new PlanCodeDto(insuredCoverage.PlanCode)).WithDescriptiveCode().WithSpecifications().WithTermsDetails().Execute();
                    donnees.DonneesPdf.ProtectionsPdf.Add(MapProtectionPdf(pdfCoverage,insuredCoverage.Identifier.Id));

                    if (insuredCoverage.Coverages != null)
                    {
                        foreach (var subCoverage in insuredCoverage.Coverages)
                        {
                            var pdfdata = _servicePdf.Get().Coverage(new PlanCodeDto(subCoverage.PlanCode)).WithDescriptiveCode().WithSpecifications().WithTermsDetails().Execute();
                            donnees.DonneesPdf.ProtectionsPdf.Add(MapProtectionPdf(pdfdata, subCoverage.Identifier.Id));
                        }
                    }

                    if (insuredCoverage.AdditionalBenefits != null)
                    {
                        foreach (var subCoverage in insuredCoverage.AdditionalBenefits)
                        {
                            var pdfdata = _servicePdf.Get().Coverage(new PlanCodeDto(subCoverage.PlanCode)).WithDescriptiveCode().WithSpecifications().WithTermsDetails().Execute();
                            donnees.DonneesPdf.ProtectionsPdf.Add(MapProtectionPdf(pdfdata, subCoverage.Identifier.Id));
                        }
                    }
                }

                var garanties = projection?.Contract?.Applicants?.AdditionalBenefits ?? new List<AdditionalBenefit>();
                foreach (var garanti in garanties)
                {
                    var pdfCoverage = _servicePdf.Get().Coverage(new PlanCodeDto(garanti.PlanCode)).WithDescriptiveCode().WithSpecifications().WithTermsDetails().Execute();
                    donnees.DonneesPdf.ProtectionsPdf.Add(MapProtectionPdf(pdfCoverage, garanti.Identifier.Id));
                }
            }
        }

        private static ProtectionPdf MapProtectionPdf(IDetailedCoverageDto coverage, string idProtection)
        {
            return new ProtectionPdf
                   {
                       IdProtection = idProtection,
                       CodePlan = coverage.PlanCode.Code,
                       DateVersionProduit = coverage.ProductVersionDate,
                       DescriptiveCodeInfos = coverage.DescriptiveCodeInfos,
                       Specification = MapSpecificationPdf(coverage.Specifications)
                   };
        }

        private static SpecificationProtection MapSpecificationPdf(IDetailedCoverageSpecificationsDto specification)
        {
            return new SpecificationProtection
                   {
                IsChildModule = specification.IsChildModule,
                       IsAcceleratedPaymentRider = specification.IsAcceleratedPaymentRider,
                       IsAdditionalBenefit = specification.IsAdditionalBenefit,
                       IsChildModulePlus = specification.IsChildModulePlus,
                       IsComplementaryIncome = specification.IsComplementaryIncome,
                       IsCoverage = specification.IsCoverage,
                       IsCriticalIllness = specification.IsCriticalIllness,
                       IsCriticalIllnessChildModule = specification.IsCriticalIllnessChildModule,
                       IsGuaranteedPremiumRider = specification.IsGuaranteedPremiumRider,
                       IsHospitalization = specification.IsHospitalization,
                       IsIncreasedBenefit = specification.IsIncreasedBenefit,
                       IsIntegratedCoverageDecreasing = specification.IsIntegratedCoverageDecreasing,
                       IsInvalidityCoverage = specification.IsInvalidityCoverage,
                       IsLifeCoverage = specification.IsLifeCoverage,
                       IsPickaTerm = specification.IsPickaTerm,
                       IsRembFlexPrime = specification.IsFlexibleReturnOfPremium,
                       IsRembPrimeDeces = specification.IsReturnOfPremiumUponDeath,
                       IsResiliable = specification.IsResiliable,
                       IsSubCoverage = specification.IsSubCoverage,
                       IsTermCoverage = specification.IsTermCoverage,
                       IsWithoutInsuranceVolume = specification.IsWithoutInsuranceVolume,
                       IsCashValueAvailable = specification.IsCashValueAvailable,
                       IsRenewable = specification.IsRenewable
            };
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("Cas\\CAS_ACCESVIE.json", "cas")]
        public void Illustrations_CasAccesVie()
        {
            var cas = File.ReadAllText(@"Cas\CAS_ACCESVIE.json", Encoding.GetEncoding("UTF-8"));
            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);
            var document = _exportService.ExportToPDF(donnees);
            PrintDocumentInTemp(document);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("Cas\\ACCES_VIE_TEST.json", "cas")]
        public void Illustrations_NouvelleVenteAccesVie()
        {
            var cas = File.ReadAllText(@"Cas\ACCES_VIE_TEST.json", Encoding.GetEncoding("UTF-8"));
            var projection = JsonConvert.DeserializeObject<Projection>(cas, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });
            var donnees = CreerDonneesIllustration(projection);

            donnees.ParametreRapport.PagesSelectionnees = new List<string>()
            {
                "PageTitre",
                "DescriptionsProtections",
                "SommaireProtections",
                "ApercuProtections",
                "NotesIllustration"
            };

            CreerDonneePdf(donnees, projection);

            var document = _exportService.ExportToPDF(donnees);
            PrintDocumentInTemp(document);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("Cas\\CieContractant.json", "cas")]
        public void ExportToPDF_WhenCompagnieContractante_ThenImpression()
        {
            var cas = File.ReadAllText(@"Cas\CieContractant.json", Encoding.GetEncoding("UTF-8"));
            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);
            var document = _exportService.ExportToPDF(donnees);

            PrintDocumentInTemp(document);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("cas\\CAS_TESTS.json", "cas")]
        public void Illustrations_NouvelleVenteCasTests()
        {
            var cas = File.ReadAllText(@"Cas\CAS_TESTS.json", Encoding.GetEncoding("UTF-8"));
            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);
            var document = _exportService.ExportToPDF(donnees);
            PrintDocumentInTemp(document);
        }        

        [TestMethod, TestCategory("LocalOnly")]
        public void Illustrations_TestUserPath()
        {
            if (!_mustShowOnConsole) return;

            foreach (Environment.SpecialFolder item in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                Console.WriteLine(@"{0}={1}", item, Environment.GetFolderPath(item));
            }

            var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var commonDocuments = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            var commonApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            Console.WriteLine();
            Console.WriteLine(@"LocalApplicationData={0}", localApplicationData);
            Console.WriteLine(@"ApplicationData={0}", applicationData);
            Console.WriteLine(@"CommonDocuments={0}", commonDocuments) ;
            Console.WriteLine(@"CommonApplicationData={0}", commonApplicationData);
        }
        
        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("cas\\AssureEnfantCieContractant.json", "cas")]
        public void Illustrations_NouvelleVenteTraditionnelAvecAssureEnfant()
        {
            var cas = File.ReadAllText(@"Cas\AssureEnfantCieContractant.json", Encoding.GetEncoding("UTF-8"));
            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);
            var document = _exportService.ExportToPDF(donnees);
            PrintDocumentInTemp(document);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("cas\\CAS_PROPRIETE_PARTAGEE.json", "cas")]
        public void ExportToPDF_WhenConceptProprietePartagee_ThenCreatePDFReport()
        {
            var cas = File.ReadAllText(@"Cas\CAS_PROPRIETE_PARTAGEE.json", Encoding.GetEncoding("UTF-8"));
            var donneesIllustration = JsonConvert.DeserializeObject<DonneesIllustration>(cas);
            var pdfDocument = _exportService.ExportToPDF(donneesIllustration);
            PrintDocumentInTemp(pdfDocument);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("Cas\\CAS_PROTECTIONS_RENOUVELABLES.json", "cas")]        
        public void ExportToPDF_WhenProtectionsRenouvelables_ThenCreatePDFReport()
        {
            var cas = File.ReadAllText(@"Cas\CAS_PROTECTIONS_RENOUVELABLES.json", Encoding.GetEncoding("UTF-8"));            

            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);                        

            var document = _exportService.ExportToPDF(donnees);

            PrintDocumentInTemp(document);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("Cas\\CAS_PROTECTIONS_RENOUVELABLES_SURPRIME.json", "cas")]
        public void ExportToPDF_WhenProtectionsRenouvelablesAvecSurprimes_ThenCreatePDFReport()
        {
            var cas = File.ReadAllText(@"Cas\CAS_PROTECTIONS_RENOUVELABLES_SURPRIME.json", Encoding.GetEncoding("UTF-8"));

            var donnees = JsonConvert.DeserializeObject<DonneesIllustration>(cas);            

            var document = _exportService.ExportToPDF(donnees);

            PrintDocumentInTemp(document);
        }

        [TestMethod]
        public void ObtenirInformationPagesRapport_WhenGenesis_ThenReturnsPages()
        {
            var pages = _exportService.ObtenirInformationPagesRapport(
                new DonneesRapport
                {
                    Produit = Produit.Genesis, 
                    Etat = Etat.EnVigueur
                });

            pages.Should().NotBeEmpty();
        }

        private static void PrintDocumentInTemp(PdfDocument document)
        {
            if (document.Content == null) return;

            var path = Path.GetTempFileName();
            var pathPdf = Path.ChangeExtension(path, ".illustration.pdf");
            File.Move(path, pathPdf);
            using (var stream = File.Create(pathPdf))
            {
                stream.Write(document.Content, 0, document.Content.Length);
            }
        }
    }
}
