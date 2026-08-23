using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;

using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss : ScarletBoss
{
    private Rectangle _lavaArenaRectangle;
    private Vector2 _arenaCenter;
    private Vector2 _teleportPosition;



    private Vector2 _centerPoint;
    private Vector2 _coilStartPoint;
    private Vector2 _targetPoint;
    private Vector2 _initialVelocity;
    private float _afterImageAlpha;
    private bool _showAfterImages;
    private float _mouthAuraAlpha;
    private bool _showMouthAura;
    private float _rekfireballAlpha;
    public class RekSegment
    {
        public RekSegment(Vector2 _position, Vector2 _size, float _scale, int _bodyFrame)
        {
            position = _position;
            size = _size;
            scale = _scale;
            bodyFrame = _bodyFrame;
        }
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 size;
        public Vector2 initialPosition;
        public float scale;
        public float rotation;
        public bool isBurning;
        public bool isBurningNoWarning;
        public bool inLava;
        public bool deadly;
        public bool noWorm;
        public float burnAlpha;
        public int bodyFrame;
        public float sawBladeAlpha;
        public float lastSawBladeAlpha;
    }
    public ChainWithLengths _chain;
    public ChainWithLengths Chain
    {
        get
        {
            if (_chain == null)
            {

                _chain = new(NPC.Center, 80, 39);
                for (int i = 0; i < _chain.lengths.Length; i++)
                {
                    _chain.lengths[i] = MathHelper.Lerp(63, 16, (float)i / _chain.lengths.Length);
                }
            }
            return _chain;
        }
    }

    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,

        //Phase 1 Attacks
        Eruption,
        CoilDash,
        VolcanicMeteor,
        Pacman,
        Ouroboros,

        FireBreath,
        FireBreathV2,

        //Phase 2 Attacks
        Enflame,
        Husk,
        VolcanicSpear,
        BlowtorchBreath,

        Tired,
        Death
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get
        {
            return (AIState)NPC.ai[1];
        }
        set
        {
            NPC.ai[1] = (float)value;
        }
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCount => ref NPC.ai[3];


    public const string ANIM_IDLE = "Idle";
    public const string ANIM_MOUTHOPEN = "MouthOpen";
    public const string ANIM_MOUTHOPEN_HOLD = "MouthOpenHold";
    public const string ANIM_SPEAR_READY = "SpearReady";
    public const string ANIM_SPEAR_HOLD = "SpearHold";
    public const string ANIM_MOUTH_BIG_OPEN = "MouthBigOpen";
    public const string ANIM_MOUTH_BIG_OPEN_READY = "MouthBigOpenReady";
    public const string ANIM_MOUTH_BIG_OPEN_HOLD = "MouthBigOpenHold";
    public const string ANIM_MOUTH_BITE = "Bite";
    public const string ANIM_EYELESS_HUSK = "Husk";

    private Outliner _outliner;

        
    private AseAnimator Animator => this.GetAnimator();


    private int _phase;
    private int _patternIndex;
    private bool _roar;
    private bool _noWorm;
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
    private AIState GetNextAttack(int phase, int patternIndex)
    {
        switch (phase)
        {
            case 0:
                {
                    switch (patternIndex)
                    {
                        case 0:
                            return AIState.Eruption;
                        case 1:
                            return AIState.CoilDash;
                        case 2:
                            return AIState.Ouroboros;
                        case 3:
                            return AIState.Pacman;
                        case 4:
                            return AIState.FireBreath;
                    }
                }
                break;
            case 1:
                {
                    switch (patternIndex)
                    {
                        case 0:
                            return AIState.Enflame;
                        case 1:
                            return AIState.Ouroboros;
                        case 2:
                            return AIState.CoilDash;
                        case 3:
                            return AIState.Tired;
                        case 4:
                            return AIState.VolcanicSpear;
                    }
                }
                break;
            case 2:
                {
                    switch (patternIndex)
                    {
                        case 0:
                            return AIState.Enflame;
                        case 1:
                            return AIState.Eruption;
                        case 2:
                            return AIState.Pacman;
                        case 3:
                            return AIState.Tired;
                        case 4:
                            return AIState.Husk;
                    }
                }
                break;
            case 3:
                {
                    switch (patternIndex)
                    {
                        case 0:
                            return AIState.Enflame;
                        case 1:
                            return AIState.FireBreathV2;
                        case 2:
                            return AIState.Tired;
                        case 3:
                            return AIState.BlowtorchBreath;
                    }
                }
                break;
        }

        return AIState.Idle;
    }

    private Asset<Texture2D>[] _bodySegments;
    private Asset<Texture2D>[] BodySegmentsTextures
    {
        get
        {
            if (_bodySegments == null)
            {
                _bodySegments = new Asset<Texture2D>[8];
                for (int i = 0; i < _bodySegments.Length; i++)
                {
                    _bodySegments[i] = ModContent.Request<Texture2D>(base.Texture + $"_Body_{i}");
                }
            }
            return _bodySegments;
        }
    }

    private Asset<Texture2D> _sawTextureAsset;
    private Asset<Texture2D> SawTextureAsset
    {
        get
        {
            _sawTextureAsset ??= ModContent.Request<Texture2D>(base.Texture + "_Saw");
            return _sawTextureAsset;
        }
    }

    private RekSegment[] _segments;
    public RekSegment[] Segments
    {
        get
        {
            if (_segments == null)
            {
                List<RekSegment> segmentsList = new List<RekSegment>();
                Vector2 startSize = new Vector2(64, 64);
                Vector2 endSize = new Vector2(32, 32);

                var randBigFrame = (float p) => Main.rand.Next(0, 3);
                var randSmallFrame = () => Main.rand.Next(3, 7);
                float numPoints = 36;
                for (float i = 0; i < numPoints; i++)
                {
                    float progress = i / numPoints;
                    int bodyFrame = 0;
                    if (progress > 0.5f)
                    {
                        bodyFrame = randSmallFrame();
                    }
                    else
                    {
                        bodyFrame = randBigFrame(progress);
                    }

                    Vector2 size = Vector2.Lerp(startSize, endSize, i / numPoints);
                    RekSegment segment = new RekSegment(NPC.Center + new Vector2(i * -0.1f, 0f), size, 1, bodyFrame);
                    segmentsList.Add(segment);
                }

                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 5));
                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 6));
                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 7));
                _segments = segmentsList.ToArray();
            }
            return _segments;
        }
    }

    private bool FacingLeft
    {
        get
        {
            return Vector2.Dot(-Vector2.UnitX, NPC.rotation.ToRotationVector2()) > 0;
        }
    }
    private AIState TestAttack => AIState.FireBreathV2;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCSets.UseAseprite[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 80;
        NPC.defense = 10;
        NPC.lifeMax = 5500;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.boss = true;
        NPC.npcSlots = 10f;


        //Setup the music and boss bar
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Rek");
        NPC.aiStyle = -1;
    }



    private void ProduceWaterRipples()
    {
        WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();
        foreach(var segment in Segments)
        {
            // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
            float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
            Vector2 rippleSize = new Vector2(32, 32);
            Vector2 ripplePos = segment.position;

            // WaveData is encoded as a Color.
            Color waveData = new Color(2.5f, waveSine * 0.3f, 0, 1f) * Math.Abs(waveSine);
            shaderData.QueueRipple(ripplePos, waveData, rippleSize, RippleShape.Square, rotation: 0);
        }

    }

    public override void AI()
    {
        base.AI();
        if (_arenaCenter == Vector2.Zero)
        {
            _arenaCenter = TileUtilities.GuessArenaCenter(NPC.Center);
            _lavaArenaRectangle = ArenaRectangleUpToLava();
        }

        if (Main.netMode != NetmodeID.Server)
        {
            ProduceWaterRipples();
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                SwitchState(AIState.Despawn);
            }
        }
        if (_teleportPosition != Vector2.Zero)
        {
            Vector2 diff = _teleportPosition - NPC.Center;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = _teleportPosition;
            }
            NPC.velocity = Vector2.Zero;
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        _outliner.SetDefaults();
        for (int i = 0; i < Segments.Length; i++)
        {
            var segment = Segments[i];
            segment.deadly = false;
            segment.isBurningNoWarning = false;

            //This value you should be set specifically for everything that uses saw visual
            segment.sawBladeAlpha = 0;
        }
        _showAfterImages = false;
        _ouroborosTrail = false;
        _showMouthAura = false;
        _rekfireballAlpha = 0;
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
                break;

            case AIState.Tired:
                AI_Tired();
                break;

            case AIState.Eruption:
                AI_Eruption();
                break;

            case AIState.CoilDash:
                AI_CoilDash();
                break;

            case AIState.VolcanicMeteor:
                AI_VolcanicMeteor();
                break;
            case AIState.Pacman:
                AI_Pacman();
                break;
            case AIState.Ouroboros:
                AI_Ouroboros();
                break;

            case AIState.FireBreath:
                AI_FireBreath();
                break;

            case AIState.FireBreathV2:
                AI_FireBreathV2();
                break;

            case AIState.Enflame:
                AI_Enflame();
                break;

            case AIState.Husk:
                AI_Husk();
                break;

            case AIState.VolcanicSpear:
                AI_VolcanicSpear();
                break;

            case AIState.BlowtorchBreath:
                AI_BlowtorchBreath();
                break;

            case AIState.Death:
                AI_Death();
                break;
        }

        float targetOuroAlpha = _ouroborosTrail ? 1f : 0f;
        _ouroborosAlpha = MathHelper.Lerp(_ouroborosAlpha, targetOuroAlpha, 0.1f);
        if (_showAfterImages)
        {
            _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, 1f, 0.3f);
        }
        else
        {
            _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, 0f, 0.3f);
        }

        if (_showMouthAura)
        {
            _mouthAuraAlpha = MathHelper.Lerp(_mouthAuraAlpha, 1f, 0.03f);
        }
        else
        {
            _mouthAuraAlpha = MathHelper.Lerp(_mouthAuraAlpha, 0f, 0.03f);
        }
        _outliner.Update();
        NPC.spriteDirection = 1;
        if (NPC.velocity.X < 0)
            NPC.direction = -1;
        else
            NPC.direction = 1;


        if (FacingLeft)
        {
            this.SetSpriteEffects(SpriteEffects.FlipVertically);
        }
        else
        {
            this.SetSpriteEffects(SpriteEffects.None);
        }

        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 32; i++)
        {
            Chain.ResolveBackToRoot();
        }


        for (int i = 0; i < Segments.Length; i++)
        {
            var segment = Segments[i];
            segment.burnAlpha = MathHelper.Lerp(segment.burnAlpha, (segment.isBurning || segment.isBurningNoWarning) ? 1f : 0f, 0.05f);
            if(segment.lastSawBladeAlpha < 0.5f && segment.sawBladeAlpha > 0.5f)
            {

                for(int k = 0; k < 4; k++)
                {
                    float upRotation = segment.rotation + MathHelper.PiOver2;
                    Vector2 upVec = upRotation.ToRotationVector2();
                    upVec *= Main.rand.NextFloat(5f, 15f);
                    upVec = upVec.RotatedByRandom(0.4f);
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                    {
                        position = segment.position,
                        velocity = upVec,
                        timeLeft = 45,
                        innerColor = Color.Yellow.ToVector4(),
                        outerColor = Color.Red.ToVector4()
                    });
                }
            }
            segment.lastSawBladeAlpha = segment.sawBladeAlpha;
            if ((segment.isBurning || segment.isBurningNoWarning) && Main.rand.NextBool(32))
            {
                Dust.NewDustPerfect(segment.position + Main.rand.NextVector2Circular(48, 48), DustID.Torch, -Vector2.UnitY, Scale: 2f);
            }
        }

        for (int i = 0; i < Segments.Length; i++)
        {
            Segments[i].isBurning = false;
        }

        for (int i = Segments.Length - 1; i >= 0; i--)
        {
            var segment = Segments[i];
            if (!segment.noWorm)
            {
                segment.position = Chain.points[i];
                if (i == 0)
                {
                    segment.rotation = NPC.rotation;
                }
                else
                {
                    segment.rotation = (Chain.points[i] - Chain.points[i - 1]).ToRotation();

                }
            }

        }
    }




    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCount = 0;
            NPC.netUpdate = true;
        }
    }

    private void Teleport(Vector2 teleportPos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = teleportPos;
            NPC.netUpdate = true;
        }
    }





    private void AI_Despawn()
    {
        Timer++;
        if (Timer >= 120)
        {
            NPC.active = false;
        }
        NPC.velocity.Y -= 0.5f;
        NPC.velocity.X *= 0.98f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
    }

    public override bool AllowNameplateToBeShown()
    {
        return _roar;
    }

    private void NextState()
    {
        if (TestAttack != default)
        {
            SwitchState(TestAttack);
            return;
        }
        if (MultiplayerHelper.IsHost)
        {
            AIState state = GetNextAttack(_phase, _patternIndex);
            _patternIndex++;
            SwitchState(state);
        }
    }


    private void AI_Idle()
    {
        _patternIndex = 0;
        ResetLavaSegments();
        Timer++;
        if (Timer >= 20)
        {
            NextState();
        }
    }

    private void AI_Tired()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    public override void OnKill()
    {
        base.OnKill();
    }

}
