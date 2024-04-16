using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class WigglerStick : ModProjectile
    {
        private float _lighting;
        private bool _setOffset;
        private Vector2 _offset;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void AI()
        {
            int targetNpc = (int)Projectile.ai[0];
            NPC target = Main.npc[targetNpc];
            if (target.active && !_setOffset)
            {
                _offset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f); 
                _setOffset = true;
            } 
            else if (!target.active)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<WigglerShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            } 
            else
            {
                Vector2 targetPos = target.position - _offset + new Vector2(0.001f, 0.001f); 
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                ref float detonationTimer = ref Projectile.ai[1];
                detonationTimer--;
                if(detonationTimer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
                }

                if(detonationTimer < 0)
                {
                    ShakeModSystem.Shake = 3;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<KaBoomBlue>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
                    Projectile.Kill();
                }

            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(60))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                _lighting += 0.01f;
                Lighting.AddLight(Main.screenPosition - Projectile.position, Color.White.ToVector3() * _lighting * Main.essScale); ;
            
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, speed * 4);
                d.noGravity = true;
            }
        }
    }
}
