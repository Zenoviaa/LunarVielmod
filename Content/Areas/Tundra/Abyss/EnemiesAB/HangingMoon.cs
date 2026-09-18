using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;



public class MiniWhiteMoth : ModNPC
{
    private enum AIState
    {
        FindHome,
        Attack,
        FlyAway
    }

    private float _alpha;
    private float _alpha2;
    private ref float Timer => ref NPC.ai[0];
    private NPC Mommy => Main.npc[(int)NPC.ai[1]];
    private AIState State
    {
        get => (AIState)NPC.ai[2];
        set => NPC.ai[2] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[3];
    private float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    private Vector2 _startOffset;
    private const string ANIM_IDLE = "Idle";
    public override string Texture => TextureRegistry.EmptyTexture;
    private Player MyTarget => Main.player[NPC.target];
    private Outliner _outliner;
    private bool _contactDamage;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 1;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startOffset);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startOffset = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 12;
        NPC.damage = 36;
        NPC.defense = 18;
        NPC.lifeMax = 35;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = AssetReferences.Assets.Sounds.Abyss.MothFeatherDeath.Asset with { PitchVariance = 0.3f };
        NPC.value = 563f;
        NPC.knockBackResist = .45f;
        NPC.aiStyle = -1;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
    }
    public override void AI()
    {
        base.AI();
        _contactDamage = false;
        _alpha = MathHelper.Clamp(_alpha2, 0f, 1f);
        _alpha2 += 0.05f;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.FindHome:
                AI_FindHome();
                break;
            case AIState.Attack:
                AI_Attack();
                break;
            case AIState.FlyAway:
                AI_FlyAway();
                break;
        }

        if (Main.rand.NextBool(64))
        {
            Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, Scale: 0.8f);
        }
        _outliner.Update();
        if (!Mommy.active || Mommy.type != ModContent.NPCType<HangingMoon>())
        {
            bool found = false;
            foreach(var npc in Main.ActiveNPCs)
            {
                if(npc.type == ModContent.NPCType<HangingMoon>())
                {
                    NPC.ai[1] = npc.whoAmI;
                    found = true;
                }
            }
            if (!found)
            {
                if (State != AIState.FlyAway)
                {
                    SwitchState(AIState.FlyAway);
                }
            }

        }
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        NPC.rotation = NPC.velocity.X * 0.03f;
    }

    private void AI_FindHome()
    {
        Timer++;
        Vector2 home = Mommy.Center;
        Vector2 flyHomeVelocity = home - NPC.Center;
        flyHomeVelocity = flyHomeVelocity.SafeNormalize(Vector2.Zero);
        float dist = Vector2.Distance(home, NPC.Center);
        float speed = MathF.Min(dist, 7);
        flyHomeVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, flyHomeVelocity, 0.02f);
    //    NPC.velocity = NPC.velocity.RotatedBy(0.03f);
        NPC.TargetClosest();
        float distanceToPlayer = Vector2.Distance(NPC.Center, MyTarget.Center);
        if (dist <= 32)
        {
            if (distanceToPlayer <= 512)
            {
                SwitchState(AIState.Attack);
            }
        }
    }

    private void AI_Attack()
    {
        Timer++;
        if (!NPC.HasValidTarget)
        {
            SwitchState(AIState.FindHome);
        }

        switch (AttackCycle)
        {

            case 0:
                {
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        _startOffset = Main.rand.NextVector2CircularEdge(64, 64);
                        NPC.netUpdate = true;
                    }

                    _outliner.warning = true;
                    Vector2 startPos = MyTarget.Center + _startOffset;
                    Vector2 movementVEl = startPos - NPC.Center;
                    movementVEl = movementVEl.SafeNormalize(Vector2.Zero);

                    float dist = Vector2.Distance(startPos, NPC.Center);
                    float speed = MathF.Min(dist, 7);
                    movementVEl *= speed;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, movementVEl, 0.02f + Timer * 0.001f);
                    if (dist <= 64)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 1:
                {
                    _outliner.warning = true;
                    NPC.velocity *= 0.96f;
                    NPC.velocity -= (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.1f;
                    if (Timer >= 30)
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
                        _startOffset = (MyTarget.Center - NPC.Center);
                        _startOffset = _startOffset.SafeNormalize(Vector2.Zero);
                        NPC.velocity -= _startOffset * 9;
                    }
                    _contactDamage = true;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, _startOffset * 14, 0.04f);
                    _outliner.attacking = true;
                    if (Timer >= 60)
                    {
                        SwitchState(AIState.FindHome);
                    }
                }
                break;
        }
    }

    private void AI_FlyAway()
    {
        Timer++;
        NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY, 0.02f);
        if (Timer >= 120)
        {
            NPC.active = false;
        }
        _alpha = MathHelper.Lerp(1f, 0f, Timer / 120f);
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffectsMini(NPC);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor * _alpha);


        Color glowColor = Color.White * Glow * _alpha * 0.5f;
        glowColor.A = 0;
        NPC.DrawAnimator(spriteBatch, glowColor);


        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        drawer.color = Color.White * Glow * 0.2f * _alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.25f;
        spriteBatch.Draw(drawer);

        OutlineRenderer.Queue(DrawOutlineWhite);
        return false;
    }

    public void DrawOutlineWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor * _alpha);
    }
}

public class HangingMoon : ModNPC
{
    private enum AIState
    {
        Sway,
        Attract_Moths
    }
    private float _alpha;
    private float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    public const float AGGRO_DISTANCE = 256 * 256;
    private UnifiedRandom _random;
    private Vector2 _rootPoint;
    private Vector2 _floorPoint;
    private float _numSegments;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float SwayTimer => ref NPC.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_rootPoint);
        writer.WriteVector2(_floorPoint);
        writer.Write(_numSegments);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _rootPoint = reader.ReadVector2();
        _floorPoint = reader.ReadVector2();
        _numSegments = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 4;
        this.AddToAbyss();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frame.Y = frameHeight * 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 32;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.lifeMax = 140;
        NPC.defense = 8;
        NPC.damage = 1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = AssetReferences.Assets.Sounds.Abyss.MothFeatherDeath.Asset with { PitchVariance = 0.3f };
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        if (_rootPoint == Vector2.Zero)
        {
            _rootPoint = MovementUtilities.FindCeiling(NPC.Center);
            _floorPoint = MovementUtilities.FindFloor(_rootPoint);

            NPC.Center = _rootPoint;
            _numSegments = Vector2.Distance(_floorPoint, _rootPoint) / 14f;
        }
        if(_alpha < 1f)
            _alpha += 0.05f;
        if (Main.rand.NextBool(4))
        {
            var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(64, 64), DustID.GemDiamond, Scale: 1f);
            d.noGravity = true;
        }
        SwayTimer++;
        float oscillation = MathF.Sin(SwayTimer * 0.03f) * 0.5f + 0.5f;
        Vector2 ovalOffset = Vector2.Lerp(-Vector2.UnitX * 16, Vector2.UnitX * 16, oscillation);

        float dist = Vector2.Distance(_rootPoint, _floorPoint);
        float myLength = MathF.Max(100, dist / 2);

        Vector2 swingPoint = _rootPoint + ovalOffset + Vector2.UnitY * myLength;
        Vector2 targetVelocity = (swingPoint - NPC.Center);
        NPC.velocity = targetVelocity * 0.2f;
        switch (State)
        {
            case AIState.Sway:
                AI_Sway();
                break;
            case AIState.Attract_Moths:
                AI_AttractMoths();
                break;
        }
        Lighting.AddLight(NPC.Center, Vector3.One * 0.2f);
    }

    private bool HasFollowers()
    {
        foreach(var npc in Main.ActiveNPCs)
        {
            if (npc.ai[1] == NPC.whoAmI && npc.type == ModContent.NPCType<MiniWhiteMoth>())
                return true;
        }
        return false;
    }
    private void AI_Sway()
    {
        NPC.TargetClosest();
        Timer++;

        if (Timer >= 180 && NPC.HasValidTarget && Vector2.DistanceSquared(NPC.Center, Main.player[NPC.target].Center) < AGGRO_DISTANCE && !HasFollowers())
        {
            SwitchState(AIState.Attract_Moths);
        }
    }

    private void AI_AttractMoths()
    {
        Timer++;
        if (Timer % 18 == 0)
        {
            if (MultiplayerHelper.IsHost)
            {
                int spawnX = (int)NPC.Center.X;
                int spawnY = (int)NPC.Center.Y;
                spawnX += Main.rand.Next(-512, 512);
                spawnY += Main.rand.Next(-512, 512);
                NPC.NewNPC(NPC.GetSource_FromAI(), spawnX, spawnY, ModContent.NPCType<MiniWhiteMoth>(), ai1: NPC.whoAmI);
            }
        }
        if (Timer >= 58)
        {
            SwitchState(AIState.Sway);
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return base.SpawnChance(spawnInfo);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        _random ??= new();
        _random.SetSeed(NPC.whoAmI);
        Vector2 pos = _rootPoint;
        float rot = (NPC.Center - _rootPoint).ToRotation();
        for (float f = 0; f < _numSegments; f++)
        {
            float alpha = f / _numSegments;
            Vector2 chainPos = Vector2.Lerp(_rootPoint, NPC.Center, alpha);
            SpritebatchDrawer vineDrawer = SpritebatchDrawer.FromNPC(NPC);
            vineDrawer.VerticalFrame(_random.Next(0, 2), 4);
            if (f >= _numSegments - 1)
                vineDrawer.VerticalFrame(2, 4);
            vineDrawer.TopCenterOrigin();
            vineDrawer.rotation = rot + MathHelper.PiOver2;
            vineDrawer.worldPosition = chainPos;
            vineDrawer.color *= _alpha;
            spriteBatch.Draw(vineDrawer);
        }

        SpritebatchDrawer head = SpritebatchDrawer.FromNPC(NPC);
        head.color *= _alpha;
        spriteBatch.Draw(head);


        Color glowColor = Color.White * Glow;
        glowColor.A = 0;
        head.color = glowColor;
        head.color *= _alpha;
        spriteBatch.Draw(head);

        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        drawer.color = Color.White * Glow * 0.2f * _alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConvulgingMater>(), minimumDropped: 1, maximumDropped: 4));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffectsPlanty(NPC);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
