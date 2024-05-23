using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Event.GreenSun.Dulacrowe
{
    internal class TulacBombProj : ModProjectile
    {
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
        private bool SwapColor;
        private bool CondenseColor;
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
            if(Timer == 1)
            {
           //     SoundStyle soundStyle = SoundRegistry.Niivi_BigCharge;
            //    soundStyle.Volume = 0.7f;
           //     SoundEngine.PlaySound(soundStyle, Projectile.position);
            }


            if (Timer > 0 && Timer < 120)
            {
                Scale = MathHelper.Lerp(Scale, 1f, 0.1f);
            }
            if (Timer == 120)
            {
              //  SoundEngine.PlaySound(SoundRegistry.Niivi_PrimGrow1, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);

                screenShaderSystem.TintScreen(Color.White, 0.3f, timer: 15);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower1") { Pitch = Main.rand.NextFloat(-10f, 10f) }, Projectile.Center);
                float num = 4;
                for (float i = 0; i < num; i++)
                {
                    float progress = i / num;
                    float rot = MathHelper.TwoPi * progress;
                    Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                    Vector2 velocity = direction * 13;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                }
            }
            if(Timer > 120 && Timer < 240)
            {
                Scale = MathHelper.Lerp(Scale, 2f, 0.1f);
            }
            if(Timer == 240)
            {
                //   SoundEngine.PlaySound(SoundRegistry.Niivi_PrimGrow1, Projectile.position);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower1") { Pitch = Main.rand.NextFloat(-10f, 10f) }, Projectile.Center);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);

                screenShaderSystem.TintScreen(Color.White, 0.3f, timer: 15);

                float num = 8;
                for (float i = 0; i < num; i++)
                {
                    float progress = i / num;
                    float rot = MathHelper.TwoPi * progress;
                    Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                    Vector2 velocity = direction * 13;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer > 240 && Timer < 360)
            {
                Scale = MathHelper.Lerp(Scale, 4f, 0.1f);
            }
            if (Timer == 360)
            {
                SwapColor = true;
                //   SoundEngine.PlaySound(SoundRegistry.Niivi_PrimGrow2, Projectile.position);

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower1") { Pitch = Main.rand.NextFloat(-10f, 10f) }, Projectile.Center);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024, 16);

                screenShaderSystem.TintScreen(Color.White, 0.3f, timer: 15);

                float num = 64;
                for (float i = 0; i < num; i++)
                {
                    float progress = i / num;
                    float rot = MathHelper.TwoPi * progress;
                    Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                    Vector2 velocity = direction * 13;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer > 360 && Timer < 480)
            {
                Scale = MathHelper.Lerp(Scale, 6, 0.1f);
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


            screenShaderSystem.TintScreen(Color.DarkSeaGreen, 0.2f);
            float maxDetectDistance = 3000;
            float maxSpeed = 4;
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
            if(player != null)
            {
                AI_MoveToward(player.Center, maxSpeed);
            }



            if(Timer % 20 == 0)
            {
                if (player != null)
                {
                    float starRadius = 5;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 projSpawn = Projectile.Center + Main.rand.NextVector2CircularEdge(starRadius, starRadius);
                        Vector2 direction = projSpawn.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4);
                        Vector2 velocity = direction * 22;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), projSpawn, velocity,
                            ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
                    }
                    Vector2 diffVelocity = player.velocity - player.oldVelocity;
                    Projectile.position += diffVelocity;
                }
            }
            
            if(Timer == 300)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_Starence, Projectile.position);
            }



            if(Timer >= 575)
            {
                CondenseColor = true;
                Scale = MathHelper.Lerp(Scale, VectorHelper.Osc(0f, 8f, speed: 16f), 0.3f);
            }

            if(Timer >= 600)
            {
                //KABOOM
                screenShaderSystem.UnTintScreen();
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
            var texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/GreenSunEffect2");

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            Color drawColor = (Color)GetAlpha(lightColor);
            float drawScale = Projectile.scale * Scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            if (CondenseColor)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
         

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
            shader.UseColor(Color.ForestGreen);
            if (SwapColor)
            {
                shader.UseColor(Color.DarkOrange);
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
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekShockwave") { Pitch = Main.rand.NextFloat(-10f, 10f) }, Projectile.Center);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 4000, 80);
            float num = 16;
            for(float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 33;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                    ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 8;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 15;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 4;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 12;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            num = 2;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                float rot = MathHelper.TwoPi * progress;
                Vector2 direction = Vector2.UnitY.RotatedBy(rot);
                Vector2 velocity = direction * 6;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<TulacroweFireball>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner);
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, speed * 17, Scale: 5f);
                d.noGravity = true;

                Vector2 speeda = Main.rand.NextVector2CircularEdge(4f, 4f);
                var da = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speeda * 11, Scale: 5f);
                da.noGravity = false;

                Vector2 speedab = Main.rand.NextVector2CircularEdge(5f, 5f);
                var dab = Dust.NewDustPerfect(Projectile.Center, DustID.CoralTorch, speeda * 30, Scale: 5f);
                dab.noGravity = false;
            }
        }
    }
}
