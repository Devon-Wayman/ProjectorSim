using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectionCalc_NET.Services
{
    public static class SpecLoader
    {
        public static Dictionary<string, JsonElement> LoadProfiles(string specPath)
        {
            if (!File.Exists(specPath))
                throw new FileNotFoundException($"Spec file not found: {specPath}");

            var json = File.ReadAllText(specPath);

            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("profiles", out var profilesEl) ||
                profilesEl.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException("Spec file must contain a non-empty object at data['profiles'].");
            }

            var dict = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (var prop in profilesEl.EnumerateObject())
            {
                // CRITICAL: Clone so the JsonElement is independent of the disposed JsonDocument
                dict[prop.Name] = prop.Value.Clone();
            }

            if (dict.Count == 0)
                throw new ArgumentException("Spec file must contain a non-empty object at data['profiles'].");

            return dict;
        }
    }
}