using Dfe.Complete.Constants;

namespace Dfe.Complete.Extensions
{
	public static class DateOnlyExtensions
	{
		public static string ToUkDateString(this DateOnly dateTime) => dateTime.ToString(DateFormatConstants.DateUkFormat);

		public static string ToDateString(this DateOnly? dateTime, bool includeDayOfWeek = false)
		{
			if (!dateTime.HasValue)
			{
				return string.Empty;
			}
			return ToDateString(dateTime.Value, includeDayOfWeek);
		}

		public static string ToDateString(this DateOnly dateTime, bool includeDayOfWeek = false)
		{
			if (includeDayOfWeek)
			{
				return dateTime.ToString(DateFormatConstants.DateWithDayOfTheWeek);
			}
			return dateTime.ToString(DateFormatConstants.DateWithoutDayOfTheWeek);
		}

		public static DateOnly FirstOfMonth(this DateOnly thisMonth, int monthsToAdd)
		{
			var month = (thisMonth.Month + monthsToAdd) % 12;
			if (month == 0) month = 12;
			var yearsToAdd = (thisMonth.Month + monthsToAdd - 1) / 12;
			return new DateOnly(thisMonth.Year + yearsToAdd, month, 1);
		}

        public static string ToDateMonthYearString(this DateOnly? dateTime)
        {
			if (!dateTime.HasValue)
			{
				return string.Empty;
			}

            return dateTime.Value.ToString(DateFormatConstants.MonthAndYearFormat);
        }
    }
}