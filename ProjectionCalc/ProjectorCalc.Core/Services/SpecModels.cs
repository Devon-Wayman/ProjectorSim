using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Projection.Core.Services
{

    public sealed class SpecFile
    {
        [JsonPropertyName("profiles")]
        public Dictionary<string, Profile> Profiles { get; set; } = new Dictionary<string, Profile>();
    }

    public sealed class Profile
    {
        [JsonPropertyName("projector")]
        public ProjectorSpec Projector { get; set; } = new ProjectorSpec();

        [JsonPropertyName("lens")]
        public LensSpec Lens { get; set; } = new LensSpec();
    }

    public sealed class ProjectorSpec
    {
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; } = "";

        [JsonPropertyName("model")]
        public string Model { get; set; } = "";

        [JsonPropertyName("native_aspect_ratio")]
        public string NativeAspectRatio { get; set; } = "16:9";
    }

    public sealed class LensSpec
    {
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; } = "";

        [JsonPropertyName("model")]
        public string Model { get; set; } = "";

        [JsonPropertyName("throw_ratio")]
        public double ThrowRatio { get; set; }
    }
}