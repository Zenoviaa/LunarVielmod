using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class MeleeHitbox : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[2];
    private int HitboxWidth => (int)Projectile.ai[0];
    private int HitboxHeight => (int)Projectile.ai[1];

    public static MeleeHitbox Create(IEntitySource source, Vector2 position, int width, int height, int lifetime, int damage, float kb)
    {
        Projectile p = Projectile.NewProjectileDirect(source, position, Vector2.Zero, 
            ModContent.ProjectileType<MeleeHitbox>(), damage, kb, Main.myPlayer, ai0: width, ai1: height, ai2: lifetime);
        return p.ModProjectile as MeleeHitbox;
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override void AI()
    {
        base.AI();
        Timer--;
        if (Timer <= 0)
            Projectile.Kill();
    }
}
public class Overseer : ModNPC,
    IDrawOutlines
{
    private enum AIState : byte
    {
        Idle,
        Walk,
        Chase,
        Swipe_Start,
        Swipe
    }

    private AIState State
    {
        get => (AIState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }

    private ref float Timer => ref NPC.ai[1];
    private Player PlayerTarget => Main.player[NPC.target];

    private bool _contactDamage;
    private Color _outlineColor;
    private Color _targetOutlineColor;
    private Vector2 _breathingScale;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 24;
        this.AddToAbyss();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 100;
        NPC.height = 90;
        NPC.lifeMax = 480;
        NPC.defense = 8;
        NPC.noTileCollide = false;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.DeathSound = SoundID.NPCDeath46;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
    }

    private const string Anim_Idle = "idle";
    private const string Anim_Walk = "walk";
    private const string Anim_Swipe_Read = "swipeready";
    private const string Anim_Swipe = "swipe";
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
                SetupAnimator();
            return _animator;
        }
    }
    private int SwipeDamage => 40;
    private void SetupAnimator()
    {
        _animator = new Animator();
        Vector2 animationDrawOrigin = new Vector2(44, 62) * 2;
        var idle = new SpriteAnimation(0, 0, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Idle, idle);

        var walk = new SpriteAnimation(1, 8, isLooping: true, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Walk, walk);

        var swipeReady = new SpriteAnimation(9, 14, isLooping: false, drawOriginOverride: animationDrawOrigin);
        _animator.AddAnimation(Anim_Swipe_Read, swipeReady);

        var swipe = new SpriteAnimation(15, 22, isLooping: false, drawOriginOverride: animationDrawOrigin);
        swipe.frameSpeed = 0.25f;
        _animator.AddAnimation(Anim_Swipe, swipe);
    }


    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void AI()
    {
        base.AI();

        _targetOutlineColor = Color.Transparent;
        _contactDamage = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Walk:
                AI_Walk();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Swipe_Start:
                AI_SwipeStart();
                break;
            case AIState.Swipe:
                AI_Swipe();
                break;
        }

        float stepSpeed = 1;
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref stepSpeed, ref NPC.gfxOffY);
        _breathingScale = Vector2.Lerp(Vector2.One, new Vector2(1.05f, 0.95f), ExtraMath.Osc(0f, 1f));
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.2f);
        NPC.spriteDirection = NPC.direction;
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

    private bool IsTargetChaseable()
    {
        //It has to be looking at you to aggro
        float distToTarget = Vector2.Distance(NPC.Center, PlayerTarget.Center);
        int facingDirection = NPC.direction;
        Vector2 lookDirection = Vector2.UnitX * facingDirection;
        Vector2 directionToPlayer = PlayerTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
        float dp = Vector2.Dot(directionToPlayer, lookDirection);
        float yDist = MathF.Abs(PlayerTarget.Bottom.Y - NPC.Bottom.Y);
        return dp > 0 && yDist <= 64 && NPC.HasValidTarget ;
    }

    private void AI_Idle()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }

        Animator.PlayAnimation(Anim_Idle);
        NPC.velocity.X *= 0.9f;
        if(IsTargetChaseable())
        {
            SwitchState(AIState.Chase);
        } else if (Timer > 120)
        {
            SwitchState(AIState.Walk);
        }
    }

    private void WalkParticles()
    {
        if (Timer % 8 == 0)
        {
            Vector2 pos = NPC.Bottom;
            pos.X += Main.rand.NextFloat(-8f, 8f);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;

            DustParticle dp = DustParticle.Spawn(pos, -Vector2.UnitY, spawnParams);
            dp.noTileCollide = true;
            dp.gravity *= 0.2f;
            dp.Scale *= 0.5f;
        }

    }
    private void AI_Walk()
    {
        Timer++;

        NPC.TargetClosest(false);
        if(Timer >= 360)
        {
            SwitchState(AIState.Idle);
        }

        Animator.PlayAnimation(Anim_Walk);
        Vector2 walkVelocity = Vector2.UnitX * NPC.direction;
        walkVelocity *= 1.5f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, walkVelocity.X, 0.1f);
        if (NPC.collideX)
            NPC.direction *= -1;
        if (IsTargetChaseable())
        {
            SwitchState(AIState.Chase);
        }
        WalkParticles();
    }

    private void AI_Chase()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest(false);
        }
       
        Animator.PlayAnimation(Anim_Walk);

        NPC.direction = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
        float movementSpeed = 3f;
        Vector2 walkVelocity = Vector2.UnitX * NPC.direction;
        walkVelocity *= movementSpeed;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, walkVelocity.X, 0.1f);
        float xDist = MathF.Abs(PlayerTarget.Center.X - NPC.Center.X);
        float yDist = MathF.Abs(PlayerTarget.Center.Y - NPC.Center.Y);
        if (xDist < 100)
        {
            SwitchState(AIState.Swipe_Start);
        }
        else if (xDist > 500 || yDist > 200) 
        {
            SwitchState(AIState.Idle);
        }
        WalkParticles();
    }

    private void AI_SwipeStart()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/BoneCrackle");
            chargeSound.PitchVariance = 0.3f;
            chargeSound.Volume = 1f;
            SoundEngine.PlaySound(chargeSound, NPC.position);
        }
        Animator.PlayAnimation(Anim_Swipe_Read);
        NPC.velocity.X *= 0.8f;
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Swipe);
        }
        _targetOutlineColor = Color.Yellow;
    }

    private void AI_Swipe()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune_Fade");
            chargeSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(chargeSound, NPC.position);
            FXUtil.ShakeCamera(NPC.Center, 1024, 4);
            if (MultiplayerHelper.IsHost)
            {
                Vector2 position = NPC.Center;
                position += Vector2.UnitX * NPC.direction * 64;
                MeleeHitbox.Create(NPC.GetSource_FromThis(), position, width: 64, height: 128, 20, 
                    damage: SwipeDamage, kb: 1);
            }
        }

        if(Timer < 30)
        {
            _contactDamage = true;
        }
        Animator.PlayAnimation(Anim_Swipe);
        NPC.velocity.X *= 0.9f;

        if (Animator.IsFinished())
        {
            SwitchState(AIState.Idle);
        }
        _targetOutlineColor = Color.Red;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }

    public override void OnKill()
    {
        base.OnKill();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConvulgingMater>(), minimumDropped: 1, maximumDropped: 4));
    }

    #region Draw Code
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
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - screenPos;
        drawPos.Y += NPC.Size.Y / 2;

        Vector2 drawOrigin = GetDrawOrigin();

        float drawRotation = NPC.rotation;
        Vector2 drawScale = NPC.scale * Vector2.One;
        drawScale *= _breathingScale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;
        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);

        /*
        Texture2D texture2 = AssetManager.GlowMask.SpiralVortex2.Value;
        Vector2 drawOrigin2 = texture2.Size() * 0.5f;
        spriteBatch.Draw(texture2, NPC.Center - screenPos, null, drawColor, 0, drawOrigin2, 0.1f, SpriteEffects.None, 0);*/
        return false;
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - Main.screenPosition;
        drawPos.Y += NPC.Size.Y / 2;

        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = NPC.scale * Vector2.One;
        drawScale *= _breathingScale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;


        float outlineOffset = 2;
        Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
        Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
        Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
        Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
        Color outlineColor = _outlineColor;

        spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }
    #endregion

}
