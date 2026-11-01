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
    bool _simpleChainRendering;
    Vector2 _vec1;
    Vector2 _vec2;
    Vector2 _vec3;
    Vector2 _teleportPosition;
    Outliner _outliner;
    Vector2[] SegmentOrigins = new Vector2[]
    {
        //Head
        new(76, 56),
        
        //neck
        new(42, 60),

        //right after neck
        new(42, 60),

        new(48, 60),

        new(52, 60),

        new(52, 60),

        new(61, 56),
            
        new(58, 62),

        new(58, 62),

        new(57, 59),
    };

    enum AIState : byte
    {
        Spawn,
        Idle,
        Despawn,
        FlyAroundPlayer,
        PrismBomb,
        FrostBreath,
        StarBomb,
        PrismaticRay,
        Death,
    }
    int _wingFrame;
    float _wingFrameCounter;
    int BodyEndIndex => 5;
    float _arenaY;
    int SegmentCount => 16;
    Vector2 _ikTarget;
    Vector2 _ikInitialDirection;
    float[] Sizes
    {
        get
        {
            field ??= new float[10];
            field[0] = 88;
            field[1] = 26;
            field[2] = 26;
            field[3] = 26;
            field[4] = 20;
            field[5] = 20;
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
                    scale = MathHelper.Lerp(1f, 1.1f, EasingFunction.QuadraticBump((float)i / (float)SegmentCount));
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
                field = new int[SegmentCount];
                field[0] = 0;
                for (var i = 1; i < 3; i++)
                {
                    field[i] = 1;
                }
                for (var i = 3; i < SegmentCount; i++)
                {
                    field[i] = (int)MathHelper.Lerp(2, 5, (float)i / (float)SegmentCount);
                }
                field[SegmentCount - 1] = 5;
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

    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float SpawnTime => 60;
    float DespawnTime => 60;
    float IdleTime => 120;
    int WingFrameCount => 9;
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.Niivi.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_vec1);
        writer.WriteVector2(_vec2);
        writer.WriteVector2(_vec3);
        writer.WriteVector2(_teleportPosition);
        writer.Write(_arenaY);
    }
    
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _vec1 = reader.ReadVector2();
        _vec2 = reader.ReadVector2();
        _vec3 = reader.ReadVector2();
        _teleportPosition = reader.ReadVector2();
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
        NPC.defense = 48;
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
        switch (State)
        {
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
        }
    }
    void TryLookAtTarget()
    {

    }

    void TryLookAtBomb()
    {

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
    void AI_Idle()
    {
        Timer++;
        var vel = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
        NPC.velocity = vel;
        if(Timer >= IdleTime)
        {
            SwitchState(AIState.FrostBreath);
        }
        NPC.rotation = (MyTarget.Center - NPC.Center).ToRotation();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }


}
