using ReLogic.Content;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;
using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public class RekBoss : ScarletBoss
{
    private Vector2 _arenaCenter;
    private Vector2 _teleportPosition;
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
        public float scale;
        public float rotation;
        public bool isBurning;
     
        public float burnAlpha;
        public int bodyFrame;
    }
    public ChainWithLengths _chain;
    public ChainWithLengths Chain
    {
        get
        {
            if (_chain == null)
            {

                _chain = new(NPC.Center, 80, 39);
                for(int i = 0; i < _chain.lengths.Length; i++)
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



    private int _phase;
    private int _patternIndex;

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
                            return AIState.VolcanicMeteor;
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
                            return AIState.VolcanicMeteor;
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

                var randBigFrame = () => Main.rand.Next(0, 4);
                var randSmallFrame = () => Main.rand.Next(5, 7);
                float numPoints = 36;
                for(float i = 0; i < numPoints; i++)
                {
                    float progress = i / numPoints;
                    int bodyFrame = 0;
                    if(progress > 0.5f)
                    {
                        bodyFrame = randSmallFrame();
                    }
                    else
                    {
                        bodyFrame = randBigFrame();
                    }

                    Vector2 size = Vector2.Lerp(startSize, endSize, i / numPoints);
                    RekSegment segment = new RekSegment(NPC.Center + new Vector2(i * -0.1f, 0f), size, 1, bodyFrame);
                    segmentsList.Add(segment);
                }

                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 5));
                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 6));
                segmentsList.Add(new RekSegment(NPC.Center + new Vector2(segmentsList.Count * -0.1f, 0f), endSize, 1, 7));
                _segments =  segmentsList.ToArray();
            }
            return _segments;
        }
    }

    private AIState TestAttack => AIState.Eruption;
    private float Eruption_PrepTime => 90;
    private float Eruption_GraceTime => 40;
    private float Eruption_SinTime => 620;
    private float Eruption_SinHeight => 64;
    private float Eruption_SinFrequency => 0.04f;

    public override string Texture => TextureRegistry.EmptyTexture;
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

    

    public override void AI()
    {
        base.AI();
        if(_arenaCenter == Vector2.Zero)
        {
            _arenaCenter = TileUtilities.GuessArenaCenter(NPC.Center);
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                SwitchState(AIState.Despawn);
            }
        }
        if(_teleportPosition != Vector2.Zero)
        {
            Vector2 diff = _teleportPosition - NPC.Center;
            for(int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = _teleportPosition;
            }
            NPC.velocity = Vector2.Zero;
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        _outliner.SetDefaults();

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
        _outliner.Update();
        NPC.spriteDirection = 1;

        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 32; i++)
        {
            Chain.ResolveBackToRoot();
        }

        for(int i = 0; i < Segments.Length; i++)
        {
            var segment = Segments[i];
            segment.burnAlpha = MathHelper.Lerp(segment.burnAlpha, segment.isBurning ? 1f : 0f, 0.05f);
            segment.position = Chain.points[i];
            if (segment.isBurning && Main.rand.NextBool(5))
            {
                Dust.NewDustPerfect(segment.position + Main.rand.NextVector2Circular(48, 48),DustID.Torch, -Vector2.UnitY, Scale: 2f);
            }
        }
        for (int i = 0; i < Segments.Length; i++)
        {
            Segments[i].isBurning = false;
        }

        for (int i = Segments.Length - 1; i >= 0; i--)
        {
            if(i == 0)
            {
                Segments[i].rotation = NPC.rotation;
            }
            else
            {
                Segments[i].rotation = (Chain.points[i] - Chain.points[i - 1]).ToRotation();

            }
     
        }
    }
    private void AI_BlowtorchBreath()
    {

    }
    private void AI_CoilDash()
    {

    }
    private void AI_Enflame()
    {

    }

    private Vector2 FindEruptionLeft()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X--;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X += 1;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }
    private Vector2 FindEruptionRight()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X --;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }

    private void AI_Eruption()
    {
        Timer++;
        Vector2 eruptionLeft = FindEruptionLeft();
        Vector2 eruptionRight = FindEruptionRight();
        eruptionLeft.Y -= 384;
        eruptionRight.Y -= 384;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                    }

                    //Here I want rek to go on the left side of the arena and go along the floor
                    //So from left to right, and then wiggling up and down
                    //Move like worm motion might be enouugh but if it doesn't look cool we'll just calculate it

                    //It reks idle state he's perched up and wiggling side to side like a cobra
                    //SO we just make him go down and out in this case
                    NPC.velocity.X += NPC.direction;
                    NPC.velocity.Y += 0.05f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
             
                    if (Timer >= Eruption_PrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(eruptionLeft);
                    }

                    if (Timer >= Eruption_GraceTime)
                    {
                        if (Timer % 40 == 0)
                        {
                            if (MultiplayerHelper.IsHost)
                            {
                                //The sound will be on the projectile
                                ProjFirer firer = ProjFirer.From<VulcanEruption>(NPC);
                                int segmentIndex = Main.rand.Next(3, Segments.Length);
                                ref var segment = ref Segments[segmentIndex];
                                firer.position = segment.position;
                                firer.velocity = -Vector2.UnitY * 512;
                                firer.ai0 = NPC.whoAmI;
                                firer.ai1 = segmentIndex;
                                firer.New();
                            }
                        }
                    }

                    Vector2 pointToMoveTo = Vector2.Lerp(eruptionLeft, eruptionRight, Timer / Eruption_SinTime);
                    pointToMoveTo.Y += MathF.Sin(Timer * Eruption_SinFrequency) * Eruption_SinHeight;
                    Vector2 targetVelocity = pointToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.4f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Eruption_SinTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity.X += NPC.direction;
                    NPC.velocity.Y += 0.05f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    if (Timer >= Eruption_PrepTime)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
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

    private void Teleport(Vector2 teleportPos)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = teleportPos;
            NPC.netUpdate = true;
        }
    }

    private void AI_VolcanicSpear()
    {

    }
    private void AI_VolcanicMeteor()
    {

    }
    private void AI_Ouroboros()
    {

    }
    private void AI_Husk()
    {

    }
    private void AI_FireBreathV2()
    {

    }
    private void AI_Death()
    {

    }
    private void AI_FireBreath()
    {

    }
    private void AI_Despawn()
    {
        Timer++;
        if(Timer >= 120)
        {
            NPC.active = false;
        }
        NPC.velocity.Y -= 0.5f;
        NPC.velocity.X *= 0.98f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
    }

    public override bool AllowNameplateToBeShown()
    {
        return State != AIState.Spawn;
    }

    private void NextState()
    {
        if(TestAttack != default)
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
    private void AI_Spawn()
    {
        Timer++;
        if(Timer >= 180)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Idle()
    {
        _patternIndex = 0;
        Timer++;
        if(Timer >= 180)
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

    private void DrawSegment(int index)
    {
        ref RekSegment segment = ref Segments[index];
        Asset<Texture2D> textureAsset = BodySegmentsTextures[segment.bodyFrame];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, segment.position);
        drawer.rotation = segment.rotation;
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(0, 3);
                drawer.CenterOrigin();
                break;
            case 5:
            case 6:
                drawer.CenterOrigin();
                break;
        }

        Main.spriteBatch.Draw(drawer);
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(1, 3);
                drawer.CenterOrigin();
                drawer.color = Color.White * segment.burnAlpha * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.5f;
                drawer.color.A = 0;
                Main.spriteBatch.Draw(drawer);

                Vector2 pos = drawer.worldPosition;
                for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
                {
                    Main.spriteBatch.Draw(drawer with { worldPosition = pos + (f+Main.GlobalTimeWrappedHourly*2).ToRotationVector2() * ExtraMath.Osc(4f, 8f, speed: 2) * segment.burnAlpha} );
                }
                break;
            case 5:
            case 6:

                break;
        }
    }
    private void DrawSegmentWhite(int index)
    {
        ref RekSegment segment = ref Segments[index];
        Asset<Texture2D> textureAsset = BodySegmentsTextures[segment.bodyFrame];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, segment.position);
        drawer.rotation = segment.rotation;
        switch (segment.bodyFrame)
        {
            default:
                drawer.VerticalFrame(0, 3);
                drawer.CenterOrigin();
                break;
            case 5:
            case 6:
                drawer.CenterOrigin();
                break;
        }
        Color color = Color.Yellow * segment.burnAlpha;
        drawer.color = color;
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //Ok, so we draw everything here yah?
        for(int i = 0; i < Segments.Length; i++)
        {
            DrawSegment(i);
        }

        NPC.DrawAnimator(spriteBatch, drawColor);
        OutlineRenderer.Queue(DrawWhite);
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
        for (int i = 0; i < Segments.Length; i++)
        {
            DrawSegmentWhite(i);
        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
    }
}
