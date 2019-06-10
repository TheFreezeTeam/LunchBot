namespace DiscordBotSample.Extensions
{
  using System;

  public static class DateTimeExtentions
  {
    public static DateTime GetNextWeekday(this DateTime aStartDateTime, DayOfWeek aDayOfWeek)
    {
      // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
      int daysToAdd = ((int)aDayOfWeek - (int)aStartDateTime.DayOfWeek + 7) % 7;
      return aStartDateTime.AddDays(daysToAdd);
    }
  }
}
