namespace DiscordBotSample.Entities
{
  /// <summary>
  /// This is base interface for all Entities.
  /// </summary>
  /// <remarks>
  /// I normally use a GUID on everything even if it is not the primary key.
  /// it helps by being able to migrate data.  If I copy records from one db to another the
  /// auto incrementing id could and likely would be different where the GUID would be the same.
  /// Leaving out the GUID to start with 
  /// TODO: consider adding GUID.
  /// </remarks>
  public interface IEntity
  {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// It is nice to ability to have a generic collection Entities
    /// Also nice to have clarity when dealing with many Ids with <Entity>Id
    /// To allow for both means you can (see Code below)
    /// </remarks>
    /// <code>
    /// Example:
    /// public int ApplicationId { get; set; }
    /// public int Id => ApplicationId;
    /// </code>
    int Id { get; }
  }
}
