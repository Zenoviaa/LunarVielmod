using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using Terraria;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity.Projectiles
{
    public class BloodRain : ScarletProjectile,
        IDrawSanguineBlood
    {
        private float _trailWidth;

        private ref float Timer => ref Projectile.ai[0];
        private ref float Size => ref Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Projectile.velocity.Y < 20)
            {
                Projectile.velocity.Y += 0.4f;
            }
            if (Timer >= 540f)
            {
                _trailWidth = MathHelper.Lerp(_trailWidth, 0f, 0.1f);
                Projectile.velocity *= 0.9f;
                if (Projectile.velocity.Length() <= 1f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                _trailWidth = MathHelper.Lerp(_trailWidth, 1f, 0.1f);
            }


            Projectile.velocity.X = -2;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(Size, 0, completionRatio) * _trailWidth * MathF.Sin(completionRatio * 4);
        }

        public void DrawToSanguineMask(SpriteBatch spriteBatch)
        {
            var shader = BasicLaserAlphaShader.Instance;
            shader.Tiling = Vector2.One * 1;
            shader.LaserTexture = TrailRegistry.LightningTrail2;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
        }
    }
}
