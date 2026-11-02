using Stellamod.Common;
using Stellamod.Common.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviPingPongArena : ModProjectile
{
    enum AIState
    {
        ComeIn,
        Idle,
        GoOut
    }

    float _timer;
    Vector2 Origin
    {
        get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
    }
    float Left => Origin.X - NiiviPingPongBall.ArenaSize.X * 0.5F;
    float Right => Origin.X + NiiviPingPongBall.ArenaSize.X * 0.5F;
    float Top => Origin.Y - NiiviPingPongBall.ArenaSize.Y * 0.5F;
    float Bottom => Origin.Y + NiiviPingPongBall.ArenaSize.Y * 0.5F;
    float ComeInTime => 60;
    AIState State
    {
        get => (AIState)Projectile.ai[2];
        set => Projectile.ai[2] = (float)value;
    }

    public override string Texture => ModType.NoTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_timer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _timer = reader.ReadSingle();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.timeLeft = 24000;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.light = 0.5F;
        Projectile.penetrate = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    bool AnyBalls()
    {
        var type = ModContent.ProjectileType<NiiviPingPongBall>();
       foreach(var proj in Main.ActiveProjectiles)
        {
            if (proj.type == type)
                return true;
        }
        return false;
    }

    void SwitchState(AIState state)
    {
        _timer = 0;
        State = state;
        Projectile.netUpdate = true;
    }

    public override void AI()
    {
        base.AI();
        _timer++;
        Projectile.Center = Origin;
        switch (State)
        {
            case AIState.ComeIn:
                if(_timer >= ComeInTime)
                {
                    SwitchState(AIState.Idle);
                }
                break;
            case AIState.Idle:
                if (!AnyBalls())
                {
                    SwitchState(AIState.GoOut);
                }
                break;
            case AIState.GoOut:
                {
                    if(_timer >= ComeInTime)
                    {
                        Projectile.Kill();
                    }
                }
                break;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }
}

public class NiiviPingPongBall : ModProjectile
{
    enum AIState
    {
        Fly,
        BigBounce
    }

    float _bounceTimer;
    public static Vector2 ArenaSize => new Vector2(1280, 512);
    float Left => Origin.X - ArenaSize.X * 0.5F;
    float Right => Origin.X + ArenaSize.X * 0.5F;
    float Top => Origin.Y - ArenaSize.Y * 0.5F;
    float Bottom => Origin.Y + ArenaSize.Y * 0.5F;
    float SquishTime => 42;
    float _bounceCount;
    AIState _state;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.NiiviPingPongBall.KEY;
    Vector2 Origin
    {
        get
        {
            return new Vector2(Projectile.ai[0], Projectile.ai[1]);
        }
    }
    Vector2 OrbScale => Vector2.Lerp(Vector2.One, new Vector2(1.1f), EasingFunction.OutSine(_bounceTimer / SquishTime));
    ref float Timer => ref Projectile.ai[2];
    float PingPongBounces => 12;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_bounceCount);
        writer.Write((float)_state);
        writer.Write(_bounceTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _bounceCount = reader.ReadSingle();
        _state = (AIState)reader.ReadSingle();
        _bounceTimer = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(24);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 64;
        Projectile.timeLeft = 1200;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 1f;
        Projectile.penetrate = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        if (_bounceTimer > SquishTime * 0.5f)
            return false;
        return true;
    }
    public override void AI()
    {
        base.AI();

        switch (_state)
        {
            case AIState.Fly:
                AI_Fly();
                break;
            case AIState.BigBounce:
                AI_BigBounce();
                break;
        }
    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        _state = state;
        Projectile.netUpdate = true;
    }

    void BounceEffect()
    {
        _bounceTimer = SquishTime;
        var impactBounceSound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain.Asset with { PitchVariance = 0.7F };
        SoundEngine.PlaySound(impactBounceSound, Projectile.position);

        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(4, 12));
        fac.particleCount *= 3;
        ParticleUtils.CreateSwirlingDustCircle(fac);

        FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
    }

    void AI_Fly()
    {
        Timer++;
        if (Projectile.Center.X < Left)
        {
            BounceEffect();
            Projectile.velocity.X = MathF.Abs(Projectile.velocity.X);
        }
        else if (Projectile.Center.Y < Top)
        {
            BounceEffect();
            Projectile.velocity.Y = MathF.Abs(Projectile.velocity.Y);
        }
        else if (Projectile.Center.Y > Bottom)
        {
            BounceEffect();
            Projectile.velocity.Y = -MathF.Abs(Projectile.velocity.Y);
        }
        else if (Projectile.Center.X > Right)
        {
            SwitchState(AIState.BigBounce);
        }
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.04F;
    }

    void AI_BigBounce()
    {
        Timer++;
        if (Timer == 1)
        {
            BounceEffect();
            _bounceCount++;
            if (this.OwnedByLocalClient())
            {
                Projectile.velocity.X = -6 * MathHelper.Lerp(1f, 1.5f, _bounceCount / PingPongBounces);
                Projectile.velocity.Y = Main.rand.NextBool(2) ? -4 : 4;
                Projectile.netUpdate = true;
            }
        }

        if (_bounceCount >= PingPongBounces)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //Draw ball with shader
        var pass = AssetReferences.Effects.Niivi.PrismLightningOrb.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.bounceTime = _bounceTimer / SquishTime;
        pass.Apply();
        var sb = Main.spriteBatch;
        using (sb.Ctx(sb.Parameters with { effect = pass.Shader }))
        {
            var drawer = Projectile.Drawer;
            drawer.scale *= OrbScale;
            sb.Draw(drawer);
        }
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Icey, new Vector2(4, 12));
        fac.particleCount *= 8;
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }
}
