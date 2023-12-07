using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class ClockworkBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }

        public Vector3 HuntrianColorXyz;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, new Color(83, 254, 164), lightColor, 2);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(83, 254, 164), Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            //Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1; 
            Projectile.velocity *= 0.92f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.velocity.Length() <= 0.1f && Projectile.active)
            {
                Projectile.Kill();
            }

            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.Center, 4, 4, DustID.GemEmerald);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald, speed, Scale: 1f);
                d.noGravity = true;
            }

            float damage = Projectile.damage;
            damage *= 0.5f;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<KaBoomMagic2>(), (int)damage, 3);
            p.friendly = true;
            p.usesLocalNPCImmunity = true;
            p.localNPCHitCooldown = -1;

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DeathShotBomb2"), Projectile.position);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 2f);
        }
    }
}
