using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class VoidBlasterExsplosion : ModProjectile
    {
        public float Rot;
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 150;
            Projectile.height = 28;
            Projectile.width = 60;
            Projectile.extraUpdates = 1;
        }

        private NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public override void AI()
        {

            Projectile.Center = Owner.Center;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<VoidBlasterSpawnEffect>(), Projectile.damage / 4, 1, Projectile.owner, 0, 0);
            }
            if (Projectile.ai[0] == 50)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DodgerBlue, 1f).noGravity = true;
                }
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DodgerBlue, 1.5f).noGravity = true;
                }
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterHits = 0;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().VoidBlasterNPC = null;
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<VoidBlasterExplosionBomb>(), Projectile.damage * 3, 1, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2524f, 240f);
                Projectile.alpha = 0;
            }
            if (Projectile.ai[0] >= 50)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0, 0.4f);
            }
        }


    }
}