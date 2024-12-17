using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class CandleShotBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("DeathShotBomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 25;
            Projectile.height = 80;
            Projectile.width = 80;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            var EntitySource = Projectile.GetSource_FromThis();
            Timer++;
            if (Timer == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Infernis1"), Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 1024, 8);
                int Sound = Main.rand.Next(1, 3);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 1f).noGravity = true;
                }
                for (float f = 0; f < 6; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Orange,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(12, 25),
                        baseSize: Main.rand.NextFloat(0.08f, 0.2f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

            }

            if (Timer == 14 && Main.myPlayer == Projectile.owner)
            {

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<BongoBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if (Timer == 22)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSkullDust>(), Vector2.Zero, 0, Color.Purple, 1f).noGravity = true;
            }
        }
    }
}
