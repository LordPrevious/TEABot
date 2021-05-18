using System.Collections.Generic;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Common list datastructure for list:* statements
    /// 
    /// List items are maps from keys to TSValues.
    /// Keys are usually derived from wildcard value names, e.g. with
    /// "!data.id" and "!data.name", adding "!data.*" to a list results
    /// in the keys "id" and "name".
    /// </summary>
    public class TSValueList : List<Dictionary<string, TSValue>>
    {
    }
}
