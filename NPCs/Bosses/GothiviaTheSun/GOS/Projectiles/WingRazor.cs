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



namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles
{
    internal class WingRazor : ModProjectile
    {

        private ref float ai_Counter => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 225;
            Projectile.height = 225;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
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

            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;

         
            if (Timer == 1)
            {
                for (int inn = 0; inn < 38; inn++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Turquoise, 1f).noGravity = true;
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


                    
                }
            }
            ai_Counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.Center = npc.Center;

            UpdateFrame(0.6f, 1, 60);
        }

       



       
        public override void OnKill(int timeLeft)
        {
            for (int inn = 0; inn < 38; inn++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Turquoise, 1f).noGravity = true;
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


                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<GothBlastExplosionProj>(), 40, 1, Main.myPlayer, 0, 0);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<GothBlastExplosionProj2>(), 40, 1, Main.myPlayer, 0, 0);


            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothExplode") { Pitch = Main.rand.NextFloat(-5f, 5f) }, Projectile.Center);
            var EntitySource = Projectile.GetSource_FromThis();
            float num = 8;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
            switch (Main.rand.Next(2))
            {
                case 0:
                    for (int i = 0; i < num; i++)
                    {
                        float progress = (float)i / num;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 velocity = rot.ToRotationVector2();
                        if (StellaMultiplayer.IsHost)
                        {
                            float knockback = 1;
                            Projectile.NewProjectile(EntitySource, Projectile.Center, velocity,
                                ModContent.ProjectileType<GothFireBlowtorchBlastProj>(), 1800, knockback, Main.myPlayer);
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < num; i++)
                    {
                        float progress = (float)i / num;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 velocity = rot.ToRotationVector2();
                        if (StellaMultiplayer.IsHost)
                        {
                            float knockback = 1;
                            Projectile.NewProjectile(EntitySource, Projectile.Center, velocity,
                                ModContent.ProjectileType<GothSunBlowtorchBlastProj>(), 1800, knockback, Main.myPlayer);
                        }
                    }
                    break;


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

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirnTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirnTrail").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirnTrail").Value;
            var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/DirnTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(25, 250, 250, 70) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(100f), (p) => new Color(10, 255, 250, 80) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;

            }
            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(120f), (p) => new Color(55, 200, 225, 70) * (1f - p));
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

            Rectangle rectangle = new Rectangle(0, 0, 225, 225);
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
