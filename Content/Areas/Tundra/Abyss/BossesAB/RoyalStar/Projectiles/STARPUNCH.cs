using Stellamod.Common.Particles;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARPUNCH : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 5;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 36;
    }
    public override void AI()
    {
        base.AI();
        Timer++;

        if(Timer == 1)
        {
            for(float f = 0; f < 3; f++)
            {
                Vector2 spawnPosition = Projectile.Center;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                spawnScale *= 3;
                var fx = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
                fx.Scale *= 2;
            }

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 8;
            for(float f = 0; f < 32; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    velocity = Main.rand.NextVector2Circular(32, 32),
                    innerColor = Color.LightGoldenrodYellow.ToVector4(),
                    outerColor = Color.DarkGoldenrod.ToVector4(),
                });
            }

            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.DarkGoldenrod, 15, 0.16f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (float f = 0; f < 12; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5, 5);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), velocity, 0, Color.Yellow, Main.rand.NextFloat(2f, 5f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Gold,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.02f, 0.16f),
                    duration: Main.rand.NextFloat(12, 24));
                particle.Scale *= 3;
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
        Projectile.frame = (int)(Timer / 3f);
        if (Projectile.frame >= Main.projFrames[Type])
            Projectile.Kill();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Gold;
        drawer.scale *= 1.1f;
        Main.spriteBatch.Draw(drawer);

        drawer.scale = Vector2.One;
        drawer.color = Color.White;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
