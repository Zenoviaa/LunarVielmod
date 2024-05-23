using Microsoft.Xna.Framework;
using Stellamod.Dusts;
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
        private ref float Timer => ref Projectile.ai[0];
        private ref float Speed => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }

        public Vector3 HuntrianColorXyz;
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Teal.ToVector3() * 1.75f * Main.essScale);

        }

        public override void AI()
        {
            //Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1; 
            Timer++;
            if(Timer <= 2 && Main.myPlayer == Projectile.owner)
            {
                Speed = Main.rand.NextFloat(0.92f, 0.94f);
                Projectile.netUpdate = true;
            }
            Projectile.velocity *= Speed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.velocity.Length() <= 0.1f && Projectile.active)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Teal, 1f).noGravity = true;
            }

            float damage = Projectile.damage;
            damage *= 0.5f;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<KaBoomMagic2>(), (int)damage, 3, Projectile.owner);
            p.friendly = true;
            p.usesLocalNPCImmunity = true;
            p.localNPCHitCooldown = -1;

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity2"), Projectile.position);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 2f);
        }
    }
}
