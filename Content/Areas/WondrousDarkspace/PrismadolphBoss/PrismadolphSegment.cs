using ReLogic.Content;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss.Projectiles;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

public class PrismadolphMapLayer : ModMapLayer
{
    // Here you can define where to put this layer in the vanilla map layer order.
    // In this case we go before the pings layer since all other vanilla map layers are ordered before pings,
    public override Position GetDefaultPosition() => new Before(IMapLayer.Pings);

    // In the Draw method, we draw everything. Consulting vanilla examples in the source code is a good resource for properly using this Draw method.
    public override void Draw(ref MapOverlayDrawContext context, ref string text)
    {
        // Here we define the scale that we wish to draw the icon when hovered and not hovered.
        const float scaleIfNotSelected = 1f;
        const float scaleIfSelected = scaleIfNotSelected * 2f;


        // Here we retrieve the texture of the Skeletron boss head so that we can draw it. Remember that not all textures are loaded by default, so you might need to do something like `Main.instance.LoadItem(ItemID.BoneKey);` in your code to ensure the texture is loaded.
        var prismadolphIcon = AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph_BossIcon.Asset;
        if (context.Draw(prismadolphIcon.Value, PrismadolphRoamingSystem.RoamingPosition.ToTileCoordinates().ToVector2(), Color.White,
               new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
        {
            // When the icon is being hovered by the users mouse, we set the mouse text to the localized text for "The Dungeon"
            text = LangText.Common("BellFlower");
        }
    }
}
public class PrismadolphRoamingSystem : ModSystem
{
    float _roamingTimer;
    public static Vector2 StartPoint
    {
        get
        {
            Vector2 pos = new();
            pos.X = ((float)Main.maxTilesX * 16) * 0.5F;
            pos.Y = (SavedGenerationParameters.DarkspaceTop * 16) + 500;
            return pos;
        }
    }
    public static float SpawnRadiusSquared => 2048F * 2048F;
    public static int PrismadolphType { get; private set; }
    public static Vector2 RoamingDirection { get; set; }
    public static Vector2 RoamingPosition { get; set; }
    public override void OnModLoad()
    {
        base.OnModLoad();
        PrismadolphType = ModContent.NPCType<Prismadolph>();
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.WriteVector2(RoamingPosition);
        writer.WriteVector2(RoamingDirection);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        RoamingPosition = reader.ReadVector2();
        RoamingDirection = reader.ReadVector2();
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        _roamingTimer++;
        if (ShouldChangeRoamingDirection())
        {
            _roamingTimer = 0;
            RoamingDirection = Main.rand.NextBool() ? -Vector2.UnitX : Vector2.UnitX;
        }
        if (ShouldResetRoamingPosition())
        {
            RoamingPosition = StartPoint;
        }

        var roamingVelocity = RoamingDirection * 2;
        var samplePoint = RoamingPosition;
        samplePoint.X *= 0.2F;
        samplePoint.Y *= 0.2F;
        roamingVelocity.Y += MathF.Sin(PreGeneratedNoise.SampleRand((int)samplePoint.X, (int)samplePoint.Y)) * 0.03F;
        RoamingPosition += roamingVelocity;

        if (!MultiplayerHelper.IsHost)
            return;
        if (DownedBossTracker.IsDowned(DownedBossFlag.Prismadolph))
            return;
        if(Main.GameUpdateCount % 15 == 0)
        {
            if (!NPC.AnyNPCs(PrismadolphType))
            {
                foreach (var player in Main.ActivePlayers)
                {
                    var dsq = Vector2.DistanceSquared(RoamingPosition, player.Center);
                    if(dsq < SpawnRadiusSquared)
                    {
                        NPC.NewNPC(new EntitySource_Misc("ctx"), (int)RoamingPosition.X, (int)RoamingPosition.Y, PrismadolphType);
                    }
                }
            }
        }

    }
    bool ShouldChangeRoamingDirection()
    {
        if (RoamingDirection == default)
            return true;
        if (_roamingTimer >= 2400)
            return true;
        return false;
    }
    bool ShouldResetRoamingPosition()
    {
        if (RoamingPosition == default)
            return true;
        var xDist = (RoamingPosition.X - StartPoint.X);
        xDist = MathF.Abs(xDist);
        if (xDist >= 10000)
            return true;
        return false;
    }
}
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
    enum AIState : byte
    {

        Lurk,
        ForegroundLurk,
        Circle,
        WanderOff
    }
    List<PrismadolphSegment> Segments => field ??= new();
    bool _spawnSegments;
    public ChainWithLengths Chain
    {
        get
        {
            if (field == null)
            {
                field = new(NPC.Center, 100, 16);
            }
            return field;
        }
    }

    float RotOffset => -MathHelper.PiOver2;
    bool InPhase2 => NPC.life < NPC.lifeMax * 0.8F;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    float CircleOffset => 252;
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
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 80;
        NPC.defense = 97;
        NPC.lifeMax = 12_000;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.dontTakeDamage = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.boss = false;
        NPC.npcSlots = 10f;


        //Setup the music and boss bar
        if (!Main.dedServ)
            Music = AssetReferences.Assets.Music.NostalgicFoe.Slot;
        NPC.aiStyle = -1;
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
                var frame = 0;

                if (i == 0)
                    frame = 0;
                else if (i == Chain.points.Length - 1)
                    frame = 2;
                else
                    frame = 1;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type, ai0: NPC.whoAmI, ai2: i, ai3: frame);
            }

            _spawnSegments = true;
        }
        Segments.Clear();
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.ModNPC is PrismadolphSegment segment && npc.ai[0] == NPC.whoAmI)
            {
                Segments.Add(segment);
            }
        }

        switch (State)
        {
            case AIState.Lurk:
                AI_Lurk();
                break;
            case AIState.Circle:
                AI_Circle();
                break;
            case AIState.WanderOff:
                AI_WanderOff();
                break;
        }
    }

    public override void PostAI()
    {
        base.PostAI();
        NPC.rotation = NPC.velocity.ToRotation();
        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 8; i++)
        {
            Chain.ResolveBackToRoot();
        }
    }
    void AI_Lurk()
    {
        Timer++;
        NPC.TargetClosest();
        //Since bro is in the darkspace gonna make this work interestingly
        Vector2 posToMoveTo = PrismadolphRoamingSystem.RoamingPosition;
        Vector2 velToMoveTo = (posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToMoveTo, 0.2f);
        if (InPhase2)
        {
            SwitchState(AIState.Circle);
        }
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
            SwitchState(AIState.WanderOff);
        }

        Vector2 offset = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
        offset = offset.RotatedBy(0.05F);
        Vector2 posToMoveTo = MyTarget.Center + offset * CircleOffset;
        Vector2 velToMoveTo = (posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToMoveTo, 0.2f);
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
                NPC.active = false;
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
            else
            {
                rot = NPC.rotation;
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
                rot = (nextPos - pos).ToRotation();
            }
            else
            {
                rot = NPC.rotation;
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
                rot = (nextPos - pos).ToRotation();
            }
            else
            {
                rot = NPC.rotation;
            }
            rot += RotOffset;
            segment.DrawArmor(spriteBatch, screenPos, drawColor, rot);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawUnderbody(spriteBatch, screenPos, drawColor);
        DrawArmor(spriteBatch, screenPos, drawColor);
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

public class PrismadolphSegment : ModNPC
{
    float _aggression;
    float _jiggle;
    float _timer;
    enum AIState : byte
    {
        Passive,
        Aggressive,
        SuperAggressive
    }

    enum Segment : byte
    {
        Head,
        Body,
        Tail
    }

    Prismadolph Parent
    {
        get
        {
            var parent = Main.npc[(int)NPC.ai[0]];
            if (parent.ModNPC is Prismadolph p)
            {
                return p;
            }
            else
            {
                return null;
            }
        }
    }

    float _armorProgress;
    float _chargeTimer;
    float _healTimer;
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    public override string Texture => AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph.KEY;
    int SegmentIndex => (int)NPC.ai[2];
    Vector2 SegmentPosition
    {
        get => Parent.Chain.points[SegmentIndex];
    }
    Vector2 NextSegmentPosition
    {
        get => Parent.Chain.points[SegmentIndex + 1];
    }
    //ref float Timer => ref NPC.ai[3];
    Segment SegmentType
    {
        get => (Segment)NPC.ai[3];
        set => NPC.ai[3] = (float)value;
    }
    int Damage => 35;
    int BigDamage => 65;
    float JiggleRotOffset => MathF.Sin(_jiggle * 0.3f) * 0.2f * MathHelper.Lerp(0f, 1f, _jiggle / 30F);
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_aggression);
        writer.Write(_timer);
        writer.Write(_armorProgress);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _aggression = reader.ReadSingle();
        _timer = reader.ReadSingle();
        _armorProgress = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        Main.npcFrameCount[Type] = 9;
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
        NPC.defense = 50;
        NPC.width = NPC.height = 100;
        NPC.damage = 70;
        NPC.lifeMax = 500;
        NPC.value = 5000f;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 5;
        NPC.noGravity = true;
        NPC.aiStyle = -1;
        NPC.lavaImmune = true;
        NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        NPC.DeathSound = SoundID.DD2_CrystalCartImpact;
    }

    void MakeImmune()
    {
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontCountMe = true;
    }

    void Heal()
    {
        var hp = (float)NPC.lifeMax * 0.1F;
        Parent.NPC.life += (int)(hp);
        _healTimer = 60;
        PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 60, 256);
    }

    void ManageHealingEffectsDecreaseTimer()
    {
        if (_healTimer > 0)
        {
            if (_healTimer % 2 == 0)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(70, 70);
                PS.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = -Vector2.UnitY * 8,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Green.ToVector4(),
                    timeLeft = 90,
                    scale = new Vector2(Main.rand.NextFloat(0.6F, 1.1F)),
                    color = Color.White.ToVector4(),
                });
            }

            _healTimer--;
        }
    }
    public override void AI()
    {
        base.AI();
        if (Parent == null)
        {
            NPC.active = false;
            return;
        }
        if(_chargeTimer > 0)
        {
            _chargeTimer--;
        }

        if (SegmentType == Segment.Head || SegmentType == Segment.Tail)
        {
            MakeImmune();
        }

        var targetArmorOn = _chargeTimer > 0 ? 1 : 0;
        _armorProgress = MathHelper.Lerp(_armorProgress, targetArmorOn, 0.05F);
        _aggression--;
        if (_jiggle > 0)
        {
            _jiggle--;
        }

        NPC.realLife = Parent.NPC.whoAmI;
        NPC.Center = Parent.Chain.points[SegmentIndex];
        switch (State)
        {
            case AIState.Passive:
                AI_Passive();
                break;
            case AIState.Aggressive:
                AI_Aggressive();
                break;
            case AIState.SuperAggressive:
                AI_SuperAggressive();
                break;
        }
        ManageHealingEffectsDecreaseTimer();
        Lighting.AddLight(NPC.Center, new Vector3(0.6F));
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            _timer = 0;
            NPC.netUpdate = true;
        }
    }
    void AI_Passive()
    {
        _timer++;
        if (_aggression >= 150)
        {
            SwitchState(AIState.Aggressive);
        }
    }
    void AI_Aggressive()
    {
        _timer++;
        if (_aggression <= 20)
        {
            SwitchState(AIState.Passive);
        }
        NPC.TargetClosest();

        _chargeTimer = 15;
        if (Main.rand.NextBool(2))
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(
                NPC.Center + Main.rand.NextVector2Circular(80, 80),
                TriColorPalette.Moonly,
                new Vector2(5, 25));

            ParticleUtils.CreateSwirlingDustBurst(fac);
        }

        if (Main.rand.NextBool(8))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
            var vel = (NPC.Center - pos) * 0.04F;
            Dust.NewDustPerfect(pos, DustID.Electric, vel, Scale: 0.3F);
        }

        if (_timer == 1 && MultiplayerHelper.IsHost)
        {
            var target = NPC.PlayerImTargeting;
            var vel = (target.Center - NPC.Center);
            vel *= 1.3f;

            var firer = ProjFirer.From<WondrousLightningZap>(NPC);
            firer.damage = Damage;
            firer.velocity = vel;
            firer.ai0 = NPC.whoAmI;
            firer.ai2 = 60;
            firer.New();
        }
        if (_timer >= 100)
        {
            _timer = 0;
        }

        if (_aggression >= 800)
        {
            SwitchState(AIState.SuperAggressive);
        }
    }
    void AI_SuperAggressive()
    {
        _chargeTimer = 30;
        _timer++;
        if (_timer == 1 && MultiplayerHelper.IsHost)
        {
            var target = NPC.PlayerImTargeting;
            var vel = (target.Center - NPC.Center);
            vel *= 1.3f;

            var firer = ProjFirer.From<WondrousLightningZap>(NPC);
            firer.damage = BigDamage;
            firer.velocity = vel;
            firer.ai0 = NPC.whoAmI;
            firer.ai1 = 2;
            firer.ai2 = 60;
            firer.New();
        }

        if (_timer >= 100)
        {
            Heal();
            _aggression = 0;
            _timer = 0;
            SwitchState(AIState.Passive);
        }
    }


    public void Charge()
    {
        _chargeTimer = 15;
    }

    public void DrawJigglyCore(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        if (SegmentType == Segment.Tail)
            return;
        if (SegmentType == Segment.Head)
            return;

        var tex = TextureAssets.Npc[Type].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, NPC.Center);
        drawer.VerticalFrame(Prismadolph.FRAME_CORE, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_CORE_WHITE, Main.npcFrameCount[Type]);
        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 0.2f, speed: 0.2f));
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
        // throw new NotImplementedException();
    }

    public void DrawJigglyCoreBloom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
        drawer.color = Color.Lerp(Color.DarkBlue, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 0.2f));
        drawer.color.A = 0;
        drawer.scale *= 0.8F;
        spriteBatch.Draw(drawer);
    }

    public void DrawArmor(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        if (SegmentType == Segment.Head)
        {
            DrawArmorHead(spriteBatch, screenPos, drawColor, rot);
        }
        else if (SegmentType == Segment.Tail)
        {
            DrawArmorTail(spriteBatch, screenPos, drawColor, rot);
        }
        else
        {
            var tex = TextureAssets.Npc[Type].Value;
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, Vector2.Lerp(SegmentPosition, NextSegmentPosition, _armorProgress));
            drawer.VerticalFrame(Prismadolph.FRAME_ARMOR, Main.npcFrameCount[Type]);
            drawer.CenterOrigin();
            drawer.rotation = rot + JiggleRotOffset;
            spriteBatch.Draw(drawer);
        }
    }

    public void DrawArmorTail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var tex = TextureAssets.Npc[Type].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, NPC.Center);
        drawer.VerticalFrame(Prismadolph.FRAME_TAIL, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_TAIL_GLOW, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 0.2f, speed: 0.2f));
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }

    public void DrawArmorHead(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var tex = TextureAssets.Npc[Type].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, NPC.Center);
        drawer.VerticalFrame(Prismadolph.FRAME_HEAD, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_HEAD_GLOW, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 0.2f, speed: 0.2f));
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        base.ModifyIncomingHit(ref modifiers);

        if (State == AIState.Passive || State == AIState.SuperAggressive)
            modifiers.FinalDamage *= 0.9F;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        _aggression += 90;
        _jiggle = 30;
    }

}
