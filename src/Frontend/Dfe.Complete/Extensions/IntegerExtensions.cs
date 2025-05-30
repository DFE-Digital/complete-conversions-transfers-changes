﻿namespace Dfe.Complete.Extensions
{
	public static class IntegerExtensions
	{
		public static string AsPercentageOf(this int? part, int? whole)
		{
			if (!whole.HasValue || !part.HasValue)
			{
				return "";
			}
			return string.Format("{0:F0}%", (100d / whole) * part);
		}

		public static int? ToInt(string value)
		{
			if (int.TryParse(value, out var result))
			{
				return result;
			}
			return null;
		}

		public static string ToResultsCountMessage(this int value)
        {
            return $"{value} result{(value == 1 ? "" : "s")} found";
        }
    }
}
