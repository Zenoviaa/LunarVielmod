using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Swingers
{
    internal class VulcanBreakerBlowtorchProj : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;
        public override string Texture => TextureRegistry.EmptyTexture;
        //Ai
        private ref float Timer => ref Projectile.ai[0];
        private float LifeTime => 45;
        private float BlowtorchDistance => 512;

        //Draw Code
        private Vector2[] LinePos;
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            LinePos = new Vector2[5];
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
            }

            if (Projectile.scale < 1f || Timer <= 1)
            {
                Projectile.scale = MathF.Sin(Timer / 600f * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutExpo(progress);
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * easedProgress
                    * BlowtorchDistance, i / 8f));
            }
            LinePos = points.ToArray();
        }

        public override bool ShouldUpdatePosition()
        {
            //Returning false makes velocity not move the projectile
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 60);
        }

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < LifeTime / 3f)
            {
                mult = Projectile.timeLeft / (float)LifeTime / 3f;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult * MathF.Sin((1f - completionRatio) * 0.5f);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio);
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = LinePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 32, ref collisionPoint))
                    return true;
            }

            //Return false to not use default collision
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            BeamDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            BeamDrawer.SpecialShader.UseColor(Color.Lerp(Color.White, Color.OrangeRed, 0.3f));
            BeamDrawer.SpecialShader.SetShaderTexture(TrailRegistry.BeamTrail);
            BeamDrawer.DrawPixelated(LinePos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}