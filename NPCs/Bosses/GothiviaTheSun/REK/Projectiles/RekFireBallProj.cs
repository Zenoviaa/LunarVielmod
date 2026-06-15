
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    public class RekFireBallProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
        }


        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballShoot"), Projectile.position);
            }

            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 0.2f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballDeath"), Projectile.position);
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 0.5f);
            }
            for (int i = 0; i < 2; i++)

            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(),
                    (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.5f).noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 12 * 40);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.DarkCyan, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }
}
