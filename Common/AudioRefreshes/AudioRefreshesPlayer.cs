using Stellamod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.AudioRefreshes;

public class AudioRefreshesPlayer : ModPlayer
{
    bool _oldWet;
    bool IsWalkingOnSnow()
    {
        //check tile below
        var pointBelow = Player.Bottom.ToTileCoordinates();
        var standingTile = Main.tile[pointBelow];

        //We're actually walking on a snow tile
        if (!TileID.Sets.Ices[standingTile.type] && !TileID.Sets.IcesSlush[standingTile.type])
            return false;

        //The tile is active
        if (!standingTile.HasTile)
            return false;

        //Make sure the player is moving
        if ((Player.position - Player.oldPosition) == Vector2.Zero)
            return false;

        //The sound shouldn't play every tick
        if (Main.GameUpdateCount % 8 != 0)
            return false;
        return true;
    }

    bool JumpedInWater()
    {
        return Player.wet;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if(IsWalkingOnSnow())
        {
            AudioRefreshesHelper.PlayIceFootstepSound(Player.Bottom);
        }

        if(JumpedInWater() && !_oldWet)
        {
            SoundEngine.PlaySound(AssetReferences.Assets.Sounds.RefreshNAmbience.Watersubmerge.Asset with { PitchVariance = 0.6f }, Player.position);
            _oldWet = true;
        } else if(!JumpedInWater() && _oldWet)
        {
            SoundEngine.PlaySound(AssetReferences.Assets.Sounds.RefreshNAmbience.WaterEmerge.Asset with { PitchVariance = 0.6f }, Player.position);
            _oldWet = false;
        }
    }
}
