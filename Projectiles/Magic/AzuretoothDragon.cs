using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class AzuretoothDragon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 150;

            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            float maxDetectDistance = 1500;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (closestNpc != null)
            {
                Vector2 targetVelocity = Projectile.Center.DirectionTo(closestNpc.Center) * 16;
                Vector2 velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.08f);
                Projectile.velocity = velocity;
                Projectile.alpha++;
                if (Projectile.alpha >= 255)
                    Projectile.alpha = 255;
            }

            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.5f);
            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.04f, Color.Transparent, completionRatio);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.alpha -= 50;
            Projectile.velocity *= 2;
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Frostburn2, 60);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return base.PreDraw(ref lightColor);
        }


        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public new Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;

        public override void PostDraw(Color lightColor)
        {

        }

        private void Visuals()
        {
            int frameSpeed = 2;
            DrawHelper.AnimateTopToBottom(Projectile, frameSpeed);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightSkyBlue, 1f).noGravity = true;
        }
    }
}
