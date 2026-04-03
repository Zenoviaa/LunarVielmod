using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;

/*
 * 
 *
 *X appears directly on the ground right underneath you and after a moment, 
 *a sound cue comes in too before and Steamroller drills through the ground at a fast pace, 
 *shooting into the air and then waiting a moment before trying to drill back down on top of you, you have to dodge twice

Steamroller pops its head out like snagrets and tries to start drilling on top of you but gets stuck in the ground with its head and he starts drilling, 
creating a bunch of flying rocks that come out to hit you

Steamroller comes out and starts to shoot little bombs from the side with like cool spell circles 
and stuff while being up in the air arched over

Dune jump, where he comes out of the ground over you and leaps over basically, you just have to not move for this

You see rocks rumbling under the ground as he starts doing a dung defender type attack, and stops and pokes his head out and goes back in for a minute

Phase two, he splits in half and basically this one goes on the other side of you, or it tries to attack right after the other, 
since this is a slow timing boss this will work

Pops off its head as it comes out the ground and shoots itself at you, detaching itself as the rest of the body goes underground,
the head drills into the ground as well and you just have to dodge really, it goes back underground after this attack to reconnect

 */


public class SteamrollerBody : ModNPC,
    IDrawOutlines
{
    private Vector2 _squishScale;
    private NPC Parent
    {
        get => Main.npc[(int)NPC.ai[0]];
    }

    private enum AIState
    {
        Spin_Slow,
        Spin_Fast,
        Cannon_ComeOut,
        Cannon_Shoot,
        Cannon_Idle
    }

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private int VerletIndex
    {
        get => (int)NPC.ai[2];
        set => NPC.ai[2] = (float)value;    
    }
    private bool _hasOffset;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 29;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _squishScale = Vector2.One;
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 100;
        NPC.defense = 14;
        NPC.lifeMax = 8000;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
    }
    private const string Anim_SpinSlow = "spinslow";
    private const string Anim_SpinFast = "spinfast";
    private const string Anim_CannonComeOut = "cannoncomeout";
    private const string Anim_CannonShoot = "cannonshoot";
    private const string Anim_CannonIdle = "cannonidle";
    public Animator _animator;
    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = new Animator();
                var idle = new SpriteAnimation(0, 3, isLooping: true);
                _animator.AddAnimation(Anim_SpinSlow, idle);

                var running = new SpriteAnimation(4, 12, isLooping: true, frameSpeed: 0.35f);
                _animator.AddAnimation(Anim_SpinFast, running);

                var cannotComeOut = new SpriteAnimation(12, 18, isLooping: false);
                _animator.AddAnimation(Anim_CannonComeOut, cannotComeOut);

                var cannotShoot = new SpriteAnimation(18, 28, isLooping: false);
                _animator.AddAnimation(Anim_CannonShoot, cannotShoot);

                var cannonIde = new SpriteAnimation(28, 29, isLooping: true);
                _animator.AddAnimation(Anim_CannonIdle, cannonIde);
            }

            return _animator;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        if (!_hasOffset)
        {
            for(int i =0; i < VerletIndex; i++)
            {
                Animator.Update();
                Animator.Update();
            }
            _hasOffset = true;
        }
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    public override void AI()
    {
        base.AI();
        if(Parent.ModNPC is Steamroller steamroller)
        {
            Vector2 targetPosition = steamroller.GetSegmentPosition(VerletIndex);
            Vector2 npcVelocity = targetPosition - NPC.Center;
            NPC.velocity = npcVelocity;

            Vector2 nextPosition = steamroller.GetSegmentPosition(VerletIndex - 1);
            Vector2 diff = nextPosition - targetPosition;
            float angle = diff.ToRotation();
            angle += MathHelper.PiOver2;
            NPC.rotation = angle;
        }

        switch (State)
        {
            case AIState.Spin_Slow:
                AI_SpinSlow();
                break;
            case AIState.Spin_Fast:
                AI_SpinFast();
                break;
            case AIState.Cannon_ComeOut:
                AI_CannonComeOut();
                break;
            case AIState.Cannon_Shoot:
                AI_CannonShoot();
                break;
            case AIState.Cannon_Idle:
                AI_CannonIdle();
                break;
        }
    }

    private void AI_SpinSlow()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
    }
    private void AI_SpinFast()
    {
        Animator.PlayAnimation(Anim_SpinFast);
    }
    private void AI_CannonComeOut()
    {
        Animator.PlayAnimation(Anim_CannonComeOut);
    }
    private void AI_CannonShoot()
    {
        Animator.PlayAnimation(Anim_CannonShoot);
    }
    private void AI_CannonIdle()
    {
        Animator.PlayAnimation(Anim_CannonIdle);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    private Vector2 GetDrawOrigin()
    {
        if (_animator == null)
            return NPC.frame.Size() / 2f;
        Vector2? drawOrigin = _animator.GetDrawOrigin();
        if (drawOrigin.HasValue)
            return drawOrigin.Value;
        return NPC.frame.Size() / 2f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (VerletIndex == 0)
        {
            if (Parent.ModNPC is Steamroller steamroller)
            {
                steamroller.Draw(spriteBatch, screenPos, drawColor);
            }
                return false;
        }
     
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - screenPos;
        //drawPos.Y += NPC.Size.Y / 2;

        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        return false;
    }



    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {

    }
}
public class Steamroller : ScarletBoss,
    IDrawOutlines
{
    private const string Anim_SpinSlow = "spinslow";
    private const string Anim_SpinFast = "spinfast";
    private enum AIState
    {
        IdleDrill,

        X_Drill_Start,
        X_Drill_Rise,
        X_Drill_Fall,

        Snagret_PopStart,
        Snagret_PopRise,
        Snagret_PopFallNStuckk,

        DuneJump_Start,
        DuneJump_Fall,

        DungDefenderRock_Start,
        DungDefenderRock_Blast,
        DungDefenderRock_End,

        Phase_Transition,

        Cannon_Start,
        Cannon_Fire,
        Cannon_End,

        HeadPop_Start,
        HeadPop_Drill,
        HeadPop_Fall
    }
    
    private bool _contactDamage;
    private Color _targetOutlineColor;
    private Color _outlineColor;
    private Vector2 _squishScale;
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if(_animator == null)
            {
                _animator = new Animator();
                var idle = new SpriteAnimation(0, 3, isLooping: true);
                _animator.AddAnimation(Anim_SpinSlow, idle);

                var running = new SpriteAnimation(4, 12, isLooping: true, frameSpeed: 0.35f);
                _animator.AddAnimation(Anim_SpinFast, running);
            }
 
            return _animator;
        }
    }

    public VerletChain _verletChain;
    public VerletChain VerletChain
    {
        get
        {
            if(_verletChain == null)
            {
                _verletChain = new VerletChain(NPC.Center, NPC.Center + Vector2.UnitX * 1000, 100);
            }
            return _verletChain;
        }
    }
    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if(_patternManager == null)
            {
                _patternManager = new PatternManager<AIState>();
            }
            return _patternManager;
        }
    }
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    public Vector2 GetSegmentPosition(int verletIndex)
    {
        if (verletIndex < 0)
            return NPC.Center;

        return VerletChain.points[verletIndex].position;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 12;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }


    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _squishScale = Vector2.One;
        NPC.width = 128;
        NPC.height = 128;
        NPC.damage = 100;
        NPC.defense = 28;
        NPC.lifeMax = 18000;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SitriAndTheMechs");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
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

    public override void AI()
    {
        base.AI();
        if (MultiplayerHelper.IsHost)
        {
            for(int i = VerletChain.points.Length - 1; i >= 0; i--)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)VerletChain.points[i].position.X, (int)VerletChain.points[i].position.Y, 
                    ModContent.NPCType<SteamrollerBody>(), 
                    ai0: NPC.whoAmI, ai2: i); 
            }
        }

        _contactDamage = false;
        _targetOutlineColor = Color.Transparent;
        switch (State)
        {
            case AIState.IdleDrill:
                AI_IdleDrill();
                break;
            case AIState.X_Drill_Start:
                AI_XDrillStart();
                break;
            case AIState.X_Drill_Rise:
                AI_XDrillRise();
                break;
            case AIState.X_Drill_Fall:
                AI_XDrillFall();
                break;
        }
        VerletChain.segmentLength = 92;
        VerletChain.noTileCollide = true;
        VerletChain.points[0].pinned = true;
        VerletChain.points[0].position = NPC.Center;
        VerletChain.gravity = 0;
        VerletChain.Update();
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.3f);
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
        }
    }

    private void AI_IdleDrill()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }
        Vector2 vel = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        NPC.velocity = vel * 5;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
    }

    private void AI_XDrillStart()
    {

    }

    private void AI_XDrillRise()
    {

    }

    private void AI_XDrillFall()
    {

    }
    private Vector2 GetDrawOrigin()
    {
        if (_animator == null)
            return NPC.frame.Size() / 2f;
        Vector2? drawOrigin = _animator.GetDrawOrigin();
        if (drawOrigin.HasValue)
            return drawOrigin.Value;
        return NPC.frame.Size() / 2f;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - screenPos;
        //drawPos.Y += NPC.Size.Y / 2;

        Vector2 offset = VerletChain.points[1].position - VerletChain.points[0].position;
        drawPos += offset * 0.33f;
        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
     //   throw new NotImplementedException();
    }
}
