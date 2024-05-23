using Microsoft.Xna.Framework;
using ParticleLibrary;
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

namespace Stellamod.Projectiles.Magic
{
    internal class AlcadBombProj : ModProjectile
    {
        private ref float ai_suckStrength => ref Projectile.ai[0];
        private ref float ai_suckDistance => ref Projectile.ai[1];
        private ref float ai_suckTimer => ref Projectile.ai[2];
        public bool IsCharged => ai_suckTimer >= 60;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private void AI_KeepAlive()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                    Projectile.Kill();
                else
                {
                    float completion = ai_suckTimer / 60;
                    if (completion > 1f)
                        completion = 1f;

                    float moveSpeed = completion * 6;
                    Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, Main.MouseWorld, moveSpeed);
                    Projectile.netUpdate = true;         
                    if (Main.rand.NextBool(5))
                    {
                        Dust.QuickDustLine(Projectile.Center, player.Center, 48, Color.DarkBlue);
                    }
                }
            }
        }

        private void AI_IncreaseSuckTimers()
        {
            float maxSuckStrength = 1.25f;
            float maxSuckDistance = 512;

            ai_suckStrength += 0.01f;
            ai_suckDistance++;
            ai_suckTimer++;

            if (ai_suckDistance > maxSuckDistance)
            {
                ai_suckDistance = maxSuckDistance;
            }

            if (ai_suckStrength > maxSuckStrength)
            {
                ai_suckStrength = maxSuckStrength;
            }
        }

        private void AI_Suck()
        {
            if (ai_suckTimer < 60)
                return;


            //SUCKING
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.boss)
                    continue;

                if (npc.active && npc.chaseable && npc.damage > 0 && !npc.friendly)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= ai_suckDistance)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        float suckStrength = ai_suckStrength;
                        if (distance < suckStrength)
                            suckStrength = distance;

                        Vector2 suckVelocity = direction * suckStrength;
                        npc.velocity -= suckVelocity * 0.66f;
                    }
                }
            }

            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                float distance = Vector2.Distance(Projectile.Center, item.Center);
                if (distance <= ai_suckDistance)
                {
                    Vector2 direction = item.Center - Projectile.Center;
                    direction.Normalize();
                    float suckStrength = ai_suckStrength;
                    if (distance < suckStrength)
                        suckStrength = distance;
                    item.velocity -= direction * suckStrength;
                }
            }
        }

        private void AI_SuckVisuals()
        {
            if(ai_suckTimer == 1)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                        Color.White, 1);
                }

                SoundEngine.PlaySound(SoundID.Item100, Projectile.position);
            }

            if (ai_suckTimer == 60)
            {
                Player player = Main.player[Projectile.owner];
                player.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
                //Charged Sound thingy
                for (int i = 0; i < 48; i++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
                    ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                        Color.White, 1);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"));
            }

            //1 Second Build Up
            int particleCount = ai_suckTimer < 60 ? 2 : 4;
            float speedMultiplier = ai_suckDistance / 64;
            float circularEdge = ai_suckDistance / 10;
            //VISUALS
            if (ai_suckTimer % 4 == 0)
            {
                for (int i = 0; i < particleCount; i++)
                {
        
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(circularEdge, circularEdge);
                    Vector2 speed = position.DirectionTo(Projectile.Center) * speedMultiplier;
                    Particle p = ParticleManager.NewParticle(position, speed,
                        ParticleManager.NewInstance<VoidSuckParticle>(), Color.White, scale);
                    p.scale.X *= (speedMultiplier / 1.5f);
                }
            }


            if (IsCharged && ai_suckTimer % 4 == 0)
            {
         
                for (int i = 0; i < particleCount - 2; i++)
                {
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(circularEdge, circularEdge);
                    Vector2 speed = position.DirectionTo(Projectile.Center) * speedMultiplier;
                    ParticleManager.NewParticle(position, speed,
                        ParticleManager.NewInstance<VoidParticle>(), Color.White, scale);
                }

                for (int i = 0; i < particleCount - 2; i++)
                {
                    float scale = Main.rand.NextFloat(0.5f, 1f);
                    Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(circularEdge, circularEdge);
                    Vector2 speed = position.DirectionTo(Projectile.Center) * speedMultiplier;
                    Dust.NewDustPerfect(position, DustID.GemSapphire, speed, Scale: scale).noGravity = true;
                }
            }
        }

        public override void AI()
        {
            AI_IncreaseSuckTimers();
            AI_KeepAlive();
            AI_Suck();
            AI_SuckVisuals();
            //Animate this projectile
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Don't draw until it charges
            if (IsCharged)
                return true;
            return false;
        }
    }
}
