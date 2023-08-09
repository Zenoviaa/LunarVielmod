
using Stellamod.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class KilvierBONK : ModProjectile
    {
        public bool OptionallySomeCondition { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acidius");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FrostDaggerfish);
            AIType = ProjectileID.FrostDaggerfish;
            Projectile.penetrate = 3;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.YellowGreen.ToVector3() * 1.75f * Main.essScale);

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(2) == 0)
                target.AddBuff(ModContent.BuffType<AcidFlame>(), 180);
        }
        public override bool PreAI()
        {
            if (Main.rand.Next(3) == 1)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 0);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74);
            }
            return true;
        }
   
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 0);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 74);
            }
        }

    }
}