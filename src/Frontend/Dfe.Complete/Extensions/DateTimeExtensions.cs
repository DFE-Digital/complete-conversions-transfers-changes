using System.Globalization;
using Dfe.Complete.Constants;

namespace Dfe.Complete.Extensions
{
	public static class DateTimeExtensions
	{
		// State the explicit culture so that the functions work correctly in all environments
		// TODO: Fix global culture
		private static readonly CultureInfo GbCulture = new("en-GB");
		public static string ToUkDateString(this DateTime dateTime) => dateTime.ToString(DateFormatConstants.DateUkFormat, GbCulture);

		public static string ToDateString(this DateTime? dateTime, bool includeDayOfWeek = false)
		{
			if (!dateTime.HasValue)
			{
				return string.Empty;
			}
			return ToDateString(dateTime.Value, includeDayOfWeek);
		}

		public static string ToDateString(this DateTime dateTime, bool includeDayOfWeek = false)
		{
			if (includeDayOfWeek)
			{
				return dateTime.ToString(DateFormatConstants.DateWithDayOfTheWeek, GbCulture);
			}
			return dateTime.ToString(DateFormatConstants.DateWithoutDayOfTheWeek, GbCulture);
		}

		public static DateTime FirstOfMonth(this DateTime thisMonth, int monthsToAdd)
		{
			var month = (thisMonth.Month + monthsToAdd) % 12;
			if (month == 0) month = 12;
			var yearsToAdd = (thisMonth.Month + monthsToAdd - 1) / 12;
			return new DateTime(thisMonth.Year + yearsToAdd, month, 1);
		}

		public static string ToDateMonthYearString(this DateTime date)
			=> ((DateTime?)date).ToDateMonthYearString();

		public static string ToDateMonthYearString(this DateTime? dateTime)
		{
			if (!dateTime.HasValue)
			{
				return string.Empty;
			}

			return dateTime.Value.ToString(DateFormatConstants.MonthAndYearFormat, GbCulture);
		}
	}
}