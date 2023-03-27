using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Test.Mappers;
using IAFG.IA.VE.Impression.Illustration.Tests.Helpers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using IAFG.IA.VI.Projection.Data.Enums.CashFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using DetailFluxMonetaire = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailFluxMonetaire;
using DetailPrime = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailPrime;
using ResourcesAccessorFactory = IAFG.IA.VE.Impression.Illustration.Resources.ResourcesAccessorFactory;
using SectionDetailParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailParticipationsModel;
using SectionFluxMonetaireModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionFluxMonetaireModel;
using SectionPrimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionPrimesModel;
using SectionSurprimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionSurprimesModel;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.SommaireProtectionsIllustration
{
    [TestClass]
    public class PageSommaireProtectionsIllustrationTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IReportContext _reportContext;
        private SectionSommaireProtectionsIllustrationModel _sommaireProtectionsIllustationModel;
        private SectionContractantsModel _sectionContractantsModel;
        private SectionAssuresModel _sectionAssuresModel;
        private SectionAssuresModel _sectionAssures2Model;
        private SectionProtectionsSommaireModel _sectionProtectionsSommaireModel;
        private SectionProtectionsSommaireModel _sectionProtectionsSommaireModel1;
        private SectionProtectionsSommaireModel _sectionProtectionsSommaireModel2;
        private SectionSurprimesModel _sectionSurprimesModel;
        private SectionConseillerModel _sectionConseillerModel;
        private SectionCaracteristiquesIllustrationModel _sectionCaracteristiquesIllustrationModel;
        private DetailProtection _detailProtection;
        private DetailSurprime _detailSurprime;
        private Agent _agent;
        private SectionPrimesModel _sectionPrimesModel;
        private DetailPrime _detailPrime;
        private DetailFluxMonetaire _detailFluxMonetaire;
        private SectionFluxMonetaireModel _sectionFluxMonetaireModel;
        private SectionPrimesVerseesModel _sectionPrimesVerseesModel;
        private DetailPrimesVersees _detailPrimeVersees;
        private SectionDetailParticipationsModel _sectionDetailParticipationsModel;
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestInitialize]
        public void Initialize()
        {
            _reportContext = Auto.Create<IReportContext>();
            _sommaireProtectionsIllustationModel = Auto.Create<SectionSommaireProtectionsIllustrationModel>();
            _sectionContractantsModel = Auto.Create<SectionContractantsModel>();
            _sectionAssuresModel = Auto.Create<SectionAssuresModel>();
            _sectionAssures2Model = Auto.Create<SectionAssuresModel>();
            _sectionProtectionsSommaireModel = Auto.Create<SectionProtectionsSommaireModel>();
            _sectionProtectionsSommaireModel1 = Auto.Create<SectionProtectionsSommaireModel>();
            _sectionProtectionsSommaireModel2 = Auto.Create<SectionProtectionsSommaireModel>();
            _sectionSurprimesModel = Auto.Create<SectionSurprimesModel>();
            _sectionConseillerModel = Auto.Create<SectionConseillerModel>();
            _sectionCaracteristiquesIllustrationModel = Auto.Create<SectionCaracteristiquesIllustrationModel>();
            _sectionDetailParticipationsModel = Auto.Create<SectionDetailParticipationsModel>();
            _sectionFluxMonetaireModel = Auto.Create<SectionFluxMonetaireModel>();
            _detailProtection = Auto.Create<DetailProtection>();
            _detailSurprime = Auto.Create<DetailSurprime>();
            _detailFluxMonetaire = Auto.Create<DetailFluxMonetaire>();
            _agent = Auto.Create<Agent>();
            _sectionPrimesModel = Auto.Create<SectionPrimesModel>();

            _sectionPrimesVerseesModel = Auto.Create<SectionPrimesVerseesModel>();
            _detailPrime = Auto.Create<DetailPrime>();
            _detailPrimeVersees = Auto.Create<DetailPrimesVersees>();

            //Section Contractant
            _sectionContractantsModel.Contractants.First().Sexe = Sexe.Homme;
            _sectionContractantsModel.Contractants.First().EstCompagnie = _sectionContractantsModel.EstCompagnie;
            _sectionContractantsModel.Protections = _sectionProtectionsSommaireModel;
            _sectionContractantsModel.Protections.Protections.First().AfficherCapitalAssure = true;
            _sommaireProtectionsIllustationModel.SectionContractantsModel = _sectionContractantsModel;

            //Section Assures
            _sectionProtectionsSommaireModel1.Protections.First().AfficherCapitalAssure = true;
            _sectionProtectionsSommaireModel1.Protections.First().ReferenceNotes = new List<int> {1,2};
            _sectionProtectionsSommaireModel2.Protections.First().MontantCapitalAssureActuel = 0.0d;
            _sectionProtectionsSommaireModel2.Protections.First().ReferenceNotes = null;
            
            _sectionAssuresModel.Protections = _sectionProtectionsSommaireModel1;
            _sectionProtectionsSommaireModel1.Protections.First().AfficherCapitalAssure = true;
            _sectionAssuresModel.Assures.First().Sexe = Sexe.Homme;
            _sectionAssuresModel.Assures.First().StatutFumeur = StatutTabagisme.NonFumeur;
            _sectionAssuresModel.Assures.First().Age = 14;

            _sectionAssures2Model.Protections = _sectionProtectionsSommaireModel2;
            _sectionAssures2Model.Assures.First().Sexe = Sexe.Femme;
            _sectionAssures2Model.Assures.First().StatutFumeur = StatutTabagisme.NonFumeur;
            _sectionAssures2Model.Assures.First().Age = 20;

            _sommaireProtectionsIllustationModel.SectionsAssuresModel = new List<SectionAssuresModel>()
            {
                _sectionAssuresModel,
                _sectionAssures2Model
            };

            //Section Primes
            _sectionPrimesModel.Primes = new List<DetailPrime>
                                 {
                                     _detailPrime
                                 };

            _sommaireProtectionsIllustationModel.SectionPrimesModel = _sectionPrimesModel;

            //Section PrimesVersees
            _sectionPrimesVerseesModel.FrequenceFacturation = TypeFrequenceFacturation.Mensuelle;
            _sectionPrimesVerseesModel.TitreColonnePrimesVersees = LibellesPrimeVersee.PrimesMensuellesVerseesSelectionneesPAR;
            _sectionPrimesVerseesModel.PrimesVersees = new List<DetailPrimesVersees>
            {
                _detailPrimeVersees
            };

            _sommaireProtectionsIllustationModel.SectionPrimesVerseesModel = _sectionPrimesVerseesModel;

            //Section Flux Monetaire
            _sectionFluxMonetaireModel.Details = new List<DetailFluxMonetaire> { _detailFluxMonetaire };
            _sommaireProtectionsIllustationModel.SectionFluxMonetaireModel = _sectionFluxMonetaireModel;

            //Section Detail Participations
            _sommaireProtectionsIllustationModel.SectionDetailParticipationsModel = _sectionDetailParticipationsModel;
            
            //Section Surprimes
            _detailProtection.Surprimes = new List<DetailSurprime> { _detailSurprime };
            _sectionSurprimesModel.Protections = new List<DetailProtection> { _detailProtection };
            _sommaireProtectionsIllustationModel.SectionSurprimesModel = _sectionSurprimesModel;
            
            //Section Conseiller
            _sectionConseillerModel.Conseillers = new List<Agent> { _agent };
            _sommaireProtectionsIllustationModel.SectionConseillerModel = _sectionConseillerModel;

            //Section CaractéristiqueIllustration
            _sommaireProtectionsIllustationModel.SectionCaracteristiquesIllustrationModel = _sectionCaracteristiquesIllustrationModel;
        }

        [TestMethod]
        public void AutoMappingConfigurationShouldBeValid()
        {
            AutoMapperAssertions.AssertConfigurationIsValid<PageSommaireProtectionsIllustrationMapper.ReportProfile>();
        }

        [TestMethod]
        public void Given_RessourcesFluxMonetaireTransactionDepot_THEN_ReturnDescription()
        {
            var formatter = Helpers.Helpers.CreateIllustrationReportDataFormatter();
            var types = Enum.GetValues(typeof(DepositType));

            using (new AssertionScope())
            {
                foreach (DepositType type in types)
                {
                    if (type == DepositType.Unspecified) continue;
                    if (type == DepositType.IrisPolicyLoanRefund) continue;
                    if (type == DepositType.IrisCollateralLoanRefund) continue;
                    var result = formatter.FormatterEnum("FluxMonetaireTransaction", $"DepositType.{type}");
                    result.Should().NotBeNullOrWhiteSpace();
                    result.Should().NotStartWith("DepositType.");
                }
            }
        }

        [TestMethod]
        public void Given_RessourcesFluxMonetaireTransactionRetrait_THEN_ReturnDescription()
        {
            var formatter = Helpers.Helpers.CreateIllustrationReportDataFormatter();
            var types = Enum.GetValues(typeof(WithdrawalType));

            using (new AssertionScope())
            {
                foreach (WithdrawalType type in types)
                {
                    if (type == WithdrawalType.Unspecified) continue;
                    if (type == WithdrawalType.IrisLoan) continue;
                    if (type == WithdrawalType.IrisPolicyLoan) continue;
                    var result = formatter.FormatterEnum("FluxMonetaireTransaction", $"WithdrawalType.{type}");
                    result.Should().NotBeNullOrWhiteSpace();
                    result.Should().NotStartWith("WithdrawalType.");
                }
            }
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionModel_WHEN_Map_THEN_Return_PageSommaireProtectionsViewModel()
        {
            var dataFormatter = Helpers.Helpers.CreateIllustrationReportDataFormatter(false, out var container);
            var resourcesAccessor = container.Resolve<IIllustrationResourcesAccessorFactory>();
            var cultureAccessor = container.Resolve<ICultureAccessor>();

            var pageMapper = new PageSommaireProtectionsIllustrationMapper(
                new AutoMapperFactory(dataFormatter, resourcesAccessor, _managerFactory));

            var viewModel = new PageSommaireProtectionsIllustrationViewModel();
            pageMapper.Map(_sommaireProtectionsIllustationModel, viewModel, _reportContext);

            using (new AssertionScope())
            {
                viewModel.TitreSection.Should().Be(_sommaireProtectionsIllustationModel.TitreSection);

                //Section Contractant
                viewModel.SectionContractants.TitreSection.Should().Be(_sectionContractantsModel.TitreSection);
                viewModel.SectionContractants.TauxMarginalCorporation.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxMarginaux.TauxCorporation));
                viewModel.SectionContractants.TauxMarginalParticulier.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxMarginaux.TauxParticulier));
                viewModel.SectionContractants.DividendesCorporation.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxDividendes.TauxCorporation));
                viewModel.SectionContractants.DividendesParticulier.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxDividendes.TauxParticulier));
                viewModel.SectionContractants.GainCapitalCorporation.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxGainCapital.TauxCorporation));
                viewModel.SectionContractants.GainCapitalParticulier.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotCorporation.TauxGainCapital.TauxParticulier));
                viewModel.SectionContractants.ImpotMarginal.Should().Be(dataFormatter.FormatPercentage(_sectionContractantsModel.ImpotParticulier));
                viewModel.SectionContractants.NumeroContrat.Should().Be(_sectionContractantsModel.NumeroContrat);
                viewModel.SectionContractants.Province.Should().Be(_sectionContractantsModel.Province);
                viewModel.SectionContractants.Avis.Texte.Should().Equal(_sectionContractantsModel.Avis);
                viewModel.SectionContractants.Protections.Noms.Should().Be(_sectionProtectionsSommaireModel.NomComplet);
                viewModel.SectionContractants.Protections.FrequenceFacturation.Should().Be(_sectionProtectionsSommaireModel.FrequenceFacturation);
                viewModel.SectionContractants.Protections.MontantPrimeTotal.Should().Be(_sectionProtectionsSommaireModel.MontantPrimeTotal.ToString("c2", cultureAccessor.GetCultureInfo()));
                viewModel.SectionContractants.Protections.Protections.First().Description.Should().Be(_sectionProtectionsSommaireModel.Protections.First().Description);
                viewModel.SectionContractants.Protections.Protections.First().DureeProtection.Should().Be(_sectionProtectionsSommaireModel.Protections.First().DureeProtection.ToString());
                viewModel.SectionContractants.Protections.Protections.First().DureeRenouvellement.Should().Be(_sectionProtectionsSommaireModel.Protections.First().DureeRenouvellement.ToString());
                viewModel.SectionContractants.Protections.Protections.First().MontantCapitalAssureActuel.Should().Be(dataFormatter.FormatCurrency(_sectionProtectionsSommaireModel.Protections.First().MontantCapitalAssureActuel));
                viewModel.SectionContractants.Contractants.First().Prenom.Should().Be(_sectionContractantsModel.Contractants.First().Prenom);
                viewModel.SectionContractants.Contractants.First().Nom.Should().Be(_sectionContractantsModel.Contractants.First().Nom);
                viewModel.SectionContractants.Contractants.First().AgeAssurance.Should().Be(dataFormatter.FormatAge(_sectionContractantsModel.Contractants.First().Age.GetValueOrDefault()));
                viewModel.SectionContractants.Contractants.First().Sexe.Should().Be(dataFormatter.FormatSexe(_sectionContractantsModel.Contractants.First().Sexe, TypeAffichageSexe.Sexe));
                viewModel.SectionContractants.Contractants.First().UsageTabacDetail.Should().Be(_sectionContractantsModel.EstCompagnie ? string.Empty : dataFormatter.FormatStatutTabagisme(_sectionContractantsModel.Contractants.First().StatutFumeur));

                //Section Assures
                viewModel.SectionsAssures.Count.Should().Be(2);
                viewModel.SectionsAssures.ElementAt(0).Protections.Noms.Should().Be(_sectionProtectionsSommaireModel1.NomComplet);
                viewModel.SectionsAssures.ElementAt(0).Protections.FrequenceFacturation.Should().Be(_sectionProtectionsSommaireModel1.FrequenceFacturation);
                viewModel.SectionsAssures.ElementAt(0).Protections.MontantPrimeTotal.Should().Be(_sectionProtectionsSommaireModel1.MontantPrimeTotal.ToString("c2", cultureAccessor.GetCultureInfo()));
                viewModel.SectionsAssures.ElementAt(0).Protections.Protections.First().Description.Should().Be(_sectionProtectionsSommaireModel1.Protections.First().Description);
                viewModel.SectionsAssures.ElementAt(0).Protections.Protections.First().DureeProtection.Should().Be(_sectionProtectionsSommaireModel1.Protections.First().DureeProtection.ToString());
                viewModel.SectionsAssures.ElementAt(0).Protections.Protections.First().DureeRenouvellement.Should().Be(_sectionProtectionsSommaireModel1.Protections.First().DureeRenouvellement.ToString());
                viewModel.SectionsAssures.ElementAt(0).Protections.Protections.First().MontantCapitalAssureActuel.Should().Be(dataFormatter.FormatCurrency(_sectionProtectionsSommaireModel1.Protections.First().MontantCapitalAssureActuel));
                viewModel.SectionsAssures.ElementAt(0).Protections.Protections.First().MontantPrimeMinimale.Should().Be(dataFormatter.FormatCurrency(_sectionProtectionsSommaireModel1.Protections.First().MontantPrimeMinimale) + " (1, 2)");
                viewModel.SectionsAssures.ElementAt(0).Assures.First().Prenom.Should().Be(_sectionAssuresModel.Assures.First().Prenom);
                viewModel.SectionsAssures.ElementAt(0).Assures.First().Nom.Should().Be(_sectionAssuresModel.Assures.First().Nom);
                viewModel.SectionsAssures.ElementAt(0).Assures.First().AgeAssurance.Should().Be(dataFormatter.FormatAge(_sectionAssuresModel.Assures.First().Age.GetValueOrDefault()));
                viewModel.SectionsAssures.ElementAt(0).Assures.First().Sexe.Should().Be(dataFormatter.FormatSexe(_sectionAssuresModel.Assures.First().Sexe, TypeAffichageSexe.Sexe));
                viewModel.SectionsAssures.ElementAt(0).Assures.First().CategorieFumeur.Should().BeNullOrEmpty();
                viewModel.SectionsAssures.ElementAt(0).Assures.First().EstNonAssurableDetail.Should().NotBeNullOrEmpty();
                viewModel.SectionsAssures.ElementAt(1).Protections.FrequenceFacturation.Should().Be(_sectionProtectionsSommaireModel2.FrequenceFacturation);
                viewModel.SectionsAssures.ElementAt(1).Protections.MontantPrimeTotal.Should().Be(_sectionProtectionsSommaireModel2.MontantPrimeTotal.ToString("c2", cultureAccessor.GetCultureInfo()));
                viewModel.SectionsAssures.ElementAt(1).Protections.Protections.First().Description.Should().Be(_sectionProtectionsSommaireModel2.Protections.First().Description);
                viewModel.SectionsAssures.ElementAt(1).Protections.Protections.First().DureeProtection.Should().Be(_sectionProtectionsSommaireModel2.Protections.First().DureeProtection.ToString());
                viewModel.SectionsAssures.ElementAt(1).Protections.Protections.First().DureeRenouvellement.Should().Be(_sectionProtectionsSommaireModel2.Protections.First().DureeRenouvellement.ToString());
                viewModel.SectionsAssures.ElementAt(1).Protections.Protections.First().MontantCapitalAssureActuel.Should().Be(string.Empty);
                viewModel.SectionsAssures.ElementAt(1).Protections.Protections.First().MontantPrimeMinimale.Should().Be(dataFormatter.FormatCurrency(_sectionProtectionsSommaireModel2.Protections.First().MontantPrimeMinimale));
                viewModel.SectionsAssures.ElementAt(1).Assures.First().Prenom.Should().Be(_sectionAssures2Model.Assures.First().Prenom);
                viewModel.SectionsAssures.ElementAt(1).Assures.First().Nom.Should().Be(_sectionAssures2Model.Assures.First().Nom);
                viewModel.SectionsAssures.ElementAt(1).Assures.First().AgeAssurance.Should().Be(dataFormatter.FormatAge(_sectionAssures2Model.Assures.First().Age.GetValueOrDefault()));
                viewModel.SectionsAssures.ElementAt(1).Assures.First().Sexe.Should().Be(dataFormatter.FormatSexe(_sectionAssures2Model.Assures.First().Sexe, TypeAffichageSexe.Sexe));
                viewModel.SectionsAssures.ElementAt(1).Assures.First().CategorieFumeur.Should().NotBeNullOrEmpty();
                viewModel.SectionsAssures.ElementAt(1).Assures.First().EstNonAssurableDetail.Should().NotBeNullOrEmpty();

                //Section Primes
                viewModel.SectionPrimes.Primes.Should().ContainSingle();
                viewModel.SectionPrimes.Primes.ElementAt(0).Montant.Should().Be(_detailPrime.Montant.ToString("c2", cultureAccessor.GetCultureInfo()));
                viewModel.SectionPrimes.Primes.ElementAt(0).Description.Should().Be(dataFormatter.FormatterEnum<TypeDetailPrime>(_detailPrime.TypeDetailPrime.ToString()));

                //Section PrimesVersees
                viewModel.SectionPrimesVersees.PrimesVersees.Any().Should().BeTrue();
                viewModel.SectionPrimesVersees.PrimesVersees.ElementAt(0).Montant.Should().Be(_sectionPrimesVerseesModel.PrimesVersees.ElementAt(0).FormatterMontant(dataFormatter, new ResourcesAccessorFactory(new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(new CultureAccessorEnglish())))));
                viewModel.SectionPrimesVersees.PrimesVersees.ElementAt(0).Description.Should().Be(_sectionPrimesVerseesModel.PrimesVersees.ElementAt(0).FormatterDescription(dataFormatter, new ResourcesAccessorFactory(new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(new CultureAccessorEnglish())))));
                viewModel.SectionPrimesVersees.PrimesVersees.ElementAt(0).Periode.Should().Be(_sectionPrimesVerseesModel.PrimesVersees.ElementAt(0).FormatterPeriode(dataFormatter));
                viewModel.SectionPrimesVersees.TitreColonnePrimesVersees.Should().NotBeNullOrEmpty();
                _sectionPrimesVerseesModel.FrequenceFacturation.Should().Be(TypeFrequenceFacturation.Mensuelle);

                //Section Surprimes
                viewModel.SectionSurprimes.Protections.Should().ContainSingle();
                viewModel.SectionSurprimes.MontantSurprimeTotal.Should().Be(dataFormatter.FormatCurrency(_sectionSurprimesModel.MontantSurprimeTotal));
                viewModel.SectionSurprimes.FrequenceFacturation.Should().Be(_sectionSurprimesModel.FrequenceFacturation);
                viewModel.SectionSurprimes.TitreSection.Should().Be(_sectionSurprimesModel.TitreSection);
                viewModel.SectionSurprimes.Protections.First().Surprimes.Should().ContainSingle();
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().DateLiberation.Should().Be(_detailSurprime.DateLiberation);
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().Description.Should().Be(_detailSurprime.Description);
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().EstTypeTemporaire.Should().Be(_detailSurprime.EstTypeTemporaire);
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().Terme.Should().Be(_detailSurprime.Terme.ToString());
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().TauxMontant.Should().Be(dataFormatter.FormatCurrency(_detailSurprime.TauxMontant));
                viewModel.SectionSurprimes.Protections.First().Surprimes.First().TauxPourcentage.Should().Be(dataFormatter.FormatPercentage(_detailSurprime.TauxPourcentage));

                //Section FluxMonetaire
                viewModel.SectionFluxMonetaire.TitreSection.Should().Be(_sectionFluxMonetaireModel.TitreSection);
                viewModel.SectionFluxMonetaire.Details.First().AnneeDebut.Should().Be(_detailFluxMonetaire.AnneeDebut);
                viewModel.SectionFluxMonetaire.Details.First().Description.Should().Be(_detailFluxMonetaire.FormatterDescription(dataFormatter));
                viewModel.SectionFluxMonetaire.Details.First().Montant.Should().Be(_detailFluxMonetaire.FormatterMontant(dataFormatter, new ResourcesAccessorFactory(new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(new CultureAccessorEnglish())))));
                viewModel.SectionFluxMonetaire.Details.First().Periode.Should().Be(_detailFluxMonetaire.FormatterPeriode(dataFormatter));
                viewModel.SectionFluxMonetaire.Details.First().TypeTransaction.Should().Be(_detailFluxMonetaire.TypeTransaction);

                //Section Conseiller
                viewModel.SectionConseiller.Conseillers.First().Courriel.Should().Be(_agent.Courriel);
                viewModel.SectionConseiller.Conseillers.First().NomComplet.Should().Be(dataFormatter.FormatFullName(_agent.Prenom, _agent.Nom, _agent.Initiale, _agent.Genre, string.Empty));
                viewModel.SectionConseiller.Conseillers.First().TelephoneBureau.Should().Be(dataFormatter.FormatPhoneNumber(_agent.TelephoneBureau));
                viewModel.SectionConseiller.Conseillers.First().TelephonePrincipal.Should().Be(dataFormatter.FormatPhoneNumber(_agent.TelephonePrincipal));

                //Section CaracteristiquesIllustration
                viewModel.SectionCaracteristiquesIllustration.TitreSection.Should().Be(_sectionCaracteristiquesIllustrationModel.TitreSection);
                viewModel.SectionCaracteristiquesIllustration.Valeurs.Count.Should().Be(3);
                viewModel.SectionCaracteristiquesIllustration.Libelles.Count.Should().Be(3);
            }
        }

        [TestMethod]
        public void GIVEN_Montant_THEN_ReturnFormatted()
        {
            var detailflux = new DetailFluxMonetaire
            { 
                TypeTransaction = TypeTransactionFluxMonetaire.Depot,
                TypeMontant = TypeMontantFluxMonetaires.Maximum,
                EstDepotRetraitMaximal = true,
                Montant = 123.45
            };

            var illustrationReportDataFormatter = Helpers.Helpers.CreateIllustrationReportDataFormatter(false, out var container);
            var resourcesAccessor = container.Resolve<IIllustrationResourcesAccessorFactory>();

            using (new AssertionScope())
            {
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().Be("123.45 (Maximum)");
                detailflux.Montant = 0;
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().Be("0.00 (Maximum)");
                detailflux.Montant = 45.78;
                detailflux.TypeMontant = TypeMontantFluxMonetaires.Personnalise;
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().Be("45.78");
            }
        }

        [TestMethod]
        public void GIVEN_MontantAndFrenchCulture_THEN_ReturnFormattedFrench()
        {
            var detailflux = new DetailFluxMonetaire
            {
                TypeTransaction = TypeTransactionFluxMonetaire.Depot,
                TypeMontant = TypeMontantFluxMonetaires.Maximum,
                EstDepotRetraitMaximal = true,
                Montant = 123.45
            };

            var illustrationReportDataFormatter = Helpers.Helpers.CreateIllustrationReportDataFormatter(true, out var container);
            var resourcesAccessor = container.Resolve<IIllustrationResourcesAccessorFactory>();
            
            using (new AssertionScope())
            {
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().BeOneOf("123.45 (Maximal)", "123,45 (Maximal)");
                detailflux.Montant = 0;
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().BeOneOf("0.00 (Maximal)", "0,00 (Maximal)");
                detailflux.Montant = 45.78;
                detailflux.TypeMontant = TypeMontantFluxMonetaires.Personnalise;
                detailflux.FormatterMontant(illustrationReportDataFormatter, resourcesAccessor).Should().BeOneOf("45.78", "45,78");
            }
        }
    }
}