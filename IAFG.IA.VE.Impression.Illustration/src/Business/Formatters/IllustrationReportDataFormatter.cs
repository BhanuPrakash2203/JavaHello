using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IAFG.IA.VE.Impression.Core.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.Formatters;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.Core.Types;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

namespace IAFG.IA.VE.Impression.Illustration.Business.Formatters
{
    public class IllustrationReportDataFormatter: IIllustrationReportDataFormatter
    {
        private readonly IValueFormatter _defaultFormatter;
        private readonly IDateFormatter _dateFormatter;
        private readonly ILongDateFormatter _longDateFormatter;
        private readonly ICurrencyFormatter _currencyFormatter;
        private readonly ICurrencyWithoutDecimalFormatter _currencyWithoutDecimalFormatter;
        private readonly IPercentageFormatter _percentageFormatter;
        private readonly IPercentageWithoutSymbolFormatter _percentageWithoutSymbolFormatter;
        private readonly IDecimalFormatter _decimalFormatter;
        private readonly INoDecimalFormatter _noDecimalFormatter;
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor;
        private readonly ISystemInformation _systemInformation;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IDateBuilder _dateBuilder;
        private readonly IVecteurManager _vectorManager;

        public IllustrationReportDataFormatter(
            ISystemInformation systemInformation,
            IDateFormatter dateFormatter,
            ILongDateFormatter longDateFormatter,
            IDecimalFormatter decimalFormatter,
            INoDecimalFormatter noDecimalFormatter,
            ICurrencyFormatter currencyFormatter,
            ICurrencyWithoutDecimalFormatter currencyWithoutDecimalFormatter,
            IPercentageFormatter percentageFormatter,
            IPercentageWithoutSymbolFormatter percentageWithoutSymbolFormatter,
            ICultureAccessor cultureAccessor,
            IIllustrationResourcesAccessorFactory resourcesAccessor, 
            IConfigurationRepository configurationRepository,
            IDateBuilder dateBuilder,
            IVecteurManager vectorManager)
        {
            _resourcesAccessor = resourcesAccessor;
            _configurationRepository = configurationRepository;
            _dateBuilder = dateBuilder;
            _systemInformation = systemInformation;
            _dateFormatter = dateFormatter;
            _longDateFormatter = longDateFormatter;
            _decimalFormatter = decimalFormatter;
            _noDecimalFormatter = noDecimalFormatter;
            _currencyFormatter = currencyFormatter;
            _currencyWithoutDecimalFormatter = currencyWithoutDecimalFormatter;
            _percentageFormatter = percentageFormatter;
            _percentageWithoutSymbolFormatter = percentageWithoutSymbolFormatter;
            _defaultFormatter = new ValueDefaultFormatter(cultureAccessor, dateBuilder);
            _vectorManager = vectorManager;
            
            if (_resourcesAccessor != null)
            {
                _resourcesAccessor.Contexte = ResourcesContexte.Illustration;
            }
        }

        public string FormatCurrentDate() => FormatDate(_systemInformation.CurrentDate);

        public string FormatCurrentLongDate()
        {
            return FormatLongDate(_systemInformation.CurrentDate, false);
        }

        public string FormatCurrentLongDateTime()
        {
            return FormatLongDate(_systemInformation.CurrentDate, true);
        }

        public string FormatDate(DateTime? value) => !value.HasValue ? string.Empty : _dateBuilder.WithShortDateFormat().WithInvariantCulture().Build(value.Value);

        public string FormatDateForPrivacy(DateTime? value) => !value.HasValue ? string.Empty : _dateBuilder.WithShortDateFormat().WithInvariantCulture().WithPrivacy().Build(value.Value);

        public string FormatDate(DateTime? value, bool inclureHeure, bool shouldBePrivate = false)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            if (inclureHeure)
            {
                var builder = _dateBuilder.WithShortDateFormat().WithTime().WithInvariantCulture();
                if (shouldBePrivate)
                {
                    builder = builder.WithPrivacy();
                }

                return builder.Build(value.Value);
            }

            return shouldBePrivate? FormatDateForPrivacy(value) : FormatDate(value);
        }

        public string FormatLongDate(DateTime? value) => FormatLongDate(value, false, string.Empty);

        public string FormatLongDateForPrivacy(DateTime? value) => FormatLongDate(value, true, string.Empty);

        public string FormatLongDate(DateTime? date, bool forPrivacy, string nullRessourceId)
        {
            if (!date.HasValue)
            {
                return string.IsNullOrEmpty(nullRessourceId)
                    ? string.Empty
                    : _resourcesAccessor.GetResourcesAccessor().GetStringResourceById(nullRessourceId);
            }

            return forPrivacy
                    ? _dateBuilder.WithLongDateFormat().WithPrivacy().Build(date.Value)
                    : _dateBuilder.WithLongDateFormat().Build(date.Value);
        }

        public string FormatLongDate(DateTime? value, bool inclureHeure, bool shouldBePrivate = false)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            if (inclureHeure)
            {
                var builder = _dateBuilder.WithLongDateFormat().WithTime();
                if (shouldBePrivate)
                {
                    builder = builder.WithPrivacy();
                }

                return builder.Build(value.Value);
            }

            return shouldBePrivate ? FormatLongDateForPrivacy(value) : FormatLongDate(value);
        }

        public string FormatCurrencyWithoutDecimal(int value)
        {
            return _currencyWithoutDecimalFormatter.Format(value);
        }

        public string FormatCurrencyWithoutDecimal(double value)
        {
            return _currencyWithoutDecimalFormatter.Format(value);
        }

        public string FormatCurrencyWithoutDecimal(float value)
        {
            return _currencyWithoutDecimalFormatter.Format(value);
        }

        public string FormatCurrency(int? value)
        {
            return !value.HasValue ? string.Empty : _currencyFormatter.Format(value.Value);
        }

        public string FormatCurrency(double? value)
        {
            return !value.HasValue ? string.Empty : _currencyFormatter.Format(value.Value);
        }

        public string FormatCurrency(float? value)
        {
            return !value.HasValue ? string.Empty : _currencyFormatter.Format(value.Value);
        }

        public string FormatPercentage(int value)
        {
            return _percentageFormatter.Format(value);
        }

        public string FormatPercentage(double? value)
        {
            return !value.HasValue ? string.Empty : _percentageFormatter.Format(value.Value, false);
        }

        public string FormatPercentage(float value)
        {
            return _percentageFormatter.Format(value, false);
        }

        public string FormatPercentageWithoutSymbol(int value)
        {
            return _percentageWithoutSymbolFormatter.Format(value);
        }

        public string FormatPercentageWithoutSymbol(double value)
        {
            return _percentageWithoutSymbolFormatter.Format(value, false);
        }
        
        public string FormatPercentageWithoutSymbol(double value, bool baseEst100)
        {
            return _percentageWithoutSymbolFormatter.Format(value, baseEst100);
        }

        public string FormatPercentageWithoutSymbol(float value)
        {
            return _percentageWithoutSymbolFormatter.Format(value, false);
        }

        public string FormatPercentageWithoutSymbol(float value, bool baseEst100)
        {
            return _percentageWithoutSymbolFormatter.Format(value, baseEst100);
        }

        public string FormatNoDecimal(int value)
        {
            return _noDecimalFormatter.Format(value);
        }

        public string FormatNoDecimal(double value)
        {
            return _noDecimalFormatter.Format(value);
        }

        public string FormatNoDecimal(float value)
        {
            return _noDecimalFormatter.Format(value);
        }

        public string FormatDecimal(int value)
        {
            return _decimalFormatter.Format(value);
        }

        public string FormatDecimal(double value)
        {
            return _decimalFormatter.Format(value);
        }

        public string FormatDecimal(double? value)
        {
            return value.HasValue ? _decimalFormatter.Format(value.Value) : string.Empty;
        }

        public string FormatDecimal(float value)
        {
            return _decimalFormatter.Format(value);
        }

        public string FormatNames(IEnumerable<string> names)
        {
            if (names == null)
            {
                return string.Empty;
            }

            var listNames = names.ToList();
            var andSeparator = " " + _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_et") + " ";
            if (listNames.Count <= 2)
            {
                return listNames.Aggregate((s1, s2) => s1 + andSeparator + s2);
            }

            var commaSeparatedListNames = listNames.Take(listNames.Count - 1).Aggregate((s1, s2) => s1 + ", " + s2);
            return commaSeparatedListNames + andSeparator + listNames.Last();
        }

        public string FormatFullName(string firstName, string lastName, string initial)
        {
            return FormatFullName(firstName, lastName, initial, Genre.NonDefini, string.Empty);
        }

        public string FormatFullName(string firstName, string lastName, string initial, Genre genre, string titre)
        {
            firstName = (string.IsNullOrWhiteSpace(firstName) ? string.Empty : firstName.Trim());
            lastName = (string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim());
            initial = (string.IsNullOrWhiteSpace(initial) ? string.Empty : initial.Trim());
            var titleGender = genre == Genre.Masculin
                            ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AbreviationMonsieur")
                            : genre == Genre.Feminin
                                  ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AbreviationMadame")
                                  : string.Empty;

            var completeName = "";
            if (!string.IsNullOrEmpty(firstName))
                completeName = firstName;

            if (!string.IsNullOrEmpty(initial))
                completeName = $"{completeName} {initial}".Trim();

            if (!string.IsNullOrEmpty(lastName))
                completeName = $"{completeName} {lastName}".Trim();

            if (!string.IsNullOrEmpty(titleGender))
                completeName = $"{completeName}, {titleGender}".Trim();

            if (!string.IsNullOrEmpty(titre))
                completeName = $"{completeName}, {titre}".Trim();

            return completeName;
        }

        public string FormatStatutTabagisme(StatutTabagisme value)
        {
            return FormatterEnum<StatutTabagisme>(value.ToString());
        }

        public string FormatNonAssurable(bool nonAssurable)
        {
            return nonAssurable
                ? $" {_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("NonAssurable")}"
                : string.Empty;
        }

        public string FormatUsageTabac(StatutTabagisme? value)
        {
            if (!value.HasValue || 
                value.Value == StatutTabagisme.NonApplicable ||
                value.Value == StatutTabagisme.NonDefini)
            {
                return string.Empty;
            }

            return
                $"{_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("UsageTabac")}{_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_deuxPoints")} {FormatterEnum<StatutTabagisme>(value.Value.ToString())}";
        }

        public string FormatSexe(Sexe value, TypeAffichageSexe typeAffichage)
        {
            return value.ObtenirLibelle(typeAffichage, _resourcesAccessor);
        }

        public string FormatTypeAssurance(TypeAssurance value)
        {
            return value.ObtenirLibelle(_resourcesAccessor);
        }

        public string FormatAge(int age)
        {
            return string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_X_ans"), age);
        }

        public string FormatOuiNon(bool? valeur)
        {
            return valeur == null ? string.Empty : _resourcesAccessor.GetResourcesAccessor().GetStringResourceById(valeur.Value ? "_oui" : "_non");
        }

        public string FormatterDuree(TypeDuree typeDuree, string duree, bool premierCharMajuscule = false)
        {
            var result = string.Format(typeDuree.ObtenirLibelle(_resourcesAccessor), duree);
            return !premierCharMajuscule ? result : result.PremiereLettreEnMajuscule();
        }

        public string FormatterPeriodeAnneesDebutFin(int debut, int duree)
        {
            var fin = ((debut + duree) -1);
            return $@"{_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_annees")} {debut} {_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a")} {fin}";
        }

        public string FormatterDuree(TypeDuree typeDuree, int duree, bool premierCharMajuscule = false)
        {
            if (duree < 1) return string.Empty;
            var result = string.Format(typeDuree.ObtenirLibelle(_resourcesAccessor), duree);
            if (duree != 1) return !premierCharMajuscule ? result : result.PremiereLettreEnMajuscule();
            if (typeDuree == TypeDuree.PendantNombreAnnees) result = _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_pendant_1_an");
            if (typeDuree == TypeDuree.DurantNombreAnnees) result = _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_durant_1_an");
            if (typeDuree == TypeDuree.ParMoisDurantNombreAnnees) result = _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_par_mois_durant_1_an");
            if (typeDuree == TypeDuree.NombreAnnees) result = _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_X_an");
            return !premierCharMajuscule ? result : result.PremiereLettreEnMajuscule();
        }

        public string FormatterProvince(string province)
        {
            return _configurationRepository.ObtenirLibelleProvince(province);
        }

        public string FormatterRoleAssure(bool estContractant)
        {
            return estContractant ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Contractant") : _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("Assure");
        }

        public string FormatterEnum(string nomRessource, string valeur)
        {
            return _configurationRepository.ObtenirLibelleRessource(nomRessource, valeur);
        }

        public string FormatterEnum<T>(string valeur)
        {
            return _configurationRepository.ObtenirLibelleEnum<T>(valeur);
        }

        public string FormatterEnum<T>(int valeur)
        {
            return FormatterEnum<T>(valeur, true);
        }

        public string FormatterEnum<T>(int valeur, bool leverErreur)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException($@"Ce type {nameof(T)} n'est pas une énumération!", nameof(T));
            var exists = Enum.IsDefined(type, valeur);
            if (!exists && leverErreur) throw new ArgumentException($@"Ce type {nameof(T)} ne contient pas cette valeur {valeur}!", nameof(T));
            return !exists ? string.Empty : _configurationRepository.ObtenirLibelleEnum<T>(((T)Enum.ToObject(type, valeur)).ToString());
        }

        public string FormatPhoneNumber(string valeur)
        {
            if (valeur == null) return string.Empty;

            var phoneDigits = Regex.Replace(valeur, @"\D", "");
            if (string.IsNullOrEmpty(phoneDigits)) return string.Empty;

            if (phoneDigits.StartsWith("1"))
                if (phoneDigits.Length > 11)
                {
                    var phoneRegex = new Regex(@"(\d{1})(\d{3})(\d{3})(\d{4})(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    return phoneRegex.Replace(phoneDigits, "$1 ($2) $3-$4 ($5)");
                }
                else
                {
                    var phoneRegex = new Regex(@"(\d{1})(\d{3})(\d{3})(\d{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    return phoneRegex.Replace(phoneDigits, "$1 ($2) $3-$4");
                }

            if (phoneDigits.Length > 10)
            {
                var phoneRegex = new Regex(@"(\d{3})(\d{3})(\d{4})(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return phoneRegex.Replace(phoneDigits, "($1) $2-$3 ($4)");
            }
            else
            {
                var phoneRegex = new Regex(@"(\d{3})(\d{3})(\d{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                return phoneRegex.Replace(phoneDigits, "($1) $2-$3");
            }
        }

        public string FormatterPeriodeAges(int ageDebut, int? ageFin)
        {
            return ageFin.HasValue && ageFin.Value != ageDebut && ageFin.Value > 0
                       ? string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_periode_ages"), ageDebut, ageFin)
                       : string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_periode_age"), ageDebut);
        }

        public string FormatterPeriodeAnneeMois(int? annee, int? mois)
        {
            return FormatterPeriodeAnneeMois(annee, mois, false);            
        }

        public string FormatterPeriodeAnneeMois(int? annee, int? mois, bool aVieSiNull)
        {
            if (!annee.HasValue)
            {
                var periodeAnneeMois = aVieSiNull ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_vie") : string.Empty;
                return periodeAnneeMois.PremiereLettreEnMajuscule();
            }

            var formatterPeriodeAnneeMois = !mois.HasValue
                ? string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_partir_de_annee"), annee)
                : string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_partir_du_mois_de_annee"), mois.Value, annee);
            return formatterPeriodeAnneeMois.PremiereLettreEnMajuscule();
        }

        public string FormatterPeriodeAnnees(int? anneeDebut, int? anneeFin)
        {
            return FormatterPeriodeAnnees(anneeDebut, anneeFin, false);
        }

        public string FormatterPeriodeAnnees(int? anneeDebut, int? anneeFin, bool aVieSiNull)
        {
            if (!anneeDebut.HasValue)
            {
                var formatterPeriodeAnnees = aVieSiNull ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_vie") : string.Empty;
                return formatterPeriodeAnnees.PremiereLettreEnMajuscule();
            }

            var periodeAnnees = anneeFin.HasValue && anneeFin.Value != anneeDebut.Value && anneeFin.Value > 0
                ? string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_periode_annee_a"), anneeDebut.Value, anneeFin.Value)
                : string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_periode_annee"), anneeDebut.Value);
            return periodeAnnees.PremiereLettreEnMajuscule();
        }
        
        public string FormatterPeriode(int? anneeDebut, int? anneeFin)
        {
            return FormatterPeriode(anneeDebut, anneeFin, false);
        }

        public string FormatterPeriode(int? anneeDebut, int? anneeFin, bool aVieSiNull)
        {
            if (!anneeDebut.HasValue)
            {
                var formatterPeriode = aVieSiNull ? _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_a_vie") : string.Empty;
                return formatterPeriode.PremiereLettreEnMajuscule();
            }

            var periode = anneeFin.HasValue && anneeFin.Value != anneeDebut.Value && anneeFin.Value > 0
                ? string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_periode_a"), anneeDebut.Value, anneeFin.Value)
                : anneeDebut.Value.ToString();
            return periode.PremiereLettreEnMajuscule();
        }

        public string AddColon()
        {
            return _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_deuxPoints");
        }

        public IValueFormatter GetValueFormatter(TypeAffichageValeur valeur)
        {
            IValueFormatter returnValue = null;

            switch (valeur)
            {
                case TypeAffichageValeur.Decimale:
                    returnValue = (IValueFormatter) _decimalFormatter;
                    break;
                case TypeAffichageValeur.SansDecimale:
                    returnValue = (IValueFormatter) _noDecimalFormatter;
                    break;
                case TypeAffichageValeur.Monetaire:
                    returnValue = (IValueFormatter) _currencyFormatter;
                    break;
                case TypeAffichageValeur.MonetaireSansDecimale:
                    returnValue = (IValueFormatter) _currencyWithoutDecimalFormatter;
                    break;
                case TypeAffichageValeur.Pourcentage:
                    returnValue = (IValueFormatter) _percentageFormatter;
                    break;
                case TypeAffichageValeur.PourcentageSansSymbol:
                    returnValue = (IValueFormatter) _percentageWithoutSymbolFormatter;
                    break;
                case TypeAffichageValeur.Date:
                    returnValue = (IValueFormatter) _dateFormatter;
                    break;
                case TypeAffichageValeur.LongueDate:
                    returnValue = (IValueFormatter) _longDateFormatter;
                    break;
                case TypeAffichageValeur.NonDefeni:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valeur), valeur, null);
            }

            return returnValue ?? _defaultFormatter;
        }

        public string FormatterDescription(DefinitionTitreDescription definition, DonneesRapportIllustration donnees)
        {
            return donnees.Langue == Language.English
                       ? FormatterParams(definition.DescriptionEn, definition.DescriptionParams, donnees)
                       : FormatterParams(definition.Description, definition.DescriptionParams, donnees);
        }

        public string FormatterLibellee(DefinitionLibelle definition, IReportContext context)
        {
            if (definition == null) return string.Empty;
            return context.Language == Language.English ? definition.LibelleEn : definition.Libelle;
        }

        public string FormatterLibellees(Dictionary<string, DefinitionLibelle> libelles, string value, IReportContext context)
        {
            if (libelles == null || string.IsNullOrWhiteSpace(value)) return value;
            var libelle = libelles.FirstOrDefault(x => x.Key == value).Value;
            if (libelle == null) return value;
            return context.Language == Language.French? libelle.Libelle : libelle.LibelleEn;
        }

        public Dictionary<string, string> FormatterLibellees(Dictionary<string, DefinitionLibelle> libelles, IReportContext context)
        {
            var result = new Dictionary<string, string>();
            if (libelles == null) return result;
            foreach (var item in libelles)
            {
                result.Add(item.Key, FormatterLibellee(item.Value, context));
            }
            return result;
        }

        public string FormatterLibellee(DefinitionTexteGlossaire definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return donnees.Langue == Language.English ? definition.LibelleEn : definition.Libelle;
        }

        public string FormatterAvis(DefinitionAvis definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return donnees.Langue == Language.English
                        ? FormatterParams(definition.TexteEn, definition.Params, donnees)
                        : FormatterParams(definition.Texte, definition.Params, donnees);
        }

        public string FormatterNote(DefinitionNote definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return donnees.Langue == Language.English
                        ? FormatterParams(definition.TexteEn, definition.Params, donnees)
                        : FormatterParams(definition.Texte, definition.Params, donnees);
        }

        public string FormatterNote(NoteEsperanceVie noteEsperanceVie)
        {
            return _configurationRepository.ObtenirLibelleNoteEsperanceVie(noteEsperanceVie);
        }

        public string FormatterTexte(DefinitionTexte definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return donnees.Langue == Language.English
                        ? FormatterParams(definition.TexteEn, definition.TexteParams, donnees)
                        : FormatterParams(definition.Texte, definition.TexteParams, donnees);
        }

        public string FormatterTexte(DefinitionTexteGlossaire definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;

            var libelle = donnees.Langue == Language.English ? definition.TexteEn : definition.Texte;

            if (definition.Regles == null) return libelle;

            if (definition.Regles.Any(r => r.Any(regle => regle == RegleGlossaire.BoniFideliteInvestissement)))
            {
                var parametres = new[] { _dateFormatter.FormatXeAnnee(donnees.Boni.DebutBoniFidelite), FormatPercentage(donnees.Boni.TauxBoni) };
                return FormatterParams(libelle, null, null, parametres);
            }

            return libelle;
        }

        public string FormatterTitre(DefinitionTitre definition, DonneesRapportIllustration donnees)
        {
            if (definition == null) return string.Empty;
            return donnees.Langue == Language.English
                ? FormatterParams(definition.TitreEn, definition.TitreParams, donnees)
                : FormatterParams(definition.Titre, definition.TitreParams, donnees);
        }

        public string FormatterTitre(DefinitionTitreDescription definition, DonneesRapportIllustration donnees, string[] parametres = null)
        {
            return donnees.Langue == Language.English
                ? FormatterParams(definition.TitreEn, definition.TitreParams, donnees, parametres)
                : FormatterParams(definition.Titre, definition.TitreParams, donnees, parametres);
        }

        public string FormatterTitre(DefinitionTitreDescriptionSelonProduit definition, DonneesRapportIllustration donnees, string[] parametres = null)
        {
            return donnees.Langue == Language.English
                       ? FormatterParams(definition.TitreEn, definition.TitreParams, donnees, parametres)
                       : FormatterParams(definition.Titre, definition.TitreParams, donnees, parametres);
        }

        public string FormatterNomProduit(Produit produit)
        {
            return _configurationRepository.ObtenirNomProduit(produit);
        }

        public string FormatterParams(string texte, IEnumerable<ParametreTexte> parametreTextes,
            DonneesRapportIllustration donnees, string[] parametres = null)
        {
            if (string.IsNullOrEmpty(texte))
            {
                return texte;
            }

            var listParams = new List<object>();
            if (parametres != null)
            {
                listParams.AddRange(parametres);
            }

            if (parametreTextes != null)
            {
                foreach (var param in parametreTextes)
                {
                    switch (param)
                    {
                        case ParametreTexte.Aucun:
                            break;
                        case ParametreTexte.NomProduit:
                            var nomProduit = FormatterNomProduit(donnees.Produit);
                            listParams.Add(string.IsNullOrEmpty(nomProduit) ? donnees.Produit.ToString() : nomProduit);
                            break;
                        case ParametreTexte.AnneeDebutProjection:
                            listParams.Add(donnees.Projections.AnneeDebutProjection);
                            break;
                        case ParametreTexte.AnneeFinProjection:
                            listParams.Add(donnees.Projections.AnneeFinProjection);
                            break;
                        case ParametreTexte.AnneeDebutVersementPrime:
                            listParams.Add(_vectorManager.TrouverAnneeSelonIndex(donnees.Projections.Projection, 
                                _vectorManager.TrouverIndexPremiereValeurNonNull(donnees.Projections,
                                    (int)ProjectionVectorId.TotalAnnualDeposit, TypeProjection.Normal, TypeRendementProjection.Normal)));
                            break;
                        case ParametreTexte.AnneeFinVersementPrime:
                            listParams.Add(_vectorManager.TrouverAnneeSelonIndex(donnees.Projections.Projection,
                                  _vectorManager.TrouverIndexDerniereValeurNonNull(donnees.Projections,
                                    (int)ProjectionVectorId.TotalAnnualDeposit, TypeProjection.Normal, TypeRendementProjection.Normal)));
                            break;
                        case ParametreTexte.AgeFinProjection:
                            listParams.Add(donnees.Projections.AgeFinProjection);
                            break;
                        case ParametreTexte.AgeReferenceFinProjection:
                            listParams.Add(Math.Min(donnees.Projections.AgeFinProjection, donnees.Projections.AgeReferenceFinProjection));
                            break;
                        case ParametreTexte.BoniCommission:
                            listParams.Add(FormatPercentage(donnees.BonusRate));
                            break;
                        case ParametreTexte.PourcentageTaxe:
                            //a completer si un jour le taux de taxe doit être affiché.
                            listParams.Add(0);
                            break;
                        case ParametreTexte.Province:
                            listParams.Add(FormatterProvince(donnees.ProvinceEtat.ToString()));
                            break;
                        case ParametreTexte.MontantAssureMaxCombine:
                            listParams.Add(MontantMaxCombine(donnees));
                            break;
                        case ParametreTexte.ReductionBaremeParticipations:
                            if (donnees.Projections.ProjectionDefavorable?.Variances?.EcartCompteInteret != null)
                            {
                                listParams.Add(FormatPercentage(
                                    Math.Abs(donnees.Projections.ProjectionDefavorable.Variances.EcartCompteInteret
                                        .GetValueOrDefault())));
                            }
                            else
                            {
                                listParams.Add(FormatPercentage(
                                    Math.Abs(donnees.Participations?.ReductionBaremeParticipation ?? 0)));
                            }
                            break;
                        case ParametreTexte.NomContractants:
                            listParams.Add(FormatterNomContractants(donnees));
                            break;
                        case ParametreTexte.TauxBoniPAR:
                            listParams.Add(FormatPercentage(donnees.FondsProtectionPrincipale?.DefaultRate ?? 0));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            return listParams.Any() ? string.Format(texte, listParams.ToArray()) : texte;
        }

        private string FormatterNomContractants(DonneesRapportIllustration donnees)
        {
            var listClient = donnees.Clients?.Where(c => c.EstContractant).ToList();
            if (listClient == null || !listClient.Any())
            {
                return string.Empty;
            }

            return string.Join(" - ", listClient.Select(c => FormatFullName(c.Prenom, c.Nom, c.Initiale)));
        }

        private static int MontantMaxCombine(DonneesRapportIllustration donnees)
        {
            var age = donnees.Clients.FirstOrDefault()?.AgeAssurance ?? 0;
            var boundaries = donnees.ProtectionsPDF.Where(x => x.Boundaries != null).SelectMany(x => x.Boundaries.AmountBoundaries);
            return boundaries.FirstOrDefault(x => x.MinimumIssueAge <= age && x.MaximumIssueAge >= age)?.MaximumIssueAmount ?? 0;
        }

        public string FormatterBoniInteret(ChoixBoniInteret choixBoniInteret, BoniInteret boniInteret, double? tauxBoni, int debutBoniInteret)
        {
            var sChoixBoniInteret = string.Format(FormatterEnum<ChoixBoniInteret>(choixBoniInteret.ToString()), FormatPercentage(tauxBoni)).Trim();

            if (boniInteret == BoniInteret.Regle0_AucunBoni)
            {
                
                if (choixBoniInteret == ChoixBoniInteret.FraisMinimum )
                { 
                    return $"{_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("BoniInteret")} {sChoixBoniInteret}";
                }
            }
            else
            {
                return string.Format(FormatterEnum<BoniInteret>(boniInteret.ToString()), _dateFormatter.FormatXeAnnee(debutBoniInteret), sChoixBoniInteret).Trim();
            }
            return string.Empty;
        }

        public string FormatterNomsAssures(IList<string> noms)
        {
            return noms.Aggregate((s1, s2) => s1 + " " + _resourcesAccessor.GetResourcesAccessor().GetStringResourceById("_et") + " " + s2);
        }

        public string FormatterBoniFidelite(BoniFidelite boniFidelite, int debutBoniFidelite)
        {
            if (boniFidelite == BoniFidelite.Regle7)
            {
                return string.Format(FormatterEnum<BoniFidelite>(boniFidelite.ToString()), _dateFormatter.FormatXeAnnee(debutBoniFidelite));
            }

            return FormatterEnum<BoniFidelite>(boniFidelite.ToString());
        }

        public string FormatDateEffectiveText(DateTime? date)
        {
            return date.HasValue
                   ? string.Format(_resourcesAccessor.GetResourcesAccessor().GetStringResourceById("AgeSelonLaDateEffective"),
                                   _dateBuilder.WithLongDateFormat().Build(date.Value))
                   : "";
        }
    }
}