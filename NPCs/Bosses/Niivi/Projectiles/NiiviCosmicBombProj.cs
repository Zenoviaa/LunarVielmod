using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviCosmicBombProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        private enum ActionState
        {
            Growing,
            Homing
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];
        private float Scale = 0f;
        private float DistortTimer;
        private bool SwapColor;
        private bool Distort;
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.78f;
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (Projectile.velocity.X < targetVelocity.X)
            {
                Projectile.velocity.X++;
                if (Projectile.velocity.X >= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }
            else if (Projectile.velocity.X > targetVelocity.X)
            {
                Projectile.velocity.X--;
                if (Projectile.velocity.X <= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }

            if (Projectile.velocity.Y < targetVelocity.Y)
            {
                Projectile.velocity.Y++;
                if (Projectile.velocity.Y >= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
            else if (Projectile.velocity.Y > targetVelocity.Y)
            {
                Projectile.velocity.Y--;
                if (Projectile.velocity.Y <= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
        }

        public override void AI()
        {
            //So this projectlile has quite a few states
            switch (State)
            {
                case ActionState.Growing:
                    AI_Growing();
                    break;
                case ActionState.Homing:
                    AI_Homing();
                    break;
            }

            Projectile.rotation += 0.05f;
        }

        private void AI_Growing()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            Timer++;
            if (Timer % 4 == 0)
            {
                float starRadius = 1024;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(starRadius, starRadius);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 16;
                    ParticleManager.NewParticle<StarParticle>(pos, vel, Color.White, 1f);
                }
            }
            if (Timer > 0 && Timer < 120)
            {
                Scale = MathHelper.Lerp(Scale, 1f, 0.1f);
            }
            if (Timer == 120)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<NiiviCosmicBombAbsorbProj>(), 0, 0, Projectile.owner);
                screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 15);
            }
            if(Timer > 120 && Timer < 240)
            {
                Scale = MathHelper.Lerp(Scale, 2f, 0.1f);
            }
            if(Timer == 240)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<NiiviCosmicBombAbsorbProj>(), 0, 0, Projectile.owner);
                screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 15);
            }
            if (Timer > 240 && Timer < 360)
            {
                Scale = MathHelper.Lerp(Scale, 6f, 0.1f);
            }
            if (Timer == 360)
            {
                SwapColor = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<NiiviCosmicBombAbsorbProj>(), 0, 0, Projectile.owner);
                screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 15);
            }
            if (Timer > 360 && Timer < 480)
            {
                Scale = MathHelper.Lerp(Scale, 18, 0.1f);
            }
            if(Timer >= 480)
            {
       
                State = ActionState.Homing;
                Timer = 0;
            }
        }

        private void AI_Homing()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            Timer++;
            if(Timer == 1)
            {
                screenShaderSystem.VignetteScreen(-1f);
            }


            float maxDetectDistance = 3000;
            float maxSpeed = 12;
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
            if(player != null)
            {
                AI_MoveToward(player.Center, maxSpeed);
            }

            if(Timer >= 300)
            {
                //KABOOM
                screenShaderSystem.UnVignetteScreen();
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.Red.R,
                Color.Red.G,
                Color.Red.B, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/ZuiEffect");

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            Color drawColor = (Color)GetAlpha(lightColor);
            float drawScale = Projectile.scale * Scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;
            shader.UseOpacity(0.3f);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(1f);

            //How fast the extra texture animates
            float speed = 0.2f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(Color.RoyalBlue);
            if (SwapColor)
            {
                shader.UseColor(Main.DiscoColor);
            }

            //Texture itself
            shader.UseImage1(texture);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            float drawRotation = MathHelper.TwoPi;
            float num = 16;
            for (int i = 0; i < num; i++)
            {
                float nextDrawScale = drawScale;
                float nextDrawRotation = drawRotation * (i / num);
                spriteBatch.Draw(texture.Value, drawPosition, null, drawColor, nextDrawRotation, drawOrigin, nextDrawScale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();
            spriteBatch.Begin();
            //I think that one texture will work
            //The vortex looking one
            //And make it spin
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 4000, 64);
            float num = 32;
            for(float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 33;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                    ModContent.ProjectileType<NiiviCometProj>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 16;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 15;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<NiiviCometProj>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 8;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 12;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<NiiviCometProj>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 4;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 6;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<NiiviCometProj>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, speed * 17, Scale: 5f);
                d.noGravity = true;

                Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                var da = Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, speeda * 11, Scale: 5f);
                da.noGravity = false;

                Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
                var dab = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, speeda * 30, Scale: 5f);
                dab.noGravity = false;
            }
        }
    }
}
