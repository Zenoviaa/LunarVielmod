using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class ROCKSPIKE : ModProjectile,
    IDrawToRenderTarget
{
    private int _frame;
    private Vector2 _scale;
    private float _flash;
    private float InTime => 30;
    private ref float Timer => ref Projectile.ai[0];
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
        _frame = Main.rand.Next(3);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            var rockSpikeAsound = AssetReferences.Assets.Sounds.STARR.RockSummon.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(rockSpikeAsound, Projectile.position);
            CrackVFX(Projectile.Bottom);
            DustVFX(Projectile.Bottom, -Vector2.UnitY * 7);
            for (int i = 0; i < 4; i++)
            {
                FXUtil.MakeSoilParticle(Projectile.Bottom + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1f) * 15);
            }
            for(int i = 0; i < 5; i++)
            {
                MakeRockGore(Projectile.Bottom + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(10f, 15f));
            }
        }

        Projectile.frame = _frame;

        float ratio = Timer / InTime;
        float ease = EasingFunction.OutExpo(ratio);

        Vector2 lerp1 = Vector2.Lerp(Vector2.One * 0.6f, new Vector2(1.2f), EasingFunction.OutExpo(ease));
        Vector2 lerp2 = Vector2.Lerp(new Vector2(1.2f), Vector2.One, EasingFunction.InExpo(ratio));
        Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
        _scale = lerp3;

        _flash = MathHelper.Lerp(1f, 0f, ratio);
    }

    public void MakeRockGore(in Vector2 position, in Vector2 velocity)
    {
        Gore.NewGore(Projectile.GetSource_FromThis(), position, velocity, ModContent.GoreType<IceRockGore>());
    }

    public void DustVFX(Vector2 position, Vector2 velocity)
    {
        for (float f = 0; f < 8; f++)
        {
            Vector2 spawnPosition = position;
            spawnPosition.X += Main.rand.NextFloat(-512, 512);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);
            spawnVelocity += velocity;
            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
    }
    public void CrackVFX(Vector2 position)
    {
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, timeLeft = 200 });
        Particles.CrackDust.Spawn(CrackImpactDust.Data.Default with { position = position, scale = 1.5f, timeLeft = 120 });
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Draw();
        return false;
    }

    private void Draw(Color? overrideColor = null)
    {
        SpritebatchDrawer spikeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        spikeDrawer.worldPosition = Projectile.Bottom;
        spikeDrawer.BottomCenterOrigin();
        spikeDrawer.scale *= _scale;
        Main.spriteBatch.Draw(spikeDrawer);

        spikeDrawer.color = Color.Lerp(Color.White, Color.Transparent, _flash);
        spikeDrawer.color.A = 0;
        Main.spriteBatch.Draw(spikeDrawer);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (Main.netMode == NetmodeID.Server)
            return;
        FXUtil.ShakeCamera(Projectile.position, 128, 4);
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

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        Draw(Color.Red);
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawWhite); 
    }
}
