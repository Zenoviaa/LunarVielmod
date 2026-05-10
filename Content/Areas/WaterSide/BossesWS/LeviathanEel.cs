using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;


/*
 * 
 * 
 * Phase 1

- After each attack, the eel goes away for a short time before deciding its next move





 */


public class PrismaticElectricBolt : ModProjectile
{
    private Vector2 _initialVelocity;
    private float _randRadians;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _initialVelocity = Projectile.velocity;
        }


        if(Timer % 30 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                float radians = MathHelper.ToRadians(10);
                _randRadians = Main.rand.NextFloat(-radians, radians);
                Projectile.velocity = _initialVelocity.RotatedBy(_randRadians);
                Projectile.netUpdate = true;
            }
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class SinElectricShock : ModProjectile
{
    private Vector2[] _shockPos;
    private Vector2[] _sparkPos;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        _shockPos = new Vector2[32];
        _sparkPos = new Vector2[32];
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity = -Vector2.UnitY;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            var fp = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Goldenrod);
            fp.Scale *= 1.5f;
        }
        Projectile.velocity = Projectile.velocity.RotatedBy(0.03f);
        if (Timer % 8 == 0)
        {
            DustParticle sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(12, 12), Color.White, 0.7f);
            sp.fast = true;
            sp.gravity = 0;
            sp.noTileCollide = true;
        }

        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Vector2.Zero, Color.White, 0.3f);
            sp.gravity = 0;
        }

        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        for (int i = 0; i < _shockPos.Length; i++)
        {
            ref Vector2 position = ref _shockPos[i];
            Vector2 offset = new Vector2();

            float radians = (float)i / (float)_shockPos.Length * MathHelper.TwoPi;
            radians += Timer * 0.03f;

            float radius = ExtraMath.Osc(80, 128, speed: 18, offset: Projectile.whoAmI);

            radius *= inScale * outScale;
            offset.X += MathF.Sin(radians) * radius;
            offset.Y += MathF.Cos(radians) * radius;
            offset = Vector2.Lerp(offset, Vector2.Zero, (MathF.Sin(Timer * 0.5f + i) + 0.5f) * 0.1f);
            offset += Main.rand.NextVector2Circular(6, 6);
            position = Projectile.Center + offset;

            _sparkPos[i] = Projectile.Center + offset.RotatedBy(MathHelper.PiOver4) * 0.2f * Main.rand.NextFloat(1f, 1.5f);
        }

    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitPlayer(target, ref modifiers);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawBloom(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D bloomTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowScale = Vector2.One * 0.25f;
        float rotation = Main.GlobalTimeWrappedHourly * 4;
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Main.DiscoColor;
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.1f;
        bloomDrawer.color *= outScale;
        spriteBatch.Draw(bloomDrawer);
        for (int i = 0; i < _shockPos.Length; i += 2)
        {
            Vector2 pos = _shockPos[i];

            Color glowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
            glowColor.A = 0;
            glowColor *= 0.2f;
            glowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6, offset: i);
            glowColor *= outScale;

            bloomDrawer.worldPosition = pos;
            bloomDrawer.color = glowColor;
            bloomDrawer.scale = glowScale;
            spriteBatch.Draw(bloomDrawer);
        }

        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);

        Color spiralGlowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
        spiralGlowColor.A = 0;
        spiralGlowColor *= 0.2f;
        spiralGlowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6);
        spiralGlowColor *= outScale;
        spiralDrawer.color = spiralGlowColor;
        spriteBatch.Draw(spiralDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
        PrismaticLightningRenderer.Queue(_shockPos);
        PrismaticLightningRenderer.Queue(_sparkPos);
        return false;
    }
}

public class PrismaticLightningRenderer : PixelPrimitiveRenderer<PrismaticLightningRenderer>
{
    public override BaseShader PrepareShader()
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Main.DiscoColor;
        shader.OuterColor = Color.Goldenrod;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        return shader;
    }

    public override Color GetTrailColor(float completionRatio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Main.DiscoColor, osc);
    }

    public override float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 32, completionRatio) * MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
    }
}

public class LightningCrawl : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class LeviathanEelSegment : ModNPC
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private NPC Parent => Main.npc[(int)NPC.ai[0]];
    public override bool CheckActive()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
    }
}

public class MirageShader : CrystalShader<MirageShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Texture2D NoiseTexture
    {
        set
        {
            Effect.Parameters["noiseTexture"].SetValue(value);
         //   Effect.Parameters["noiseSize"].SetValue(value.Size());
        }
    }

    public float Alpha
    {
        set
        {
            Effect.Parameters["alpha"].SetValue(value);
        }
    }
}

public class LeviathanEel : ScarletBoss
{

    [Flags]
    public enum EelVisualEffect
    {
        None = 0,
        Zappy = 1,
        WaterTrail = 2,
        Invisible = 4,
        Mirage = 8
    }
    private Chain _hairChain2;
    private Chain HairChain2
    {
        get
        {
            if (_hairChain2 == null)
            {

                _hairChain2 = new Chain(NPC.Center, 2, 128);
            }
            return _hairChain2;
        }
    }
    private Chain _hairChain;
    private Chain HairChain
    {
        get
        {
            if (_hairChain == null)
            {

                _hairChain = new Chain(NPC.Center, 2, 128);
            }
            return _hairChain;
        }
    }

    private Chain _chain;
    private Chain Chain
    {
        get
        {
            if(_chain == null)
            {
                _chain = new Chain(NPC.Center, 100, 64);
            }
            return _chain;
        }
    }
    private Asset<Texture2D> _eyebrowTextureAsset;
    private Asset<Texture2D>[] _eyeTextureAssets;
    private Asset<Texture2D>[] _segmentTextureAssets;
    private Asset<Texture2D>[] _segmentGlowTextureAssets;
    private Vector2 _facingDirection;
    private Vector2 _arenaCenter;
    private Vector2 _teleportPosition;
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;

    private float _charge;
    private float _dashTrailAlpha;
    private float _mirageAlpha;
    private Outliner _outliner;
    private bool _contactDamage;
    private bool _showDashTrail;
    private EelVisualEffect _effects;

    private enum AIState
    {
        //First let's break this down, and get all the states that we need
        //Then we can figure out which systems and projectiles we need
        //Solve smaller problems until the whole is complete
        SpawnIntro,
        Despawn,
   
        Idle,
        Death,

        S_Dash,
        Lightning_Crawl,
        Ball_Bouncer,
        Chomp,
        Lightning_Wiggle,
        Suck,
        Suck_V2,

        Tesla_Coil,
        Overcharge,
        Eyeline_Dash,
        Swallow

    }
    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private AIState _attackToTest;
    private PatternManager<AIState> _patternManagerBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new PatternManager<AIState>();
                _patternManagerBackingField.AddPattern(AIState.Lightning_Crawl, 1.0f);
            }
            return _patternManagerBackingField;
        }
    }
    private int Bite_Damage => 35;
    private float IdleTime => 360;
    private float SDashReadyTime => 120;
    private float SDashChargeTime => 24;
    private float SDashSpeed => 55;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        Main.npcFrameCount[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 12000;
        NPC.defense = 18;
        NPC.damage = 90;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;

        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/LeviathanEel");
        NPC.HitSound = SoundID.NPCHit1 with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
      //  NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    private void MoveAndSinToward(Vector2 directionToTarget, float speed)
    {
        float distance = 6;
        Vector2 initialSpeed = directionToTarget * speed;
        Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
        offset.Normalize();
        offset *= (float)(Math.Cos(Timer * 3 * (Math.PI / 180)) * (distance / 3));

        Vector2 targetVelocity = initialSpeed + offset;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.04f);
    }
    public override void AI()
    {
        base.AI();
        //Testing Attacks
        _attackToTest = AIState.S_Dash; 
        if (_arenaCenter == Vector2.Zero)
            _arenaCenter = MyTarget.Center;


        _effects = EelVisualEffect.None;
        _contactDamage = false;
        _showDashTrail = false;
        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = _teleportPosition;
            }
            _teleportPosition = Vector2.Zero;
    
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        _outliner.SetDefaults();
        _charge = MathHelper.Lerp(_charge, 0f, 0.1f);
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.SpawnIntro:
                AI_SpawnIntro();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.S_Dash:
                AI_SDash();
                break;

        }
        _outliner.Update();

        float targetMirageAlpha = _effects.HasFlag(EelVisualEffect.Mirage) ? 1f : 0f;
        _mirageAlpha = MathHelper.Lerp(_mirageAlpha, targetMirageAlpha, 0.1f);

        float targetDashTrailAlpha = _showDashTrail ? 1f : 0f;
        _dashTrailAlpha = MathHelper.Lerp(_dashTrailAlpha, targetDashTrailAlpha, 0.1f);
        Chain.pinned[0] = true;
        Chain.points[0] = NPC.Center;
        Chain.ResolveRootToBack();

        NPC.spriteDirection = 1;
        float facingRotation = _facingDirection.ToRotation();
        NPC.rotation = facingRotation;

        SimulateHair();
    }

    private void AnimateMouthBasedOnDistance()
    {
        float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
        float progress = distanceToTarget / 600f;
        progress = EasingFunction.Clamp(progress);
        float ratio = 1f - progress;
        _frame = (int)MathHelper.Lerp(0, 3, ratio);
    }
    private void Teleport(Vector2 teleportPosition)
    {
        if (!MultiplayerHelper.IsHost)
            return;

        _teleportPosition = teleportPosition;
        NPC.netUpdate = true;
    }


    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }

    private void AI_SDash()
    {
        Timer++;
        AnimateMouthBasedOnDistance();
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
            
                    }

                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    FaceVelocity();
                    if(Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startPosition = NPC.Center;
                        _initialVelocity = NPC.velocity;
                    }


                    _effects |= EelVisualEffect.Mirage;
                    //Slowly inch up to target
                    Vector2 positionToMoveTo = MyTarget.Center;
                    float distanceToTarget = Vector2.Distance(positionToMoveTo, NPC.Center);
                    Vector2 targetVector = (positionToMoveTo - NPC.Center).RotatedBy(0.05f);
                    targetVector = targetVector.SafeNormalize(Vector2.Zero);
                    MoveAndSinToward(targetVector, MathHelper.Lerp(8f, 16, EasingFunction.InOutSine(Timer / 120f)));
                    FaceVelocity();
   

                    _charge = MathHelper.Lerp(1f, 0f, distanceToTarget / 384f);
                 //   _outliner.warning = true;
                    if(distanceToTarget < 384)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        
                    }
                    NPC.velocity *= 0.92f;
                    _effects |= EelVisualEffect.Mirage;

                    // FaceVelocity();

                    _charge = MathHelper.Lerp(_charge, 1f, 0.04f);
                  //  _outliner.warning = true;
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity = Vector2.Lerp(_facingDirection * 2, -_facingDirection * 7, EasingFunction.QuadraticBump(Timer / 30f));
                    _charge = MathHelper.Lerp(_charge, 1f, 0.04f);
                    _outliner.warning = true;

                    if (Timer == 15)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<LeviathanBite>(), Bite_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }

                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    if(Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
              
                    }

                    if(Timer < 5)
                    {
                        _startPosition = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * SDashSpeed;
                    }
                  


                    _charge = MathHelper.Lerp(_charge, 1f, 0.02f);
                    _showDashTrail = true;

                    float ratio = Timer / (SDashChargeTime / 3f);
                    float ease2 = EasingFunction.InExpo(ratio);
                    Vector2 easeVelocity = Vector2.Lerp(_initialVelocity, _startPosition, ease2);
                    NPC.velocity = easeVelocity;
                    FaceVelocity();

                    _contactDamage = true;
                    _outliner.attacking = true;
                    if(Timer >= SDashChargeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    _effects |= EelVisualEffect.Mirage;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    NPC.velocity *= 0.99f;
                    FaceVelocity();

                 //   _outliner.warning = true;
                    if(Timer >= 15)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter < 3)
                        {
                            AttackCycle = 1;
                        }
                        else
                        {
                            AttackCycle++;

                        }


                    }
                }
                break;
            case 6:
                {
                    _effects |= EelVisualEffect.Mirage;
                    // NPC.velocity.X *= 0.98f;
                    // NPC.velocity.Y += 0.5f;
                    float speed = MathHelper.Lerp(0f, 45, EasingFunction.InExpo(Timer / 100f));
                    MoveAndSinToward(Vector2.UnitY, speed);
                    FaceVelocity();
                    if(Timer >= 280)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frame.Y = frameHeight * _frame;
    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y += 0.5f;
        FaceVelocity();
        if (Timer >= 180)
        {
            NPC.active = false;
        }
    }
    private void AI_SpawnIntro()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            AIState pattern = PatternManager.NextPattern();
            SwitchState(pattern);
            if(_attackToTest != AIState.SpawnIntro)
            {
                SwitchState(_attackToTest);
            }
        }
    }

    private void FaceVelocity()
    {
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.2f);
    }
    private void AI_Idle()
    {
        AttackCycle = 0;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.velocity = Vector2.Zero;
            _arenaCenter = MyTarget.Center - new Vector2(0, 128);
            Teleport(MyTarget.Center - new Vector2(1400, 0));
        }

        float speed = MathHelper.Lerp(24, 4f, EasingFunction.QuadraticBump(Timer / IdleTime));
        MoveAndSinToward(Vector2.UnitX, speed);
        FaceVelocity();
        if (Timer < IdleTime / 2f)
            _effects |= EelVisualEffect.Mirage;
        
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
    #region Dash Trail

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _dashTrailAlpha * EasingFunction.QuadraticBump(completionRatio);
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 128, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
    #endregion
    #region DrawCode
    private void LoadSegmentTextureAssets()
    {
        if (_segmentTextureAssets != null)
            return;
        _segmentTextureAssets = new Asset<Texture2D>[5];
        _segmentGlowTextureAssets = new Asset<Texture2D>[5];
        for (int i = 0; i < _segmentTextureAssets.Length; i++)
        {
            _segmentTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_{i}");
            _segmentGlowTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_{i}_Glow");
        }

        _eyeTextureAssets = new Asset<Texture2D>[3];
        for (int i = 0; i < _eyeTextureAssets.Length; i++)
        {
            _eyeTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_Eye_{i}");
        }
        _eyebrowTextureAsset = ModContent.Request<Texture2D>($"{Texture}_Eyebrow");
    }

    private void DrawSegment(int index, int segmentIndex, Color? overrideColor=null)
    {
        //Segment 0 is drawn manually
        if (index == 0)
            return;

        Vector2 root = Chain.points[index];
        Vector2 next = Chain.points[index - 1];
        float rotation = (next - root).ToRotation();
        Asset<Texture2D> segmentTextureAsset = _segmentTextureAssets[segmentIndex];
        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromTextureAsset(segmentTextureAsset, root);
        segmentDrawer.rotation = rotation;

        float ratio = (float)index / (float)(Chain.points.Length - 2);
        segmentDrawer.scale = Vector2.One * MathHelper.SmoothStep(1f, 0.85f, ratio);

        if (overrideColor.HasValue)
        {
            segmentDrawer.color = overrideColor.Value;
        }
        Main.spriteBatch.Draw(segmentDrawer);

        if(overrideColor == null)
        {
            Color lightningColor = new Color(185, 255, 234);
            float chargePerSegment = 1f / (float)Chain.points.Length;
            float myCharge = _charge - (chargePerSegment * index);
            float levelOfCharge = myCharge / chargePerSegment;
            segmentDrawer.texture = _segmentGlowTextureAssets[segmentIndex].Value;
            segmentDrawer.color = Color.Lerp(Color.Black, lightningColor, levelOfCharge) * ExtraMath.Osc(0.5f, 1f, speed: 12, offset: index);
            segmentDrawer.color.A = 0;
            Main.spriteBatch.Draw(segmentDrawer);
            Lighting.AddLight(root, lightningColor.ToVector3() * levelOfCharge * 0.3f);
        }
    }

    private void DrawAllSegments(Color? overrideColor=null)
    {
        //Draw Tail 
        int tailIndex = 4;
        int neckIndex = 0;
        DrawSegment(Chain.points.Length - 1, tailIndex, overrideColor);

        int segmentCounter = 0;
        for (int i = Chain.points.Length - 1; i > 1; i--)
        {
            segmentCounter++;
            float ratio = segmentCounter / (float)(Chain.points.Length - 2);
            int segmentTextureIndex = (int)MathHelper.Lerp(3, 1, ratio);
            DrawSegment(i, segmentTextureIndex, overrideColor);
        }

        //Draw Neck
        DrawSegment(1, neckIndex, overrideColor);
    }
    private void DrawWhites(SpriteBatch sb)
    {

        DrawAllSegments(_outliner.outlineColor);
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LoadSegmentTextureAssets();
        OutlineRenderer.Queue(DrawWhites);
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair);
        PixelationManager.QueuePrimitivesDrawAction(DrawHairBack, DrawLayer.BehindTiles);
        bool drawMirage = _mirageAlpha > 0.03f;
        if(drawMirage)
        {
            MirageShader mirageShader = MirageShader.Instance;
            mirageShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
            mirageShader.Time = Main.GlobalTimeWrappedHourly;
            mirageShader.Alpha = _mirageAlpha;
            spriteBatch.Restart(effect: mirageShader.Effect);
        }

        DrawAllSegments();
 

        //Finally attach the head

        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(segmentDrawer);

        //draw eyes
        Vector2 targetDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        for(int i = 0; i < _eyeTextureAssets.Length; i++)
        {
            Asset<Texture2D> eyeTextureAsset = _eyeTextureAssets[i];
            SpritebatchDrawer eyeDrawer = SpritebatchDrawer.FromTextureAsset(eyeTextureAsset, NPC.Center);
            eyeDrawer.spriteEffects = segmentDrawer.spriteEffects;
            eyeDrawer.rotation = segmentDrawer.rotation;
            eyeDrawer.scale = Vector2.One * NPC.scale;
            eyeDrawer.drawOrigin -= targetDirection * 10;
            spriteBatch.Draw(eyeDrawer);

            //Glow in the darkkk
            eyeDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 1, offset: i);
            eyeDrawer.color.A = 0;
            spriteBatch.Draw(eyeDrawer);
        }

        SpritebatchDrawer eyebrowDrawer = SpritebatchDrawer.FromTextureAsset(_eyebrowTextureAsset, NPC.Center);
        eyebrowDrawer.rotation = segmentDrawer.rotation;
        eyebrowDrawer.spriteEffects = segmentDrawer.spriteEffects;
        eyebrowDrawer.scale = Vector2.One * NPC.scale;
        eyebrowDrawer.drawOrigin = new Vector2(80, 64);
        spriteBatch.Draw(eyebrowDrawer);

        if (drawMirage)
        {
            spriteBatch.RestartDefaults();
        }
        return false;
    }
    #region Hair Rendering
    private void SimulateHair()
    {
        HairChain.points[0] = NPC.Center + new Vector2(-80, -64).RotatedBy(NPC.rotation);
        HairChain.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        HairChain.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            HairChain.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < HairChain.points.Length; i++)
        {
            HairChain.points[i].Y += MathHelper.Lerp(0.2f, 1f, (float)i / (float)HairChain.points.Length);
        }
        HairChain.ResolveBackToRoot();



        HairChain2.points[0] = NPC.Center + new Vector2(-64, -80).RotatedBy(NPC.rotation);
        HairChain2.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        HairChain2.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            HairChain2.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < HairChain2.points.Length; i++)
        {
            HairChain2.points[i].Y += MathHelper.Lerp(0.2f, 1f, (float)i / (float)HairChain2.points.Length);
        }
        HairChain2.ResolveBackToRoot();
    }
    private float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 0, ratio)  * EasingFunction.QuadraticBump(ratio);
    }
    private Color GetHairColor(float ratio)
    {
        return Color.DarkGray  * EasingFunction.OutExpo(ratio + 0.5f);
    }
    private Color GetHairColor2(float ratio)
    {
        return Color.Lerp(Color.DarkGray, Color.Black, 0.5f) * EasingFunction.OutExpo(ratio + 0.5f);
    }

    private void DrawHair(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, HairChain.points, GetHairColor, GetHairWidth, shader);
    }
    private void DrawHairBack(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, HairChain2.points, GetHairColor2, GetHairWidth, shader);
    }
    #endregion
    #endregion
}
