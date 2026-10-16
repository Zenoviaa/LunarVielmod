using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.NPCsJN;

public class BiggestBeeSting : ModProjectile
{
    int _frame;
    ref float Timer => ref Projectile.ai[0];
    ref float BounceCount => ref Projectile.ai[1];
    float MaxBounces => 4;
    public override string Texture => AssetReferences.Assets.Textures.Projectile.BiggestBeeStinger.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_frame);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _frame = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
        Main.projFrames[Type] =3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 360;
        Projectile.light = 0.4f;
    }

    void MakePollenGlow()
    {
        var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        var vel = (pos - Projectile.Center).Resize(Main.rand.NextFloat(10f, 15f));
        Particles.GlowDust.Spawn(new()
        {
            position = pos,
            velocity = vel,
            color = Color.Lerp(Color.Gold, Color.DarkOrange, Main.rand.NextFloat()),
            scale = Main.rand.NextFloat(0.2f, 0.35f),
            timeLeft = Main.rand.Next(45, 130)
        });
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _frame = Main.rand.Next(3);
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        Projectile.frame = _frame;
        if (Timer % 4 == 0)
        {
            MakePollenGlow();
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X;
            BounceCount++;
        }
         
        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
            BounceCount++;
        }
     
        if(BounceCount >= MaxBounces)
        {
            Projectile.Kill();
        }
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.DarkOrange, Color.Transparent, 0.3f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 32, 16))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = Color.Gold,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

    }
}

public class BiggestBee : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        StingReady,
        Sting
    }
    Vector2 _movementDirection;
    Outliner _outliner;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    const string ANIM_FLY = "Fly";
    const string ANIM_SHOOTREADY = "Shootready";
    const string ANIM_SHOOT = "Shoot";
    float IdleTime => 120;
    float ShootReadyTime => 60;
    float ShootTime => 60;
    int Damage_Sting => 34;
    float AttackDistance => 512 * 512;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_movementDirection);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _movementDirection = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 12;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToHiveKingdom();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 96;
        NPC.lifeMax = 150;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
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
            case AIState.StingReady:
                AI_StingReady();
                break;
            case AIState.Sting:
                AI_Sting();
                break;
        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(74, 61);

    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _movementDirection = Main.rand.NextVector2Circular(1, 1);
                NPC.netUpdate = true;
            }
        }

        NPC.TargetClosest();
        NPC.velocity = Vector2.Lerp(NPC.velocity, _movementDirection, 0.1f);
        NPC.velocity.Y += MathF.Sin(Timer * 0.03f) * 0.3f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        if (NPC.collideX)
            _movementDirection.X *= -1;
        
        if (NPC.collideY)
            _movementDirection.Y *= -1;

        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        NPC.spriteDirection = NPC.direction;
        NPC.PlayAnimationLooped(ANIM_FLY);

        var sqrDistance = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistance < AttackDistance && Timer > IdleTime)
        {
            SwitchState(AIState.StingReady);
        }

        if (Timer % IdleTime == 0)
        {
            if (MultiplayerHelper.IsHost)
            {
                _movementDirection = Main.rand.NextVector2Circular(1, 1);
                NPC.netUpdate = true;
            }
        }
    }


    void AI_StingReady()
    {
        _outliner.warning = true;
        Timer++;
        NPC.SpriteFaceTarget();
        NPC.velocity *= 0.96f;
        NPC.PlayAnimationOneShot(ANIM_SHOOTREADY);
        if (Timer >= ShootReadyTime)
        {
            SwitchState(AIState.Sting);
        }
    }


    void AI_Sting()
    {
        _outliner.attacking = true;
        Timer++;
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            var firer = ProjFirer.From<BiggestBeeSting>(NPC);
            firer.damage = Damage_Sting;
            firer.velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(10);
            firer.New();
        }

        if (Timer == 1)
        {
            var velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(8);
            velocity *= -1;
            NPC.velocity = velocity;
        }

        NPC.velocity *= 0.9f;
        NPC.PlayAnimationOneShot(ANIM_SHOOT);
        if (Timer >= ShootTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -5));
        this.MakeGore("_Gore_Body", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Butt", NPC.position, NPC.velocity + new Vector2(0, 5));
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
