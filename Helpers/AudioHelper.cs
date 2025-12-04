using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public static class AudioHelper
    {
        public static void Mute(int id)
        {
            Main.musicFade[id] = 0f;
        }
        public static void Mute(Mod mod, string musicPath)
        {
            Main.musicFade[MusicLoader.GetMusicSlot(mod, musicPath)] = 0f;
        }
    }
}
