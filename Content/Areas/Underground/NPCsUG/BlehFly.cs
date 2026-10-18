using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class BlehFlySpit : ModProjectile,
    IDrawToRenderTarget
{
    Vector2 VectorScale
    {
        get
        {
            Vector2 v = Vector2.One;
          v.Y *= Projectile.velocity.Length() / 25f;
          v.X *= MathHelper.Lerp(0.5f, 1f, Projectile.velocity.Length() / 25f);
            return v;
        }
    }
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Projectile.BlehFlySpit.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = Projectile.height = 32;
        Projectile.light = 0.6f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 200;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.TentacleBubbleOut.Asset with { PitchVariance = 0.5f, Pitch = 0.5f, Volume = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);
            for (var i = 0; i < 2; i++)
            {
                var pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f);
                Particles.GlowDust.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = Main.rand.Next(45, 180),
                    color = Color.Lerp(Color.DarkGreen, Color.LightGreen, Main.rand.NextFloat()),
                    scale = Main.rand.NextFloat(0.5f, 1f) * 0.25f
                });
            }
        }

        if (Timer % 16 == 0)
        {
            Particles.BackCloud.Spawn(new()
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(2, 2) * 0.3f,
                color = Color.DarkGreen,
                scale = Main.rand.NextFloat(0.85f, 1f),
                rotation = Main.rand.NextFloat() * 12f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

        if (Main.rand.NextBool(7))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, DustID.ToxicBubble, vel, Scale: 0.6f);
        }

        if (Main.rand.NextBool(12))
        {
            var pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f);
            Particles.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = Main.rand.Next(45, 180),
                color = Color.Lerp(Color.Lerp(Color.DarkGreen, Color.White, 0.5f), Color.LightGreen, Main.rand.NextFloat()),
                scale = Main.rand.NextFloat(0.5f, 1f) * 0.25f
            });
        }


        Projectile.velocity.Y += 0.3F;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if(Timer >= 15)
        {
            Projectile.tileCollide = true;
        }
        DrawHelper.AnimateTopToBottom(Projectile, 5);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.LightGreen, Color.Transparent, 0.15f);
        var drawer = Projectile.Drawer;
        drawer.scale *= VectorScale;

        //     Main.spriteBatch.Draw(drawer);
        drawer.color *= 0.4f;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        var whiteDrawer = Projectile.Drawer;
        whiteDrawer.scale *= VectorScale;
        whiteDrawer.color = Color.White;
        whiteDrawer.color.A = 0;
        whiteDrawer.color *= MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 60f));
        Main.spriteBatch.Draw(whiteDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + Projectile.velocity, TriColorPalette.Foresty, new Vector2(5, 15f));
        fac.particleCount /= 4;
        ParticleUtils.CreateSwirlingDustBurst(fac);
    }

    public void DrawToRenderTargets()
    {
        void DrawOutline(SpriteBatch sb)
        {
            var drawer = Projectile.Drawer with { color = Color.Red };
            drawer.scale *= VectorScale;
            sb.Draw(drawer);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}

public class BlehFly : ModNPC,
    IDrawToRenderTarget
{
    Pathfinder _pathfinder;
    enum AIState : byte
    {
        Idle,
        Move,
        ChaseTarget,
        SpitReady,
        Spit,
        Sit,
        SitDown,
    }
    Outliner _outliner;
    Vector2 _oldPlayerPos;
    Vector2 _home;
    Vector2 _posToMoveTo;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_FLY = "Fly";
    const string ANIM_FASTFLY = "FastFly";
    const string ANIM_SPIT = "Spit";
    const string ANIM_SIT = "Sit";
    float IdleTime => 129;
    float HomeRadius => 196;
    float AlertDistance => 454 * 454;
    float ChaseDistance => 384 * 384;
    float AttackDistance => 100 * 100;
    float MoveTime => 90;
    float ChaseSpeed => 5;
    float SpitTime => 60;
    float SpitReadyTime => 60;
    int Damage_Spit => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_home);
        writer.WriteVector2(_posToMoveTo);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _home = reader.ReadVector2();
        _posToMoveTo = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 10;
        NPC.width = 80;
        NPC.height = 25;
        NPC.lifeMax = 96;
        NPC.defense = 7;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
        if (_home == default(Vector2))
        {
            _home = NPC.Center + new Vector2(0, -64);
            NPC.netUpdate = true;
        }

        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.ChaseTarget:
                AI_ChaseTarget();
                break;
            case AIState.Move:
                AI_Move();
                break;
            case AIState.Spit:
                AI_Spit();
                break;
            case AIState.SpitReady:
                AI_SpitReady();
                break;
            case AIState.Sit:
                AI_Sit();
                break;
            case AIState.SitDown:
                AI_SitDown();
                break;
        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(45, 38);

    }

    void FaceVelocity()
    {
        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        NPC.spriteDirection = NPC.direction;
    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    void AI_SitDown()
    {
        NPC.TargetClosest(faceTarget: false);
        Timer++;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 3f, 0.03f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.05f);

        NPC.PlayAnimationLooped(ANIM_FLY);
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (MoonUtils.IsGrounded(NPC))
        {
            SwitchState(AIState.Sit);
        }
    }

    void AI_Sit()
    {
        NPC.TargetClosest(faceTarget: false);
        Timer++;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 3f, 0.03f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.05f);

        NPC.PlayAnimationLooped(ANIM_SIT);
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AlertDistance)
        {
            SwitchState(AIState.Idle);
        }
    }
    void AI_Idle()
    {
        NPC.TargetClosest(faceTarget: false);
        Timer++;
        NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, MathF.Sin(Timer * 0.03f + NPC.whoAmI) * 1), 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);

        NPC.PlayAnimationLooped(ANIM_FLY);
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Move);
            var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
            if (sqrDistToTarget < ChaseDistance)
            {
                SwitchState(AIState.ChaseTarget);
            }
            if (sqrDistToTarget > AlertDistance)
            {
                SwitchState(AIState.SitDown);
            }
        }
   
    }

    void AI_Move()
    {
        NPC.TargetClosest(faceTarget: false);
        Timer++;
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            _posToMoveTo = _home + Main.rand.NextVector2Circular(HomeRadius, HomeRadius);
            NPC.netUpdate = true;
        }

        var targetVelocity = (_posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = NPC.velocity.MoveTowards(targetVelocity, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);
        NPC.PlayAnimationLooped(ANIM_FLY);
        FaceVelocity();

        if (Timer >= MoveTime)
        {
            SwitchState(AIState.Idle);
        }

        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < ChaseDistance)
        {
            SwitchState(AIState.ChaseTarget);
        }
    }

    void AI_ChaseTarget()
    {
        Timer++;
        if (Timer == 1)
        {
            PlayBuzzSound();
        }

        MoonUtils.CreateNewPathIfTooFar(_pathfinder,
            currentPosition: NPC.Center,
            destination: NPC.PlayerImTargeting.Center, ref _oldPlayerPos);

        MoonUtils.AIWalk_FlyChase(_pathfinder, NPC, NPC.PlayerImTargeting.Center, ChaseSpeed);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        NPC.PlayAnimationLooped(ANIM_FLY);
        FaceVelocity();
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AttackDistance)
        {
            SwitchState(AIState.SpitReady);
        }
    }

    void AI_Spit()
    {
        _outliner.attacking = true;
        Timer++;
        NPC.velocity *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_SPIT);
        NPC.SpriteFaceTarget();
        if(Timer == 35)
        {
            var spitSound = AssetReferences.Assets.Sounds.DMHeart_Shot.Asset with { PitchVariance = 0.5f, Pitch = -0.5f };
            SoundEngine.PlaySound(spitSound, NPC.position);
        }
        if(Timer % 1 == 0 && Timer > 30)
        {

        }
        if(Timer == 35)
        {
            for(var i = 0; i < 32; i++)
            {
                var velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(15);
                velocity.Y -= 4;
                velocity = velocity.RotatedByRandom(0.5f);
                velocity *= Main.rand.NextFloat(0.6f, 1f);
                Particles.ToxicBubbleDust.Spawn(new()
                {
                    position = NPC.Center,
                    velocity = velocity,
                    scale = Main.rand.NextFloat(0.5f, 1f),
                    timeLeft = Main.rand.Next(25, 70),
                    color = Color.White with { A = 0 }
                });
            }
        }
        if (Timer == 35 && MultiplayerHelper.IsHost)
        {
            for(var i = 0; i < 16; i++)
            {
                var firer = ProjFirer.From<BlehFlySpit>(NPC);
                firer.damage = Damage_Spit;

                firer.velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(15);
                firer.velocity.Y -= 4;
                firer.velocity = firer.velocity.RotatedByRandom(0.5f);
                firer.velocity *= Main.rand.NextFloat(0.4f, 1f);
                firer.New();
            }


        }

        if (Timer >= SpitTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void PlayBuzzSound()
    {
        var buzzSound = AssetReferences.Assets.Sounds.BeeBuzz.Asset with { PitchVariance = 0.5f, Pitch = -0.5f };
        SoundEngine.PlaySound(buzzSound, NPC.position);
    }

    void AI_SpitReady()
    {
        _outliner.warning = true;
        Timer++;
        if(Timer == 1)
        {
            PlayBuzzSound();
        }

        NPC.velocity *= 0.92f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, MathF.Sin(Timer * 0.03f) * 1), 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);
        NPC.PlayAnimationLooped(ANIM_FASTFLY);
        NPC.SpriteFaceTarget();
        if (Timer >= SpitReadyTime)
        {
            SwitchState(AIState.Spit);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;

        if (NPC.life > 0)
            return;

        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 16))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                color = Color.DarkGreen * 0.6f,
                scale = Main.rand.NextFloat(0.85f, 1f),
                rotation = Main.rand.NextFloat() * 12f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }
        this.MakeGore("_Gore_Wings", NPC.position, NPC.velocity + new Vector2(0, -2));
        this.MakeGore("_Gore_Butt", NPC.position, NPC.velocity + new Vector2(-2, 0));
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(2, 0));
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
