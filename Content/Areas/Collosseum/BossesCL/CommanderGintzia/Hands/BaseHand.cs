using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands;

public abstract class BaseHand : ModNPC,
    IDrawOutlines
{
    protected enum AIState
    {
        Orbit,
        Attack,
        DoAttack,
        Despawn,
        DoTransition,
        Transition,
        DoDeath,
        TelegraphAttack,
    }

    private float OrbitProgress;
    private float DespawnProgress;
    private Color _outlineColor;
    protected Color TargetOutlineColor;
    protected ref float Timer => ref NPC.ai[0];
    protected AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    protected int ParentIndex
    {
        get => (int)NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    protected ref float RotationTimer => ref NPC.ai[3];

    protected NPC Parent => Main.npc[ParentIndex];
    protected Player Target => Main.player[NPC.target];
    protected Vector2 DirectionToTarget
    {
        get
        {
            return (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        }
    }

    public float TrailAlpha;
    public bool DrawWindTrail;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 16;
        NPC.height = 16;
        NPC.damage = 40;
        NPC.defense = 1;
        NPC.lifeMax = 100;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return State == AIState.Attack;
    }

    public override void AI()
    {
        base.AI();
        _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
        RotationTimer++;
        if (State == AIState.DoAttack)
        {
            SwitchState(AIState.Attack);
        }
        if (State == AIState.DoTransition)
        {
            SwitchState(AIState.Transition);
        }
        if (State == AIState.DoDeath)
        {
            SwitchState(AIState.Despawn);
        }
        if (DrawWindTrail)
        {
            TrailAlpha += 0.01f;
            if (TrailAlpha >= 1f)
                TrailAlpha = 1f;
        }
        else
        {
            TrailAlpha -= 0.01f;
            if (TrailAlpha <= 0)
                TrailAlpha = 0f;
        }
        bool shouldKill = !Parent.active || Parent.type != ModContent.NPCType<CommanderGintzia>();
        if (shouldKill && State != AIState.Despawn)
        {
            SwitchState(AIState.Despawn);
        }

        switch (State)
        {
            case AIState.Orbit:
                AI_Orbit();
                break;
            case AIState.TelegraphAttack:
                AI_TelegraphAttack();
                break;
            case AIState.Attack:
                AI_Attack();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Transition:
                AI_Transition();
                break;
        }
    }

    private void AI_Transition()
    {
        Timer++;
        float progress = Timer / 180f;
        float easedProgress = Easing.SpikeOutCirc(progress);
        RotationTimer += easedProgress * 3f;
        AI_Orbit();
        if (Timer >= 180)
        {
            SwitchState(AIState.Orbit);
        }
    }
    private void AI_Despawn()
    {
        TargetOutlineColor = Color.Transparent;
        Timer++;
        DespawnProgress = Timer / 60f;
        NPC.velocity *= 0.92f;
        if (Timer >= 60)
        {
            NPC.Kill();
        }
    }

    protected virtual void AI_Orbit()
    {
        TargetOutlineColor = Color.Transparent;
        float swingRange = MathHelper.TwoPi;
        float swingXRadius = 128;
        float swingYRadius = 48;
        float swingProgress = RotationTimer / 120f;
        float xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
        float yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
        Vector2 offset = new Vector2(xOffset, yOffset);
        Vector2 targetCenter = Parent.Center + offset + new Vector2(0, -16);
        Vector2 targetVelocity = (targetCenter - NPC.Center) * 0.25f;
        OrbitProgress += 0.001f;
        if (OrbitProgress >= 1f)
            OrbitProgress = 1f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, OrbitProgress);

        float targetRotation = (targetCenter - NPC.Center).ToRotation();
        NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
    }

    private void AI_TelegraphAttack()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle summonSound = AssetRegistry.Sounds.Collosseum.GintzeHandSummon;
            summonSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(summonSound, NPC.position);

            PixelPrimitiveCircleFactory.CreateClosingGustCircle(NPC.Center);
            NPC.TargetClosest();
        }

        TargetOutlineColor = Color.Yellow;
        float time = 60f;
        float ratio = Timer / time;
        float ease = EasingFunction.InOutExpo(ratio);
        NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY * 8 * (1f - (ease)), 0.1f);
        NPC.velocity *= 0.99f;
        NPC.rotation *= 0.9f;
        if (Timer >= time)
        {
            SwitchState(AIState.Attack);
        }
    }
    protected virtual void AI_Attack()
    {
        Timer++;
        OrbitProgress = 0f;
    }


    public virtual Color StripColors(float progressOnStrip)
    {
        //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
        Color result = Color.Lerp(Color.LightGray, Color.White,
            Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
        //     result.A /= 2;
        result *= TrailAlpha;
        return result;
    }

    public virtual float StripWidth(float progressOnStrip)
    {
        return MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
    }
    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPos = NPC.Center - screenPos;
        Vector2 drawOrigin = texture.Size() / 2f;
        float drawRotation = NPC.rotation;
        float drawScale = NPC.scale;
        float dp = 1f - DespawnProgress;
        float outlineOffset = 2;


        Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
        Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
        Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
        Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
        Color outlineColor = _outlineColor;
        SpriteEffects spriteEffects = SpriteEffects.None;

        spriteBatch.Draw(texture, left, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, right, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, up, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, down, null, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (TrailAlpha > 0f)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, StripColors, StripWidth, shader, offset: NPC.Size / 2);
        }

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPos = NPC.Center - screenPos;
        Vector2 drawOrigin = texture.Size() / 2f;
        float drawRotation = NPC.rotation;
        float drawScale = NPC.scale;
        float dp = 1f - DespawnProgress;

        if (State == AIState.Orbit)
        {
            drawColor = Color.Lerp(drawColor, Color.Black, 0.4f);
        }

        spriteBatch.Draw(texture, drawPos, null, drawColor * dp, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
        return false;
    }

    protected void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    public override bool CheckActive()
    {
        if (!Parent.active || Parent.type != ModContent.NPCType<CommanderGintzia>())
            return true;
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
        for (int i = 0; i < 12; i++)
        {
            float progress = i / 12f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * 4;
            Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, vel);
        }
    }


}
