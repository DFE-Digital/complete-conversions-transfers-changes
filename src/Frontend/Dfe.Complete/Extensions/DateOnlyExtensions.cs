using System.Globalization;
using Dfe.Complete.Constants;
using DfE.CoreLibs.Utilities.Constants;

namespace Dfe.Complete.Extensions
{
	public static class DateOnlyExtensions
	{
		// State the explicit culture so that the functions work correctly in all environments
		// TODO: Fix global culture
		private static readonly CultureInfo GbCulture = new("en-GB");
		public static string ToUkDateString(this DateOnly dateOnly) => dateOnly.ToString(DateFormatConstants.DateUkFormat, GbCulture);

		public static string ToDateString(this DateOnly? dateOnly, bool includeDayOfWeek = false)
		{
			if (!dateOnly.HasValue)
			{
				return string.Empty;
			}
			return ToDateString(dateOnly.Value, includeDayOfWeek);
		}

		public static string ToDateString(this DateOnly dateOnly, bool includeDayOfWeek = false)
		{
			if (includeDayOfWeek)
			{
				return dateOnly.ToString(DateFormatConstants.DateWithDayOfTheWeek, GbCulture);
			}
			return dateOnly.ToString(DateFormatConstants.DateWithoutDayOfTheWeek, GbCulture);
		}

		public static DateOnly FirstOfMonth(this DateOnly thisMonth, int monthsToAdd)
		{
			var month = (thisMonth.Month + monthsToAdd) % 12;
			if (month == 0) month = 12;
			var yearsToAdd = (thisMonth.Month + monthsToAdd - 1) / 12;
			return new DateOnly(thisMonth.Year + yearsToAdd, month, 1);
		}

        public static string ToMonthYearString(this DateOnly? dateOnly)
        {
			if (!dateOnly.HasValue)
			{
				return string.Empty;
			}

            return dateOnly.Value.ToString(DateFormatConstants.MonthAndYearFormat, GbCulture);
        }
        
        public static string ToFullDateMonthYearString(this DateOnly? dateOnly)
        {
	        if (!dateOnly.HasValue)
	        {
		        return string.Empty;
	        }

	        return dateOnly.Value.ToString(DateFormatConstants.FullMonthAndYearFormat, GbCulture);
        }
    }
}