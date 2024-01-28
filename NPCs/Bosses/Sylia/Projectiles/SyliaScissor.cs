using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using System.Diagnostics.Metrics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class SyliaScissor : ModProjectile
    {
        private bool _sync;
        public Vector2 startCenter;
        public Vector2 targetCenter;
        public int delay;
        public bool playedSound;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(startCenter);
            writer.WriteVector2(targetCenter);
            writer.Write(delay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            startCenter = reader.ReadVector2();
            targetCenter = reader.ReadVector2();
            delay = reader.ReadInt32();
        }


        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel=1f)
        {
            //This code should give quite interesting movement

            //Accelerate to being on top of the player
            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X > distX)
                    Projectile.velocity.X = distX;

            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X < distX)
                    Projectile.velocity.X = distX;       
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y+=accel;
                if (Projectile.velocity.Y > distY)
                    Projectile.velocity.Y = distY;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y < distY)
                    Projectile.velocity.Y = distY;
            }
        }

        private ref float ai_Counter => ref Projectile.ai[0];
        public override void AI()
        {
            //Sync the initial values
            if (!_sync && Main.myPlayer == Projectile.owner)
            {
                Projectile.netUpdate = true;
                _sync = true;
            }

            delay--;      
            Vector2 direction = startCenter.DirectionTo(targetCenter);//(targetCenter - startCenter).SafeNormalize(Vector2.Zero);
            if (delay <= 0)
            {   
                float targetRotation = direction.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.75f);
                Projectile.velocity = direction * 24;
                if (!playedSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2"), Projectile.position);
                    playedSound = true;
                }
            }
            else if (delay == 1)
            {
                Dust.QuickDustLine(Projectile.Center, startCenter, 32f, Color.Violet);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 1f);
                    dust.noGravity = true;
                }

                Projectile.Center = startCenter;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), Projectile.position);
            }
            else
            {
                AI_Movement(startCenter, 25, accel: 5);
                ai_Counter++;
                Projectile.rotation += ai_Counter * 0.01f;
            }

            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 175), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }
}
