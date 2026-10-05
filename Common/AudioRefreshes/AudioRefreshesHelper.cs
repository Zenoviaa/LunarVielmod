using Stellamod.Core;
using Terraria.Audio;

namespace Stellamod.Common.AudioRefreshes;

public static class AudioRefreshesHelper
{
    public static void PlayChestOpenSound(Vector2 position)
    {
        var openSound = AssetReferences.Assets.Sounds.RefreshNAmbience.ChestOpen.Asset with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(openSound, position);
    }
    public static void PlayEquipVanitySound()
    {
        var sound = AssetReferences.Assets.Sounds.RefreshNAmbience.VanityArmorOn.Asset with { PitchVariance = 0.6f };
        SoundEngine.PlaySound(sound);
    }

    public static void PlayIceFootstepSound(Vector2 position)
    {
        var sound = AssetReferences.Assets.Sounds.RefreshNAmbience.IceFoot.Asset with { PitchVariance = 0.6f };
        SoundEngine.PlaySound(sound, position);         
    }
}
