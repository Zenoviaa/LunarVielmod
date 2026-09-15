using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
public class STARROCKCRASH : ModProjectile
{
    private int _frame;


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_frame);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _frame = reader.ReadInt32();
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _frame = Main.rand.Next(4);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
       
    }

    public override void AI()
    {
        base.AI();
        Projectile.frame = _frame;
        Projectile.velocity.Y += 0.5f;
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        OutlineRenderer.Queue(DrawWhites);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    private void DrawWhites(SpriteBatch spriteSB)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Red;
        Main.spriteBatch.Draw(drawer);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    private void PlayBreakSound()
    {
        var soundStyle = AssetReferences.Assets.Sounds.STARR.RockSmash.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(soundStyle, Projectile.position);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        PlayBreakSound();
        for (int i = 0; i < 4; i++)
        {
            Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
            Gore.NewGore(Projectile.GetSource_FromThis(), position, velocity, ModContent.GoreType<IceRockGore>());
        }

        for (int f = 0; f < 2; f++)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        for (int i = 0; i < 16; i++)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(spawnPosition, DustID.Stone, spawnVelocity);
        }
    }
}
