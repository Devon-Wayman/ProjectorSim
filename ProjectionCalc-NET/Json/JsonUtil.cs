using System.Globalization;
using System.Text.Json;

namespace ProjectionCalc_NET.Json
{
    public class JsonUtil
    {
        public static JsonElement GetObj(JsonElement parent, string name)
        {
            if (parent.ValueKind == JsonValueKind.Object && parent.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.Object)
            {
                return el;
            }

            // mimic the method in which pythons dict.get(key, {}) functions
            using var emptyDoc = JsonDocument.Parse("{}");
            return emptyDoc.RootElement.Clone();
        }

        public static string? GetString(JsonElement obj, string name)
        {
            if (obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out var el))
            {
                return el.ValueKind switch
                {
                    JsonValueKind.String => el.GetString(),
                    JsonValueKind.Number => el.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => el.GetRawText()
                };
            }

            return null;
        }

        public static double GetDouble(JsonElement obj, string name)
        {
            if (obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out var el))
            {
                if (el.ValueKind == JsonValueKind.Number && el.TryGetDouble(out var num))
                {
                    return num;
                }

                if (el.ValueKind == JsonValueKind.String)
                {
                    var s = el.GetString();

                    if (!string.IsNullOrWhiteSpace(s) && double.TryParse(s, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    {
                        return v;
                    }
                }
            }

            return 0.0;
        }
    }
}