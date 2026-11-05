using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;


public class LineRig
{
    public struct LineSegment
    {
        public Vector2 position;

        //Remember 0 is right
        public float angleBehind;
        public float length;
        public int frameIndex;
    }


    public LineRig(int numPoints)
    {
        segments = new LineSegment[numPoints];
    }

    public LineSegment[] segments;
    public void Resolve(Vector2 rootPosition, float rootAngle)
    {
        //0 is the head
        segments[0].position = rootPosition;
        segments[0].angleBehind = rootAngle;

        for(var i = 1; i < segments.Length; i++)
        {
            ref var prevSegment = ref segments[i - 1];
            ref var curSegment = ref segments[i];
            curSegment.position = (prevSegment.position - prevSegment.angleBehind.ToRotationVector2() * curSegment.length);
        }
    }
}
public partial class Niivi : ScarletBoss,
    IDrawToRenderTarget
{

    float _crystalCharge;
    bool _useFK;
    float _pattern;
    bool _blend;
    bool _isAfterImaging;
    bool _isTrailing;
    bool _isArmorAura;
    bool _isBLACK;
    bool _isPrismatic;
    float _invisAlpha;
    float _prismaticAlpha;
    float _afAlpha;
    float _trailingAlpha;
    float _armorAlpha;
    float _blackAlpha;
    bool _simpleChainRendering;
    Vector2 _arenaCenter;


    Vector2 _iceCrystalPosition;
    Vector2 _starCrystalPosition;
    Vector2 _lightningCrystalPosition;

    Vector2 _vec1;
    Vector2 _vec2;
    Vector2 _vec3;
    Vector2 _teleportPosition;
    Outliner _outliner;
    Vector2[] SegmentPositions => field ??= new Vector2[SegmentCount];
    Vector2[] SegmentOrigins = new Vector2[]
    {
        //Head
        new(110, 48),
        
        //neck
        new(88, 58),

        //right after neck
        new(88, 58),

        new(88, 58),

        new(88, 58),

        new(94, 61),

        new(96, 61),
            
        new(109, 51),

        new(98, 61),

        new(98, 61),

        new(97, 57),

        new(88, 58),
         new(88, 58),
    };

    enum AIState : byte
    {
        Spawn,
        Idle,
        Despawn,
        FlyAroundPlayer,

        //Phase 1
        //Niivi gets attack variations in phase 2
        FrostBreath,
        StarBomb,
        BlastBreath,
        PingPong,
        AsrielLightning,
        PhaseTransition,

        //SPECIALS
        PrismBomb,
        SUPERPRISMATICRAY,
        VoidSlash,
        Death,
    }
    int _wingFrame;
    float _wingFrameCounter;
    float _arenaY;
    int SegmentCount => 12;
    Vector2 _ikTarget;
    Vector2 _ikInitialDirection;
    float[] WantedAngles => field ??= new float[SegmentCount];
    float[] CurrentAngles => field ??= new float[SegmentCount];
    float[] Sizes
    {
        get
        {
            field ??= new float[13];
            field[0] = 88;
            field[1] = 26;
            field[2] = 26;
            field[3] = 26;
            field[4] = 26;
            field[5] = 20;
            field[6] = 20;
            field[11] = 26;
            field[12] = 26;
            return field;
        }
    }
    int ArmSegmentIndex => 2;
    int LegSegmentIndex => 7;
    int WingSegmentIndex => 2;
    Vector2 FrontWingDrawOrigin => new Vector2(229, 194);
    Vector2 BackWingDrawOrigin => new Vector2(234, 207);
    float[] SegmentRotations => field ??= new float[SegmentCount];
    float[] SegmentBaseScales
    {
        get
        {
            if (field == null)
            {
                field = new float[SegmentCount];
               for(var i = 0; i < SegmentCount; i++)
                {
                    ref var scale = ref field[i];

                    scale = MathHelper.Lerp(0.8F, 1.1F, EasingFunction.QuadraticBump((float)i / (float)SegmentCount));
                    if (i == 0)
                        scale = 1f;
                }
            }
            return field;
        }
    }
    int[] SegmentFrameIndices
    {
        get
        {

            if(field == null)
            {
                var v = 0;
                field = new int[SegmentCount];
                field[0] = 0;
                for (var i = 1; i < 3; i++)
                {
                    field[i] = 1;
                }
                field[3] = 2;
                field[4] = 11;
                field[5] = 2;
                field[6] = 11;
                field[7] = 3;
                field[8] = 4;
                field[9] = 4;

                field[10] = 12;
        
                field[11] = 5;
                field[SegmentCount - 1] = 6;
            }

            return field;
        }
    }
    LineRig ForwardKinematicsRig
    {
        get
        {
            if (field == null)
            {
                field = new(SegmentCount);
                for (var i = 0; i < field.segments.Length; i++)
                {
                    ref var segment = ref field.segments[i];
                    segment.position = new Vector2(i * -16, 0);
                    segment.angleBehind = 0;
                    segment.length = Sizes[SegmentFrameIndices[i]] * SegmentBaseScales[i];
                }
            }
            return field;
        }
    }
    ChainWithLengths Chain
    {
        get
        {
            if (field == null)
            {
                field = new ChainWithLengths(NPC.Center, 16, SegmentCount);
                for (var i = 0; i < field.points.Length; i++)
                {
                    ref var position = ref field.points[i];
                    ref var length = ref field.lengths[i];
                    position = new Vector2(i * 64, 0);
                    length = Sizes[SegmentFrameIndices[i]] * SegmentBaseScales[i];
                }
            }

            return field;

        }
    }
    FabrikSegment[] Segments
    {
        get
        {
            if(field == null)
            {
                field = new FabrikSegment[SegmentCount];
                for (var i = 0; i < field.Length; i++)
                {
                    ref var segment = ref field[i];
                    segment.position = new Vector2(i * 64, 0);
                    segment.length = Sizes[SegmentFrameIndices[i]] * SegmentBaseScales[i];
                }
            }

     
            return field;
        }
        set
        {
            field = value;
        }
    }

    PatternManager<AIState> RegularAttacks
    {
        get
        {
            if(field == null)
            {
                field = new();
                field.AddPattern(AIState.FrostBreath, 1f);
                field.AddPattern(AIState.StarBomb, 1f);
                field.AddPattern(AIState.BlastBreath, 1f);
              //  field.AddPattern(AIState.PingPong, 1f);
            }
            return field;
        }
    }

    PatternManager<AIState> SpecialAttacks
    {
        get
        {
            if (field == null)
            {
                field = new();
            //    field.AddPattern(AIState.SUPERPRISMATICRAY, 1f);
            //    field.AddPattern(AIState.VoidSlash, 1f);
                field.AddPattern(AIState.PrismBomb, 1f);
            }
            return field;
        }
    }

    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    ref float AttackCount => ref NPC.ai[3];
    float SpawnTime => 60;
    float DespawnTime => 60;

    int WingFrameCount => 9;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.Niivi.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_vec1);
        writer.WriteVector2(_vec2);
        writer.WriteVector2(_vec3);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_arenaCenter);
        writer.WriteVector2(_iceCrystalPosition);
        writer.WriteVector2(_lightningCrystalPosition);
        writer.WriteVector2(_starCrystalPosition);
        writer.Write(_arenaY);
    }
    
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _vec1 = reader.ReadVector2();
        _vec2 = reader.ReadVector2();
        _vec3 = reader.ReadVector2();
        _teleportPosition = reader.ReadVector2();
        _arenaCenter = reader.ReadVector2();
        _iceCrystalPosition = reader.ReadVector2();
        _lightningCrystalPosition = reader.ReadVector2();
        _starCrystalPosition = reader.ReadVector2();
   
        _arenaY = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        //We can still keep fair hitbox sizes though
        NPC.width = NPC.height = 128;

        //We have to upscale this boss cause he's really hard and you're not really supposed to be him lol
        NPC.damage = 1;
        NPC.defense = 54;
        NPC.lifeMax = 200_000;

        NPC.scale = 1f;
        NPC.aiStyle = -1;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Niivi");
        }
    }

    public override void AI()
    {
        base.AI();
        NPC.TargetClosest();
     
        if (_arenaY == 0)
        {
            _arenaCenter = NPC.Center;
            NPC.TargetClosest();
            _arenaY = MyTarget.Top.Y;
            NPC.netUpdate = true;
        }

        DomainExpansionManager.UseDomain(new DomainParameters
        {
            noWings = true,
            inSpace = true,
            hoveringPlatform = true,
            hoverPlatformY = _arenaY,
            noRender = true
        });

        if(_teleportPosition != Vector2.Zero)
        {
            NPC.velocity = Vector2.Zero;
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        _simpleChainRendering = false;

        NiiviRoamingHelper.LastKnownNiiviLocation = NPC.Center;
        Lighting.AddLight(NPC.Center, new Vector3(0.8f));
        _outliner.SetDefaults();
        _isAfterImaging = false;
        _isArmorAura = false;
        _isTrailing = false;
        _isBLACK = false;
        _isPrismatic = false;
        _useFK = false;
        _invisAlpha = MathHelper.Lerp(_invisAlpha, 1f, 0.03f);
        _crystalRotationRadians += 0.015f;
        _crystalCharge = MathHelper.Lerp(_crystalCharge, 0f, 0.03f);
        switch (State)
        {
            case AIState.AsrielLightning:
                AI_AsrielLightning();
                break;
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.FrostBreath:
                AI_FrostBreath();
                break;
            case AIState.PrismBomb:
                AI_PrismaticBomb();
                break;
            case AIState.StarBomb:
                AI_StarBomb();
                break;
            case AIState.BlastBreath:
                AI_BlastBreath();
                break;
            case AIState.PingPong:
                AI_PingPong();
                break;
        }
        RotateCrystals(NPC.Center, 100);
        _afAlpha = MathHelper.Lerp(_afAlpha, _isAfterImaging ? 1f : 0f, 0.1f);
        _armorAlpha = MathHelper.Lerp(_armorAlpha, _isArmorAura ? 1f : 0f, 0.1f);
        _trailingAlpha = MathHelper.Lerp(_trailingAlpha, _isTrailing ? 1f : 0f, 0.1f);
        _blackAlpha = MathHelper.Lerp(_blackAlpha, _isBLACK ? 1f : 0f, 0.1f);
        _prismaticAlpha = MathHelper.Lerp(_prismaticAlpha, _isPrismatic ? 1f : 0f, 0.1f);
    }

    void ChooseAttack()
    {
        return;

        if(_pattern >= 3)
        {
            _pattern = 0;
            SwitchState(SpecialAttacks.NextPattern());
        }
        else
        {
            SwitchState(RegularAttacks.NextPattern());
            _pattern++;
        }
        //        SwitchState(SpecialAttaa cks.NextPattern());
        SwitchState(AIState.StarBomb);
    }

    void ResetSegments()
    {
        var segments = Segments;
        for (var i = 0; i < segments.Length; i++)
        {
            ref var segment = ref segments[i];
            segment.position = NPC.Center + _ikInitialDirection * 32;
        }
        Segments = segments;
    }
    public override void PostAI()
    {
        base.PostAI();
        _outliner.Update();


        //Test
    
        var segments = Segments;
        FabrikSolver.Solve(ref segments, NPC.Center, _ikTarget, 4);

        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for(var i = 0; i < 16; i++)
        {
            Chain.ResolveBackToRoot();
        }

        for(var i = 0; i < segments.Length; i++)
        {
            ref var segment = ref segments[i];
            Lighting.AddLight(segment.position, new Vector3(1f));
        }
        if (_useFK)
        {
            ForwardKinematicsRig.Resolve(NPC.Center, 0);
            for(var i = 0; i < SegmentPositions.Length; i++)
            {
                ref var pos = ref SegmentPositions[i];
                pos = ForwardKinematicsRig.segments[i].position;
            }
        } 
        else if (_simpleChainRendering)
        {
            for (var i = 0; i < SegmentPositions.Length; i++)
            {
                ref var pos = ref SegmentPositions[i];
                pos = Chain.points[i];
            }
        }
        else
        {
            for (var i = 0; i < SegmentPositions.Length; i++)
            {
                ref var pos = ref SegmentPositions[i];
                pos = Segments[i].position;
            }
        }
        //After setting the position, calculate the angles between
        for(var i = 1; i < SegmentPositions.Length; i++)
        {
            WantedAngles[i] = (SegmentPositions[i] - SegmentPositions[i - 1]).ToRotation();
        }
        for (var i = 1; i < CurrentAngles.Length; i++)
        {
            ref var currentAngle = ref CurrentAngles[i];
            ref var wantedAngle = ref WantedAngles[i];
            if(NPC.direction == -1 && _useFK)
                wantedAngle = MathHelper.Pi  - wantedAngle;
            currentAngle = Utils.AngleLerp(currentAngle, wantedAngle, 0.1f);
        }

        Vector2 current = NPC.Center;
        for(var i = 1; i < CurrentAngles.Length; i++)
        {
            current += CurrentAngles[i].ToRotationVector2() * ForwardKinematicsRig.segments[i].length;
            SegmentPositions[i] = current;
        }

        _wingFrameCounter++;
        if(_wingFrameCounter >= 4)
        {
            _wingFrameCounter = 0;
            _wingFrame++;
            _wingFrame %= WingFrameCount;
        }
        Segments = segments;
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCycle = 0;
            AttackCount = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    public override bool AllowNameplateToBeShown()
    {
        return State == AIState.Idle;
    }

    void AI_Spawn()
    {
        Timer++;
        if(Timer >= SpawnTime)
        {
            SwitchState(AIState.Idle);
        }
    }
    void AI_Despawn()
    {
        Timer++;
        if(Timer >= DespawnTime)
        {
            NPC.active = false;
        }
    }
 

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(NPC.life <= 0)
        {
            if(State != AIState.Death)
            {
                SwitchState(AIState.Death);
            }
            NPC.life = 1;
        }
    }
}
