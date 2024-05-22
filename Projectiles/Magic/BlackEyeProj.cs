using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BlackEyeProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 17, Scale: 5f);
                    d.noGravity = true;

                    Vector2 speeda = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var da = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speeda * 11, Scale: 5f);
                    da.noGravity = false;

                    Vector2 speedab = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                    var dab = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speeda * 30, Scale: 5f);
                    dab.noGravity = false;
                }
                Projectile.velocity = Vector2.Zero;
                Projectile.velocity += -Vector2.UnitY * 4;
            }
            else
            {
                Projectile.velocity *= 0.8f;
            }

            if(Timer == 30)
            {
                float maxDetectDistance = 2400;
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
                if(npc != null)
                {
                    Vector2 velocity = Projectile.Center.DirectionTo(npc.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<BlackEyeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
              
            }

            if(Timer >= 120)
            {
                Projectile.Kill();
            }
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);


            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 17, Scale: 5f);
                d.noGravity = true;

                Vector2 speeda = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                var da = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speeda * 11, Scale: 5f);
                da.noGravity = false;

                Vector2 speedab = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
                var dab = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speeda * 30, Scale: 5f);
                dab.noGravity = false;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 120);
        }
    }
}
