using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;

//VERLIA TODO:
//Two bouncing moons attack
//NEW scrolling moon and glowing moon shader very cool
//NEW moon particle, make some cool like space-y dust

//Blue Magic Sword
//NEW magic sword glow mask
//Should be blue

//Spinning little moons
//Tiny little moons with the same new moon shader
//Cool explosion using the new radial shear shader that we made
//Sparkling little moon dusts would be super cool

//Blade dance
//Basically the same as legacy, not much has to change

//Clone summon
//She creates a really cool mirror and her clones pop out

//Phase 2 transition
//Desperation big moon
//Really cool blue moon
//Moon blade sprites and afterimage effects when it explodes
//When she starts this there's a cool circle that comes in and pulls you inward once she teleports to her center

//Sword Fall
//Blue magical swords spiral in and then glisten/bright/flash up, and then they come down with a bit of easing
//Camera will slightly pan upward so you can see them better like Bishinine's rain


//Running
//basic afterimage and trailing effects
//cool moon magic

//Dash Slash
//Very fast, uses the slash animations we already have just with a bit more

//Blade Dance V2
//Goes into the big sword, shouldn't look like the other one it'll be a lot more elegant, blue, magical, and moony

//Dying
//Verlia slows, down, falls down and looks into the mirror
//She sees a clone of herself before the mirror cracks and she dies
//and her sword drops on the ground (you can't pick it up it's just visual)
public class VerliaWings2Shader : CrystalShader<VerliaWings2Shader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _perlinNoiseSizeParam;
    private EffectParameter _scrollingTextureSizeParam;
    private EffectParameter _tilingParam;
    private EffectParameter _distortionStrengthParam;
    private EffectParameter _bloomColorStartParam;
    private EffectParameter _bloomColorEndParam;
    private EffectParameter _frequencyParam;

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }
    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }
    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            _scrollingTextureSizeParam ??= Effect.Parameters["scrollingTextureSize"];
            _scrollingTextureSizeParam.SetValue(value.Size());
        }
    }

    public Texture2D PerlinNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[2] = value;
            _perlinNoiseSizeParam ??= Effect.Parameters["perlinNoiseSize"];
            _perlinNoiseSizeParam.SetValue(value.Size());
        }
    }

    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }

    public float DistortionStrength
    {
        set
        {
            _distortionStrengthParam ??= Effect.Parameters["distortionStrength"];
            _distortionStrengthParam.SetValue(value);
        }
    }

    public Color BloomColorStart
    {
        set
        {
            _bloomColorStartParam ??= Effect.Parameters["bloomColorStart"];
            _bloomColorStartParam.SetValue(value.ToVector3());
        }
    }

    public Color BloomColorEnd
    {
        set
        {
            _bloomColorEndParam ??= Effect.Parameters["bloomColorEnd"];
            _bloomColorEndParam.SetValue(value.ToVector3());
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}
public class VerlianWingsShader : CrystalShader<VerlianWingsShader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _perlinNoiseSizeParam;
    private EffectParameter _scrollingTextureSizeParam;
    private EffectParameter _tilingParam;
    private EffectParameter _distortionStrengthParam;
    private EffectParameter _bloomColorStartParam;
    private EffectParameter _bloomColorEndParam;
    private EffectParameter _frequencyParam;

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }
    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }
    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            _scrollingTextureSizeParam ??= Effect.Parameters["scrollingTextureSize"];
            _scrollingTextureSizeParam.SetValue(value.Size());
        }
    }

    public Texture2D PerlinNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[2] = value;
            _perlinNoiseSizeParam ??= Effect.Parameters["perlinNoiseSize"];
            _perlinNoiseSizeParam.SetValue(value.Size());
        }
    }

    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }

    public float DistortionStrength
    {
        set
        {
            _distortionStrengthParam ??= Effect.Parameters["distortionStrength"];
            _distortionStrengthParam.SetValue(value);
        }
    }

    public Color BloomColorStart
    {
        set
        {
            _bloomColorStartParam ??= Effect.Parameters["bloomColorStart"];
            _bloomColorStartParam.SetValue(value.ToVector3());
        }
    }

    public Color BloomColorEnd
    {
        set
        {
            _bloomColorEndParam ??= Effect.Parameters["bloomColorEnd"];
            _bloomColorEndParam.SetValue(value.ToVector3());
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}
public class VerliaShockwaveShader : CrystalShader<VerliaShockwaveShader>
{
    private EffectParameter _timeParam;
    private EffectParameter _frequencyParam;
    private EffectParameter _amplitudeParam;
    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }

    public float Frequency
    {
        set
        {
            _frequencyParam ??= Effect.Parameters["frequency"];
            _frequencyParam.SetValue(value);
        }
    }

    public float Amplitude
    {
        set
        {
            _amplitudeParam ??= Effect.Parameters["amplitude"];
            _amplitudeParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Amplitude = 0.5f;
        Frequency = 4.0f;
    }
}
public class ScrollingMoonShader : CrystalShader<ScrollingMoonShader>
{
    private EffectParameter _scrollOffsetParam;
    private EffectParameter _imageSizeParam;
    private EffectParameter _maskSizeParam;
    private EffectParameter _bendStrengthParam;
    private EffectParameter _tilingParam;
    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public float BendStrength
    {
        set
        {
            _bendStrengthParam ??= Effect.Parameters["bendStrength"];
            _bendStrengthParam.SetValue(value);
        }
    }
    public Vector2 ScrollOffset
    {
        set
        {
            _scrollOffsetParam ??= Effect.Parameters["scrollOffset"];
            _scrollOffsetParam.SetValue(value);
        }
    }

    public Vector2 ImageSize
    {
        set
        {
            _imageSizeParam ??= Effect.Parameters["imageSize"];
            _imageSizeParam.SetValue(value);
        }
    }

    public Vector2 MaskSize
    {
        set
        {
            _maskSizeParam ??= Effect.Parameters["maskSize"];
            _maskSizeParam.SetValue(value);
        }
    }


    /// <summary>
    /// Sets the Sampler 1 state to Point Clamp and Texture 1 to the passed value, while also automatically setting the ImageSize parameter of the shader
    /// </summary>
    public Texture2D ScrollingTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[1] = value;
            ImageSize = value.Size();
        }
    }
}
public class StarMixShader : CrystalShader<StarMixShader>
{
    private EffectParameter _tilingParam;
    private EffectParameter _matrixParam;
    private EffectParameter _laserTextureParam;
    private EffectParameter _innerTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _innerColorParam;
    private EffectParameter _outerColorParam;


    public Matrix TransformMatrix
    {
        set
        {
            _matrixParam ??= Effect.Parameters["transformMatrix"];
            _matrixParam.SetValue(value);
        }
    }

    public Asset<Texture2D> MaskTexture
    {
        set
        {
            _laserTextureParam ??= Effect.Parameters["maskTexture"];
            _laserTextureParam.SetValue(value.Value);
        }
    }

    public Asset<Texture2D> InnerTexture
    {
        set
        {
            _innerTextureParam ??= Effect.Parameters["innerTexture"];
            _innerTextureParam.SetValue(value.Value);
        }
    }


    public Color InnerColor
    {
        set
        {
            _innerColorParam ??= Effect.Parameters["innerColor"];
            _innerColorParam.SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            _outerColorParam ??= Effect.Parameters["outerColor"];
            _outerColorParam.SetValue(value.ToVector3());
        }
    }

    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }


    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TransformMatrix = TrailDrawer.WorldViewPoint2;
        InnerColor = Color.Yellow;
        OuterColor = Color.Red;

        BlendState = BlendState.AlphaBlend;
        MaskTexture = TrailRegistry.BeamTrail;
        Time = Main.GlobalTimeWrappedHourly * 24;
        Tiling = Vector2.One;
    }
}

public class Verlia : ScarletBoss,
    IDrawOutlines
{
    private enum AIState
    {

        Spawn,
        Despawn,
        Idle,
        ReallyIdle,
        IdleOut,

        //Phase 1
        Two_Bouncing_Moons,
        Blue_Magic_Sword,
        Spining_Little_Moons,
        Blade_Dance,
        Clone_Summon,

        //Phase 2
        Phase_2_Transition,
        Desperation_Big_Moon,
        Sword_Fall,
        Running,
        Dash_Slash,
        Blade_Dance_V2,

        Death_Start,
        Death,
    }

    private Asset<Texture2D> _wingTextureAsset;
    private Asset<Texture2D> _wingOutlineTextureAsset;
    private Asset<Texture2D> _wingTextureAsset2;

    private Color _outlineColor;
    private bool _dying;
    private bool _warning;
    private bool _attacking;
    private bool _showTrail;
    private bool _bladeDanceV2;
    private bool _magicSwordV2;
    private bool _contactDamage;
    private bool _showWings;
    private bool _showMagicCircle;
    private bool _hasDoneDoubleMoon;
    private float _magicCircleAlpha;
    private float _trailAlpha;
    private float _traveledDistance;
    private bool _phase2;
    private float _wingScale;
    private float _inwardAlpha;
    private Vector2 _startPosition;
    private Vector2 _startVelocity;
    private Vector2 _runningVelocity;
    private Vector2 _teleportPosition;
    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            _patternManager ??= new PatternManager<AIState>(
                new Tuple<AIState, float>(AIState.Two_Bouncing_Moons, 1.0f),
                new Tuple<AIState, float>(AIState.Spining_Little_Moons, 1.0f),
                new Tuple<AIState, float>(AIState.Blue_Magic_Sword, 1.0f),
                new Tuple<AIState, float>(AIState.Clone_Summon, 1.0f),
                new Tuple<AIState, float>(AIState.Blade_Dance, 1.0f),
                new Tuple<AIState, float>(AIState.Blade_Dance_V2, 1.0f),
                new Tuple<AIState, float>(AIState.Desperation_Big_Moon, 1.0f),
                new Tuple<AIState, float>(AIState.Running, 1.0f),
                new Tuple<AIState, float>(AIState.Sword_Fall, 1.0f));
            return _patternManager;
        }
    }


    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {
                _animatorBackingField = CreateAnimator();
                _animatorBackingField.PlayAnimation(ANIM_SUMMON);
            }

            return _animatorBackingField;
        }
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private int Twin_Bouncing_Moon_Damage => 50;

    private int Mini_Moon_Damage => 20;
    private int Moon_Slash_Damage => 35;

    private int Moon_Slash_Hold_Damage => 70;
    private int Moon_Shot_Damage => 20;
    private int Moon_Blade_damage = 30;
    private int Great_Blade_Damage => 50;
    private int Desperation_Moon_Damage => 80;
    private int Altide_Sword_Damage => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_startVelocity);
        writer.WriteVector2(_runningVelocity);
        writer.Write(_traveledDistance);
        writer.Write(_dying);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _startVelocity = reader.ReadVector2();
        _runningVelocity = reader.ReadVector2();
        _traveledDistance = reader.ReadSingle();
        _dying = reader.ReadBoolean();
    }

    #region Defaults
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 80;
        NPC.defense = 15;
        NPC.lifeMax = 12000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VerliaOfTheMoon");
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // Sets the description of this NPC that is listed in the bestiary
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Verlia, The Empress of the Stars and moon, Vixyl's sister and a master magic swordswoman."))
            });
    }

    #endregion
    private float _stepDistance;
    private void JapanFootsteps()
    {
        float traveledDistance = Vector2.Distance(NPC.position, NPC.oldPosition);
        _stepDistance += traveledDistance;
        if (_stepDistance >= 100)
        {
            Vector2 pos = NPC.Bottom + new Vector2(Main.rand.NextFloat(-16, 16), 0);
            pos.Y += 0;
            var circleStep = LegacyParticle.NewParticle<CircleStepParticle>(pos, Vector2.UnitY);
            circleStep.color = Color.White;
            circleStep.Rotation = NPC.rotation;
            circleStep.Scale *= 0.5f;
            _stepDistance = 0;
        }
    }

    public override void AI()
    {
        base.AI();
        _showTrail = false;
        _attacking = false;
        _warning = false;
        _bladeDanceV2 = false;
        _contactDamage = false;
        _showWings = false;
        _showMagicCircle = false;
        if (_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        _inwardAlpha = MathHelper.Lerp(_inwardAlpha, 0f, 0.1f);

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (State != AIState.Despawn)
                    SwitchState(AIState.Despawn);
            }
        }
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;

            case AIState.Spawn:
                AI_Spawn();
                break;

            case AIState.Idle:
                AI_Idle();
                _showWings = true;
                break;

            case AIState.ReallyIdle:
                AI_ReallyIdle();
                _showWings = true;
                break;
            case AIState.IdleOut:
                AI_IdleOut();
                //_showWings = true;
                break;
            case AIState.Two_Bouncing_Moons:
                AI_TwoBouncingMoons();
                //  _showMagicCircle = true;
                break;

            case AIState.Spining_Little_Moons:
                AI_SpinningLittleMoons();
                //  _showMagicCircle = true;
                break;

            case AIState.Blade_Dance:
                AI_BladeDance();
                // _showMagicCircle = true;
                break;

            case AIState.Blue_Magic_Sword:
                AI_BlueMagicSword();
                //  _showMagicCircle = true;
                break;

            case AIState.Blade_Dance_V2:
                _bladeDanceV2 = true;
                AI_BladeDance();
                break;

            case AIState.Clone_Summon:
                AI_CloneSummon();
                //  _showMagicCircle = true;
                break;

            case AIState.Running:
                AI_Running();
                break;

            case AIState.Sword_Fall:
                AI_SwordFall();
                //   _showMagicCircle = true;
                break;

            case AIState.Desperation_Big_Moon:
                AI_DesperationBigMoon();
                //   _showMagicCircle = true;
                break;
        }


        if (State != AIState.Idle && Animator.GetAnimation() != "Explode")
            _showWings = true;


        float targetMagicCircleAlpha = _showMagicCircle ? 1f : 0f;
        _magicCircleAlpha = MathHelper.Lerp(_magicCircleAlpha, targetMagicCircleAlpha, 0.1f);

        float targetWingScale = _showWings ? 1f : 0f;
        _wingScale = MathHelper.Lerp(_wingScale, targetWingScale, 0.1f);
        _trailAlpha = MathHelper.Lerp(_trailAlpha, _showTrail ? 1f : 0f, 0.1f);
        if (_attacking)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else if (_warning)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.1f);
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.Y -= 1f;
        FaceTarget();
        Animator.PlayAnimation(ANIM_EXPLODE);
        if (Timer >= 90)
        {
            NPC.active = false;
        }
    }

    private void ReallyIdle()
    {

    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            _traveledDistance = 0;
            AttackCounter = 0;
            AttackCycle = 0;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private bool IsBlacklisted(AIState state)
    {
        if (_phase2)
        {
            switch (state)
            {
                case AIState.Blade_Dance:
                case AIState.Clone_Summon:
                    return true;
            }
        }
        else
        {
            switch (state)
            {
                case AIState.Desperation_Big_Moon:
                case AIState.Blade_Dance_V2:
                case AIState.Sword_Fall:
                    return true;
            }
        }
        return false;
    }
    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            AIState state;
            if (!_phase2 && NPC.life < NPC.lifeMax * 0.5f)
            {
                state = AIState.Desperation_Big_Moon;
            }
            else
            {
                state = PatternManager.NextPattern();
                while (IsBlacklisted(state))
                    state = PatternManager.NextPattern();
            }

            if (!_hasDoneDoubleMoon)
            {
                state = AIState.Two_Bouncing_Moons;
                _hasDoneDoubleMoon = true;
            }

            SwitchState(state);


        }

        if (_dying)
        {
            SwitchState(AIState.Desperation_Big_Moon);
        }

        //   SwitchState(AIState.Desperation_Big_Moon);
    }

    private void Teleport(Vector2 pos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = pos;
            NPC.netUpdate = true;
        }
    }

    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }

    private void TeleportsBehindYou(float dist = 64)
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
            NPC.TargetClosest();
            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            Teleport(MyTarget.Center + dir * Vector2.UnitX * dist);
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }

    private void TeleportsAboveYou()
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
            NPC.TargetClosest();
            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            Teleport(MyTarget.Center + -Vector2.UnitY * 100);
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }

    private void TeleportAbovePlayer(float height)
    {
        NPC.TargetClosest();
        Vector2 playerPosition = MyTarget.Center;// + -Vector2.UnitY * 384;
        Point tile = playerPosition.ToTileCoordinates();
        tile = TileUtilities.FallToSolidTile(tile);
        Vector2 bottomWorld = tile.ToWorldCoordinates();
        bottomWorld.Y -= height;
        Teleport(bottomWorld);
    }

    private void TeleportsFarAboveYou()
    {
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, NPC.position);

            _startVelocity = NPC.velocity;
            float dir = Main.rand.NextBool(2) ? -1 : 1;
            TeleportAbovePlayer(144);


        }
        if (Timer == 5)
        {
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
            fx.Scale *= 1.8f;
        }

        NPC.velocity = Vector2.Zero;
        FaceTarget();
        Animator.PlayAnimation(ANIM_TELEPORTIN);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }
    }


    private void SpinAroundMoon()
    {
        Vector2 pos = NPC.Center;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<VerliaDesperationMoon>())
            {
                pos = proj.Center;
                break;
            }
        }
        //   _showMagicCircle = true;

        float inTime = 80f;
        float ratio = Timer / inTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 rotationOffset = (NPC.Center - pos).SafeNormalize(Vector2.Zero);
        Vector2 targetPos = pos + rotationOffset.RotatedBy(0.015f) * 400;
        Vector2 targetVelocity = (targetPos - NPC.Center);
        float dist = Vector2.Distance(NPC.Center, targetPos);
        float ratio2 = dist / 384f;
        ratio2 = 1f - ratio2;
        float velEase = EasingFunction.InOutSine(ratio2);
        NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, velEase);
    }
    private void SpinAroundMoonUntilBelow()
    {
        Vector2 pos = NPC.Center;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == ModContent.ProjectileType<VerliaDesperationMoon>())
            {
                pos = proj.Center;
                break;
            }
        }


        Vector2 rotationOffset = (NPC.Center - pos).SafeNormalize(Vector2.Zero);
        Vector2 below = Vector2.UnitY;
        float dp = Vector2.Dot(below, rotationOffset);

        if (dp < 0.99f)
        {
            Vector2 targetPos = pos + rotationOffset.RotatedBy(0.025f) * 400;
            Vector2 targetVelocity = (targetPos - NPC.Center);
            float dist = Vector2.Distance(NPC.Center, targetPos);
            float ratio2 = dist / 384f;
            ratio2 = 1f - ratio2;
            float velEase = EasingFunction.InOutSine(ratio2);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, velEase);
        }
        else
        {
            NPC.velocity *= 0.8f;
            AttackCycle++;
            Timer = 0;
        }
    }


    private VerliaDesperationMoon _moon;

    private void AI_DesperationBigMoon()
    {
        _phase2 = true;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    _showMagicCircle = true;
                    _warning = true;
                    NPC.velocity *= 0.9f;
                    //CameraTargetSystem.AddTarget(NPC.Center);
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _warning = true;
                    NPC.velocity *= 0.9f;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            var modProj = Projectile.NewProjectileDirect(SourceFromThis, NPC.Center + -Vector2.UnitY * 384, Vector2.Zero,
                                ModContent.ProjectileType<VerliaDesperationMoon>(), Desperation_Moon_Damage, 1, Main.myPlayer);
                            _moon = modProj.ModProjectile as VerliaDesperationMoon;
                            if (_moon != null)
                            {
                                _moon.parentIndex = NPC.whoAmI;
                            }
                        }
                    }

                    FaceTarget();
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    FaceTarget();
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Timer >= 200)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                }
                break;
            case 4:
                {
                    if (Timer == 1)
                    {
                        _startVelocity = NPC.velocity;
                    }
                    FaceTarget();
                    Vector2 pos = NPC.Center;
                    if (_moon != null)
                    {
                        pos = _moon.Projectile.Center;
                    }

                    _showMagicCircle = true;
                    Animator.PlayAnimation(ANIM_PULSE);
                    SpinAroundMoon();
                    if (Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    if (Timer == 1)
                    {
                        int style = 1;
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos2 = NPC.Center;
                            pos2.Y -= 32;
                            pos2 += Main.rand.NextVector2Circular(32, 32);


                            Projectile.NewProjectile(NPC.GetSource_FromThis(), pos2, Vector2.Zero,
                                ModContent.ProjectileType<MoonShot>(), Moon_Shot_Damage, 0f, Owner: Main.myPlayer,
                            ai1: style,
                            ai2: 1);
                        }
                    }
                    Animator.PlayAnimation(ANIM_DOWN);
                    FaceTarget();
                    SpinAroundMoon();
                    if (Timer >= 100)
                    {
                        AttackCycle++;
                        AttackCounter++;
                        Timer = 0;
                    }
                }
                break;
            case 6:
                {
                    Animator.PlayAnimation(ANIM_IDLESUMMON);
                    FaceTarget();
                    SpinAroundMoon();
                    if (AttackCounter == 1)
                    {
                        if (Timer >= 200)
                        {
                            Timer = 0;
                            AttackCycle -= 2;
                        }
                    }

                    if (AttackCounter == 2)
                    {
                        if (Timer >= 50)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 7:
                {


                    Animator.PlayAnimation(ANIM_HOLDUP);
                    FaceTarget();
                    SpinAroundMoonUntilBelow();
                }
                break;
            case 8:
                {
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {

                        if (_moon != null)
                        {
                            _moon.Projectile.ai[1] = _dying ? 5 : 4;
                        }
                    }
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    FaceTarget();
                    NPC.velocity *= 0.8f;

                    if (_moon != null)
                    {
                        _moon.parentIndex = NPC.whoAmI;
                    }

                    if (_moon != null && _moon.holdState == 1)
                    {
                        Timer = 0;
                        AttackCycle++;
                        NPC.netUpdate = true;
                    }
                }
                break;
            case 9:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Moaning"), NPC.position);
                    }
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    FaceTarget();
                    NPC.velocity *= 0.8f;
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 10:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Huhhuh"), NPC.position);
                    }

                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        if (_moon != null)
                        {
                            _moon.holdState++;
                            _moon.Projectile.netUpdate = true;
                        }
                    }
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    FaceTarget();

                    NPC.velocity.X *= 0.9f;
                    NPC.velocity.Y = MathHelper.Lerp(1.25f, -2.5f, EasingFunction.InOutSine(Timer / 30f));

                    if (_dying)
                    {
                        if (Timer >= 30f)
                        {
                            Timer = 0;
                            AttackCycle += 2;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (Timer >= 30)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }

                }
                break;
            case 11:
                {
                    ExplodeOut();
                }
                break;
            case 12:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Moaning"));
                    }

                    Animator.PlayAnimation(ANIM_HOLDUP);
                    NPC.velocity.X *= 0.9f;
                    FaceTarget();
                    if (Timer >= 75)
                    {

                        if (_moon == null || !_moon.Projectile.active)
                        {
                            NPC.Kill();
                        }
                    }
                }
                break;
        }
    }

    private void AI_SwordFall()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _showMagicCircle = true;
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                    }
                    _warning = true;
                    TeleportsAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"), NPC.position);
                    }

                    FaceTarget();
                    _warning = true;
                    float range = 1280;
                    float numBlades = 32;
                    float timeBetweenBlades = 5;
                    if (Timer % timeBetweenBlades == 0 && AttackCounter < numBlades)
                    {
                        AttackCounter++;
                        if (MultiplayerHelper.IsHost)
                        {
                            float ratio = AttackCounter / numBlades;

                            float offset = MathHelper.Lerp(range, 0f, ratio);
                            Vector2 pos = NPC.Center;
                            pos.X += offset;
                            pos.Y -= 256;
                            Vector2 vel = Vector2.UnitY * 15;
                            Projectile.NewProjectile(SourceFromThis, pos, vel, ModContent.ProjectileType<MoonBlade>(), Moon_Blade_damage, 1, Main.myPlayer);


                            pos.X = NPC.Center.X - offset;
                            Projectile.NewProjectile(SourceFromThis, pos, vel, ModContent.ProjectileType<MoonBlade>(), Moon_Blade_damage, 1, Main.myPlayer);

                        }
                    }
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (AttackCounter >= numBlades)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ExplodeOut();
                }
                break;
        }
    }

    private void ExplodeOut()
    {
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"), NPC.position);
        }
        if (Timer == 1)
        {

            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
            fx.Scale *= 2f;

            for (float f = 0; f < 4f; f++)
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
                //      sp.behindLayer = true;
                sp.noShrink = true;
                sp.fadeToColor = Color.Black;
                sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
            }

        }


        NPC.velocity *= 0.9f;
        Animator.PlayAnimation(ANIM_EXPLODE);
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Running()
    {
        float circleSize = 1024;
        float dashCount = 14;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsBehindYou(256);
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), NPC.position);
                    }
                    FaceTarget();
                    _warning = true;
                    Animator.PlayAnimation(ANIM_READYDASH);
                    if (Timer % 6 == 0)
                    {
                        Vector2 pos = NPC.Center;
                        float range = Main.rand.NextFloat(128, 196);
                        pos += Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = NPC.Center - pos;
                        vel *= 0.1f;
                        FXUtil.GlowStretch(pos, vel);
                    }

                    float time = 90f;
                    _inwardAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.Clamp(Timer / time));
                    if (Animator.IsFinished() && Timer >= 90f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _contactDamage = true;
                    if (Timer == 1)
                    {
                        _startPosition = NPC.Center;
                        _traveledDistance = 0;
                        SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/VDisappear");
                        inSound.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(inSound, NPC.position);
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);


                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
                        fx.Scale *= 1.8f;
                        float speed = MathHelper.Lerp(10f, 35, EasingFunction.Clamp(AttackCounter / (dashCount / 2f)));
                        _runningVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * speed;

                        FaceTarget();
                    }

                    if (Timer == 4 && MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, ModContent.ProjectileType<StarryMoonTrail>(), 1, 1, Main.myPlayer, ai0: NPC.whoAmI);
                    }

                    if (AttackCounter == 0)
                    {
                        _startVelocity = _runningVelocity.SafeNormalize(Vector2.Zero);

                    }
                    DustTrailSmokeEffects();
                    JapanFootsteps();

                    _runningVelocity *= 1.02f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, _runningVelocity, 0.85f);

                    _attacking = true;
                    _showTrail = true;
                    Animator.PlayAnimation(ANIM_RUN);



                    Vector2 nextPosition = NPC.Center + NPC.velocity;
                    Vector2 edgePosition = _startPosition + _runningVelocity.SafeNormalize(Vector2.Zero) * circleSize;
                    Vector2 dirToEdge = (edgePosition - nextPosition).SafeNormalize(Vector2.Zero);
                    float dp = Vector2.Dot(_runningVelocity.SafeNormalize(Vector2.Zero), dirToEdge);

                    if (dp < 0 && Timer >= 15)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {

                    if (AttackCounter < dashCount)
                    {
                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
                        fx.Scale *= 2;
                        float numDust = 16;
                        for (float n = 0; n < numDust; n++)
                        {
                            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                            spawnParams.outerColor = Color.Blue;
                            var dp = DustParticle.Spawn(NPC.Center, vel, spawnParams);
                            dp.gravity = 0;
                            dp.dampening = 0.05f;
                            dp.noTileCollide = true;
                        }

                        NPC.velocity *= 0.9f;
                        if (MultiplayerHelper.IsHost)
                        {


                            Vector2 pos = MyTarget.Center;

                            float ratio = AttackCounter / dashCount;
                            pos += -_startVelocity.RotatedBy(MathHelper.TwoPi * ratio * 2) * circleSize * 0.65f;
                            Teleport(pos);
                        }

                        Timer = 0;
                        AttackCycle--;
                        AttackCounter++;
                    }
                    else
                    {
                        ExplodeOut();
                    }


                }
                break;

        }
    }

    private void AI_CloneSummon()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veriappear"), NPC.position);
                    }
                    NPC.velocity *= 0.9f;
                    FaceTarget();
                    _warning = true;
                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Timer % 6 == 0)
                    {
                        Vector2 pos = NPC.Center;
                        float range = Main.rand.NextFloat(128, 196);
                        pos += Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = NPC.Center - pos;
                        vel *= 0.1f;
                        FXUtil.GlowStretch(pos, vel);
                    }

                    _showMagicCircle = true;
                    float time = 90f;
                    _inwardAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.Clamp(Timer / time));
                    if (Animator.IsFinished() && Timer >= 90f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
                        fx.Scale *= 2f;


                        for (float f = 0; f < 32f; f++)
                        {
                            Vector2 velocity = Main.rand.NextVector2CircularEdge(18, 18);
                            var dp = DustParticle.Spawn(NPC.Center, velocity);
                            dp.outerColor = Color.Blue;
                            dp.gravity = 0;
                            dp.noTileCollide = true;
                            dp.Scale *= 0.8f;
                            dp.dampening = 0.05f;
                        }

                        _startVelocity = NPC.velocity;
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VeriButterfly"), NPC.position);
                    }
                    _warning = true;
                    if (Timer % 18 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 spawnPos = NPC.Center;
                            NPC.NewNPC(SourceFromThis, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<VerliaClone>(), ai1: 1);
                            // Vector2 forwardVector = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                            // Projectile.NewProjectile(SourceFromThis, NPC.Center, forwardVector, ModContent.ProjectileType<VerliaGreatBlade>(), Great_Blade_Damage, 1, Main.myPlayer, ai2: NPC.whoAmI);
                        }
                    }


                    if (Timer == 60)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"), NPC.position);
                    }
                    // NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_PULSE);
                    if (Animator.IsFinished() && Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void DustTrailEffects()
    {
        if (Timer % 12 == 0)
        {
            var sp = MoonSpiralParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
        if (Timer % 6 == 0)
        {
            var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.flickering = true;
            sp.gravity = 0;
        }
        if (Timer % 4 == 0)
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
            sp.behindLayer = true;
            sp.noShrink = true;
            sp.fadeToColor = Color.Black;
            sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
        }
    }
    private void DustTrailSmokeEffects()
    {
        if (Timer % 12 == 0)
        {
            var sp = MoonSpiralParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
        }
        if (Timer % 6 == 0)
        {
            var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.flickering = true;
            sp.gravity = 0;
        }
    }
    private void ThrowOutStars()
    {
        int max = 3;
        for (int i = 0; i < max; i++)
        {
            if (MultiplayerHelper.IsHost)
            {
                float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                Vector2 x = new Vector2(speedXb, speedXa);
                Projectile.NewProjectile(SourceFromThis, NPC.Center, x, ModContent.ProjectileType<AltideSword>(), 1, 1, Main.myPlayer);
            }
        }
    }
    private void AI_BlueMagicSword()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {

                    if (_magicSwordV2)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    else
                    {
                        _warning = true;
                        TeleportsFarAboveYou();
                    }

                }
                break;
            case 1:
                {

                    _showMagicCircle = true;
                    //CameraTargetSystem.AddTarget(NPC.Center);
                    _warning = true;
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            if (_phase2)
                                ThrowOutStars();
                            Vector2 forwardVector = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                            var proj = Projectile.NewProjectileDirect(SourceFromThis, NPC.Center, forwardVector, ModContent.ProjectileType<VerliaGreatBlade>(), Great_Blade_Damage, 1, Main.myPlayer, ai2: NPC.whoAmI);

                            if (_magicSwordV2)
                            {
                                if (proj.ModProjectile is VerliaGreatBlade greatBlade)
                                {
                                    greatBlade.noBreak = true;
                                    greatBlade.Projectile.netUpdate = true;
                                }
                            }

                        }
                    }
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_SWORD);
                    if (Animator.IsFinished() && Timer >= 240)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                                ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }
                    }
                    _attacking = true;
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_SWORDSLASH);
                    if (_magicSwordV2)
                    {
                        if (Animator.IsFinished())
                        {
                            Timer = 0;
                            AttackCycle += 2;
                        }

                    }
                    else
                    {
                        if (Animator.IsFinished())
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }

                }
                break;
            case 3:
                {
                    ExplodeOut();
                }
                break;
            case 4:
                {
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_IDLESUMMON);
                    if (Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                        //SwitchState(AIState.Idle);
                    }

                }
                break;
            case 5:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_BladeDance()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    TeleportsBehindYou();
                }
                break;
            case 1:
                {
                    CameraTargetSystem.AddTarget(NPC.Center);
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost && _phase2)
                        {
                            ThrowOutStars();
                        }
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;
                        SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/Moaning");
                        inSound.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(inSound, NPC.position);
                    }

                    FaceTarget();


                    float ratio = Timer / 90f;
                    float ease = EasingFunction.InOutExpo(ratio);
                    Vector2 inverseDir = MyTarget.Center.X > NPC.Center.X ? -Vector2.UnitX : Vector2.UnitX;
                    Vector2 targetPosition = MyTarget.Center + inverseDir * 128;
                    Vector2 targetVelocity = targetPosition - NPC.Center;
                    Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    NPC.velocity = interpolatedVelocity;

                    _warning = true;
                    Animator.PlayAnimation(ANIM_SWORD);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        Vector2 dir2 = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                        NPC.velocity = dir2 * 12;
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }

                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                    }
                    if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
                    {
                        SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                        effectsPlayer.darknessCurve = MathHelper.Lerp(0.75f, 0f, EasingFunction.InExpo(Timer / 30f));
                    }
                    NPC.velocity *= 0.94f;

                    _attacking = true;
                    Animator.PlayAnimation(ANIM_SWORDSLASH);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {

                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    float speed = 12;
                    speed = MathHelper.SmoothStep(speed, 2, Timer / 80f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity * speed, 0.5f);
                    FaceTarget();
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                                ModContent.ProjectileType<MoonSlashHold>(), Moon_Slash_Hold_Damage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                        }
                    }
                    if (Timer % 3 == 0)
                    {
                        var sp = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(15, 15));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                        sp.outerColor = Color.Blue;
                        sp.fast = true;
                        sp.gravity = 0;
                        sp.dampening = 0.05f;
                    }
                    if (Timer % 12 == 0)
                    {
                        var sp = MoonSpiralParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                    }
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                        sp.flickering = true;
                        sp.gravity = 0;
                    }
                    if (Timer % 4 == 0)
                    {
                        var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                        sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
                        sp.behindLayer = true;
                        sp.noShrink = true;
                        sp.fadeToColor = Color.Black;
                        sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
                    }


                    _showTrail = true;
                    _attacking = true;


                    Animator.PlayAnimation(ANIM_DOWN);
                    if (Animator.IsFinished())
                    {
                        if (_bladeDanceV2)
                        {
                            _magicSwordV2 = true;
                            SwitchState(AIState.Blue_Magic_Sword);
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 4:
                {
                    _showMagicCircle = true;
                    NPC.velocity *= 0.9f;
                    int style = 1;
                    if (Timer == 45)
                        style = 2;
                    if (Timer == 65)
                        style = 3;
                    if (Timer == 25 || Timer == 45 || Timer == 65)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 pos = NPC.Center;
                            pos.Y -= 32;
                            pos += Main.rand.NextVector2Circular(32, 32);


                            Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Vector2.Zero,
                                ModContent.ProjectileType<MoonShot>(), Moon_Shot_Damage, 0f, Owner: Main.myPlayer,
                            ai1: style);
                        }

                    }
                    _attacking = true;
                    Animator.PlayAnimation(ANIM_PULSE);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_SpinningLittleMoons()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Hyuh"), NPC.position);
                    }

                    _showMagicCircle = true;
                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center);
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    NPC.velocity += targetVelocity * MathHelper.Lerp(0f, 0.33f, Timer / 60f);
                    NPC.velocity *= 0.9f;
                    Animator.PlayAnimation(ANIM_PULSE);

                    _warning = true;
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _showTrail = true;
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                        _startVelocity = NPC.velocity;
                    }
                    DustTrailEffects();

                    if (Timer > 60 && Timer % 25 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 awayFromPlayer = (NPC.Center - MyTarget.Center);
                            awayFromPlayer = awayFromPlayer.SafeNormalize(Vector2.Zero);
                            awayFromPlayer *= 15;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, awayFromPlayer,
                                ModContent.ProjectileType<VerliaMiniMoon>(), Mini_Moon_Damage, 1, Main.myPlayer);
                        }
                    }

                    _attacking = true;

                    float revolutionTime = 180f;
                    float radiansToOffset = MathHelper.TwoPi * 2f / revolutionTime;
                    Vector2 offset = -Vector2.UnitY;
                    offset *= 196;
                    offset = offset.RotatedBy(radiansToOffset * Timer);
                    Vector2 targetPosition = MyTarget.Center + offset;
                    Vector2 targetVelocity = targetPosition - NPC.Center;

                    float ease = EasingFunction.InOutExpo(Timer / 100f);
                    Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    NPC.velocity = interpolatedVelocity;
                    NPC.rotation = NPC.velocity.X * 0.025f;
                    FaceTarget();
                    Animator.PlayAnimation(ANIM_SPIN);
                    if (Timer >= revolutionTime * 2)
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 3:
                {
                    JapanFootsteps();
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/VerliaSONATO"), NPC.position);
                    }
                    _showTrail = true;
                    Animator.PlayAnimation(ANIM_RUN);
                    if (NPC.velocity.X < 15)
                        NPC.velocity.X += 0.5f;
                    else
                        NPC.velocity.X *= 1.01f;
                    NPC.velocity.Y *= 0.9f;
                    NPC.rotation *= 0.9f;
                    if (Timer >= 90)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    ExplodeOut();
                }
                break;
        }
    }
    private void AI_TwoBouncingMoons()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _warning = true;
                    TeleportsFarAboveYou();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Moaning"), NPC.position);
                    }

                    _showMagicCircle = true;
                    NPC.velocity *= 0.9f;
                    NPC.rotation *= 0.9f;

                    Animator.PlayAnimation(ANIM_HOLDUP);
                    if (Timer == 10)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: 1);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<VerliaBouncingMoon>(), Twin_Bouncing_Moon_Damage, 1, Main.myPlayer, ai2: -1);
                        }
                    }

                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    _warning = true;
                }
                break;
            case 2:
                if (Timer == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Huhhuh"), NPC.position);
                    var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
                    fx.Scale *= 2f;
                }

                if (Timer == 60)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VerliaSONATO"));
                }
                _attacking = true;
                Animator.PlayAnimation(ANIM_PULSE);
                if (Animator.IsFinished())
                {
                    Timer = 0;
                    AttackCycle++;
                }
                break;
            case 3:


                ExplodeOut();
                break;
        }
    }
    private void AI_Spawn()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }

    private void AI_IdleOut()
    {
        Timer++;
        FaceTarget();
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/VTeleportOut"), NPC.position);
        }
        if (Timer == 1)
        {

            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightSkyBlue, Color.DarkBlue);
            fx.Scale *= 2f;

            for (float f = 0; f < 4f; f++)
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                sp.Scale *= Main.rand.NextFloat(0.5f, 0.75f);
                //      sp.behindLayer = true;
                sp.noShrink = true;
                sp.fadeToColor = Color.Black;
                sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);
            }

        }


        NPC.velocity *= 0.9f;
        Animator.PlayAnimation(ANIM_EXPLODE);
        if (Animator.IsFinished())
        {
            ChooseAttack();
        }
    }

    private void AI_ReallyIdle()
    {
        Timer++;
        FaceTarget();
        Animator.PlayAnimation(ANIM_IDLESUMMON);

        NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 2f;
        if (Timer >= 100)
        {
            SwitchState(AIState.IdleOut);
        }
    }
    private void AI_Idle()
    {
        Timer++;
        _magicSwordV2 = false;
        switch (AttackCycle)
        {
            case 0:
                {
                    TeleportsAboveYou();
                }
                break;
            case 1:
                {

                    SwitchState(AIState.ReallyIdle);
                }
                break;
        }

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            if (!_dying)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    ScreenShaderSystem tintSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    tintSystem.TintScreen(Color.White, 0.2f, 15);
                    float numDust = 16;
                    for (float f = 0; f < numDust; f++)
                    {
                        Vector2 offset = Main.rand.NextVector2CircularEdge(64, 64);
                        DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                        spawnParams.outerColor = Color.Blue;
                        var dp = DustParticle.Spawn(NPC.Center + offset, offset * 0.3f, spawnParams);
                        dp.gravity = 0;
                        dp.noTileCollide = true;
                        dp.dampening = 0.05f;
                        dp.Scale *= Main.rand.NextFloat(0.5f, 1f);
                    }
                    FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue);
                    PixelPrimitiveCircleFactory.CreateVerliaMoonBoom(NPC.Center);
                    SoundStyle desperationReady = AssetRegistry.Sounds.Bishinine.BishinineFastfall;
                    SoundEngine.PlaySound(desperationReady);
                }
            }
            _dying = true;
            NPC.life = 1;
        }
    }

    #region Drawing


    public const string ANIM_SUMMON = "Summon";
    public const string ANIM_IDLESUMMON = "IdleSummon";
    public const string ANIM_UNSUMMON = "UnSummon";
    public const string ANIM_HOLDUP = "HoldUp";
    public const string ANIM_SWORD = "Sword";
    public const string ANIM_SWORDSLASH = "SwordSlash";
    public const string ANIM_DOWN = "Down";
    public const string ANIM_PULSE = "Pulse";
    public const string ANIM_EXPLODE = "Explode";
    public const string ANIM_TELEPORTIN = "TeleportIn";
    public const string ANIM_RUN = "Run";
    public const string ANIM_READYDASH = "ReadyDash";
    public const string ANIM_DASHFRAME = "DashFrame";
    public const string ANIM_SPIN = "Spin";
    public Animator CreateAnimator()
    {
        var animator = new Animator();
        Vector2 drawOrigin = new Vector2(100, 114);
        var idle = new SpriteAnimation(0, 1, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_SUMMON, idle);

        var swordHold = new SpriteAnimation(0, 4, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_IDLESUMMON, swordHold);

        var handOut = new SpriteAnimation(0, 3, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_UNSUMMON, handOut);

        var lookOver = new SpriteAnimation(0, 7, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_HOLDUP, lookOver);

        var morph = new SpriteAnimation(0, 10, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORD, morph);

        var swimming = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORDSLASH, swimming);

        var battle = new SpriteAnimation(0, 9, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_DOWN, battle);

        var forwardSlash = new SpriteAnimation(0, 21, isLooping: false, drawOrigin, frameSpeed: 0.22f);
        animator.AddAnimation(ANIM_PULSE, forwardSlash);

        var backSlash = new SpriteAnimation(0, 8, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_EXPLODE, backSlash);

        var foundYou = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_TELEPORTIN, foundYou);

        var holding = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.3f);
        animator.AddAnimation(ANIM_RUN, holding);

        var bigSlash = new SpriteAnimation(0, 7, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_READYDASH, bigSlash);

        var running = new SpriteAnimation(0, 0, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        animator.AddAnimation(ANIM_DASHFRAME, running);

        var running2 = new SpriteAnimation(0, 8, isLooping: true, drawOrigin, frameSpeed: 0.35f);
        animator.AddAnimation(ANIM_SPIN, running2);
        return animator;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(184);
        NPC.frame.Height = 184;
        NPC.frame.Width = 266;
    }

    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 16, EasingFunction.QuadraticBump(ratio)) * _trailAlpha;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio) * _trailAlpha;
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, laserShader, NPC.Size * 0.5f);
    }

    private void DrawAfterImage(SpriteBatch spriteBatch)
    {
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Restart(effect: whiteShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];
            Vector2 oldCenter = pos + NPC.Size * 0.5f;
            drawer.worldPosition = oldCenter;
            drawer.color = Color.Lerp(Color.LightBlue, Color.Blue, i / (float)NPC.oldPos.Length);
            drawer.color *= MathHelper.Lerp(1f, 0f, i / (float)NPC.oldPos.Length);
            drawer.color *= _trailAlpha;
            drawer.color *= 0.7f;
            spriteBatch.Draw(drawer);

        }
        spriteBatch.RestartDefaults();
    }
    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor;
        drawer.worldPosition += screenPos;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return false;

        PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.BehindNPCsWithOutline);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, NPC.Center);
        glowDrawer.scale *= 0.35f * _trailAlpha * ExtraMath.Osc(0.66f, 1f, speed: 2);
        glowDrawer.color = Color.LightSkyBlue * _trailAlpha;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        spriteBatch.Draw(glowDrawer);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSpellCircle, DrawLayer.BehindNPCsWithOutline);
        if (_inwardAlpha > 0)
        {
            SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
            sbDrawer.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(_inwardAlpha)) * 0.4f;
            sbDrawer.color.A = 0;
            sbDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, _inwardAlpha) * 3f;
            sbDrawer.rotation = Main.GlobalTimeWrappedHourly;
            spriteBatch.Draw(sbDrawer);
        }
        DrawAfterImage(spriteBatch);
        DrawWings(spriteBatch, screenPos, drawColor);

        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }
    private float HeightOffset
    {
        get
        {
            return 0;
        }
    }

    private float WingOffset
    {
        get
        {
            return -24;
        }
    }

    private float WingVelocity
    {
        get
        {
            return 15;
        }
    }
    private Vector2 LeftWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.6f, EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float LeftWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(12), EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            return rot;
        }
    }
    private Vector2 RightWingScale
    {
        get
        {
            Vector2 leftWingScale = Vector2.One;
            leftWingScale.X = MathHelper.Lerp(1f, 0.6f, EasingFunction.Clamp(NPC.velocity.X / WingVelocity));
            leftWingScale *= _wingScale;
            return leftWingScale;
        }
    }
    private float RightWingRotation
    {
        get
        {
            float rot = MathHelper.Lerp(0, MathHelper.ToRadians(-12), EasingFunction.Clamp(NPC.velocity.X / -WingVelocity));
            return rot;
        }
    }

    private void DrawPixelatedSpellCircle(SpriteBatch sb, Vector2 screenPos)
    {
        DrawSpellCircle(sb);
    }
    private void DrawSpellCircle(SpriteBatch sb)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>(Texture + "_MagicCircle"), NPC.Center);
        drawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 4)) * _magicCircleAlpha;
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly;
        drawer.scale *= 0.8f;
        drawer.scale *= MathHelper.Lerp(1.5f, 1f, _magicCircleAlpha);
        sb.Draw(drawer);
    }
    private void DrawWings_Inner(SpriteBatch spriteBatch)
    {

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.color = Color.DeepSkyBlue * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 0.66f;
        glowDrawer.scale *= 0.6f;
        glowDrawer.scale *= _wingScale;
        glowDrawer.rotation = MathHelper.ToRadians(degrees);

        glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);



        glowDrawer.rotation = MathHelper.ToRadians(-degrees);
        glowDrawer.drawOrigin.X = glowDrawer.texture.Size().X - glowDrawer.drawOrigin.X;
        //    glowDrawer.drawOrigin = new Vector2(AssetManager.GlowMask.SimpleGlowCircle.Width() * 0.2f, AssetManager.GlowMask.SimpleGlowCircle.Height() * 0.5f);
        spriteBatch.Draw(glowDrawer);


        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        // wingDrawer.drawOrigin.X = WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.DarkBlue;

        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.scale = LeftWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);
    }

    private void DrawWings(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _wingTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Wing");
        _wingOutlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_WingOutline");
        _wingTextureAsset2 ??= ModContent.Request<Texture2D>(Texture + "_WingSprite");

        VerlianWingsShader wingShader = VerlianWingsShader.Instance;
        wingShader.BloomColorStart = Color.White;
        wingShader.BloomColorEnd = Color.Lerp(Color.Lerp(Color.Blue, Color.Black, 0.5f), Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 2));
        wingShader.PerlinNoiseTexture = AssetManager.Noise.Whirly.Value;
        wingShader.ScrollingTexture = TrailRegistry.WaterTrail.Value;
        wingShader.DistortionStrength = 0.15f;
        wingShader.MaskSize = _wingTextureAsset.Size();
        wingShader.Frequency = 1f;
        wingShader.Tiling = Vector2.One * 2.5f;
        wingShader.ScrollOffset = new Vector2(-Main.GlobalTimeWrappedHourly * 0.4f, 0.0f);

        //     DrawWings_Inner2(spriteBatch);

        DrawWings_Inner(spriteBatch);
        //return;

        float degrees = -MathHelper.Lerp(8, 15, ExtraMath.Osc(0f, 1f, speed: 3));
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);

        SpritebatchDrawer wingDrawer;

        //Draw main wings
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.5f) * 0.5f;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        wingDrawer.scale = RightWingScale;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.scale = LeftWingScale;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(wingDrawer);

        //Draw stars in wings
        spriteBatch.Restart(effect: wingShader.Effect, sortMode: SpriteSortMode.Immediate);
        wingShader.Tiling = Vector2.One * 16f;
        wingShader.ScrollingTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise2").Value;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.Lerp(Color.White, Color.Black, 0.35f) * 0.6f;
        wingDrawer.color.A = 0;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);



        wingShader.BloomColorEnd = Color.White;
        wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingOutlineTextureAsset, NPC.Center);

        wingDrawer.LeftCenterOrigin();
        wingDrawer.drawOrigin.X += WingOffset;
        wingDrawer.worldPosition.Y -= HeightOffset;
        wingDrawer.color = Color.White;
        wingDrawer.scale = RightWingScale;
        wingDrawer.rotation = MathHelper.ToRadians(degrees) + RightWingRotation;
        spriteBatch.Draw(wingDrawer);

        wingDrawer.rotation = MathHelper.ToRadians(-degrees) + LeftWingRotation;
        wingDrawer.drawOrigin.X = wingDrawer.texture.Size().X - wingDrawer.drawOrigin.X;
        wingDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        wingDrawer.scale = LeftWingScale;
        spriteBatch.Draw(wingDrawer);

        spriteBatch.RestartDefaults();

    }
    #endregion
    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.Verlia);
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
        //      throw new System.NotImplementedException();
    }
}
