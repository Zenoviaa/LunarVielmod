using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Pikmin
{
    internal class PurplePikminAttack : ModProjectile
    {
        private float _lighting;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 19;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;

        }
        int Attacktime = 0;
        private bool _setOffset;
        private Vector2 _offset;
        public override void AI()
        {


            int targetNpc = (int)Projectile.ai[0];
            NPC target = Main.npc[targetNpc];
            if (target.active && !_setOffset)
            {
                _offset = (target.position - Projectile.position);
                _setOffset = true;
            }
            else if (!target.active)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<PurplePikminThrow>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.position - _offset;
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }


            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
            Attacktime++;

            if (Attacktime >= 40)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<AivanKaboom>(), Projectile.damage * 3, 0, Projectile.owner);
                switch (Main.rand.Next(5))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminhit1") with { Volume = 0.4f });
                        break;

                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminhit2") with { Volume = 0.4f });
                        break;

                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminhit3") with { Volume = 0.4f });
                        break;

                    case 3:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminhit4") with { Volume = 0.4f });
                        break;

                    case 4:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminhit5") with { Volume = 0.4f });
                        break;

                }
                Attacktime = 0;

            }
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 3);
            if (Main.rand.NextBool(60))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
            }
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
