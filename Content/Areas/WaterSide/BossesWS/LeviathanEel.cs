using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
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

/// <summary>
/// This is just for the Eel's hitbox, all drawcode and AI is handled by the boss NPC
/// </summary>
public class MultiHitboxSegment : ModNPC
{
    public int Parent => (int)NPC.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 10000;
        NPC.defense = 18;
        NPC.damage = 90;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
    }
}

public static class TileUtilities
{
    public static Point FallToSolidTile(Point tile)
    {
        return FallToSolidTile(tile.X, tile.Y);
    }
    public static Point FallToSolidTile(int x, int y)
    {
        Point start = new Point(x, y);
        Point current = start;
        for (int i = 0; i < Main.maxTilesY; i++)
        {
            if (WorldGen.InWorld(current.X, current.Y) && WorldGen.SolidTile(current.X, current.Y))
                return current;
            current.Y += 1;
        }
        return Point.Zero;
    }
    public static Point RiseToSolidTile(int x, int y)
    {
        Point start = new Point(x, y);
        Point current = start;
        for (int i = 0; i < Main.maxTilesY; i++)
        {
            if (WorldGen.InWorld(current.X, current.Y) && WorldGen.SolidTile(current.X, current.Y))
                return current;
            current.Y -= 1;
        }
        return Point.Zero;
    }
}
public class LeviathanEel : ScarletBoss,
    IDrawOutlines
{
    public struct EelSegment
    {
        public Vector2 position;
        public Vector2 oldPosition;
        public Vector2 velocity;
    }

    private Vector2 _facingDirection;
    private Vector2 _arenaCenter;

    //Sound Effects
    private SoundStyle _sandDashHideAwaySound;
    private SoundStyle _sandDashWarningSound;
    private SoundStyle _sandDashRushSound;
    private SoundStyle _sandDashEndingSound;
    private SoundStyle _sandSpiralDashSound;

    private Color _outlineColor;
    private Color _targetOutlineColor;
    private bool _contactDamage;
    private enum AIState
    {
        //First let's break this down, and get all the states that we need
        //Then we can figure out which systems and projectiles we need
        //Solve smaller problems until the whole is complete

        SpawnIntro,
        Idle,

        /*
         * 
         * - The ground or ceiling rumbles, and the eel charges through after sand clouds telegraph where he’s coming from for a bit, 
            this gradually gets faster and on the last one he does a cool circle and dashes into you before going holographic again
         */

        SandDashHideAway,
        SandDash_Warning,
        SandDash,
        SandSpiralDash,
        SandDashEnd,

        /*
         * 
           - The eel comes in from the bottom left or bottom right of the arena and goes straight in a sining motion, 
           it slowly becomes visible and rainbowy electricity comes out of it, electrifying the area around its body
         */

        SinElectric_HideAway,
        SinElectric_ComeIn,
        SinElectric_Shock,
        SinElectric_GoOut,
        SinElectric_End,

        /*
         * 
         * 
         
           - The eel comes in from the top and goes down in a large sining motion all around the arena,
           while doing so it flickers in and out and shoots precise electric bolts from the different parts of its body directly at you

         */

        SpiralSinElectricBolt_HideAway,
        SpiralSinElectricBolt_MoveAround,
        SpiralSinElectricBolt_ChargeUp,
        SpiralSinElectricBolt_Shoot,
        SpiralSinElectricBolt_End,

        /*
         * 
         * 
         *
         *
         *
         *
            - A dust cloud appears on either left or right wall, and the eel quickly pokes out and yells, 
            then it opens its mouth with a really cool animation and starts sucking you in, 
            bubbles and dust and everything goes into it, after it finishes its eyes and gills light up a glowy color 
            and it shoots three powerful lightning blasts, before going away

                Opens it’s mouth and tries to suck everything in, including you and the water,
                you just have to run the other way and youll be fine, but if you get eaten its insta death, good telegraph for this too (so melee)
         */

        SuckingBlast_HideAway,
        SuckingBlast_PokeOut,
        SuckingBlast_Suck,
        SuckingBlast_ChargeUp,
        SuckingBlast_LightningZap,

        //This is the one that instant-kills you, it has a different indication
        SuckingBlast_REALLYSuck,


        //PHASE 2 ATTACKS

        /*
         * - The screen shakes for a bit and half the arena drains out, with three platforms floating to the top of the water

         */
        Phase2Transition_WaterDrain,
        Phase2Transition_Yell,

        //- All of the eels previous attacks get faster, and he has some new ones

        /*
         * - The eel dives into the water and charges up as much electricity as he can, 
         * electrifying the entire body of water and having some stray bolts shoot up from it
         */

        ElectricDive_HideAway,
        ElectricDive_Shocking,
        ElectricDive_End,


        /*
         * 
        - A dust cloud appears above one of the platforms, and the eel dives directly on top of it, destroying it,
        it takes a while before it comes back

         */

        PlatformSmash_HideAway,
        PlatformSmash_Dash,
        PlatformSmash_End,

        /*
         * 
         * - Several bubbles appear underneath one of the platforms, 
         * and after a while a violent waterfall pushes the platform upward into the ceiling, dealing a ton of damage

         */

        PlatformBubble_HideAway,
        PlatformBubble_Rush,
        PlatformBubble_End,


        /*
         * 
         * 
         * - The eel breathes in and creates toxic bubbles that slowly float towards you, after a while the explode into lightning fields
         */

        ToxicBubble_HideAway,
        ToxicBubble_Ready,
        ToxicBubble_Breath,
        ToxicBubble_End,

        /*
         * 
         * - The arena refills back up with water, 
         * and the eel goes into hiding for a little while,
         * after a bit, the floor shakes and breaks, and bubbles start rushing down,
         * forcibly pulling you downward, you can fall faster if you hold down.
         * While you’re falling, the eel will sometimes come in from the side and try to ram you, 
         * and will shoot 1 slightly homing projectile down from its body each time, 
         * while shocking the surrounding water. Once you reach the bottom, 
         * a geyser explodes and pushes you back up at insane speed, and the eel chases you back to the original arena

         */

    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float OvalTimer => ref NPC.ai[2];
    private ref float AttackCycle => ref NPC.ai[3];

    private PatternManager<AIState> _patternManagerBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.SandDashHideAway, 1.0f));
            }
            return _patternManagerBackingField;
        }
    }

    private EelSegment[] _segments;
    public int SegmentCount => 50;

    private float SandDashHideAwayTime => 100;
    private float SandDashStartWarningTime => 120;
    private float SandDashEndWarningTime => 30;
    private float SandDashCount => 5;
    private float SandDashRushTime => 30;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _segments = new EelSegment[SegmentCount];
        NPC.width = 64;
        NPC.height = 64;
        NPC.lifeMax = 10000;
        NPC.defense = 18;
        NPC.damage = 90;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
    }


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);

    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromNPC(NPC);
        /*
        for (int i = 0; i < SegmentCount; i++)
        {
            EelSegment eelSegment = _segments[i];
            segmentDrawer.worldPosition = eelSegment.position;
            spriteBatch.Draw(segmentDrawer);
        }
        */

        spriteBatch.Draw(segmentDrawer);
        return false;
    }

    public override void AI()
    {
        base.AI();
        if (_arenaCenter == Vector2.Zero)
            _arenaCenter = MyTarget.Center;
        for (int i = 0; i < _segments.Length; i++)
        {
            ref EelSegment eelSegment = ref _segments[i];
            eelSegment.position = NPC.Center;
            eelSegment.position -= Vector2.UnitX * 64 * i;
        }

        _contactDamage = false;
        _targetOutlineColor = Color.Transparent;
        switch (State)
        {
            case AIState.SpawnIntro:
                AI_SpawnIntro();
                break;
            case AIState.Idle:
                AI_Idle();
                break;


            case AIState.SandDashHideAway:
                AI_SandDashHideAway();
                break;
            case AIState.SandDash_Warning:
                AI_SandDashWarning();
                break;
            case AIState.SandDash:
                AI_SandDash();
                break;
            case AIState.SandDashEnd:
                AI_SandDashEnd();
                break;
            case AIState.SandSpiralDash:
                AI_SandSpiralDash();
                break;

        }
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.3f);
        //Set the facing the direction
        float facingRotation = _facingDirection.ToRotation();
        NPC.rotation = facingRotation;
    }

    #region Mirage Visuals
    private void GoMirage()
    {

    }

    private void GoInvisible()
    {

    }

    private void GoVisible()
    {

    }

    #endregion
    #region Sand Dash

    private void AI_SandDashHideAway()
    {
        Timer++;
        if(Timer == 1)
        {
            //Play the sound
            SoundEngine.PlaySound(_sandDashHideAwaySound, NPC.position);
            NPC.TargetClosest();
        }

        //Slowly face down and go down for a bit
        float time = SandDashHideAwayTime;
        float ease = EasingFunction.QuadraticBump(Timer / time);
        float downSpeed = MathHelper.Lerp(0, 8, ease);
        Vector2 targetMovementVelocity = Vector2.UnitY * downSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetMovementVelocity, 0.3f);
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f);

        //Have segments go into mirage stage for a bit
        if(Timer == 20)
        {
            GoMirage();
        }

        if(Timer % 5 == 0 && Timer < 60)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BreatheBubble);
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.Red);
            p.Scale *= 0.33f;
        }

        //Eventually turn invisible
        if (Timer == 60)
        {
            GoInvisible();
        }

        if(Timer >= time)
        {
            SwitchState(AIState.SandDash_Warning);
        }
    }

    private void AI_SandDashWarning()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundEngine.PlaySound(_sandDashWarningSound, NPC.position);
            GoInvisible();
            NPC.TargetClosest();
        }

        ShakeModSystem.Shake = 3;
        //Dust clouds appear above or below you, snapping to the nearest tile, this is where bro will dash from
        //Alright, we have a steam particle we can use for that
        Vector2 positionToDashTo = MyTarget.Center + new Vector2(0, 182);
        Point point = positionToDashTo.ToPoint();
        Point tileToComeFrom = TileUtilities.FallToSolidTile(point);
        Vector2 dustCenter = tileToComeFrom.ToWorldCoordinates();
        if(Timer % 5 == 0)
        {
            Vector2 positionToSpawnDustFrom = dustCenter;
            positionToSpawnDustFrom += Main.rand.NextVector2Circular(196, 16);
            ThickSmokeParticle.Spawn(positionToSpawnDustFrom, -Vector2.UnitY, Color.SandyBrown);
        }


        float sandDashProgress = (AttackCycle+1) / SandDashCount;
        float warningTime = MathHelper.Lerp(SandDashStartWarningTime, SandDashEndWarningTime, sandDashProgress);
        if(Timer >= warningTime)
        {
            SwitchState(AIState.SandDash);
        }
    }

    private void AI_SandDash()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundEngine.PlaySound(_sandDashRushSound, NPC.position);
            GoVisible();
            NPC.Center = MyTarget.Center + Vector2.UnitY * 1500;
        }

        Vector2 dashVelocity = -Vector2.UnitY * 30;
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f);
        NPC.velocity = Vector2.Lerp(NPC.velocity, dashVelocity, 0.3f);

        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        if(Timer >= SandDashRushTime)
        {
            SwitchState(AIState.SandDashEnd);
        }
    }

    private void AI_SandDashEnd()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundEngine.PlaySound(_sandDashEndingSound, NPC.position);
        }

        NPC.velocity *= 0.8f;
        if(Timer >= 5)
        {
            AttackCycle++;
            if(AttackCycle < SandDashCount)
            {
                SwitchState(AIState.SandDash_Warning);
            }
            else
            {
                SwitchState(AIState.SandSpiralDash);
            }
        }
    }

    private void AI_SandSpiralDash()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundEngine.PlaySound(_sandSpiralDashSound, NPC.position);
            NPC.TargetClosest();
        }
    }

    #endregion

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            Timer = 0;
            NPC.netUpdate = true;
        }
    }

    private void AI_SpawnIntro()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }
    #region Movement
    private Vector2 GetNextPointToTrack(float time)
    {
        float xRadius = 700;
        float yRadius = xRadius * 0.15f;
        Vector2 pointOnOval = _arenaCenter;

        float radians = time * 0.02f;
        pointOnOval.X += MathF.Sin(radians) * xRadius;
        pointOnOval.Y += MathF.Cos(radians) * yRadius;

        float osc = MathF.Sin(time * 0.25f) * 0.5f + 0.5f;
        if (Vector2.Distance(pointOnOval, _arenaCenter) < xRadius * 0.75f)
        {
            pointOnOval = Vector2.Lerp(pointOnOval, _arenaCenter, osc * 0.05f);
        }

        return pointOnOval;
    }

    private bool HasCrossedPoint(Vector2 current, Vector2 target)
    {
        float distanceToPoint = Vector2.Distance(current, target);
        return distanceToPoint < 8f;
    }
    #endregion

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {

        }
    }

    private void AI_Idle()
    {
        AttackCycle = 0;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            _arenaCenter = MyTarget.Center - new Vector2(0, 128);
        }

        //For this movement we'll just have him moving in an oval
        //But he should have a lot of different movement types
        //First lets make that oval shape
        Vector2 pointOnOval = GetNextPointToTrack(OvalTimer);
        if (HasCrossedPoint(NPC.Center, pointOnOval))
        {
            OvalTimer++;
        }

        Vector2 vectorToOvalPoint = pointOnOval - NPC.Center;
        vectorToOvalPoint = vectorToOvalPoint.SafeNormalize(Vector2.Zero);

        Vector2 targetVelocity = vectorToOvalPoint;
        float movementSpeed = 3;
        targetVelocity *= movementSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.2f);
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        // throw new NotImplementedException();
    }
}
