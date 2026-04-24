using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using System.Threading;
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


public class PrismaticElectricBolt : ModProjectile
{
    private Vector2 _initialVelocity;
    private float _randRadians;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _initialVelocity = Projectile.velocity;
        }


        if(Timer % 30 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                float radians = MathHelper.ToRadians(10);
                _randRadians = Main.rand.NextFloat(-radians, radians);
                Projectile.velocity = _initialVelocity.RotatedBy(_randRadians);
                Projectile.netUpdate = true;
            }
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class SinElectricShock : ModProjectile
{
    private Vector2[] _shockPos;
    private Vector2[] _sparkPos;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        _shockPos = new Vector2[32];
        _sparkPos = new Vector2[32];
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.velocity = -Vector2.UnitY;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            var fp = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Goldenrod);
            fp.Scale *= 1.5f;
        }
        Projectile.velocity = Projectile.velocity.RotatedBy(0.03f);
        if (Timer % 8 == 0)
        {
            DustParticle sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(12, 12), Color.White, 0.7f);
            sp.fast = true;
            sp.gravity = 0;
            sp.noTileCollide = true;
        }

        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(128, 128), Vector2.Zero, Color.White, 0.3f);
            sp.gravity = 0;
        }

        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        for (int i = 0; i < _shockPos.Length; i++)
        {
            ref Vector2 position = ref _shockPos[i];
            Vector2 offset = new Vector2();

            float radians = (float)i / (float)_shockPos.Length * MathHelper.TwoPi;
            radians += Timer * 0.03f;

            float radius = ExtraMath.Osc(80, 128, speed: 18, offset: Projectile.whoAmI);

            radius *= inScale * outScale;
            offset.X += MathF.Sin(radians) * radius;
            offset.Y += MathF.Cos(radians) * radius;
            offset = Vector2.Lerp(offset, Vector2.Zero, (MathF.Sin(Timer * 0.5f + i) + 0.5f) * 0.1f);
            offset += Main.rand.NextVector2Circular(6, 6);
            position = Projectile.Center + offset;

            _sparkPos[i] = Projectile.Center + offset.RotatedBy(MathHelper.PiOver4) * 0.2f * Main.rand.NextFloat(1f, 1.5f);
        }

    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitPlayer(target, ref modifiers);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawBloom(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D bloomTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowScale = Vector2.One * 0.25f;
        float rotation = Main.GlobalTimeWrappedHourly * 4;
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Main.DiscoColor;
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.1f;
        bloomDrawer.color *= outScale;
        spriteBatch.Draw(bloomDrawer);
        for (int i = 0; i < _shockPos.Length; i += 2)
        {
            Vector2 pos = _shockPos[i];

            Color glowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
            glowColor.A = 0;
            glowColor *= 0.2f;
            glowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6, offset: i);
            glowColor *= outScale;

            bloomDrawer.worldPosition = pos;
            bloomDrawer.color = glowColor;
            bloomDrawer.scale = glowScale;
            spriteBatch.Draw(bloomDrawer);
        }

        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);

        Color spiralGlowColor = Color.Lerp(Color.White, Main.DiscoColor, 0.6f);
        spiralGlowColor.A = 0;
        spiralGlowColor *= 0.2f;
        spiralGlowColor *= ExtraMath.Osc(0.6f, 1f, speed: 6);
        spiralGlowColor *= outScale;
        spiralDrawer.color = spiralGlowColor;
        spriteBatch.Draw(spiralDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBloom);
        PrismaticLightningRenderer.Queue(_shockPos);
        PrismaticLightningRenderer.Queue(_sparkPos);
        return false;
    }
}

public class PrismaticLightningRenderer : PixelPrimitiveRenderer<PrismaticLightningRenderer>
{
    public override BaseShader PrepareShader()
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.White;
        shader.InnerColor = Main.DiscoColor;
        shader.OuterColor = Color.Goldenrod;
        shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
        shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        return shader;
    }

    public override Color GetTrailColor(float completionRatio)
    {
        float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
        return Color.Lerp(Color.White, Main.DiscoColor, osc);
    }

    public override float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 32, completionRatio) * MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
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
    private Vector2 _teleportPosition;
    private Vector2 _dashDirection;
    private Vector2 _dustPosition;



    private Color _outlineColor;
    private Color _targetOutlineColor;
    private bool _contactDamage;
    private bool _effectZappy;
    private bool _effectWaterTrail;
    private bool _effectInvisible;
    private bool _effectMirage;
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

    private AIState _attackToTest;
    private EelSegment[] _segments;
    public int SegmentCount => 50;

    private float IdleTime => 480;

    private float SandDashHideAwayTime => 100;
    private float SandDashStartWarningTime => 120;
    private float SandDashEndWarningTime => 30;
    private float SandDashCount => 8;
    private float SandDashRushTime => 30;


    //Sin Electric Attack
    private float SinElectricHideAwayTime => 100;

    private float SinElectricComeInTime => 100;
    private float SinElectricShockTime => 120;
    private int SinElectricDamage => 40;
    private float SinElectricGoOutTime => 60;
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
        NPC.lifeMax = 12000;
        NPC.defense = 18;
        NPC.damage = 90;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;

        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/LeviathanEel");
        NPC.HitSound = SoundID.NPCHit1 with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
      //  NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }


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
        //Testing Attacks
        _attackToTest = AIState.SinElectric_HideAway; 
        if (_arenaCenter == Vector2.Zero)
            _arenaCenter = MyTarget.Center;
        for (int i = 0; i < _segments.Length; i++)
        {
            ref EelSegment eelSegment = ref _segments[i];
            eelSegment.position = NPC.Center;
            eelSegment.position -= Vector2.UnitX * 64 * i;
        }

        _effectZappy = false;
        _effectWaterTrail = false;
        _effectInvisible = false;
        _effectMirage = false;
        _contactDamage = false;

        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

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

            case AIState.SinElectric_HideAway:
                AI_SinElectricHideAway();
                break;
            case AIState.SinElectric_ComeIn:
                AI_SinElectricComeIn();
                break;
            case AIState.SinElectric_Shock:
                AI_SinElectricShock();
                break;
            case AIState.SinElectric_GoOut:
                AI_SinElectricGoOut();
                break;
            case AIState.SinElectric_End:
                AI_SinElectricEnd();
                break;

            case AIState.SpiralSinElectricBolt_HideAway:
                AI_SpiralSinElectricBoltHideAway();
                break;
            case AIState.SpiralSinElectricBolt_MoveAround:
                AI_SpiralSinElectricBoltMoveAround();
                break;
            case AIState.SpiralSinElectricBolt_ChargeUp:
                AI_SpiralSinElectricBoltChargeUp();
                break;
            case AIState.SpiralSinElectricBolt_Shoot:
                AI_SpiralSinElectricBoltShoot();
                break;
            case AIState.SpiralSinElectricBolt_End:
                AI_SpiralSinElectricBoltEnd();
                break;

            case AIState.SuckingBlast_HideAway:
                AI_SuckingBlastHideAway();
                break;
            case AIState.SuckingBlast_PokeOut:
                AI_SuckingBlastPokeOut();
                break;
            case AIState.SuckingBlast_Suck:
                AI_SuckingBlastSuck();
                break;
            case AIState.SuckingBlast_ChargeUp:
                AI_SuckingBlastChargeUp();
                break;
            case AIState.SuckingBlast_LightningZap:
                AI_SuckingBlastLightningZap();
                break;
            case AIState.SuckingBlast_REALLYSuck:
                AI_SuckingBlastReallySuck();
                break;

            case AIState.Phase2Transition_WaterDrain:
                AI_Phase2TransitionWaterDrain();
                break;
            case AIState.Phase2Transition_Yell:
                AI_Phase2TransitionYell();
                break;

            case AIState.ElectricDive_HideAway:
                AI_ElectricDiveHideAway();
                break;
            case AIState.ElectricDive_Shocking:
                AI_ElectricDiveShocking();
                break;
            case AIState.ElectricDive_End:
                AI_ElectricDiveEnd();
                break;

            case AIState.PlatformSmash_HideAway:
                AI_PlatformSmashHideAway();
                break;
            case AIState.PlatformSmash_Dash:
                AI_PlatformSmashDash();
                break;
            case AIState.PlatformSmash_End:
                AI_PlatformSmashEnd();
                break;

            case AIState.PlatformBubble_HideAway:
                AI_PlatformBubbleHideAway();
                break;
            case AIState.PlatformBubble_Rush:
                AI_PlatformBubbleRush();
                break;
            case AIState.PlatformBubble_End:
                AI_PlatformBubbleEnd();
                break;

            case AIState.ToxicBubble_HideAway:
                AI_ToxicBubbleHideAway();
                break;
            case AIState.ToxicBubble_Ready:
                AI_ToxicBubbleReady();
                break;
            case AIState.ToxicBubble_Breath:
                AI_ToxicBubbleBreath();
                break;
            case AIState.ToxicBubble_End:
                AI_ToxicBubbleEnd();
                break;
        }
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.3f);
        //Set the facing the direction
        float facingRotation = _facingDirection.ToRotation();
        NPC.rotation = facingRotation;
    }

    private void Teleport(Vector2 teleportPosition)
    {
        if (!MultiplayerHelper.IsHost)
            return;

        _teleportPosition = teleportPosition;
        NPC.netUpdate = true;
    }

    #region Toxic Bubble

    private void AI_ToxicBubbleHideAway()
    {

    }

    private void AI_ToxicBubbleReady()
    {

    }

    private void AI_ToxicBubbleBreath()
    {

    }

    private void AI_ToxicBubbleEnd()
    {

    }
    #endregion

    #region Platform Bubble
    private void AI_PlatformBubbleHideAway()
    {

    }

    private void AI_PlatformBubbleRush()
    {

    }

    private void AI_PlatformBubbleEnd()
    {

    }
    #endregion

    #region Platform Smash
    private void AI_PlatformSmashHideAway()
    {

    }

    private void AI_PlatformSmashDash()
    {

    }

    private void AI_PlatformSmashEnd()
    {

    }
    #endregion

    #region Electric Dive
    private void AI_ElectricDiveHideAway()
    {

    }

    private void AI_ElectricDiveShocking()
    {

    }

    private void AI_ElectricDiveEnd()
    {

    }
    #endregion

    #region Phase 2 Transition
    private void AI_Phase2TransitionWaterDrain()
    {

    }

    private void AI_Phase2TransitionYell()
    {

    }
    #endregion

    #region Sucking Blast
    private void AI_SuckingBlastHideAway()
    {

    }

    private void AI_SuckingBlastPokeOut()
    {


    }

    private void AI_SuckingBlastSuck()
    {

    }

    private void AI_SuckingBlastChargeUp()
    {

    }

    private void AI_SuckingBlastLightningZap()
    {

    }

    private void AI_SuckingBlastReallySuck()
    {

    }
    #endregion

    #region Spiral Sin Electric Bolt
    private void AI_SpiralSinElectricBoltHideAway()
    {

    }

    private void AI_SpiralSinElectricBoltMoveAround()
    {

    }

    private void AI_SpiralSinElectricBoltChargeUp()
    {

    }
    private void AI_SpiralSinElectricBoltShoot()
    {

    }

    private void AI_SpiralSinElectricBoltEnd()
    {

    }
    #endregion

    #region Sin Electric
    private void AI_SinElectricHideAway()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle hideAwaySound = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune_Fade");
            hideAwaySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hideAwaySound, NPC.position);
            NPC.TargetClosest();
        }

        Vector2 targetVelocity = -Vector2.UnitY * 3;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);

        Vector2 targetFacingDirection = NPC.velocity.SafeNormalize(Vector2.Zero);
        _facingDirection = Vector2.Lerp(_facingDirection, targetFacingDirection, 0.3f);
        _targetOutlineColor = Color.Yellow;
        if(Timer >= 60)
        {
            _effectInvisible = true;
        }

        if(Timer >= SinElectricHideAwayTime)
        {
            SwitchState(AIState.SinElectric_ComeIn);
        }
    }

    private void AI_SinElectricComeIn()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle soulShot = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune_SoulShot");
            soulShot.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soulShot, NPC.position);
            NPC.TargetClosest();
            Teleport(MyTarget.Center - Vector2.UnitX * 1200);
        }

        _effectMirage = true;
        Vector2 targetVelocity = Vector2.UnitX * 12;
        targetVelocity.Y += MathF.Sin(Timer * 0.2f) * 2;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);

        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f);
        _targetOutlineColor = Color.Yellow;
        if (Timer >= SinElectricComeInTime)
        {
            SwitchState(AIState.SinElectric_Shock);
        }
    }

    private void AI_SinElectricShock()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }


        NPC.velocity *= 0.98f;
        if(Timer == 30)
        {
            SoundStyle electrify = AssetRegistry.Sounds.LeviathanEel.Electrify;
            electrify.PitchVariance = 0.3f;
            SoundEngine.PlaySound(electrify, NPC.position);
            FXUtil.ShakeCamera(MyTarget.Center, 1024, 3);
            for (int i = 0; i < _segments.Length; i += 4)
            {
                if (MultiplayerHelper.IsHost)
                {
                    //Spawn electric fields around the body
                    Projectile.NewProjectile(SourceFromThis, _segments[i].position, Vector2.Zero,
                        ModContent.ProjectileType<SinElectricShock>(), SinElectricDamage, 1, Main.myPlayer);
                }    
            }
        }

        _targetOutlineColor = Color.Red;
        if (Timer >= SinElectricShockTime)
        {
            SwitchState(AIState.SinElectric_GoOut);
        }
    }

    private void AI_SinElectricGoOut()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        Vector2 targetVelocity = -Vector2.UnitY * 3;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);

        Vector2 targetFacingDirection = NPC.velocity.SafeNormalize(Vector2.Zero);
        _facingDirection = Vector2.Lerp(_facingDirection, targetFacingDirection, 0.3f);
        _targetOutlineColor = Color.Yellow;
        if (Timer >= 60)
        {
            _effectInvisible = true;
        }

        if (Timer >= SinElectricGoOutTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_SinElectricEnd()
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
            NPC.TargetClosest();
        }

        //Slowly face down and go down for a bit
        float time = SandDashHideAwayTime;
        float ease = EasingFunction.QuadraticBump(Timer / time);
        float downSpeed = MathHelper.Lerp(0, 25, ease);
        Vector2 targetMovementVelocity = Vector2.UnitY * downSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetMovementVelocity, 0.3f);
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f);

        //Have segments go into mirage stage for a bit
        if(Timer >= 20 && Timer < 60)
        {
            _effectMirage = true;
        }

        if(Timer % 5 == 0 && Timer < 60)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BreatheBubble);
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.Red);
            p.Scale *= 0.33f;
        }

        if(Timer >= 60)
        {
            _effectInvisible = true;
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
            SoundStyle earthQuake = AssetRegistry.Sounds.LeviathanEel.EarthRumble;
            earthQuake.PitchVariance = 0.3f;
            SoundEngine.PlaySound(earthQuake, MyTarget.position);
            NPC.TargetClosest();
        }

        _effectInvisible = true;
        ShakeModSystem.Shake = 2;
        //Dust clouds appear above or below you, snapping to the nearest tile, this is where bro will dash from
        //Alright, we have a steam particle we can use for that
        Vector2 positionToDashTo = MyTarget.Center + new Vector2(0, -182) + Vector2.UnitX * MyTarget.velocity.X * 64;
        Point point = positionToDashTo.ToTileCoordinates();
        Point tileToComeFrom = TileUtilities.FallToSolidTile(point);
       _dustPosition = tileToComeFrom.ToWorldCoordinates();
        if(Timer % 5 == 0)
        {
            Vector2 positionToSpawnDustFrom = _dustPosition;
            positionToSpawnDustFrom += Main.rand.NextVector2Circular(64, 16);
            var sp = SmokeParticle.Spawn(positionToSpawnDustFrom, -Vector2.UnitY, Color.SandyBrown);
            sp.initialColor = Color.SandyBrown * 0.65f;
         
        }
        if (Timer % 5 == 0)
        {
            Vector2 positionToSpawnDustFrom = _dustPosition;
            positionToSpawnDustFrom += Main.rand.NextVector2Circular(64, 16);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.outerColor = Color.SandyBrown;
            Vector2 dustVelocity = -Vector2.UnitY * 8;
            if (AttackCycle == SandDashCount - 1)
            {
                spawnParams.scaleRange *= 2f;
                dustVelocity *= 2f;
            }


            var sp = DustParticle.Spawn(positionToSpawnDustFrom, dustVelocity, spawnParams);
            sp.noTileCollide = true;
          //  sp.initialColor = Color.SandyBrown * 0.65f;

        }


        float sandDashProgress = (AttackCycle+1) / SandDashCount;
        float ease = EasingFunction.OutExpo(sandDashProgress);
        float warningTime = MathHelper.Lerp(SandDashStartWarningTime, SandDashEndWarningTime, ease);
        warningTime *= AttackCycle == SandDashCount - 1 ? 2f : 1f;
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
            FXUtil.ShakeCamera(MyTarget.position, 1024, 8);
            SoundStyle sandDash = AssetRegistry.Sounds.LeviathanEel.SandDash;
            sandDash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sandDash, MyTarget.position);
            Teleport(_dustPosition + Vector2.UnitY * 1000);
         
        }

        if(Timer == 15)
        {
            int[] gores = AutoGoreLoader.FindGores("GrayRock");

            Vector2 pos = _dustPosition;
            foreach (int g in gores)
            {
                Gore.NewGore(NPC.GetSource_FromThis(),
                   _dustPosition - new Vector2(0, 32) + Main.rand.NextVector2Circular(64, 16),
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }

            var p = Particle<ThickSmokeParticle>.Spawn(_dustPosition, Vector2.Zero, Color.DarkGray);

        }

        if (Timer % 5 == 0)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BreatheBubble);
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.Red);
            p.Scale *= 0.33f;
        }

        Vector2 dashVelocity = -Vector2.UnitY * 45;
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f);
        NPC.velocity = Vector2.Lerp(NPC.velocity, dashVelocity, 0.3f);

        _effectWaterTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;


        float timeMult = 1f;
        if(AttackCycle == SandDashCount - 1)
        {
            timeMult *= 0.66f;
        }
        if(Timer >= SandDashRushTime * timeMult)
        {
            AttackCycle++;
            if (AttackCycle < SandDashCount)
            {
                SwitchState(AIState.SandDashEnd);
            }
            else
            {
                SwitchState(AIState.SandSpiralDash);
            }
         
        }
    }

    private void AI_SandDashEnd()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle sandFade = AssetRegistry.Sounds.LeviathanEel.SandFade;
            sandFade.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sandFade, MyTarget.position);
        }

        NPC.velocity *= 0.8f;
        if(Timer >= 5)
        {
            SwitchState(AIState.SandDash_Warning);
        }
    }

    private void AI_SandSpiralDash()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }

        _targetOutlineColor = Color.Red;
        _contactDamage = true;


        //For this attack, he'll spin around a bit and then dash at you again
        if(Timer < 30)
        {
            NPC.velocity = NPC.velocity.RotatedBy(0.06f);
            _facingDirection = NPC.velocity.SafeNormalize(Vector2.Zero);

            _dashDirection = (MyTarget.Center - NPC.Center);
            _dashDirection = _dashDirection.SafeNormalize(Vector2.Zero);
        } 
        else if (Timer < 60)
        {
            if(Timer == 31)
            {
                FXUtil.ShakeCamera(MyTarget.position, 1024, 8);
                SoundStyle sandDash = AssetRegistry.Sounds.LeviathanEel.SandDash;
                sandDash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(sandDash, MyTarget.position);
            }


            float dashSpeed = 45;
            Vector2 targetDashVelocity = _dashDirection * dashSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetDashVelocity, 0.3f);
            _facingDirection = NPC.velocity.SafeNormalize(Vector2.Zero);
        }
        else
        {
            if (Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BreatheBubble);
                var p = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.Red);
                p.Scale *= 0.33f;
            }

            if (Timer >= 90)
            {
                SwitchState(AIState.Idle);
            }
        }
        //  SwitchState(AIState.Idle);
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
            AIState pattern = PatternManager.NextPattern();
            SwitchState(pattern);
            if(_attackToTest != AIState.SpawnIntro)
            {
                SwitchState(_attackToTest);
            }
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
        OvalTimer++;
        //For this movement we'll just have him moving in an oval
        //But he should have a lot of different movement types
        //First lets make that oval shape
        Vector2 pointOnOval = GetNextPointToTrack(OvalTimer);


        Vector2 vectorToOvalPoint = pointOnOval - NPC.Center;
        vectorToOvalPoint = vectorToOvalPoint.SafeNormalize(Vector2.Zero);

        Vector2 targetVelocity = vectorToOvalPoint;
        float movementSpeed = MathHelper.SmoothStep(3f, 10f, EasingFunction.InOutSine(OvalTimer / 120f));
        targetVelocity *= movementSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.15f);
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.2f);
        if(Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        // throw new NotImplementedException();
    }
}
