using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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
            Projectile.timeLeft = 60;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Red * 0.4f, 1, 30);

                var sound1 = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                sound1.Pitch = -0.5f;
                sound1.PitchVariance = 0.2f;
                SoundEngine.PlaySound(sound1, Projectile.position);
            }
            float time = Timer / 60f;
            float ease = EasingFunction.OutExpo(time);
            List<Vector2> blastPoints = new List<Vector2>();
            float numPoints = 80;
            for (float f = 0; f < numPoints; f++)
            {
                float completionRatio = f / numPoints;
                completionRatio *= ease;
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
            float widthMult = MathHelper.Lerp(0f, 1f, (float)Projectile.timeLeft / 10f);
            widthMult = MathHelper.Clamp(widthMult, 0f, 1f);
            return MathHelper.SmoothStep(256, 0, completionRatio) * MathF.Sin(completionRatio * 4) * widthMult;
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
