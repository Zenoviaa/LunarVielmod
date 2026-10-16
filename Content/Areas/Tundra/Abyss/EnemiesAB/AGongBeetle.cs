using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public abstract class AGongBeetleGong : ModProjectile,
    IDrawToRenderTarget
{
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.light = 0.6f;
        Projectile.timeLeft = 240;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity.Y += 0.3f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.Y != oldVelocity.Y)
            Projectile.velocity.Y = -oldVelocity.Y;
        if (Projectile.velocity.X != oldVelocity.X)
            Projectile.velocity.X = -oldVelocity.X;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.DarkGoldenrod, Color.Transparent, 0.25f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        void DrawOutline(SpriteBatch sb)
        {
            sb.Draw(Projectile.Drawer with { color = Color.Red });
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}
public class BigGongBeetleGong : AGongBeetleGong
{
    public override string Texture => AssetReferences.Assets.Textures.Projectile.GongBeetleBigGong.KEY;

}
public class SmallGongBeetleGong : AGongBeetleGong
{
    public override string Texture => AssetReferences.Assets.Textures.Projectile.GongBeetleSmallGong.KEY;


}

public class GongBeetleSmall : AGongBeetle
{
    int Damage_Gong => 30;
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 47;
        NPC.height = 43;
    }

    public override void AI()
    {
        base.AI();
        NPC.DrawOrigin = new Vector2(25, 38);
    }
    protected override void DropGong()
    {
        base.DropGong();
        var firer = ProjFirer.From<SmallGongBeetleGong>(NPC);
        firer.damage = Damage_Gong;
        firer.New();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -5));
        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(5, 0));
    }
}

public class GongBeetleBig : AGongBeetle
{
    int Damage_Gong => 40;
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 68;
        NPC.height = 71;
    }
    public override void AI()
    {
        base.AI();
        NPC.DrawOrigin = new Vector2(35, 70);
    }

    protected override void DropGong()
    {
        base.DropGong();
        var firer = ProjFirer.From<BigGongBeetleGong>(NPC);
        firer.damage = Damage_Gong;
        firer.New();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -5));
        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(5, 0));
    }
}
public abstract class AGongBeetle : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        JumpReady,
        Jump,
        Land
    }


    Vector2 _startJumpPos;
    Vector2 _endJumpPos;

    Outliner _outliner;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    bool HasGong
    {
        get => NPC.ai[2] == 0;
        set => NPC.ai[2] = value ? 0 : 1;
    }
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_IDLE = "Idle";
    const string ANIM_JUMPREADY = "JumpReady";
    const string ANIM_JUMP = "Jump";
    const string ANIM_JUMPLAND = "JumpLand";
    const string ANIM_IDLENOBELL = "IdleNoBell";
    const string ANIM_JUMPREADYNOBELL = "JumpReadyNoBell";
    const string ANIM_JUMPNOBELL = "JumpNoBell";
    const string ANIM_JUMPLANDNOBELL = "JumpLandNoBell";
    protected virtual float JumpHeight => 128;
    protected virtual float IdleTime => 90;
    protected virtual float JumpTime => 45;
    protected virtual float JumpToPlayerRange => 196 * 196;
    protected virtual float JumpReadyTime => 45;
    protected virtual float LandTime => 30;
    protected virtual float DropGongXDist => 40;
    protected virtual void DropGong()
    {

    }
    void PlayIdleAnim()
    {
        if (HasGong)
            NPC.PlayAnimationLooped(ANIM_IDLE);
        else
            NPC.PlayAnimationLooped(ANIM_IDLENOBELL);
    }
    void PlayJumpReadyAnim()
    {
        if (HasGong)
            NPC.PlayAnimationLooped(ANIM_JUMPREADY);
        else
            NPC.PlayAnimationLooped(ANIM_JUMPREADYNOBELL);
    }
    void PlayJumpAnim()
    {
        if (HasGong)
            NPC.PlayAnimationLooped(ANIM_JUMP);
        else
            NPC.PlayAnimationLooped(ANIM_JUMPNOBELL);
    }
    void PlayJumpLandAnim()
    {
        if (HasGong)
            NPC.PlayAnimationLooped(ANIM_JUMPLAND);
        else
            NPC.PlayAnimationLooped(ANIM_JUMPLANDNOBELL);
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startJumpPos);
        writer.WriteVector2(_endJumpPos);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startJumpPos = reader.ReadVector2();
        _endJumpPos = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToStarrTemple();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 10;
        NPC.width = 34;
        NPC.height = 36;
        NPC.lifeMax = 25;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }
    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Land:
                AI_Land();
                break;
            case AIState.Jump:
                AI_Jump();
                break;
            case AIState.JumpReady:
                AI_JumpReady();
                break;
        }
        _outliner.Update();
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void AI_JumpReady()
    {
        Timer++;
        _outliner.warning = true;
        Timer++;
        NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(Timer / JumpReadyTime));
        PlayJumpReadyAnim();
        if (Timer >= JumpReadyTime)
        {
            SwitchState(AIState.Jump);
        }
    }

    void AI_Idle()
    {
        NPC.TargetClosest();
        Timer++;
        NPC.velocity.X *= 0.94f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.04f);
        PlayIdleAnim();
        if (Timer >= IdleTime)
        {
            var sqrDistToPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
            if (sqrDistToPlayer < JumpToPlayerRange)
            {
                _startJumpPos = NPC.Bottom;
                _endJumpPos = NPC.PlayerImTargeting.Center + new Vector2(0, -100);
                _endJumpPos = TileUtilities.FallToSolidTile(_endJumpPos);
                _endJumpPos.Y -= NPC.height;
                _endJumpPos.Y += 4;
            }
            else
            {
                _startJumpPos = NPC.Bottom;
                if (MultiplayerHelper.IsHost)
                {
                    _endJumpPos = _startJumpPos;
                    var dir = Main.rand.NextBool(2) ? -1 : 1;
                    _endJumpPos += new Vector2(dir * Main.rand.NextFloat(64, 128), 0);
                    _endJumpPos.Y -= 100;
                    _endJumpPos = TileUtilities.FallToSolidTile(_endJumpPos);
                    _endJumpPos.Y -= NPC.height;
                    _endJumpPos.Y += 4;
                    NPC.netUpdate = true;
                }
            }
            SwitchState(AIState.Jump);
        }
    }

    void AI_Land()
    {
        Timer++;
        if (Timer == 1)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Bottom, TriColorPalette.Icey, new Vector2(5, 15f));
            ParticleUtils.CreateSwirlingDustCircle(fac);
        }

        NPC.velocity.X *= 0.96f;
        NPC.VectorScale = Vector2.Lerp(new Vector2(1.2f, 0.7f), Vector2.One, EasingFunction.InSine(Timer / LandTime));
        NPC.rotation = 0;
        PlayJumpLandAnim();
        if (Timer >= LandTime)
        {
            SwitchState(AIState.Idle);
        }
    }


    void AI_Jump()
    {
        Timer++;
        MoonUtils.AIJump_IvythornSlime(NPC, _startJumpPos, _endJumpPos, JumpHeight, Timer / JumpTime);
        PlayJumpAnim();
        var xDist = MathF.Abs(NPC.PlayerImTargeting.Center.X - NPC.Center.X);
        if(Timer >= JumpTime * 0.5f && HasGong && xDist < DropGongXDist)
        {
            DropGong();
        }
        if (Timer >= JumpTime)
        {
            SwitchState(AIState.Land);
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
