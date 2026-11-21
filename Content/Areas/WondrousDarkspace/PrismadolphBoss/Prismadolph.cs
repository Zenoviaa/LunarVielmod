using ReLogic.Content;
using Stellamod.Common.Particles;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Rendering;
using Stellamod.Core.Rendering.RTs;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;


public class Prismadolph : ScarletBoss
{
    public const int FRAME_HEAD = 0;
    public const int FRAME_HEAD_GLOW = 1;
    public const int FRAME_HEAD_WHITE = 2;
    public const int FRAME_ARMOR = 3;
    public const int FRAME_TAIL = 4;
    public const int FRAME_TAIL_GLOW = 5;
    public const int FRAME_TAIL_WHITE = 6;
    public const int FRAME_CORE = 7;
    public const int FRAME_CORE_WHITE = 8;
    public const int FRAME_CORE_PURPLE = 9;
    enum AIState : byte
    {
        Lurk,
        ForegroundLurk,


        Circle,
        WanderOff,
        Death
    }
    List<PrismadolphSegment> Segments => field ??= new();
    bool _spawnSegments;
    public ChainWithLengths Chain
    {
        get
        {
            if (field == null)
            {
                field = new(NPC.Center, 80, 48);
            }
            return field;
        }
    }

    Vector2 _lurkDirection;
    float RotOffset => -MathHelper.PiOver2;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float MovementCycle => ref NPC.ai[2];
    ref float MovementTimer => ref NPC.ai[3];
    public float deathProgress;
    float DeathTime => 360F;

    public float alpha;
    public float aggression;
    public float TopPatrolRange => SavedGenerationParameters.DarkspaceTop * 16 - 100;
    public float BottomPatrolRange => TopPatrolRange + 1200;
    public override string Texture => AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph.KEY;
    public override Asset<Texture2D> BossIconTextureAsset => AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph_BossIcon.Asset;
    public override Asset<Texture2D> BossBarTextureAsset => AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph_BossBar.Asset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCSets.Heavy[Type] = true;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(aggression);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        aggression = reader.ReadSingle();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override bool CanHitNPC(NPC target)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 80;
        NPC.defense = 97;
        NPC.lifeMax = 44_000;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.boss = false;
        NPC.npcSlots = 10f;
        NPC.aiStyle = -1;
        NPC.behindTiles = true;
        if (!Main.dedServ)
            Music = AssetReferences.Assets.Music.DeadlyFoe.Slot;
    }
    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }
    public override bool AllowNameplateToBeShown()
    {
        return base.AllowNameplateToBeShown() && NPC.boss;
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();

        //Spawn 
        if (MultiplayerHelper.IsHost && !_spawnSegments)
        {
            var type = ModContent.NPCType<PrismadolphSegment>();

            for (var i = 0; i < Chain.points.Length; i++)
            {
                ref var point = ref Chain.points[i];
                var ratio = (float)i / (float)Chain.points.Length;
                point -= PrismadolphRoamingSystem.RoamingDirection * ratio * 3096;
                var frame = 0;

                if (i == 0)
                    frame = 0;
                else if (i == Chain.points.Length - 1)
                    frame = 2;
                else
                    frame = 1;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)point.X, (int)point.Y, type, ai0: NPC.whoAmI, ai2: i, ai3: frame);
            }

            _spawnSegments = true;
        }

        if(Main.netMode != NetmodeID.Server)
        {
            var p = Main.LocalPlayer.GetModPlayer<PrismadolphCrawlPlayer>();
            var v = new Vector2(999999);
            var d = 9999999f;
            for(var i = 0; i < Chain.points.Length; i++)
            {
                ref var pos = ref Chain.points[i];
                var d2 = Vector2.DistanceSquared(pos, p.Player.Center);
                if(d2 < d)
                {
                    v = pos;
                    d = d2;
                }
            }
            p.closestToPlayer = v;
        }
        PrismadolphRoamingSystem.RoamingPosition = NPC.Center;

        if (NPC.life >= NPC.lifeMax)
            NPC.life = NPC.lifeMax;
        Segments.Clear();
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.ModNPC is PrismadolphSegment segment && npc.ai[0] == NPC.whoAmI)
            {
                Segments.Add(segment);
            }
        }
        if (NPC.life <= 1 && State != AIState.Death)
            SwitchState(AIState.Death);

        switch (State)
        {
            case AIState.ForegroundLurk:
                AI_ForegroundLurk();
                break;
            case AIState.Lurk:
                AI_Lurk();
                break;
            case AIState.Circle:
                AI_Circle();
                break;
            case AIState.WanderOff:
                AI_WanderOff();
                break;
            case AIState.Death:
                AI_Death();
                break;
        }
    }

    public override void PostAI()
    {
        base.PostAI();
        NPC.rotation = NPC.velocity.ToRotation();
        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 54; i++)
        {
            Chain.ResolveBackToRoot();
        }
    }
    void AI_Death()
    {
        Timer++;

        if(NPC.velocity.Length() > 0.03f)
            NPC.velocity *= 0.98F;
        deathProgress = Timer / DeathTime;
        if(Timer >= DeathTime)
        {
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 25, 0.18F);
            fx.Scale *= 1.7F;

            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.White, 60, 2048);
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.White, 45, 1024);
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.White, 35, 512);

            //I love this little dust
            for (var i = 0; i < 64; i++)
            {
                var ratio = Main.rand.NextFloat();
                var pos = NPC.Center;
                var vel = Main.rand.NextVector2Circular(24, 24);
                PS.GlowDust.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    color = NextRandomDustColor() with { A = 0 },
                    timeLeft = Main.rand.Next(60, 120),
                    scale = Main.rand.NextFloat(0.6f, 1f) * 0.2f,
                });
            }
            NPC.Kill();
        }
    }
    Color NextRandomDustColor()
    {
        Color dustColor = Color.DeepSkyBlue;
        dustColor.ScrollHue(Main.rand.NextFloat(-24f, 24f));
        return dustColor;
    }

    void CheckDspawn()
    {
        bool farEnough = true;
        foreach (var player in Main.ActivePlayers)
        {
            var sqd = Vector2.DistanceSquared(NPC.Center, player.Center);
            if (sqd < 8000 * 8000)
                farEnough = false;
        }
        if (farEnough)
            SwitchState(AIState.WanderOff);
    }

    void AI_ForegroundLurk()
    {

        alpha = MathHelper.Lerp(0f, 1f, Timer / 30F);
        NPC.TargetClosest();
        NPC.velocity = PrismadolphRoamingSystem.RoamingDirection * 55;
        NPC.velocity.Y += MathF.Sin(Timer * 0.05f) * 1;
        if (Vector2.DistanceSquared(NPC.PlayerImTargeting.Center, NPC.Center) > 5000 * 5000) 
        {
            
            SwitchState(AIState.Lurk);
            
        }
    }
    float To(float start, float end)
    {
        if (end < start)
            return -1;
        else
            return 1;
    }
    float Dist(float start, float end)
    {
        return MathF.Abs(end - start);
    }
    void AI_Lurk()
    {
        alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
        if (_lurkDirection == default)
            _lurkDirection = PrismadolphRoamingSystem.RoamingDirection;
        Timer += 0.003f;
        MovementTimer++;
        NPC.TargetClosest();
        Vector2 targetVelocity = Vector2.Zero;

        if(aggression >= 300)
        {
            MovementTimer = 0;
            aggression = 0;
            MovementCycle = 2;
        }

        //Move up and down in a waving pattern
        switch (MovementCycle)
        {
            case 0:
                {
                    targetVelocity = _lurkDirection * 1.4F;
                    targetVelocity.Y = To(NPC.Center.Y, TopPatrolRange) * 3;
                    if(Dist(NPC.Center.Y, TopPatrolRange) <= 50)
                    {
                        MovementCycle = 1;
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.02f);
                }
                break;
            case 1:
                {
                    targetVelocity = _lurkDirection * 1.4F;
                    targetVelocity.Y = To(NPC.Center.Y, BottomPatrolRange) * 3;
                    if (Dist(NPC.Center.Y, BottomPatrolRange) <= 50)
                    {
                        MovementCycle = 0;
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.02f);
                }
                break;
            case 2:
                {
                    var velToTarget = ProjectileHelper.SimpleHomingVelocity(NPC.Center, MyTarget.Center, NPC.velocity, 0.8F);

                    NPC.velocity = Vector2.Lerp(NPC.velocity, velToTarget, 1f);
                    if (MovementTimer >= 600)
                    {
                        MovementTimer = 0;
                        MovementCycle = 0;
                    }
                }
                break;
        }


        CheckDspawn();
    }

    void AI_Circle()
    {
        Timer++;
        //Find a player target, closest to them I guess.
        //From that player target get a point around them that's a circ
        var target = NPC.PlayerImTargeting;
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if(!NPC.HasValidTarget)
                SwitchState(AIState.WanderOff);
        }
        var osc = MathHelper.Lerp(3, 12, MathF.Sin(Timer * 0.04f) * 0.5F + 0.5F);
        Vector2 posToMoveTo = MyTarget.Center;
        Vector2 velToMoveTo = ProjectileHelper.SimpleHomingVelocity(NPC.Center, posToMoveTo, NPC.velocity.Resize(9), osc);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToMoveTo, 0.2f);
        /*
        Vector2 offset = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
        offset = offset.RotatedBy(0.05F);
        Vector2 posToMoveTo = MyTarget.Center + offset * CircleOffset;
        Vector2 velToMoveTo = (posToMoveTo - NPC.Center).Resize(6);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToMoveTo, 0.2f);*/
        CheckDspawn();
    }

    void AI_WanderOff()
    {
        Timer++;
        //Since bro is in the darkspace gonna make this work interestingly
        Vector2 posToMoveTo = PrismadolphRoamingSystem.RoamingPosition;
        Vector2 velToMoveTo = (posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToMoveTo, 0.2f);


        if(Timer >= 180)
        {
            bool farEnough = true;
            foreach (var player in Main.ActivePlayers)
            {
                var sqd = Vector2.DistanceSquared(NPC.Center, player.Center);
                if (sqd < 2048 * 2048)
                    farEnough = false;
            }
            if (farEnough)
            {
                NPC.active = false;
                PrismadolphRoamingSystem.CanDoJumpscare = true;
            }
            else
                SwitchState(AIState.Lurk);
        }
    }


    void DrawUnderbody(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //loop over all the segments and everything that's underneath
        for (var i = 0; i < Segments.Count; i++)
        {
            var segment = Segments[i];
            var pos = Chain.points[i];
            float rot = 0F;
            if (i < Segments.Count - 1)
            {
                var nextPos = Chain.points[i + 1];
                rot = (nextPos - pos).ToRotation();
            }
            else if(i > 0)
            {
                var nextPos = Chain.points[i - 1];
                rot = (nextPos - pos).ToRotation();
            }
            rot += RotOffset;
            segment.DrawJigglyCore(spriteBatch, screenPos, drawColor, rot);
        }
        for (var i = 0; i < Segments.Count; i++)
        {
            var segment = Segments[i];
            var pos = Chain.points[i];
            float rot = 0F;
            if (i < Segments.Count - 1)
            {
                var nextPos = Chain.points[i + 1];
                rot = (pos - nextPos).ToRotation();
            }
            else if(i > 0)
            {
                var nextPos = Chain.points[i - 1];
                rot = (nextPos - pos).ToRotation();
            }
            rot += RotOffset;
            segment.DrawJigglyCoreBloom(spriteBatch, screenPos, drawColor, rot);
        }
    }
    void DrawArmor(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for (var i = 0; i < Segments.Count; i++)
        {
            var segment = Segments[i];
            var pos = Chain.points[i];
            float rot = 0F;
            if (i < Segments.Count - 1)
            {
                var nextPos = Chain.points[i + 1];
                rot = (pos - nextPos).ToRotation();
            }
            else if(i > 0)
            {
                var nextPos = Chain.points[i - 1];
                rot = (nextPos - pos).ToRotation();
            }
            rot += RotOffset;
            segment.DrawArmor(spriteBatch, screenPos, drawColor, rot);
        }
    }

    void DrawEntireSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawUnderbody(spriteBatch, screenPos, drawColor);
        DrawArmor(spriteBatch, screenPos, drawColor);

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawLayerHooks.OverWaterDrawActions.Enqueue((SpriteBatch sb) =>
        {
            sb.EndOut(out var oldParameters);
            using var st = RT.Context(RenderTargets.ScreenTarget);


            using (RT.Clear(st, Color.Transparent))
            {
                sb.Begin(oldParameters with { matrix = Matrix.Identity });
                DrawEntireSprite(sb, screenPos, drawColor);
                sb.End();
            }


            sb.Begin(oldParameters);
            sb.Draw(st, Vector2.Zero, null, Color.White * alpha, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        });

        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.Prismadolph);
    }
}
