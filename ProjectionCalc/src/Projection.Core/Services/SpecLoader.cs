using System;
using System.IO;
using System.Text.Json;

namespace Projection.Core.Services
{

    public static class SpecLoader
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public static SpecFile LoadFromPath(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Spec file not found: {path}");
            }

            string json = File.ReadAllText(path);
            SpecFile spec = JsonSerializer.Deserialize<SpecFile>(json, Options)
                       ?? throw new InvalidOperationException("Spec file could not be parsed.");

            if (spec.Profiles is null || spec.Profiles.Count == 0)
            {
                throw new ArgumentException("Spec file must contain a non-empty object at data['profiles'].");
            }

            return spec;
        }
    }
}
