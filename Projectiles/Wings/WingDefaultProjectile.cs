using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Wings
{
    internal abstract class WingDefaultProjectile : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        public int FrameSpeed;
        public bool AlwaysAnimate = false;
        public int AccessoryItemType;
        public Vector2 WingOffset;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hide = true;
            Projectile.timeLeft = int.MaxValue;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            base.AI();
            if (!Owner.HasItemEquipped(AccessoryItemType))
            {
                Projectile.Kill();
                return;
            }

            if(Owner.direction >= 0)
            {
                Projectile.Center = Owner.Center + WingOffset * (-Owner.direction);
            }
            if (Owner.direction < 0)
            {
                Projectile.Center = Owner.Center - WingOffset * (-Owner.direction);
            }


            Projectile.spriteDirection = Owner.direction;


            if(AlwaysAnimate || IsFlying())
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= FrameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                Projectile.frame = 0;
            }
        }

        private bool IsFlying()
        {
            return Owner.controlJump && !Owner.mount.Active && Owner.wingTime > 0;
        }

        private bool IsHovering()
        {
            return Owner.controlDown && Owner.controlJump && !Owner.mount.Active && Owner.wingTime > 0;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
