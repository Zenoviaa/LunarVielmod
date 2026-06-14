using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles
{
    public class HavocLaserBigProj : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public override string Texture => TextureRegistry.EmptyTexture;
        public const float LaserLength = 2400f;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 72;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;
            Projectile.velocity = Owner.rotation.ToRotationVector2();
            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Projectile.scale = MathF.Sin(Time / 600f * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;


            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.5f);
            Time++;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool? CanDamage() => Time >= 60f;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
