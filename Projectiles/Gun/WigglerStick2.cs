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
    internal class WigglerStick2 : ModProjectile
    {
        private float _lighting;
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
            Projectile.timeLeft = 60 * 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void AI()
        {

            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                ref float detonationTimer = ref Projectile.ai[1];
                detonationTimer--;

                if (detonationTimer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"));
                }

                if (detonationTimer < 0)
                {
                    ShakeModSystem.Shake = 3;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<KaBoomBlue>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
                    Projectile.Kill();
                }

            }

            Projectile.velocity = Vector2.Zero;
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
