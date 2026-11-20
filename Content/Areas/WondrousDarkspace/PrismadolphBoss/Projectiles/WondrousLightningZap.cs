using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

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
    int _seed;
    UnifiedRandom Random => field ??= new();
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
    float StrikeTime => IsBig ? 120 : 55;
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
        if (_seed == 0)
            _seed = Main.rand.Next(1, 10000);
    //    Main.NewText(IsBig);
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
        Projectile.Center = Parent.Center;
        _timer++;
        if(Parent.ModNPC is PrismadolphSegment seg)
        {
            seg.ChargeAttackProgress = MathF.Max(seg.ChargeAttackProgress, _timer / TelegraphTime);
        }
        ConstantlyCreateFaintAmbientLightningParticlesBetweenStartAndEnd();
        if (_timer >= TelegraphTime)
        {
            if (IsBig)
                SwitchState(AIState.BigStrike);
            else
                SwitchState(AIState.Strike);
        }
    }

    void CreateBranches()
    {
        Random.SetSeed(_seed);
        _branches = new Branch[Random.Next(4, 7)];
        for(var i = 0; i < _branches.Length; i++)
        {
            _branches[i].points = new Vector2[32];
            Vector2 start;
            Vector2 end;
            if(i == 0)
            {
                start = StrikeStart;
                end = StrikeEndPoint;
            }
            else
            {
                var ratio = Random.NextFloat(0.2f, 0.7F);
                start = _branches[0].points[Random.Next(0, 7)];
                end = start + Projectile.velocity.Resize(Random.NextFloat(100, 512)).RotatedBy(Random.NextFloat(-0.5F, 0.5F));
            }

            MoonUtils.CreateLightningBoltBranch(ref _branches[i].points, start, end, _branches[i].points.Length, 1.5F, Random);
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
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain1.Asset;
                break;
            case 1:
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain2.Asset;
                break;
            case 2:
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain3.Asset;
                break;
        }
        sound.PitchVariance = 1F;
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
                color = NextRandomDustColor() with { A = 0 },
                fadeToColor = Color.DarkBlue
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
        FXUtil.ShakeCamera(Projectile.Center, 1024, 16);

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
                color = NextRandomDustColor() with { A = 0},
                fadeToColor = Color.DarkBlue
            });
        }

        //I love this little dust
        for (var i = 0; i < 16; i++)
        {
            var ratio = Main.rand.NextFloat();
            var pos = Vector2.Lerp(StrikeStart, StrikeEndPoint, ratio);
            var vel = Projectile.velocity.RotatedByRandom(0.5f).Resize(Main.rand.NextFloat(6F, 25F));
            PS.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = NextRandomDustColor() with { A = 0 },
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.6f, 1f) * 0.2f,
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
        if(State == AIState.BigStrike)
            Projectile.Center = Parent.Center;
        if (IsBig)
        {
            if(_timer == 1)
            {
                CreateLightningDustParticlesBetweenStartAndEnd();

                var sound = AssetReferences.Assets.Sounds.Lighting2.Asset;
                sound.PitchVariance = 0.7F;
                SoundEngine.PlaySound(sound, Projectile.position);
            }
            if (LunarVeilClientConfig.Instance.DramaticEffects)
            {
                SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                effectsPlayer.darknessCurve = MathHelper.Lerp(0.25f, 0f, EasingFunction.InOutExpo(_timer / StrikeTime));
            }
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
        var w = 24 * MathHelper.SmoothStep(1f, 0.4f, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress) * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        if (IsBig)
            w *= 2.5F;
        return w;
    }
    float GetTrailWidth2(float ratio)
    {
        var w = 24 * MathHelper.SmoothStep(1f, 0.4f, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress) * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        w *= 7;
        return w;
    }
    float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth2(ratio) * 1.2f;
    }

    Color GetTrailColor(float ratio)
    {

        Color glowColor = Color.DarkBlue;
       // glowColor.ScrollHue(360 * ratio);

        //I wanna make it flicker violenty for a few frames and go back to it's like bloom color
        glowColor = Color.Lerp(glowColor, Color.White, ExtraMath.Osc(0f, 1f, speed: 40) * MathHelper.SmoothStep(1f, 0f, StrikeProgress));
        return glowColor;
    }
    Color GetTrailColorTelegraph(float ratio)
    {

        Color glowColor = Color.DarkBlue;
        // glowColor.ScrollHue(360 * ratio);

        //I wanna make it flicker violenty for a few frames and go back to it's like bloom color
        glowColor = Color.Lerp(glowColor, Color.White, ExtraMath.Osc(0f, 1f, speed: 40) * MathHelper.SmoothStep(1f, 0f, StrikeProgress));
        glowColor *= 0.25F;
        return glowColor;
    }
    void DrawLightningBolt(GraphicsDevice gDevice)
    {
        CreateBranches();
        //Draw all branches
        List<VertexPositionColorTexture> batch = new();
        for(var i = 0; i < _branches.Length; i++)
        {
            ref var branch = ref _branches[i];

            if(State == AIState.Telegraph || State == AIState.BigTelegraph)
            {
                batch.AddRange(DrawUtilities.PrepareSimpleTrailingNoSmoothing(branch.points, GetTrailColorTelegraph, GetTrailWidth));
            }
            else
            {
                batch.AddRange(DrawUtilities.PrepareSimpleTrailingNoSmoothing(branch.points, GetTrailColor, GetTrailWidth));
            }
        
        }
        //Main.NewText(_branches.Length);
        _indices ??= DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);  
        var pass = AssetReferences.Effects.Darkspace.WondrousLightningStrike.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.distortionStrength = 0.05f;
        pass.Parameters.spriteTexture = AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value;
        pass.Parameters.noiseTexture = AssetReferences.Assets.Noise.PainterlyNoise.Asset.Value;
        pass.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), _indices, pass.Shader);
    
        if(State == AIState.BigStrike)
        {
            var p = Color.Pink;
            p.ScrollHue(Main.GlobalTimeWrappedHourly * 54);
            var shader = FixedRichLaserShader.Instance;
            shader.LaserColor = Color.IndianRed;
            shader.InnerColor = p;
            shader.OuterColor = Color.Violet;
            var poss = DrawUtilities.InterpolateBetweenPoints(StrikeStart, StrikeEndPoint, 64);
            TrailDrawer.Draw(Main.spriteBatch, poss, GetTrailColor, GetTrailWidth2, shader);

            BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
            bloomTrailShader.InnerColor = Color.SkyBlue;
            bloomTrailShader.OuterColor =p;
            TrailDrawer.Draw(Main.spriteBatch, poss, GetTrailColor, GetTrailWidth3, bloomTrailShader);
        }
      //  TrailDrawer.Draw(, GetTrailColor, GetTrailWidth, pass.Shader);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawLightningBolt, DrawLayer.OverPlayers);
    }
}
