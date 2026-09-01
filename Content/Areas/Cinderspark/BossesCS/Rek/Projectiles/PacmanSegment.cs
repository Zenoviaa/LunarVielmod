using Stellamod.Assets;
using Stellamod.Common.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class PacmanSegment : ModProjectile
{
    private float _timer;
    private Vector2 _initialPosition;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private int SegmentIndex => (int)Projectile.ai[1];
    private ref float KillMe => ref Projectile.ai[2];
    private float EaseInTime => 120;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_timer);
        writer.WriteVector2(_initialPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _timer = reader.ReadSingle();
        _initialPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 700;
        Projectile.light = 0.78f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        _timer++;
        if (_timer == 1)
        {
            _initialPosition = Projectile.Center;
        }
        if (Parent.ModNPC is RekBoss rek)
        {
            var segment = rek.Segments[SegmentIndex];
            segment.position = Projectile.Center;
            segment.rotation += 0.05f;
        }

        Vector2 nextPos = _initialPosition + Projectile.velocity;
        if (Main.rand.NextBool(2))
        {
            Vector2 vel = Main.rand.NextVector2Circular(4, 4);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel = vel.RotatedByRandom(MathHelper.ToRadians(6));
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64, 64), DustID.Torch, vel, Scale: 2f);
        }

        float ratio = _timer / EaseInTime;
        float ease = EasingFunction.InOutExpo(ratio);
        Vector2 interpolatedPos = Vector2.Lerp(_initialPosition, nextPos, ease);
        Projectile.Center = interpolatedPos;
        if (KillMe > 0)
        {
            Projectile.Kill();
        }
    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
        float glowAlpha = EasingFunction.InOutSine(_timer / 60f);
        var glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, Projectile.Center);
        glowDrawer.scale *= 0.48f;
        glowDrawer.color = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 12)) * ExtraMath.Osc(0.5f, 0.75f, speed: 8) * glowAlpha;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (Parent.ModNPC is RekBoss rek)
        {
            var segment = rek.Segments[SegmentIndex];
            segment.noWorm = false;
        }

        for (float f = 0; f < 6; f++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(18, 18),
                timeLeft = 100,
                innerColor = color.ToVector4(),
                outerColor = Color.Red.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(1f, 2f))
            });
        }


        //      FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red, duration: 12, baseSize: 0.24f);
        SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, Projectile.position);
    }
}
