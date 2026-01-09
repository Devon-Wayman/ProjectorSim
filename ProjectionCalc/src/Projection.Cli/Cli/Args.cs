namespace Projection.Cli.CliMods
{
    public sealed class Args
    {
        public string Spec { get; set; } = "specs.json";

        // default projector key if no key is provided
        public string Key { get; set; } = "ESPSON_EB-PU2213B__ELPX02S";

        public bool List { get; set; }

        public double? Distance { get; set; }
        public string DistanceUnit { get; set; } = "m";
        public string OutUnit { get; set; } = "in";

        public string? AspectOverride { get; set; }
    }
}