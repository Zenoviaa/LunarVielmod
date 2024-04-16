using Stellamod.Helpers;
using Stellamod.Projectiles.Gun;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;

namespace Stellamod.Projectiles.GunHolster
{
    internal class BubbleBussyStickyProj : ModProjectile
    {
        private bool _setOffset;
        private Vector2 _offset;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
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
                    ModContent.ProjectileType<BubbleBussyProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.position - _offset + new Vector2(0.001f, 0.001f);
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);

        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 4);
                d.noGravity = true;
            }
        }
    }
}