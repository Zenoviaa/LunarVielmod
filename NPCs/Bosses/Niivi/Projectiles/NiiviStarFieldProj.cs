using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviStarFieldProj : ModProjectile
    {
        public override string Texture => TextureRegistry.FlowerTexture;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LifeTime => 680;
        private float MaxScale => 4.66f;
        private float Scale;
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.hide = true;
        }

        public override void AI()
        {
            ShakeModSystem.Shake = 2;
            if(Timer < 60)
            {
                Scale = MathHelper.Lerp(Scale, 1f, 0.04f);
            } else if (Timer > 600)
            {
                Scale = MathHelper.Lerp(Scale, 0f, 0.04f);
            }
  
            Timer++;
            if(Timer == 1)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048, 32);
                SoundEngine.PlaySound(SoundRegistry.Niivi_Voidfield, Projectile.position);
            }

            if(Timer == 300)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_Voidence, Projectile.position);
            }

            if (Main.netMode != NetmodeID.Server && !Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                float rippleCount = 5;
                float rippleSize = 25;
                float rippleSpeed = 40;
                Terraria.Graphics.Effects.Filters.Scene.Activate("Shockwave", Projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(Projectile.Center);
            }

            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                float distortStrength = 500;
                float progress = Timer / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }

            if(Timer % 12 == 0 && Timer >= 60)
            {
                float maxDetectDistance = 2000;
                Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, maxDetectDistance);
                if(player != null)
                {
                    Vector2 randOffset = Main.rand.NextVector2Circular(64, 64);
                    Vector2 predictiveOffset = player.velocity.SafeNormalize(Vector2.Zero) * 8;
                    Vector2 spawnPoint = player.Center + predictiveOffset + randOffset;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, Vector2.Zero,
                        ModContent.ProjectileType<NiiviStarFieldBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    randOffset = Main.rand.NextVector2Circular(512, 512);
                    spawnPoint = player.Center + randOffset;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPoint, Vector2.Zero,
                         ModContent.ProjectileType<NiiviStarFieldBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                }
            }

            if (Timer % 7 == 0)
            {
                Vector2 randOffset = Main.rand.NextVector2Circular(512, 512);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Color[] colors = new Color[] { Color.LightCyan, Color.Cyan, Color.White, Color.White };
                Color color = colors[Main.rand.Next(0, colors.Length)];
                float scale = Main.rand.NextFloat(0.5f, 0.8f);
                ParticleManager.NewParticle<StarParticle>(Projectile.Center + randOffset, velocity, color, scale);
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player npc = Main.player[i];

                if (npc.active)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance >= 512)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction;
                        npc.wingTime = npc.wingTimeMax;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightCyan.R,
                Color.LightCyan.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>(TextureRegistry.CircleOutline);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Calculate the scale with easing
            Color drawColor = (Color)GetAlpha(lightColor);
            float drawScale = Projectile.scale * MaxScale * Scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

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
                spriteBatch.Draw(texture.Value, drawPosition, null, (Color)GetAlpha(lightColor), nextDrawRotation, drawOrigin, nextDrawScale, SpriteEffects.None, 0f);
            }


            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            behindNPCsAndTiles.Add(index);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.TintScreen(Color.Black, 0.5f, timer: 60);
            SoundEngine.PlaySound(SoundRegistry.Niivi_PrimAm, Projectile.position);
            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
        }
    }
}
