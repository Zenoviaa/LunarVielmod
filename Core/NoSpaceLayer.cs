using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core;

public class NoSpaceLayerPlayer : ModPlayer
{
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if(Player.ZoneSkyHeight)
            Player.gravity = Player.defaultGravity;
    }
}
