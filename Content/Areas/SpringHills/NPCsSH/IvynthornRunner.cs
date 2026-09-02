using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class IvynthornRunner : ModNPC
{
    private enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Turn,
        Jump
    }

    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }


    private float IdleTime => 60;
    private float AggroRange => 200;
    private float ChaseRange => 400;
    private float MaxPatrolWalkSpeed => 1.25f;
    private float MaxChaseSpeed => 2.5f;

    private Color _outlineColor;
    private bool _isDangerous;
    private bool _isWarning;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        Main.npcFrameCount[Type] = 14;
        this.AddToSpringHills();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.lifeMax = 50;
        NPC.defense = 0;
        NPC.damage = 10;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = false;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _isDangerous;
    }
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0f;
        }

        switch (State)
        {
            default:
            case AIState.Idle:
                if (_frame >= 4f)
                {
                    _frame = 0;
                }
                break;
            case AIState.Chase:
            case AIState.Patrol:
                if (_frame < 5 || _frame >= 13f)
                {
                    _frame = 5;
                }
                break;
            case AIState.Turn:
                _frame = 13;
                break;
            case AIState.Jump:
                _frame = 4;
                break;
        }

        NPC.frame.Y = frameHeight * _frame;
    }
    private void SwitchState(AIState state)
    {
        State = state;
        Timer = 0;
        NPC.netUpdate = true;
    }

    public override void AI()
    {
        base.AI();

        _isDangerous = false;
        _isWarning = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Patrol:
                AI_Patrol();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Turn:
                AI_Turn();
                break;
            case AIState.Jump:
                AI_Jump();
                break;
        }
        if(_isDangerous)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.2f);
        } else if (_isWarning)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.2f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.2f);
        }
    }

    private void AI_Idle()
    {

        Timer++;
        NPC.velocity.X *= 0.9f;
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Turn);
        }
    }

    private void AI_Patrol()
    {
        Timer++;
        NPC.spriteDirection = -NPC.direction;
        NPC.rotation = NPC.velocity.X * 0.005f;
        float currentSpeed = NPC.velocity.X;
        float acceleration = 0.5f;
        if (currentSpeed > -MaxPatrolWalkSpeed && NPC.direction == -1)
        {
            NPC.velocity.X -= acceleration;
            if (NPC.velocity.X < -MaxPatrolWalkSpeed)
            {
                NPC.velocity.X = -MaxPatrolWalkSpeed;
            }
        }
        else if (currentSpeed < MaxPatrolWalkSpeed && NPC.direction == 1)
        {
            NPC.velocity.X += acceleration;
            if (NPC.velocity.X > MaxPatrolWalkSpeed)
            {
                NPC.velocity.X = MaxPatrolWalkSpeed;
            }
        }

        if (NPC.collideX)
        {
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        //Check fi should turn
        if (Timer >= 400)
        {
            SwitchState(AIState.Turn);
        }

        NPC.TargetClosest();
        if (NPC.HasValidTarget)
        {
            Player target = Main.player[NPC.target];
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            float yDistance = MathF.Abs(target.Bottom.Y - NPC.Bottom.Y);
            if (distanceToTarget <= AggroRange && yDistance <= 16)
            {
                SwitchState(AIState.Jump);
            }
        }
    }

    private void AI_Chase()
    {
        Timer++;
        NPC.spriteDirection = -NPC.direction;
        NPC.rotation = NPC.velocity.X * 0.025f;

        float currentSpeed = NPC.velocity.X;
        float maxSpeed = MaxChaseSpeed;
        float acceleration = 0.5f;

        float speedInterpolant = MathHelper.Clamp(Timer / 60f, 0, 1);
        maxSpeed *= MathHelper.Lerp(0f, 2, speedInterpolant);
        if (currentSpeed > -maxSpeed && NPC.direction == -1)
        {
            NPC.velocity.X -= acceleration;
            if (NPC.velocity.X < -maxSpeed)
            {
                NPC.velocity.X = -maxSpeed;
            }
        }
        else if (currentSpeed < maxSpeed && NPC.direction == 1)
        {
            NPC.velocity.X += acceleration;
            if (NPC.velocity.X > maxSpeed)
            {
                NPC.velocity.X = maxSpeed;
            }
        }

        if (Timer >= 30)
        {
            _isDangerous = true;
        }

        if (NPC.collideX)
        {
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }

        if (NPC.HasValidTarget)
        {
            Player target = Main.player[NPC.target];
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            if (distanceToTarget > ChaseRange)
            {
                SwitchState(AIState.Idle);
            }
        }
        else
        {

            SwitchState(AIState.Idle);
        }
    }

    private void AI_Turn()
    {
        Timer++;
        if (Timer == 30)
        {
            if (NPC.direction == 0)
                NPC.direction = 1;
            NPC.direction *= -1;
            SwitchState(AIState.Patrol);
        }
        NPC.velocity.X *= 0.95f;
        NPC.rotation *= 0.99f;

    }

    private void AI_Jump()
    {
        _isWarning = true;
        Timer++;
        if (Timer == 1)
        {
            NPC.velocity.Y -= 5;
            var jumpSound = AssetReferences.Assets.Sounds.LilJump.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(jumpSound, NPC.position);
        }

        NPC.velocity.X *= 0.9f;
        NPC.rotation = 0;
        if (Timer > 10 && NPC.collideY)
        {
            SwitchState(AIState.Chase);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
        drawPos.Y -= 5;
        float outlineOffset = 2;
        Vector2 left = Vector2.UnitX * -outlineOffset;
        Vector2 right = Vector2.UnitX * outlineOffset;
        Vector2 up = Vector2.UnitY * -outlineOffset;
        Vector2 down = Vector2.UnitY * outlineOffset;
        SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

        Color outlineColor = _outlineColor;
        Vector2 drawOrigin = NPC.frame.Size() / 2;
        spriteBatch.Restart(effect: whiteShader.Effect);

        spriteBatch.Draw(texture, drawPos + left, NPC.frame, outlineColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + right, NPC.frame, outlineColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + up, NPC.frame, outlineColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        spriteBatch.Draw(texture, drawPos + down, NPC.frame, outlineColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);

        spriteBatch.RestartDefaults();
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ivythorn>(), minimumDropped: 1, maximumDropped: 3));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life <= 0)
        {
            //Create gores
            for (int k = 0; k < 2; k++)
            {
                Vector2 pos = NPC.position;
                pos.X += Main.rand.Next(0, NPC.width);
                pos.Y += Main.rand.Next(0, NPC.height);
                DustParticle dp = Particle<DustParticle>.Spawn(pos, Vector2.UnitX * hit.HitDirection * Main.rand.NextFloat(1f, 4f), Scale: 0.5f);
                dp.outerColor = Color.DarkGray;
                dp.gravity = 0.01f;
                dp.fast = true;
            }


            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Top").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Bottom").Type;

            // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity + new Vector2(0, -3), headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity + new Vector2(0, -3), legGore);
        }
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
