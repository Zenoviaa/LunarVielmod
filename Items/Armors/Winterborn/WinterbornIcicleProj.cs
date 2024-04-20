using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Winterborn
{
    internal class WinterbornIcicleProj : ModProjectile
    {
        private float _dustTimer;
        private float Health
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float Timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                Timer += Main.rand.NextFloat(0, 240);
                Projectile.netUpdate = true;
            }

            _dustTimer++;
            if(_dustTimer >= 24)
            {
                _dustTimer = 0;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), newColor: Color.LightCyan, Scale: 0.6f);
                Dust.NewDustPerfect(Projectile.position, 
                    ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 1f)).RotatedByRandom(19.0), 0, Color.LightCyan, 0.5f).noGravity = true;
            }

            AI_RotateAroundOwner();
            AI_CollideWithProjectiles();
        }

        private void AI_RotateAroundOwner()
        {
            float offsetProgress = Timer / 240;
            float degrees = offsetProgress * MathHelper.TwoPi;
            float circleDistance = 64;
            Vector2 circleCenter = Owner.Center;
            Vector2 circleOffset = new Vector2(circleDistance, 0);
            Vector2 rotatedCirclePosition = circleCenter + circleOffset.RotatedBy(degrees);
            Projectile.Center = rotatedCirclePosition;

            float osc = MathF.Sin(offsetProgress) * 16;
            Projectile.Center += new Vector2(osc, 0).RotatedBy(degrees);
            //Projectile.rotation = Owner.Center.DirectionTo(Projectile.Center).ToRotation();
        }

        private void AI_CollideWithProjectiles()
        {
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.hostile)
                {
                    Rectangle otherRect = p.getRect();
                    if (Projectile.Colliding(myRect, otherRect) && p.active && p.penetrate == 0)
                    {
                        for (int t = 0; t < 8; t++)
                        {
                            Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                            float scale = Main.rand.NextFloat(0.5f, 0.75f);
                            Dust.NewDustPerfect(Projectile.Center, DustID.Ice, speed, newColor: Color.White, Scale: scale);
                        }
        
                        Health -= p.damage;
                        if(Health <= 0)
                        {
                            Projectile.Kill();
                        }

                        SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
                        p.Kill();
                
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightCyan, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int t = 0; t < 8; t++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                float scale = Main.rand.NextFloat(0.5f, 0.75f);
                Dust.NewDustPerfect(Projectile.Center, DustID.Ice, speed, newColor: Color.White, Scale: scale);
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            Health -= damageDone;
            if(Health <= 0)
            {
                Projectile.Kill();
            }
        }
    }
}
