namespace DiscordBotSample.Extensions
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public static class ListExtentions
  {
    public static T NextAfter<T>(this IList<T> aList, T aItem)
    {
      int indexOf = aList.IndexOf(aItem);
      return aList[indexOf == aList.Count - 1 ? 0 : indexOf + 1];
    }

    public static T Random<T>(this IList<T> aList)
    {
      var random = new Random();
      return aList[random.Next(aList.Count())];
    }
  }
}
