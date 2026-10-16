using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class IvythornSlimeSpike : ModProjectile,
    IDrawToRenderTarget
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Projectile.IvynthornSlimeSpike.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 12;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.1f;
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer >= 15)
            Projectile.tileCollide = true;
        if (Timer % 7 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Scale: Main.rand.NextFloat(0.5f, 1f));
        }

        Projectile.velocity.Y += 0.3f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.RosyBrown, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
     
        for(var i = 0; i < 8; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
            var vel = (pos - Projectile.Center).Resize(Main.rand.NextFloat(1, 3f));
            Dust.NewDustPerfect(pos, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.6f, 2f));
        }
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

public class IvythornSlime : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        HopReady,
        Hop,
        Land,
        EvilHopReady,
        EvilHop,
        EvilLand
    }

    Vector2 _hopStart;
    Vector2 _hopEnd;
    float _hopDirection;

    float IdleTime => 60;
    float AttackDist => 196 * 196;
    float HopReadyTime => 30;
    float HopDistance => 16 * 7;
    float HopHeight => 154;
    float HopTime => 45;
    float SlamTime => 30;
    int Damage_Spike => 15;

    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    bool _contactDamage;
    Outliner _outliner;

    ref float AttackCycle => ref NPC.ai[2];
    const string ANIM_IDLE = "Idle";
    const string ANIM_JUMP = "Jump";
    const string ANIM_SLAM = "Slam";
    public override string Texture => TextureRegistry.EmptyTexture;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_hopStart);
        writer.WriteVector2(_hopEnd);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _hopStart = reader.ReadVector2();
        _hopEnd = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToSpringHills();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        if (!_contactDamage)
            return false;
        return base.CanHitPlayer(target, ref cooldownSlot);
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

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        _contactDamage = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;

            case AIState.HopReady:
                AI_HopReady();
                break;

            case AIState.Hop:
                AI_Hop();
                break;

            case AIState.Land:
                AI_Land();
                break;

            case AIState.EvilHopReady:
                AI_EvilHopReady();
                break;

            case AIState.EvilHop:
                AI_EvilHop();
                break;

            case AIState.EvilLand:
                AI_EvilLand();
                break;
        }

        _outliner.Update();
        NPC.DrawOrigin = new Vector2(28, 26);
    }

    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.96f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.03f);
        NPC.VectorScale = Vector2.Lerp(NPC.VectorScale, Vector2.One, 0.12f);
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= IdleTime)
        {
            NPC.TargetClosest();
            if (NPC.HasValidTarget && Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center) < AttackDist)
            {
                SwitchState(AIState.EvilHopReady);
            }
            else
            {
                SwitchState(AIState.HopReady);
            }
        }
    }

    void AI_HopReady()
    {
        Timer++;
        NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(Timer / HopReadyTime));
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= HopReadyTime)
        {
            SwitchState(AIState.Hop);
        }
    }

    void HopMovement(float ratio)
    {
        var linear = Vector2.Lerp(_hopStart, _hopEnd, ratio);
        var upQuick = MathHelper.Lerp(0, -HopHeight, EasingFunction.OutExpo(ratio));
        var downSlow = MathHelper.Lerp(-HopHeight, 0, EasingFunction.InQuad(ratio));
        var up = MathHelper.Lerp(upQuick, downSlow, ratio);
        linear.Y += up;
        NPC.Center = linear;
        NPC.velocity = Vector2.Zero;
        NPC.VectorScale = Vector2.Lerp(new Vector2(0.8f, 1.2f), Vector2.One, EasingFunction.InSine(ratio));

        var range = 0.25f * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(ratio));
        var dir = (_hopEnd.X < _hopStart.X) ? 1 : -1;
        range *= dir;
        NPC.rotation = ExtraMath.Osc(-range, range, speed: 0, offset: ratio * 3);
    }

    void AI_Hop()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                if (Main.rand.NextBool(4))
                {
                    _hopDirection = Main.rand.NextBool(2) ? -1 : 1;
                }

                _hopStart = NPC.Bottom;
                _hopEnd = _hopStart + new Vector2(_hopDirection * HopDistance, 0) + new Vector2(0, -252);
                _hopEnd = TileUtilities.FallToSolidTileOrPlatform(_hopEnd);
                _hopEnd.Y -= NPC.height;
                _hopEnd.Y += 4;
                NPC.netUpdate = true;
            }
        }
        HopMovement(Timer / HopTime);
        NPC.PlayAnimationLooped(ANIM_JUMP);
        if (Timer >= HopTime)
        {
            SwitchState(AIState.Land);
        }
    }

    void AI_Land()
    {
        Timer++;
        NPC.velocity.X *= 0.96f;
        NPC.VectorScale = Vector2.Lerp(new Vector2(1.2f, 0.7f), Vector2.One, EasingFunction.InSine(Timer / SlamTime));
        NPC.rotation = 0;
        NPC.PlayAnimationOneShot(ANIM_SLAM);
        if (Timer >= SlamTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_EvilHopReady()
    {
        _outliner.warning = true;
        Timer++;
        NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(Timer / HopReadyTime));
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= HopReadyTime)
        {
            SwitchState(AIState.EvilHop);
        }
    }

    void AI_EvilHop()
    {
        _contactDamage = true;
        _outliner.attacking = true;
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                if (Main.rand.NextBool(4))
                {
                    _hopDirection = Main.rand.NextBool(2) ? -1 : 1;
                }

                _hopStart = NPC.Bottom;
                _hopEnd = NPC.PlayerImTargeting.Top + new Vector2(0, -252);
                _hopEnd = TileUtilities.FallToSolidTileOrPlatform(_hopEnd);
                _hopEnd.Y -= NPC.height;
                _hopEnd.Y += 4;
                NPC.netUpdate = true;
            }

        }
        HopMovement(Timer / HopTime);
        NPC.PlayAnimationLooped(ANIM_JUMP);
        if (Timer >= HopTime)
        {
            SwitchState(AIState.EvilLand);
        }
    }
    void CreateMagicalBurst()
    {
        for (var i = 0; i < 16; i++)
        {
            var pos = NPC.Bottom + Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - NPC.Bottom).Resize(Main.rand.NextFloat(10f, 15f));
            Particles.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = Color.LightGreen,
                scale = Main.rand.NextFloat(0.2f, 0.35f),
                timeLeft = Main.rand.Next(45, 130)
            });
        }
    }
    void AI_EvilLand()
    {
        _outliner.attacking = true;
        Timer++;
        if(Timer == 1)
        {
            //hornet firing sound
            var sound = SoundID.Item17 with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(sound, NPC.position);
        }

        if(Timer == 1)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Bottom, TriColorPalette.Foresty, new Vector2(5, 15f));
            ParticleUtils.CreateSwirlingDustCircle(fac);
            CreateMagicalBurst();
        }

        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            var range = 0.35f;
            for(var i = 0; i < 4f; i++)
            {
                var vel = -Vector2.UnitY;
                var ratio = i / 4f;
                var rads = MathHelper.Lerp(-range, range, ratio);
                vel = vel.RotatedBy(rads);
                vel *= 12;

                var firer = ProjFirer.From<IvythornSlimeSpike>(NPC);
                firer.damage = Damage_Spike;
                firer.velocity = vel;
                firer.New();
            }
        }
        NPC.velocity.X *= 0.96f;
        NPC.VectorScale = Vector2.Lerp(new Vector2(1.2f, 0.7f), Vector2.One, EasingFunction.InSine(Timer / SlamTime));
        NPC.rotation = 0;
        NPC.PlayAnimationOneShot(ANIM_SLAM);
        if (Timer >= SlamTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if(NPC.life <= 0)
        {
            //Create gores
            for (var i = 0; i < Main.rand.Next(5, 8); i++)
            {
                var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - NPC.Center).Resize(Main.rand.NextFloat(1, 4));
                vel.Y -= 4;
                var gore = Mod.Find<ModGore>($"{Name}_Gore_{Main.rand.Next(4)}").Type;
                Gore.NewGorePerfect(pos, vel, gore);
            }
            CreateMagicalBurst();
        }
        //

    }


    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Ivythorn>(), minimumDropped: 2, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.Gel, minimumDropped: 0, maximumDropped: 2));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutline(SpriteBatch sb)
        {
            NPC.DrawAnimator(sb, _outliner.outlineColor);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}
