using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    internal class SunLaserProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        internal PrimitiveTrail BeamDrawer;
        public ref float Time => ref Projectile.ai[0];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public override string Texture => TextureRegistry.EmptyTexture;
        public const float LaserLength = 2400f;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 124;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.hide = true;
        }
        int Timeraa = 0;
        public override void AI()
        {
            Timeraa++;


            if (Timeraa == 1)
            {
                Projectile.rotation = Main.rand.NextFloat(0, MathHelper.Pi);
            }

            float distance = 300;
            Projectile.rotation -= 0.04f;
            Projectile.Center = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * distance;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
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

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < 60)
            {
                mult = (float)Projectile.timeLeft / (float)60;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult;
        }

        public override bool ShouldUpdatePosition() => false;
        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.LightGoldenrodYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.WaterTrail);

            List<float> originalRotations = new();
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, i / 8f));
                originalRotations.Add(MathHelper.PiOver2);
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {

        }

        public override bool? CanDamage() => Time >= 70f;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCs.Add(index);
        }
    }
}
