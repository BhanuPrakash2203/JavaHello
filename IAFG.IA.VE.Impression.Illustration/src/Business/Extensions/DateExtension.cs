using System;
using EnumsProjectionData = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Extensions
{
    public static class DateExtension
    {
        public static int CalculerAge(this DateTime dateNaissance, DateTime dateReference)
        {
            var age = dateReference.Year - dateNaissance.Year;
            // Go back to the year the person was born in case of a leap year
            if (dateNaissance > dateReference.AddYears(-age)) age--;
            return age;
        }
        
        public static int CalculerAnneeContratProjection(this VI.Projection.Data.GenericDate date, DateTime dateReference)
        {
            if (date == null) return 0;
            switch (date.DateType)
            {
                case EnumsProjectionData.DateType.Unspecified:
                    return 0;
                case EnumsProjectionData.DateType.Calender:
                    if (date.CalenderDate != null)
                    {
                        if (date.CalenderDate.Value.Month < dateReference.Month) return date.CalenderDate.Value.Year - dateReference.Year;
                        return date.CalenderDate.Value.Year - dateReference.Year + 1;
                    }
                    throw new InvalidCastException();
                case EnumsProjectionData.DateType.YearContract:
                    if (date.Year != null) return date.Year.Value;
                    throw new InvalidCastException();
                case EnumsProjectionData.DateType.Contract:
                    if (date.Year != null) return date.Year.Value;
                    throw new InvalidCastException();
                case EnumsProjectionData.DateType.Automatic:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static int CalculerMoisContratProjection(this VI.Projection.Data.GenericDate date, DateTime dateReference)
        {
            if (date == null) return 0;
            switch (date.DateType)
            {
                case EnumsProjectionData.DateType.Unspecified:
                    return 0;
                case EnumsProjectionData.DateType.Calender:
                    if (date.CalenderDate != null)
                    {
                        if (date.CalenderDate.Value.Month < dateReference.Month) return 13 - (dateReference.Date.Month - date.CalenderDate.Value.Month);
                        return date.CalenderDate.Value.Month - dateReference.Month + 1;
                    }
                    throw new InvalidCastException();
                case EnumsProjectionData.DateType.Contract:
                    if (date.Month.HasValue) return date.Month.Value;
                    throw new InvalidCastException();
                case EnumsProjectionData.DateType.YearContract:
                case EnumsProjectionData.DateType.Automatic:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime? ConvertirDateProjection(this VI.Projection.Data.GenericDate date, DateTime dateReference)
        {
            if (date == null) return null;
            switch (date.DateType)
            {
                case EnumsProjectionData.DateType.Unspecified:
                    return null;
                case EnumsProjectionData.DateType.Calender:
                    return date.CalenderDate;
                case EnumsProjectionData.DateType.YearContract:
                    return dateReference.AddYears((date.Year ?? 1) - 1);
                case EnumsProjectionData.DateType.Contract:
                    return dateReference.AddYears((date.Year ?? 1) - 1).AddMonths((date.Month ?? 1) - 1);
                case EnumsProjectionData.DateType.Automatic:
                    return dateReference;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}