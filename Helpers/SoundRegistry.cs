using Terraria.Audio;

namespace Stellamod.Helpers
{
    internal static class SoundRegistry
    {
        private static string Path => "Stellamod/Assets/Sounds/";
        public static SoundStyle JugglerHit => new SoundStyle($"{Path}JugglerHit");
    }
}
