using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common;

public class StylishPlayer : ModPlayer
{
    public Vector2? moveToPosition;
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (moveToPosition.HasValue)
        {
            Vector2 vel = moveToPosition.Value - Player.Center;
            Player.velocity = vel;
            moveToPosition = null;
        }
    }
}

public static class StylishPlayerExtensions
{
    extension(Player stylishPlayer)
    {
        public Vector2? moveToPosition
        {
            get => stylishPlayer.GetModPlayer<StylishPlayer>().moveToPosition;
            set => stylishPlayer.GetModPlayer<StylishPlayer>().moveToPosition = value;
        }
    }
}
