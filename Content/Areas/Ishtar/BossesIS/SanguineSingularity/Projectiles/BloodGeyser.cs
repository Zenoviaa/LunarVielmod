using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
        private int Owner
        {
            get => (int)(Projectile.ai[1]);
        }
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
            Projectile.timeLeft = 120;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(Timer >= 60f)
            {
                if (BlastPos == null)
                    return false;
                return ProjectileHelper.OldPosColliding(BlastPos, projHitbox, targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        private void CreateGoreBurst(Vector2 position, Vector2 velocity)
        {
            int[] gores = AutoGoreLoader.FindGores("BloodChunk");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    position,
                    velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
            }

            for (float f = 0; f < 16; f++)
            {
                Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                vel *= Main.rand.NextFloat(0f, 1f);
                var d = Dust.NewDustPerfect(position, DustID.Blood, vel, newColor: Color.White);
                d.noGravity = false;
            }
        }

        public override void AI()
        {
            base.AI();
            float halfTime = 60;
            Timer++;
            if(Timer == halfTime)
            {
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Red * 0.4f, 1, 5);

                var sound1 = AssetRegistry.Sounds.SanguineSingularity.SanguineCyst;
                sound1.Pitch = 0.5f;
                sound1.PitchVariance = 0.2f;
                SoundEngine.PlaySound(sound1, Projectile.position);

                var sound2 = AssetRegistry.Sounds.SanguineSingularity.BloodyDeath;
                sound2.Pitch = -0.5f;
                sound2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(sound2, Projectile.position);

                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                ShakeModSystem.Shake = 8;
                CreateGoreBurst(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 10);
                CreateGoreBurst(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 5);
                float numDust = 8;
                for(float d = 0; d < numDust; d++)
                {
                    Vector2 dustVelocity = Projectile.velocity.RotateRandom(0.5f);
                    dustVelocity = dustVelocity.SafeNormalize(Vector2.Zero);
                    dustVelocity *= Main.rand.NextFloat(5f, 35f);
                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood, dustVelocity, newColor: Color.Red, Scale: Main.rand.NextFloat(1f, 4f));
                }
            }

            NPC npc = Main.npc[Owner];
            Projectile.Center = npc.Center;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Timer >= halfTime)
            {
                float time = (Timer - halfTime) / 60f;
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
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        private float WidthFunction(float completionRatio)
        {
            float widthMult = MathHelper.Lerp(0f, 1f, (float)Projectile.timeLeft / 10f);
            widthMult = MathHelper.Clamp(widthMult, 0f, 1f);
            return MathHelper.SmoothStep(128, 0, completionRatio) * MathF.Sin(completionRatio * 4) * widthMult;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
            Vector2 drawOrigin = new Vector2(lineTexture.Width / 2, 0);
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.Red;
            drawColor.A = 0;
            drawColor *= 0.5f;
            drawColor *= Timer / 60f;

            float widthMult = MathHelper.Lerp(0f, 1f, (float)Projectile.timeLeft / 10f);
            drawColor *= widthMult;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 scale = Vector2.One;
            scale.Y = 2;
            scale *= EasingFunction.QuadraticBump(Timer / 60f);
            spriteBatch.Draw(lineTexture, drawCenter, null, drawColor, Projectile.rotation - MathHelper.ToRadians(90), drawOrigin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
