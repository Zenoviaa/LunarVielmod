using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using System.IO;
using Stellamod.Dusts;
using Stellamod.Trails;
using Stellamod.Utilis;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Items.Accessories.Players;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Helpers;
using Stellamod.Projectiles.Visual;
using Stellamod.Buffs;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    internal class RazorBurns : ModProjectile
    {

        private ref float ai_Counter => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 175;
            Projectile.height = 175;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = 5;
            Projectile.hostile = true;
            Projectile.timeLeft = 812;
            Projectile.localNPCHitCooldown = 6;
            Projectile.usesLocalNPCImmunity = true;
        }

     

        float trueFrame = 0;
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        public override void AI()
        {
            Timer++;
            float ai1 = Projectile.whoAmI;

            if (Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<RazorRingFire>(), 40, 1, Main.myPlayer, 0f, ai1);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            ai_Counter++;

            float maxDetectDistance = 2000;
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
            if (player != null)
            {
                float moveSpeed = 16;
                float accel = 0.6f;
                AI_Movement(player.Center, moveSpeed, accel);
            }
            //Lighting
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            AI_Collide();
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(0.6f, 1, 60);
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }



        private void AI_Collide()
        {
            if (Timer < 30)
                return;
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];


                if (!p.active)
                    continue;
                if (p.type != ModContent.ProjectileType<RazorSuns>())
                    continue;
                if (p == Projectile)
                    continue;
                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    float scale = Main.rand.NextFloat(0.3f, 0.5f);
                    ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, scale);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") { Pitch = Main.rand.NextFloat(-5f, 5f) }, Projectile.Center);



                    Vector2 directionToProjectile = Projectile.Center.DirectionTo(p.Center);
                    p.velocity = directionToProjectile * 16;
                    p.ai[0] = 20;

                    Vector2 bounceVelocity = -Projectile.velocity * 1.7f;
                    Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                    Projectile.penetrate -= 1;
                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);

                    for (int inn = 0; inn < 38; inn++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
                    }

                    for (int inn = 0; inn < 28; inn++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                    }

                    for (int inn = 0; inn < 16; inn++)
                    {
                        //Get a random velocity
                        Vector2 velocity2 = Main.rand.NextVector2Circular(4, 4);

                        //Get a random
                        float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                        ParticleManager.NewParticle<BoreParticle>(Projectile.Center, velocity2, Color.White, randScale);
                    }
                    var entitySource = Projectile.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {


                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<GothCircleExplosionProj2>(), 40, 1, Main.myPlayer, 0, 0);


                    }



                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 8 * 55);
        }
        public override void OnKill(int timeLeft)
        {
            for (int inn = 0; inn < 38; inn++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }

            for (int inn = 0; inn < 28; inn++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int inn = 0; inn < 16; inn++)
            {
                //Get a random velocity
                Vector2 velocity2 = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<BoreParticle>(Projectile.Center, velocity2, Color.White, randScale);
            }
            var entitySource = Projectile.GetSource_FromThis();
            if (StellaMultiplayer.IsHost)
            {


                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<GothCircleExplosionProj2>(), 40, 1, Main.myPlayer, 0, 0);


            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }
        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 1.5f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }


        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public TrailRenderer SwordSlash3;
        public TrailRenderer SwordSlash4;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WaterTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(250, 20, 20, 70) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(100f), (p) => new Color(200, 25, 25, 80) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;

            }
            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(120f), (p) => new Color(255, 200, 2, 70) * (1f - p));
                SwordSlash3.drawOffset = Projectile.Size / 2f;

            }

            if (SwordSlash4 == null)
            {
                SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(80f), (p) => new Color(255, 255, 255, 80) * (1f - p));
                SwordSlash4.drawOffset = Projectile.Size / 2.2f;

            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);


            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(90); ;
            }
            SwordSlash.Draw(Projectile.oldPos, rotation);
            SwordSlash2.Draw(Projectile.oldPos, rotation);
            SwordSlash3.Draw(Projectile.oldPos, rotation);
            SwordSlash4.Draw(Projectile.oldPos, rotation);



            Main.spriteBatch.End();

            Main.spriteBatch.Begin();



            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new Rectangle(0, 0, 175, 175);
            rectangle.X = ((int)trueFrame % 5) * rectangle.Width;
            rectangle.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * rectangle.Height;

            Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = 0;
            float drawScale = 1.4f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                Color.White, drawRotation, origin, drawScale, SpriteEffects.None, 0f);




            return false;
        }
    }
}
