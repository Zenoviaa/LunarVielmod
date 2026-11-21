using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common;


public class NoMusicSystem : ModSystem
{
    float _oldMusicVolume;
    float _fadeOutTimer;
    public static float globalVolume = 1f;
    public static bool disableMusic;
    public const float MUSIC_FADE_OUT_TIME = 60;
    public override void PreUpdatePlayers()
    {
        base.PreUpdatePlayers();
        disableMusic = false;
    }

    public override void PostUpdatePlayers()
    {
        base.PostUpdatePlayers();
        if (disableMusic)
        {
            _fadeOutTimer--;

        }
        else
        {
            _fadeOutTimer++;
        }
        _fadeOutTimer = MathHelper.Clamp(_fadeOutTimer, 0, MUSIC_FADE_OUT_TIME);
        globalVolume = _fadeOutTimer / MUSIC_FADE_OUT_TIME;

    }

    public override void Load()
    {
        base.Load();
        On_Main.UpdateAudio += MuteMusic;

    }

    private void MuteMusic(On_Main.orig_UpdateAudio orig, Main self)
    {
        _oldMusicVolume = Main.musicVolume;
        Main.musicVolume = _oldMusicVolume * globalVolume;
        orig(self);
        Main.musicVolume = _oldMusicVolume;
    }
}
