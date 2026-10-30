using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi : ScarletBoss,
    IDrawToRenderTarget
{
    Vector2 _vec1;
    Vector2 _vec2;
    Vector2 _vec3;
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
    }
    int BodyEndIndex => 5;
    float _arenaY;
    Chain LimpChain => field ??= new Chain(NPC.Center, 80, 16);
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
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.Niivi.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_vec1);
        writer.WriteVector2(_vec2);
        writer.WriteVector2(_vec3);
        writer.Write(_arenaY);
    }
    
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _vec1 = reader.ReadVector2();
        _vec2 = reader.ReadVector2();
        _vec3 = reader.ReadVector2();
        _arenaY = reader.ReadSingle();
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
            noRender = false
        });


        NiiviRoamingHelper.LastKnownNiiviLocation = NPC.Center;
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
            case AIState.PrismBomb:
                AI_PrismaticBomb();
                break;
        }
    }
    void TryLookAtTarget()
    {

    }

    void TryLookAtBomb()
    {

    }
    public override void PostAI()
    {
        base.PostAI();
        _outliner.Update();
        LimpChain.points[0] = NPC.Center;
        LimpChain.pinned[0] = true;
        for (int i = 0; i < 32; i++)
        {
            LimpChain.ResolveBackToRoot();
        }
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
            SwitchState(AIState.PrismBomb);
        }
    }
    void DrawBody(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        //Draw tail first, then all the way up to head
        for(var i = BodyEndIndex; i >= 0; i--)
        {
            ref var point = ref LimpChain.points[i];

            var drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Npc[Type], point);
            drawer.drawOrigin = SegmentOrigins[i];
            if(i > 0)
            {
                ref var nextPoint = ref LimpChain.points[i - 1];
                drawer.rotation = (nextPoint - point).ToRotation();
            }
            drawer.VerticalFrame(i, 10);
            sb.Draw(drawer);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBody(spriteBatch, screenPos, drawColor);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public void DrawToRenderTargets()
    {
    }
}
