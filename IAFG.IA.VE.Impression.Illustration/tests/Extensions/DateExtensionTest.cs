using System;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VI.Projection.Data.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Test.Extensions
{
    [TestClass]
    public class DateExtensionTest
    {
        private readonly DateTime _dateReferenceAvantAnniversaire = new DateTime(2017, 05, 01);
        private readonly DateTime _dateReferenceApresAnniversaire = new DateTime(2017, 12, 01);
        private readonly DateTime _dateContrat = new DateTime(2021, 08, 14);

        [TestMethod]
        public void TestCalculerAge()
        {
            Assert.AreEqual(45, new DateTime(1972, 01, 09).CalculerAge(_dateReferenceAvantAnniversaire));
            Assert.AreEqual(44, new DateTime(1972, 06, 09).CalculerAge(_dateReferenceAvantAnniversaire));
            Assert.AreEqual(45, new DateTime(1972, 06, 09).CalculerAge(_dateReferenceApresAnniversaire));
        }

        [TestMethod]
        public void TestCalculerAnneeContrat()
        {
            //Test que la logique prevoie toutes les valeurs possibles.
            foreach (DateType item in Enum.GetValues(typeof(DateType)))
            {
                var genericDate = new IA.VI.Projection.Data.GenericDate
                {
                    DateType = item
                };

                switch (item)
                {
                    case DateType.Unspecified:
                        Assert.AreEqual(0, genericDate.CalculerAnneeContratProjection(_dateReferenceAvantAnniversaire));
                        break;
                    case DateType.Calender:
                        genericDate.CalenderDate = _dateContrat;
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceAvantAnniversaire.Year + 1, genericDate.CalculerAnneeContratProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceApresAnniversaire.Year, genericDate.CalculerAnneeContratProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.YearContract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceAvantAnniversaire.Year, genericDate.CalculerAnneeContratProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceApresAnniversaire.Year, genericDate.CalculerAnneeContratProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.Contract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        genericDate.Month = _dateContrat.Month - _dateReferenceAvantAnniversaire.Month;
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceAvantAnniversaire.Year, genericDate.CalculerAnneeContratProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(_dateContrat.Year - _dateReferenceApresAnniversaire.Year, genericDate.CalculerAnneeContratProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.Automatic:
                        break;
                    default:
                        Assert.AreEqual(1, genericDate.CalculerAnneeContratProjection(_dateReferenceAvantAnniversaire));
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [TestMethod]
        public void TestCalculerMoisContrat()
        {
            //Test que la logique prevoie toutes les valeurs possibles.
            foreach (DateType item in Enum.GetValues(typeof(DateType)))
            {
                var genericDate = new IA.VI.Projection.Data.GenericDate
                {
                    DateType = item
                };

                switch (item)
                {
                    case DateType.Unspecified:
                        Assert.AreEqual(0, genericDate.CalculerMoisContratProjection(_dateReferenceAvantAnniversaire));
                        break;
                    case DateType.Calender:
                        genericDate.CalenderDate = _dateContrat;
                        Assert.AreEqual(_dateContrat.Month - _dateReferenceAvantAnniversaire.Month + 1, genericDate.CalculerMoisContratProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(13 - (_dateReferenceApresAnniversaire.Month - _dateContrat.Month), genericDate.CalculerMoisContratProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.YearContract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        Assert.AreEqual(1, genericDate.CalculerMoisContratProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(1, genericDate.CalculerMoisContratProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.Contract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        genericDate.Month = _dateContrat.Month - _dateReferenceAvantAnniversaire.Month;
                        Assert.AreEqual(_dateContrat.Month - _dateReferenceAvantAnniversaire.Month, genericDate.CalculerMoisContratProjection(_dateReferenceAvantAnniversaire));
                        break;
                    case DateType.Automatic:
                        Assert.AreEqual(1, genericDate.CalculerMoisContratProjection(_dateReferenceAvantAnniversaire));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        [TestMethod]
        public void TestConvertirDate()
        {
            //Test que la logique prevoie toutes les valeurs possibles.
            foreach (DateType item in Enum.GetValues(typeof(DateType)))
            {
                var genericDate = new IA.VI.Projection.Data.GenericDate
                                  {
                                      DateType = item
                };

                switch (item)
                {
                    case DateType.Unspecified:
                        Assert.AreEqual(null, genericDate.ConvertirDateProjection(_dateReferenceAvantAnniversaire));
                        break;
                    case DateType.Calender:
                        genericDate.CalenderDate = _dateContrat;
                        Assert.AreEqual(_dateContrat, genericDate.ConvertirDateProjection(_dateReferenceAvantAnniversaire));
                        break;
                    case DateType.YearContract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        Assert.AreEqual(_dateReferenceAvantAnniversaire.AddYears(3), genericDate.ConvertirDateProjection(_dateReferenceAvantAnniversaire));
                         Assert.AreEqual(_dateReferenceApresAnniversaire.AddYears(3), genericDate.ConvertirDateProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.Contract:
                        genericDate.Year = _dateContrat.Year - _dateReferenceAvantAnniversaire.Year;
                        genericDate.Month = 0;
                        Assert.AreEqual(new DateTime(2020, 4, 1), genericDate.ConvertirDateProjection(_dateReferenceAvantAnniversaire));
                        Assert.AreEqual(new DateTime(2020, 11, 1), genericDate.ConvertirDateProjection(_dateReferenceApresAnniversaire));
                        break;
                    case DateType.Automatic:
                        Assert.AreEqual(_dateReferenceAvantAnniversaire, genericDate.ConvertirDateProjection(_dateReferenceAvantAnniversaire));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
