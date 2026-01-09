using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    /// <summary>
    /// Manages creating the hold projectile for magic tomes
    /// </summary>
    public class TomePlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (Main.myPlayer != Player.whoAmI)
                return;

            //Check if the player is holding a tome, if they are then summon a hold projectile for the animation
            var heldItem = Player.HeldItem.ModItem;
            int tomeHoldType = ModContent.ProjectileType<TomeHold>();
            if(heldItem is AbstractMagicTome tome)
            {
                if (Player.ownedProjectileCounts[tomeHoldType] == 0 && Player.controlUseItem)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, tomeHoldType, 1, 1, Player.whoAmI);
                }
            }
        }
    }
}
