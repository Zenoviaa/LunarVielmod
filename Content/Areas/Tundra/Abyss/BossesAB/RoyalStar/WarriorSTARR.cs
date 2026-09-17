using Stellamod.Common.Particles;
using Stellamod.Content.Rendering.GenericEffects;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR : ScarletBoss, IDrawToRenderTarget
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        BoulderKick,
        JumpRockSlam,
        WindUpPunch,
        FakeOutPunch,
        DiscThrow,
        Laugh,
        CapeOut,
        AnkleBreakerMaybe,
        CommandGrab,
        RockSpikeRun,
        Phase2Transition,
        Death
    }

    private Vector2 _jumpScale;
    private bool _contactDamage;
    public override string Texture => TextureRegistry.EmptyTexture;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    public bool Phase2Active { get; private set; }
    private bool ShouldBeInPhase2
    {
        get
        {
            return NPC.life < NPC.lifeMax / 2f;
        }
    }
    private float IdleTime
    {
        get
        {
            if (Phase2Active)
            {
                return 150;
            }
            return 180;
        }
    }

    private bool _canFakeOut;
    private float _starRot;
    private float _medalAlpha;
    private float _medalScale;
    private float _bigStarAlpha;
    private float _jumpingTrailAlpha;
    private float _afterImageAlpha;
    private bool _afterImages;
    private bool _fakeOut;
    private bool _eyeFlash;
    private bool _jumpingTrail;
    private bool _grabbing;
    private int _grabbedPlayer;
    private Vector2 _teleportPosition;
    private Vector2 _vector1;
    private Vector2 _vector2;
    private Vector2 _initialVelocity;
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_KICK_UP = "KickUp";
    private const string ANIM_KICK_DOWN = "KickDown";
    private const string ANIM_PUNCH_READY = "PunchReady";
    private const string ANIM_PUNCH = "Punch";
    private const string ANIM_CROUCH = "Crouch";
    private const string ANIM_PUNCH_DOWN_READY = "PunchDownReady";
    private const string ANIM_PUNCH_DOWN = "PunchDown";
    private const string ANIM_PUNCH_DOWN_OUT = "PunchDownOut";
    private const string ANIM_CAPE_OUT = "CapeOut";
    private const string ANIM_CAPE_IN = "CapeIn";
    private const string ANIM_DISC_THROW = "DiscThrow";
    private const string ANIM_RUN = "Run";
    private const string ANIM_SPIN_KICK = "SpinKick";
    private const string ANIM_SPIN_KICK_READY = "SpinKickReady";
    private const string ANIM_LOW_KICK = "LowKick";
    private const string ANIM_LOW_KICK_READY = "LowKickReady";
    private const string ANIM_GRAB_READY = "GrabReady";
    private const string ANIM_GRAB_START_SLOW = "GrabStartSlow";
    private const string ANIM_GRAB_TRY = "GrabTry";
    private const string ANIM_GRAB_THROW = "GrabThrow";
    private const string ANIM_GRAB_NO = "GrabNo";
    private const string ANIM_JUMP = "Jump";
    private const string ANIM_PUNCH_BACK = "PunchBack";
    private const string ANIM_PUNCH_BACKAIR = "PunchBackair";
    private const string ANIM_PUNCH_BACKGROUND = "Punchbackground";
    private const string ANIM_JUMPFRAME = "Jumpframe";
    private const string ANIM_JUMPTOHOVER = "JumpTohover";

    private const string ANIM_GRAB_IDLE = "GrabIdle";

    private const string ANIM_NEPHEW = "Nephew";

    private const string ANIM_HUH = "Huh";
    private PatternManager<AIState> _patternBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternBackingField == null)
            {
                _patternBackingField = new();
                _patternBackingField.AddPattern(AIState.BoulderKick, 1.0f);
                _patternBackingField.AddPattern(AIState.WindUpPunch, 1.0f);
                _patternBackingField.AddPattern(AIState.JumpRockSlam, 1.0f);
                _patternBackingField.AddPattern(AIState.RockSpikeRun, 1.0f);
            }
            return _patternBackingField;
        }
    }

    private PatternManager<AIState> _patternBackingFieldp2;
    private PatternManager<AIState> PatternManagerP2
    {
        get
        {
            if (_patternBackingFieldp2 == null)
            {
                _patternBackingFieldp2 = new();
                _patternBackingFieldp2.AddPattern(AIState.BoulderKick, 1.0f);
                _patternBackingFieldp2.AddPattern(AIState.WindUpPunch, 1.0f);
                _patternBackingFieldp2.AddPattern(AIState.JumpRockSlam, 1.0f);
                _patternBackingFieldp2.AddPattern(AIState.RockSpikeRun, 1.0f);
                _patternBackingFieldp2.AddPattern(AIState.AnkleBreakerMaybe, 1.0f);
                _patternBackingFieldp2.AddPattern(AIState.CommandGrab, 1.0f);
            }
            return _patternBackingFieldp2;
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(Phase2Active);
        writer.WriteVector2(_vector1);
        writer.WriteVector2(_vector2);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_initialVelocity);
        writer.Write(_fakeOut);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        Phase2Active = reader.ReadBoolean();
        _vector1 = reader.ReadVector2();
        _vector2 = reader.ReadVector2();
        _teleportPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _fakeOut = reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCSets.UseAseprite[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 80;
        NPC.height = 96;
        NPC.damage = 100;
        NPC.defense = 23;
        NPC.lifeMax = 22000;

        NPC.scale = 1f;
        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0;
        NPC.boss = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ROYALSTARR");
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && (_contactDamage || _grabbing);
    }

    public override bool AllowNameplateToBeShown()
    {
        return State != AIState.Spawn && State != AIState.Despawn;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
        }
    }

    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
                SwitchState(AIState.Despawn);
        }

        if (_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            NPC.velocity = Vector2.Zero;
            _teleportPosition = Vector2.Zero;
        }

        _contactDamage = false;
        _jumpingTrail = false;
        _grabbing = false;
        _afterImages = false;
        _bigStarAlpha *= 0.92f;
        _medalAlpha *= 0.92f;
        _outliner.SetDefaults();
        _starRot *= 0.92f;
        _jumpScale = Vector2.Lerp(_jumpScale, Vector2.One, 0.2f);
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
            case AIState.BoulderKick:
                AI_BoulderKick();
                break;
            case AIState.JumpRockSlam:
                AI_JumpRockSlam();
                break;
            case AIState.CapeOut:
                AI_CapeOut();
                break;
            case AIState.WindUpPunch:
                AI_WindUpPunch();
                break;
            case AIState.FakeOutPunch:
                AI_WindUpPunch();
                break;
            case AIState.DiscThrow:
                AI_DiscThrow();
                break;
            case AIState.AnkleBreakerMaybe:
                AI_AnkleBreakerMaybe();
                break;
            case AIState.CommandGrab:
                AI_CommandGrab();
                break;
            case AIState.RockSpikeRun:
                AI_RockSpikeRun();
                break;
            case AIState.Phase2Transition:
                AI_Phase2Transition();
                break;
            case AIState.Death:
                AI_Death();
                break;
        }
        _jumpingTrailAlpha = MathHelper.Lerp(_jumpingTrailAlpha, _jumpingTrail ? 1f : 0f, 0.1f);
        _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, _afterImages ? 1f : 0f, 0.1f);
        this.SetDrawOrigin(new Vector2(68, 114));
        _outliner.Update();
    }

    private void AI_Phase2Transition()
    {
        _canFakeOut = false;
        Timer++;
        if(Timer == 30)
        {
            var sound = AssetReferences.Assets.Sounds.STARR.STARRNephew1.Asset;
            sound.Volume = 0.5f;
            SoundEngine.PlaySound(sound, NPC.position);
        } 
        
        if(Timer < 130)
        {
            FXUtil.SetZoomTarget(1.2f);
            this.AseAnimator.PlayAnimation(ANIM_NEPHEW, AnimationParams.Default);
        }
        else
        {
            if (Timer % 2 == 0)
            {
                Vector2 pos = NPC.Center;
                pos.Y += Main.rand.NextFloat(-128, 128);
                Particles.GoldenLeaf.Spawn(GoldenLeaf.Data.Default with { rootPosition = pos, timeLeft = 150 });
            }
            FXUtil.SetZoomTarget(1.7f);
            this.AseAnimator.PlayAnimation(ANIM_HUH, AnimationParams.Default);
        }

        if(Timer == 130)
        {
            var sound = AssetReferences.Assets.Sounds.STARR.STARRGetAJob.Asset;
            sound.Volume = 0.5f;
            SoundEngine.PlaySound(sound, NPC.position);
        }

        StayGrounded();
        FaceTarget();

        Phase2Active = true;
        NPC.velocity.X *= 0.96f;
        CameraTargetSystem.AddTarget(NPC.Center);
        if (Timer >= 180)
        {
            SwitchState(AIState.AnkleBreakerMaybe);
        }
    }

    private void AI_Death()
    {
        Timer++;
        if (Timer == 30)
        {
            var sound = AssetReferences.Assets.Sounds.STARR.STARRItsOver.Asset;
            sound.Volume = 0.5f;
            SoundEngine.PlaySound(sound, NPC.position);
        }


        if(Timer >= 35 && Timer <= 70)
        {
            FXUtil.SetZoomTarget(1.2f);

        }
        else if (Timer >= 70)
        {
            FXUtil.SetZoomTarget(1.7f);

        }
        this.AseAnimator.PlayAnimation(ANIM_HUH, AnimationParams.Default);

        StayGrounded();
        FaceTarget();
        NPC.velocity.X *= 0.96f;
        CameraTargetSystem.AddTarget(NPC.Center);
        if (Timer >= 180)
        {
            NPC.Kill();
        }
    }

    private void AI_Spawn()
    {
        Timer++;
        StayGrounded();
        FaceTarget();
        this.AseAnimator.PlayAnimation(ANIM_IDLE);
        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Despawn()
    {
        NPC.active = false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        base.OnHitPlayer(target, hurtInfo);
        if (_grabbing)
        {
            _grabbedPlayer = target.whoAmI;
            NPC.netUpdate = true;
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            if (Phase2Active)
            {
                SwitchState(PatternManagerP2.NextPattern());
            }
            else
            {
                SwitchState(PatternManager.NextPattern());
            }
            if (State != AIState.Death && NPC.life <= 1)
                SwitchState(AIState.Death);
        }
    }


    private void AI_Idle()
    {
        _canFakeOut = true;
        Timer++;
        if(Timer == 1)
        {
            GruntSound();
            TeleportOutEffect(NPC.Center);
            Vector2 pos = NPC.Center;
            pos = TileUtilities.FallToSolidTile(pos.ToTileCoordinates()).ToWorldCoordinates();
            pos.Y -= 80;
            Teleport(pos);

            TeleportEffect(pos);
        }
        NPC.velocity.X *= 0.96f;
        NPC.noGravity = false;
        NPC.rotation *= 0.92f;
        StayGrounded();
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        FaceTarget();
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
        else if (ShouldBeInPhase2 && IsGrounded() && !Phase2Active)
        {
            SwitchState(AIState.Phase2Transition);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            NPC.life = 1;

        }
    }
    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 96, completionRatio) * 0.3f;
    }
    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }
    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio) *
            _jumpingTrailAlpha * EasingFunction.QuadraticBump(completionRatio * completionRatio);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_afterImageAlpha > 0.03f)
        {
            var whitePass = AssetReferences.Effects.CrystalShaders.SpriteWhite.CreatePixelPass();
            whitePass.Apply();
            using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = whitePass.Shader }))
            {
                foreach (OldPosition oldPos in new OldPositionEnum(NPC.oldPos))
                {
                    SpritebatchDrawer drawInfo = NPC.GetAnimatorDrawInfo(drawColor);
                    Vector2 offset = drawInfo.drawOrigin - this.AseAnimator.centerDrawOrigin;
                    drawInfo.worldPosition = oldPos.position + NPC.Size * 0.5f;
                    drawInfo.worldPosition += offset;
                    drawInfo.color = Color.Lerp(Color.Green, Color.Transparent, oldPos.progress) * 0.3f * _afterImageAlpha;
                    drawInfo.color.A = 0;
                    spriteBatch.Draw(drawInfo);
                }
            }
        }

        SpritebatchDrawer bossDrawInfo = NPC.GetAnimatorDrawInfo(drawColor);
        bossDrawInfo.scale = _jumpScale;
        spriteBatch.Draw(bossDrawInfo);

        return false;
    }

    private void DrawOutlineWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
    }

    private void DrawPixelatedSTARR(SpriteBatch spriteBatch, Vector2 sp)
    {
        var starAsset = AssetReferences.Assets.GlowMasks.FivePointedStar.Asset;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(starAsset, NPC.Center);
        drawer.color = Color.Lerp(Color.Transparent, Color.Goldenrod, _bigStarAlpha) * 0.45f;
        drawer.rotation = MathHelper.Lerp(3.14f, 0f, _bigStarAlpha) + _starRot;
        spriteBatch.Draw(drawer);

        drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
    private void DrawPixelatedSTARRMedal(SpriteBatch spriteBatch, Vector2 sp)
    {
        {
            var starAsset = AssetReferences.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.WarriorStarMedal.Asset;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(starAsset, NPC.Center);
            drawer.color = Color.DarkOrange * _medalAlpha * 0.4f;
            drawer.color.A = 0;
            drawer.scale = Vector2.One * _medalScale;
            drawer.rotation = Main.GlobalTimeWrappedHourly * 2;
            spriteBatch.Draw(drawer);
            spriteBatch.Draw(drawer);


            drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);
        }
        {
            var starAsset = AssetReferences.Assets.GlowMasks.FivePointedStar.Asset;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(starAsset, NPC.Center);
            drawer.color = Color.DarkOrange * _medalAlpha;
            drawer.color.A = 0;
            drawer.scale = Vector2.One * _medalScale * 2f;
            drawer.rotation = -Main.GlobalTimeWrappedHourly * 4;
            spriteBatch.Draw(drawer);

            drawer.color *= ExtraMath.Osc(0.25f, 1f, speed: 64);
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);
        }
    }
    public void DrawToRenderTargets()
    {
        if (_jumpingTrailAlpha > 0.03f)
        {
            var verts1 = DrawUtilities.PrepareSimpleTrailing(NPC.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, NPC.Size * 0.5f);
            var verts2 = DrawUtilities.PrepareSimpleTrailing(NPC.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, NPC.Size * 0.5f);
            ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForBigRendering(verts2);
            ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForRendering(verts1);
        }
        if (_bigStarAlpha > 0.03f)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSTARR, DrawLayer.OverPlayers);
        }
        if (_medalAlpha > 0.03f)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSTARRMedal, DrawLayer.OverPlayers);
        }
        OutlineRenderer.Queue(DrawOutlineWhite);

    }
}
