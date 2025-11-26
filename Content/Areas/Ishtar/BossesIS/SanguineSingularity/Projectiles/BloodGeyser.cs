using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

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
    public class BloodGeyser : ModProjectile,
        IDrawSanguineBlood
    {
        private Vector2[] BlastPos;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            List<Vector2> blastPoints = new List<Vector2>();
            float numPoints = 80;
            for (float f = 0; f < numPoints; f++)
            {
                float completionRatio = f / numPoints;
                Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                blastPoints.Add(point);
            }
            BlastPos = blastPoints.ToArray();
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(56, 0, completionRatio) * MathF.Sin(completionRatio * 4);
        }

        public void DrawToSanguineMask(SpriteBatch spriteBatch)
        {
            if (BlastPos == null)
                return;

            var shader = BasicLaserAlphaShader.Instance;
            shader.Tiling = Vector2.One * 1;
            shader.LaserTexture = TrailRegistry.LightningTrail2;
            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, BlastPos, ColorFunction, WidthFunction, shader);

        }
    }
}
