using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss.Projectiles;

public class WondrousLightningZap : ModProjectile,
    IDrawToRenderTarget
{
    public struct Branch
    {
        public Vector2[] points;
    }
    short[] _indices;
    Branch[] _branches;
    enum AIState : byte
    {
        Telegraph,
        Strike,
        BigTelegraph,
        BigStrike
    }

    float _timer;
    NPC Parent => Main.npc[(int)Projectile.ai[0]];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float TelegraphTime => ref Projectile.ai[2];

    bool IsBig => State == AIState.BigTelegraph || State == AIState.BigStrike;
    float StrikeTime => 60;
    float StrikeProgress => (_timer / StrikeTime);
    Vector2 StrikeStart => Projectile.Center;
    Vector2 StrikeEndPoint => Projectile.Center + Projectile.velocity;
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

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return CollisionHelper.LaserCollision(projHitbox, targetHitbox, Projectile.Center, StrikeEndPoint, 24);
        //return base.Colliding(projHitbox, targetHitbox);
    }
    public override bool CanHitPlayer(Player target)
    {
        if (State == AIState.Telegraph)
            return false;
        return base.CanHitPlayer(target);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4800;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 360;
        Projectile.light = 0.6F;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.BigTelegraph:
            case AIState.Telegraph:
                AI_Telegraph();
                break;
            case AIState.BigStrike:
            case AIState.Strike:
                AI_Strike();
                break;
        
        }
        Projectile.Center = Parent.Center;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            State = state;
            _timer = 0;
            Projectile.netUpdate = true;
        }
    }


    void AI_Telegraph()
    {
        _timer++;
        ConstantlyCreateFaintAmbientLightningParticlesBetweenStartAndEnd();
        if (_timer >= TelegraphTime)
        {
            SwitchState(AIState.Strike);
        }
    }


    bool CanRender() => _branches != null;
    void CreateBranches()
    {
        _branches = new Branch[Main.rand.Next(1, 3)];
        for(var i = 0; i < _branches.Length; i++)
        {
            _branches[i].points = new Vector2[16];
            Vector2 start;
            Vector2 end;
            if(i == 0)
            {
                start = StrikeStart;
                end = StrikeEndPoint;
            }
            else
            {
                var ratio = Main.rand.NextFloat(0f, 0.7F);
                start = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
                end = start + Projectile.velocity.Resize(Main.rand.NextFloat(100, 512));
            }

            MoonUtils.CreateLightningBoltBranch(ref _branches[i].points, start, end, _branches[i].points.Length, 0.5F);
        }
    }
    
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.AddBuff(BuffID.Electrified, 120);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Electrified, 120);
    }

    Color NextRandomDustColor()
    {
        Color dustColor = Color.DeepSkyBlue;
        dustColor.ScrollHue(Main.rand.NextFloat(-24f, 24f));
        return dustColor;
    }

    SoundStyle NextRandomZapSound()
    {
        SoundStyle sound;
        switch (Main.rand.Next(3))
        {
            default:
                sound = AssetReferences.Assets.Sounds.LeviathanEel.LeviZap2.Asset;
                break;
            case 1:
                sound = AssetReferences.Assets.Sounds.LeviathanEel.LeviZap3.Asset;
                break;
            case 2:
                sound = AssetReferences.Assets.Sounds.LeviathanEel.LeviZap2.Asset;
                break;
        }
        sound.PitchVariance = 0.7F;
        return sound;
    }

    void ConstantlyCreateFaintAmbientLightningParticlesBetweenStartAndEnd()
    {
        if (Main.rand.NextBool(2))
        {
            var ratio = Main.rand.NextFloat();
            var pos = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
            var vel = Projectile.velocity.RotatedByRandom(0.5f).Resize(Main.rand.NextFloat(3, 12));
            PS.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = Main.rand.Next(25, 50),
                scale = Main.rand.NextFloat(0.25f, 0.5f),
                color = NextRandomDustColor()
            });
        }

        if (Main.rand.NextBool(2))
        {
            var ratio = Main.rand.NextFloat();
            var pos = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
            Dust.NewDustPerfect(pos, DustID.Electric, Projectile.velocity.Resize(2).RotatedByRandom(0.5F), Scale: 0.2f);
        }
    }

    void CreateLightningDustParticlesBetweenStartAndEnd()
    {
        //Initial big flash
        FXUtil.ShakeCamera(Projectile.Center, 1024, 8);

        var glowColor = NextRandomDustColor();
        var dimColor = Color.Lerp(glowColor, Color.Black, 0.4f);

        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, glowColor, dimColor);
        fx.Scale *= 1.2f;
        fx.VectorScale.X *= 1.5F;
        fx.Rotation = Projectile.velocity.ToRotation();
        fx.noRot = true;

        var endFx = FXUtil.GlowCircleBoom(StrikeEndPoint, Color.White, glowColor, dimColor);
        endFx.Scale *= 2f;

        //swirling bit dust
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(
            Projectile.Center,
            Projectile.Center + Projectile.velocity,
            TriColorPalette.Moonly,
            new Vector2(5, 25));
        fac.particleCount *= 2;
        ParticleUtils.CreateSwirlingDustBurst(fac);

        //Lightning/electric particles
        for (var i = 0; i < 24; i++)
        {
            var ratio = Main.rand.NextFloat();
            var pos = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
            var vel = Projectile.velocity.RotatedByRandom(0.5f).Resize(Main.rand.NextFloat(6F, 25F));
            PS.LightningSpikeySpark.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = Main.rand.Next(25, 50),
                scale = Main.rand.NextFloat(0.5f, 1f),
                color = NextRandomDustColor()
            });
        }

        //I love this little dust
        for (var i = 0; i < 12; i++)
        {
            var ratio = Main.rand.NextFloat();
            var pos = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
            var vel = Projectile.velocity.RotatedByRandom(0.5f).Resize(Main.rand.NextFloat(6F, 25F));
            PS.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = NextRandomDustColor(),
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.6f, 1f),
            });
        }
    }

    void AI_Strike()
    {
        _timer++;
        if (_timer == 1)
        {
            var zapSound = NextRandomZapSound();
            SoundEngine.PlaySound(zapSound, Projectile.position);
            CreateLightningDustParticlesBetweenStartAndEnd();
        }

        if (_timer >= StrikeTime)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(36, 0, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress);
    }

    Color GetTrailColor(float ratio)
    {
        Color glowColor = Color.Red;
        glowColor.ScrollHue(360 * ratio);

        //I wanna make it flicker violenty for a few frames and go back to it's like bloom color
        glowColor = Color.Lerp(glowColor, Color.White, ExtraMath.Osc(0f, 1f, speed: 40) * MathHelper.SmoothStep(1f, 0f, StrikeProgress));
        return glowColor;
    }

    void DrawLightningBolt(GraphicsDevice gDevice)
    {
        if (!CanRender())
        {
            CreateBranches();
        }

        //Draw all branches
        List<VertexPositionColorTexture> batch = new();
        for(var i = 0; i < _branches.Length; i++)
        {
            ref var branch = ref _branches[i];
            batch.AddRange(DrawUtilities.PrepareSimpleTrailingNoSmoothing(branch.points, GetTrailColor, GetTrailWidth));
        }

        _indices ??= DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);  
        var pass = AssetReferences.Effects.Darkspace.WondrousLightningStrike.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.distortionStrength = 0.05f;
        pass.Parameters.spriteTexture = AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value;
        pass.Parameters.noiseTexture = AssetReferences.Assets.Noise.PainterlyNoise.Asset.Value;
        pass.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), _indices, pass.Shader);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawLightningBolt, DrawLayer.OverNPCs);
    }
}
