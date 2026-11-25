using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;

public class ElectroWizardZap : ModProjectile,
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
    }

    float _timer;
    NPC Parent => Main.npc[(int)Projectile.ai[0]];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float TelegraphTime => ref Projectile.ai[2];
    float StrikeTime => 55;
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
        if (_branches == null)
            return false;
        bool isHitting = false;
        for (var i = 0; i < _branches.Length; i++)
        {
            ref var p = ref _branches[i];

            if (ProjectileHelper.OldPosColliding(p.points, projHitbox, targetHitbox, 8).Value)
            {
                return true;
            }
        }

        return isHitting;
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
        Projectile.hostile = false;
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
            case AIState.Telegraph:
                AI_Telegraph();
                break;
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

        if(_timer < TelegraphTime * 0.5F)
        {
            var closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 2048);
            if (closestPlayer != null)
            {
                var targetVelocity = (closestPlayer.Center - Projectile.Center);
                targetVelocity *= 1.5F;
                Projectile.velocity = targetVelocity;
            }
        }
        ConstantlyCreateFaintAmbientLightningParticlesBetweenStartAndEnd();
        if (_timer >= TelegraphTime)
        {
            SwitchState(AIState.Strike);
        }
    }

    void CreateBranches()
    {
        Random.SetSeed(_seed);
        _branches = new Branch[Random.Next(4, 7)];
        for (var i = 0; i < _branches.Length; i++)
        {
            _branches[i].points = new Vector2[32];
            Vector2 start;
            Vector2 end;
            if (i == 0)
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
                color = NextRandomDustColor() with { A = 0 },
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
            SoundEngine.PlaySound(zapSound);
            CreateLightningDustParticlesBetweenStartAndEnd();
            Parent.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 7;
        }

        if (_timer <= 30)
            Projectile.hostile = true;
        else
            Projectile.hostile = false;

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
        var w = 36 * MathHelper.SmoothStep(1f, 0.4f, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress) * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        return w;
    }
    float GetTrailWidth2(float ratio)
    {
        var w = 36 * MathHelper.SmoothStep(1f, 0.4f, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress) * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        w *= 10;
        return w;
    }
    float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth2(ratio) * 1.2f;
    }
    float GetTrailWidth4(float ratio)
    {
        var w = 36 * MathHelper.SmoothStep(1f, 0.4f, ratio) * MathHelper.SmoothStep(1f, 0f, StrikeProgress) * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        w *= 7;
        return w;
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
        for (var i = 0; i < _branches.Length; i++)
        {
            ref var branch = ref _branches[i];

            if (State == AIState.Telegraph)
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
    }

    void DrawLightningBoltEndPoint(SpriteBatch sb, Vector2 sp)
    {
        if (_branches == null)
            return;
        if (State == AIState.Telegraph)
            return;

        for (var i = 0; i < _branches.Length; i++)
        {
            ref var branch = ref _branches[i];
            var pos = branch.points[branch.points.Length - 1];
            var glowDrawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
            glowDrawer.worldPosition = pos;
            glowDrawer.scale *= MathHelper.Lerp(0.15F, 0f, StrikeProgress);
            glowDrawer.color = Color.Lerp(Color.BlueViolet, Color.Transparent, StrikeProgress);
            glowDrawer.color.A = 0;
            sb.Draw(glowDrawer);

            glowDrawer.scale *= 0.5F;
            glowDrawer.color = Color.Lerp(Color.White, Color.Transparent, StrikeProgress);
            glowDrawer.color.A = 0;
            sb.Draw(glowDrawer);
        }
    }

    void DrawPixelatedFlash(SpriteBatch sb, Vector2 sp)
    {
        DrawLightningBoltEndPoint(sb, sp);
    }

    void DrawPixelated(GraphicsDevice gd)
    {
        DrawLightningBolt(gd);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlash);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
        //  DrawLayerHooks.OverWaterDrawActions2.Enqueue(DrawPixelated);
    }
}

public class ElectroWizard : ModNPC,
    IDrawToRenderTarget

{
    enum AIState : byte
    {
        Idle,
        Hop,
        HopHigh,
        ReadyHopTowardsTarget,
        HopTowardsTarget,
        Fall,
        Shoot
    }
    Vector2 _targetHopPosition;
    float _targetAfAlpha;
    float _afAlpha;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float HoppingSpeedTimer => ref NPC.ai[2];
    ref float HoppingSound => ref NPC.ai[3];
    Vector2 _squishScale;
    Outliner _outliner;
    const string ANIM_IDLE = "Idle";
    const string ANIM_Happy = "Happy";
    const string ANIM_ATTACK = "Attack";
    const string ANIM_ATTACKOUT = "AttackOut";
    float HopReadyTime => 140;
    float IdleTime => 100;
    float SqrAggroDistance => 16 * 16 * 16 * 16;

    int Damage => 25;
    public override string Texture => TextureRegistry.EmptyTexture;
    float SqrDistanceToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_targetHopPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetHopPosition = reader.ReadVector2();

    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }


    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToDarkspace();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 32;
        NPC.width = 34;
        NPC.height = 72;
        NPC.lifeMax = 135;
        NPC.defense = 10;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = false;
    }
    public override void AI()
    {
        base.AI();
        _targetAfAlpha = 0;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Hop:
            case AIState.HopHigh:
                AI_Hop();
                break;
            case AIState.ReadyHopTowardsTarget:
                AI_ReadyHopTowardsTarget();
                break;
            case AIState.HopTowardsTarget:
                AI_HopTowardsTarget();
                break;
            case AIState.Fall:
                AI_Fall();
                break;
        }
        _outliner.Update();
        _afAlpha = MathHelper.Lerp(_afAlpha, _targetAfAlpha, 0.1f);
        NPC.DrawOrigin = new Vector2(60, 98);
        NPC.VectorScale = _squishScale;
        Lighting.AddLight(NPC.Center, new Vector3(0.8F));
    }
    void AI_Idle()
    {
        Timer++;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        NPC.StayGroundedAndRooted();
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        _squishScale = Vector2.Lerp(new Vector2(1.05f, 0.95f), new Vector2(0.95f, 1.05f), ExtraMath.Osc(0f, 1f, speed: 3));
        if (Timer >= IdleTime)
        {
            ChooseAction();
        }
    }

    void AI_ReadyHopTowardsTarget()
    {
        var ratio = Timer / HopReadyTime;
        Timer++;
        if (MoonUtils.Rand.NextBool((int)MathHelper.Lerp(10, 2, ratio)))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
            var vel = (NPC.Center - pos) * 0.04F;
            Dust.NewDustPerfect(pos, DustID.Electric, vel, Scale: 0.7F);
        }


        HoppingSpeedTimer += ratio;
        HoppingSound += ratio;
        if(HoppingSound >= 6F)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            bellHit.Pitch = ratio * 0.5F;
            SoundEngine.PlaySound(bellHit, NPC.position);

            var fx = FXUtil.GlowCircleBoom(NPC.Bottom, Color.White * 0.4F, Color.SkyBlue * 0.4F, Color.DarkBlue * 0.4F, 25, 0.16F);
            fx.VectorScale.X *= 1.5F;
            fx.VectorScale.Y *= 0.5F;
            fx.VectorScale *= 0.5F;

            HoppingSound = 0;
        }
        NPC.PlayAnimationLooped(ANIM_Happy);
        NPC.StayGroundedAndRooted();
        NPC.SpriteFaceTarget();
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        _squishScale = Vector2.Lerp(new Vector2(1.1f, 0.9f), new Vector2(0.9f, 1.1f), MathF.Sin(HoppingSpeedTimer) * 0.5f + 0.5f);
        if (Timer >= HopReadyTime)
        {
            SwitchState(AIState.HopTowardsTarget);
        }
    }
    void AI_HopTowardsTarget()
    {
      
        Timer++;
        if (Timer == 1)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            bellHit.Volume = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);

            var so = AssetReferences.Assets.Sounds.Dreadmire_LightingRain.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(so, NPC.position);

            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
            p2.fadeToColor = Color.Black;
            p2.innerColor = Color.White;
            p2.outerColor = Color.DarkBlue;
            p2.Scale *= 0.5f;

            NPC.velocity.Y = -14;
            NPC.velocity.X = NPC.XDirectionToTarget * 6;
        }

        _targetAfAlpha = 1f;
        if (MoonUtils.Rand.NextBool(4))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
            PS.HypnoticSparkle.Spawn(new()
            {
                position = pos,
                timeLeft = 60,
                scale = 1,
                rot = 0,
            });
        }

        _squishScale.Y = MathHelper.Lerp(1f, 1.2f, MathF.Abs(NPC.velocity.Y / 15F));
        _squishScale.X = MathHelper.Lerp(1f, 0.9f, MathF.Abs(NPC.velocity.Y / 15F));


        if (Timer == 30)
        {
            if (MultiplayerHelper.IsHost)
            {
                //shoot lightning
                var target = NPC.PlayerImTargeting;
                var vel = (target.Center - NPC.Center);
                vel *= 2f;

                var firer = ProjFirer.From<ElectroWizardZap>(NPC);
                firer.damage = Damage;
                firer.velocity = vel;
                firer.ai0 = NPC.whoAmI;
                firer.ai2 = 60;
                firer.New();
            }
        }

        if (Timer < 30)
        {
            _outliner.warning = true;
            if (MoonUtils.Rand.NextBool(8))
            {
                var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
                var vel = (NPC.Center - pos) * 0.04F;
                Dust.NewDustPerfect(pos, DustID.Electric, vel, Scale: 0.7F);
            }
        }

        if(Timer < 100)
        {
            NPC.PlayAnimationOneShot(ANIM_ATTACK);
        }
        else
        {
            NPC.PlayAnimationOneShot(ANIM_ATTACKOUT);
        }

        if (Timer < 30 || Timer > 60)
        {
        
            if (NPC.velocity.Y < 15)
                NPC.velocity.Y += 0.1f;
        }


        if (Timer > 30 && Timer < 100)
        {
                _outliner.attacking = true;
                NPC.velocity *= 0.95F;
            NPC.velocity.Y -= 0.3f;
        }
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05F, 0.3f);
        NPC.SpriteFaceTarget();
        //NPC.FaceDirectionsToVelocity();
        if (NPC.IsGrounded(2, allowPlatforms: true) && Timer >= 100)
        {
            SwitchState(AIState.Fall);
        }
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HypnotizedSoul>(), minimumDropped: 1, maximumDropped: 4));
    }
    void AI_Fall()
    {
        NPC.PlayAnimationOneShot(ANIM_ATTACKOUT);
        Timer++;
        _squishScale.Y = MathHelper.Lerp(1f, 1.2f, MathF.Abs(NPC.velocity.Y / 15F));
        _squishScale.X = MathHelper.Lerp(1f, 0.9f, MathF.Abs(NPC.velocity.Y / 15F));
        if (NPC.velocity.Y < 15)
            NPC.velocity.Y += 0.5f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05F, 0.3f);
        NPC.FaceDirectionsToVelocity();
        if (NPC.IsGrounded(2, allowPlatforms: true))
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_Hop()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);

            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
            p2.fadeToColor = Color.Black;
            p2.innerColor = Color.White;
            p2.outerColor = Color.DarkBlue;
            p2.Scale *= 0.5f;

            NPC.velocity.Y = -7;
            if (State == AIState.HopHigh)
                NPC.velocity.Y -= 3;
            if (MultiplayerHelper.IsHost)
            {
                NPC.velocity.X = MoonUtils.Rand.NextBool(2) ? -1 : 1;
                NPC.velocity.X *= 3f;
                NPC.netUpdate = true;
            }
        }

        _targetAfAlpha = 1f;
        if (MoonUtils.Rand.NextBool(8))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
            PS.HypnoticSparkle.Spawn(new()
            {
                position = pos,
                timeLeft = 60,
                scale = 1,
                rot = 0,
            });
        }

        float a = MathF.Abs(NPC.velocity.Y / 15F);
        _squishScale.Y = MathHelper.Lerp(1f, 1.2f, a);
        _squishScale.X = MathHelper.Lerp(1f, 0.9f, a);
        if (NPC.velocity.Y < 15)
            NPC.velocity.Y += 0.5f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05F, 0.3f);
        NPC.PlayAnimationLooped(ANIM_IDLE);
        NPC.FaceDirectionsToVelocity();
        if (NPC.IsGrounded(2, allowPlatforms: true))
        {
            SwitchState(AIState.Idle);
        }
    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        NPC.netUpdate = true;
    }

    void ChooseAction()
    {
        NPC.TargetClosest();
        if (NPC.HasValidTarget)
        {
            //if a player is nearby, hop towards them
            if (SqrDistanceToPlayer < SqrAggroDistance)
            {
                _targetHopPosition = NPC.PlayerImTargeting.Center;
                SwitchState(AIState.ReadyHopTowardsTarget);
                return;
            }
        }

        if (MultiplayerHelper.IsHost)
        {
            SwitchState(AIState.Hop);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_afAlpha > 0.04f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                var color = Color.Lerp(Color.OrangeRed, Color.Transparent, i / (float)NPC.oldPos.Length) * _afAlpha * 0.2f;
                NPC.DrawAnimator(spriteBatch, color, pos);
            }
        }

        NPC.DrawAnimator(spriteBatch, drawColor);
        var glow = AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset;
        var drawer = SpritebatchDrawer.FromTextureWhite(glow);
        drawer.worldPosition = NPC.Center;
        drawer.color = new Color(5, 25, 25, 0);
        drawer.scale = new Vector2(0.4f);
        spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;
        this.MakeGore("_Gore_Top", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(0, 1));
    }

    public void DrawToRenderTargets()
    {
        if (_outliner.VisibleEnough)
        {
            void DrawOutline(SpriteBatch sb)
            {
                NPC.DrawAnimator(sb, _outliner.outlineColor);
            }
            OutlineRenderer.Queue(DrawOutline);
        }
        //   throw new NotImplementedException();
    }
}
