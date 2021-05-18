using System;
using System.Collections.Generic;
using System.Text.Json;
using TEABot.TEAScript;

namespace TEABot.Bot
{
    /// <summary>
    /// Extensions to TSValueList for JSON conversions
    /// </summary>
    public static class TSValueListExt
    {
        /// <summary>
        /// Convert a value list to an object which can be serialized to JSON
        /// </summary>
        /// <param name="a_valueList">The list to convert</param>
        /// <returns>An object for JSON serialization</returns>
        public static object ToUnspecificObject(this TSValueList a_valueList)
        {
            List<Dictionary<string, object>> result = new();

            foreach (var valueItem in a_valueList)
            {
                Dictionary<string, object> jsonItem = new();

                foreach (var kvp in valueItem)
                {
                    if (kvp.Value.IsText)
                    {
                        jsonItem[kvp.Key] = kvp.Value.TextValue;
                    }
                    else
                    {
                        jsonItem[kvp.Key] = kvp.Value.NumericalValue;
                    }
                }

                result.Add(jsonItem);
            }

            return result;
        }

        /// <summary>
        /// Convert from JSON to a value list
        /// </summary>
        /// <param name="a_root">A JSON element, e.g. as would result from serializing via ToUnspecificObject()</param>
        /// <returns>The value list as represented by the JSON element</returns>
        public static TSValueList FromJson(JsonElement a_root)
        {
            if (a_root.ValueKind != JsonValueKind.Array)
            {
                throw new ArgumentException(
                    String.Format("Json is of unsupported element type {0}",
                        a_root.ValueKind),
                    nameof(a_root));
            }

            TSValueList result = new();
            foreach (var jsonItem in a_root.EnumerateArray())
            {
                if (jsonItem.ValueKind != JsonValueKind.Object)
                {
                    throw new ArgumentException(
                        String.Format("Json contains element of unsupported type {0}",
                            jsonItem.ValueKind),
                        nameof(a_root));
                }
                Dictionary<string, TSValue> valueItem = new();
                foreach (var jsonValue in jsonItem.EnumerateObject())
                {
                    valueItem[jsonValue.Name] = jsonValue.Value.ValueKind switch
                    {
                        JsonValueKind.String => new TSValue(jsonValue.Value.GetString()),
                        JsonValueKind.Number => new TSValue(jsonValue.Value.GetInt64()),
                        _ => throw new ArgumentException(
                            String.Format("Json contains unsupported element type {0} for \"{1}\"", jsonValue.Value.ValueKind, jsonValue.Name),
                            nameof(jsonItem)),
                    };
                }
                result.Add(valueItem);
            }
            return result;
        }
    }
}
