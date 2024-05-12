using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Desert.Projectiles
{
    internal class DustNadoProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        private float LifeTime => 600;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Timer++;
            Vector2 velocity = -Vector2.UnitY;
            float points = 8;
            float rotation = velocity.ToRotation();
            float xRadius = 24;
            float yRadius = 384;
            float angle = MathHelper.TwoPi * 4;

            for (float i = 0; i < points; i++)
            {
                float progress = i / points;
                float xOffset = xRadius * MathF.Cos(progress * angle) * progress;
                float yOffset = yRadius * MathF.Sin(progress * angle) * progress;
                Vector2 pointOnOval = Projectile.Center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
                pointOnOval += velocity * 512 * progress;

                progress = (i + 1f) / points;
                xOffset = xRadius * MathF.Cos(progress * angle) * progress;
                yOffset = yRadius * MathF.Sin(progress * angle) * progress;
                Vector2 nextPointOnOval = Projectile.Center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
                nextPointOnOval += velocity * 512 * progress;

                Vector2 tornadoVelocity = pointOnOval.DirectionTo(nextPointOnOval) * 32 * progress;
                Dust.NewDustPerfect(nextPointOnOval, ModContent.DustType<TSmokeTornadoDust>(), tornadoVelocity, 0, Color.Lerp(Color.WhiteSmoke, Color.DarkGray, progress), MathHelper.Lerp(0.4f, 0.7f, progress)).noGravity = true;
            }

            float suckingDistance = 4000;
            float suckStrength = 0.4f;
            Vector2 center = Projectile.Center + new Vector2(0, -384);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player npc = Main.player[i];

                if (npc.active)
                {
                    float distance = Vector2.Distance(center, npc.Center);
                    if (distance <= suckingDistance)
                    {
                        Vector2 direction = npc.Center - center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.4f;
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.boss)
                    continue;

                if (npc.active && npc.chaseable && npc.damage > 0 && !npc.friendly)
                {
                    float distance = Vector2.Distance(center, npc.Center);
                    if (distance <= suckingDistance)
                    {
                        Vector2 direction = npc.Center - center;
                        direction.Normalize();
                        if (distance < suckStrength)
                            suckStrength = distance;

                        Vector2 suckVelocity = direction * suckStrength;
                        npc.velocity -= suckVelocity * 0.66f;
                    }
                }
            }

            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                float distance = Vector2.Distance(center, item.Center);
                if (distance <= suckingDistance)
                {
                    Vector2 direction = item.Center - center;
                    direction.Normalize();
                    if (distance < suckStrength)
                        suckStrength = distance;
                    item.velocity -= direction * suckStrength;
                }
            }
        }
    }
}
