using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.VilePipes.NPCsVP;

public class FlyingSpewerBullet : ModProjectile,
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
    public override string Texture => AssetReferences.Assets.Textures.Projectile.FlyingSpewerShot.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(12);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.timeLeft = 180;
        Projectile.light = 0.6f;
        Projectile.hostile = true;
        Projectile.extraUpdates = 1;
    }

    void MuzzleFlash()
    {
        var position = Projectile.Center;
        var velocity = Projectile.velocity;
        var innerColor = Color.Yellow;
        var outerColor = Color.Red;
        var p = FXUtil.GlowCircleBoom(position, innerColor, outerColor, Color.Black);
        p.Scale *= Main.rand.NextFloat(0.4f, 0.65f);

        var sp = SmokeParticle.SpawnInAlphaLayer(position, velocity * 0.2f, Color.DarkGray);
        sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
        sp.fast = true;
        sp.dampening = 0.08f;

        MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(position + velocity.SafeNormalize(Vector2.Zero) * 16, velocity.SafeNormalize(Vector2.Zero), innerColor);
        flashParticle.innerColor = Color.Yellow;
        flashParticle.bloomColor = Color.Red;
        flashParticle.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
        // flashParticle.Scale *= Main.rand.NextFloat(0.15f, 0.3f);



        for (float f = 0; f < 3; f++)
        {
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                gravity = 0f,
                innerColor = Color.Green,
                outerColor = Color.DarkGreen,
                scaleRange = new Vector2(0.3f, 1f)
            };
            var dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(0.35f) * Main.rand.NextFloat(0.25f, 0.6f), spawnParams);
            dp.dampening = 0.15f;
            dp.fast = true;
        }
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if(Timer == 1)
        {
            MuzzleFlash();
            SoundStyle s;
            if (Main.rand.NextBool(2))
            {
                s = new SoundStyle("Stellamod/Assets/Sounds/XX4160");
            }
            else
            {
                s = new SoundStyle("Stellamod/Assets/Sounds/XX41602");
            }
            s = s with { PitchVariance = 0.6f, Volume = 0.35f };
            SoundEngine.PlaySound(s, Projectile.position);
        }
        if (Main.rand.NextBool(32))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(2, 2) * 0.3f,
                color = Color.DarkGreen * 0.5f,
                scale = Main.rand.NextFloat(0.85f, 1f) * 0.5f,
                rotation = Main.rand.NextFloat() * 12f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

        if (Main.rand.NextBool(14))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, DustID.ToxicBubble, vel, Scale: 0.6f);
        }

        if (Main.rand.NextBool(24))
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


        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.scale = 0.7f;
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Green, Color.Transparent, 0.17f);
        Main.spriteBatch.Draw(Projectile.Drawer with { color = Color.White with { A = 0 } * 0.75f });

        return false;
    }

    public void DrawToRenderTargets()
    {
        void DrawOutline(SpriteBatch sb)
        {
            var drawer = Projectile.Drawer with { color = Color.Red };
            sb.Draw(drawer);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + Projectile.velocity, TriColorPalette.Foresty, new Vector2(5, 15f));
        fac.particleCount /= 4;
        ParticleUtils.CreateSwirlingDustBurst(fac);
    }
}

public class FlyingSpewer : ModNPC,
    IDrawToRenderTarget
{
    Color _useSightLineColor;
    Color _sightLineColor;
    Vector2 _patrolDirection;
    Vector2 _oldPlayerPos;
    Pathfinder _pathfinder;
    enum AIState : byte
    {
        Idle,
        Move,
        Siren,
        Chase,
        BlitzReady,
        Blitz
    }
    Vector2 SirenPosition => NPC.Center + new Vector2(0, -32);
    Vector2 GunPosition => NPC.Center + new Vector2(0, 56);
    Vector2 _fireVelocity;
    Outliner _outliner;

    const string ANIM_IDLE = "Idle";
    const string ANIM_SIRENIDLE = "SirenIdle";
    const string ANIM_SIRENIN = "SirenIn";
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    ref float SightLineRot => ref NPC.ai[2];
    ref float AngerTimer => ref NPC.ai[3];
    float SightDistance => 384;
    float _changeDirTimer;
    float SightRadians => 1.8f;
    float IdleTime => 90;
    float AttackDistance => 256 * 256;
    float ChaseSpeed => 7;
    float BlitzReadyTime => 60;
    float BlitzTime => 120;
    int Damage_SpewBullet => 35;
    float AngerTime => 45;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _pathfinder.NetSend(writer);
        writer.WriteVector2(_fireVelocity);
        writer.WriteVector2(_patrolDirection);
        writer.Write(_changeDirTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
        _fireVelocity = reader.ReadVector2();
        _patrolDirection = reader.ReadVector2();
        _changeDirTimer = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        this.AddToVilePipes();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        npcHitbox = npcHitbox.CenterPad(56);
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.width = NPC.height = 8;
        NPC.lifeMax = 150;
        NPC.defense = 10;
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
     
    
        
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Siren:
                AI_Siren();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Move:
                AI_Move();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.BlitzReady:
                AI_BlitzReady();
                break;
            case AIState.Blitz:
                AI_Blitz();
                break;
        }
        _useSightLineColor = Color.Lerp(_useSightLineColor, _sightLineColor, 0.1f);
        _outliner.Update();

        if (Main.rand.NextBool(6))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = NPC.Center,
                velocity = Main.rand.NextVector2Circular(2, 2) * 0.3f,
                color = Color.DarkGray * 0.5f,
                scale = Main.rand.NextFloat(0.85f, 1f) * 0.45f,
                rotation = Main.rand.NextFloat() * 12f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }



        NPC.DrawOrigin = new Vector2(42, 47);
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 0.8f, 0.4f));
    }

    void AI_Siren()
    {
        Timer++;
        if(Timer < 30)
        {
            NPC.PlayAnimationOneShot(ANIM_SIRENIN);
        }else
        {
            NPC.PlayAnimationLooped(ANIM_SIRENIDLE);
        }
      
        if(Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.SteamPunking.PoliceSiren.Asset with { Volume = 0.5f, Pitch = -0.2f };
            SoundEngine.PlaySound(sound, NPC.position);
        }

        if (Timer % 30 == 0)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(SirenPosition, TriColorPalette.Bloody, new Vector2(5, 15));
            ParticleUtils.CreateSwirlingDustCircle(fac);
            FXUtil.GlowCircleBoom(SirenPosition, Color.Red, Color.DarkRed, Color.Black, 25, baseSize: 0.24f);
        }
        if (Timer >= 120)
        {
            SwitchState(AIState.Chase);
        }
        MadSightLine();
        NPC.velocity.X *= 0.88f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.3f) * 2f, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0f, 0.1f);
    }
    void AI_Idle()
    {
        NPC.TargetClosest();
        Timer++;
        NPC.velocity.X *= 0.88f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.3f) * 2f, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0f, 0.1f);
        if(Timer >= IdleTime)
        {
            SwitchState(AIState.Move);
        }
        NPC.PlayAnimationLooped(ANIM_IDLE);
    }

    void GetNewPatrolDirection()
    {
        var oldDirection = _patrolDirection;
        var newDirection = Main.rand.NextVector2CircularEdge(1, 1);
        while (Vector2.Dot(oldDirection, newDirection) > 0.65f)
        {
            newDirection = Main.rand.NextVector2CircularEdge(1, 1);
        }
        _patrolDirection = newDirection;
    }

    void AI_Move()
    {
        //Alright here's how this movement statement is going to work
        //First the flying spewer will randomly choose a direction to move in, they'll rotate their sight line to face that direction
        _sightLineColor = Color.OrangeRed * 0.6f;
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _patrolDirection = Main.rand.NextVector2CircularEdge(1, 1);
                NPC.netUpdate = true;
            }
        }

        NPC.velocity = Vector2.Lerp(NPC.velocity, _patrolDirection, 0.02f);
        NPC.velocity.Y += MathF.Sin(Timer * 0.3f) * 0.02f;

        var targetRotation = NPC.velocity.X * 0.05f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.05f);
        _patrolDirection = _patrolDirection.RotatedBy(0.001f);
        //Every 4 seconds they'll randomly pick another direction
        if(MultiplayerHelper.IsHost && Timer % 800 == 0)
        {
            GetNewPatrolDirection();
            NPC.netUpdate = true;
        }

        //We want the spewer to change directions when they hit a wall, simply turning around
        if (_changeDirTimer <= 0)
        {
            if (NPC.collideX)
            {
                _changeDirTimer = 30;
                _patrolDirection.X *= -1;
            }

            if (NPC.collideY)
            {
                _changeDirTimer = 30;
                _patrolDirection.Y *= -1;
            }
        }
        else
        {
            _changeDirTimer--;
        }

        NPC.PlayAnimationLooped(ANIM_IDLE);
        //Rotate the sight light towards the direction too

        var newSightLineRot = Utils.AngleTowards(SightLineRot, _patrolDirection.ToRotation(), 0.25f);
        SightLineRot = Utils.AngleLerp(SightLineRot, newSightLineRot, 0.1f);
        var playerISee = MoonUtils.TargetPlayerBySight(NPC.Center, SightLineRot.ToRotationVector2(), SightRadians);
        if (playerISee == null)
        {
            if(AngerTimer > 0)
                AngerTimer--;
            return;
        }
        var sqrDist = Vector2.Distance(NPC.Center, playerISee.Center);
        if (sqrDist > SightDistance)
            return;


        _sightLineColor = Color.Lerp(_sightLineColor, Color.Red, AngerTimer / AngerTime);
        AngerTimer++;
        if(AngerTimer >= AngerTime)
        {
            SwitchState(AIState.Siren);
        }
    }
    void MadSightLine()
    {
        _sightLineColor = Color.Red;
        var newSightLineRot = Utils.AngleTowards(SightLineRot, (NPC.PlayerImTargeting.Center - GunPosition).ToRotation(), 0.35f);
        SightLineRot = Utils.AngleLerp(SightLineRot, newSightLineRot, 0.1f);
    }
    void AI_Chase()
    {
        MadSightLine();
        _sightLineColor *= 0.5f;
        Timer++;


        MoonUtils.CreateNewPathIfTooFar(_pathfinder,
            currentPosition: NPC.Center,
            destination: NPC.PlayerImTargeting.Center, ref _oldPlayerPos);

        MoonUtils.AIWalk_FlyChase(_pathfinder, NPC, NPC.PlayerImTargeting.Center, ChaseSpeed);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.025f, 0.1f);
        NPC.PlayAnimationLooped(ANIM_SIRENIDLE);
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AttackDistance)
        {
            SwitchState(AIState.BlitzReady);
        }
    }

    void AI_BlitzReady()
    {
        MadSightLine();
        _sightLineColor = Color.Transparent;
        _outliner.warning = true;

        Timer++;
        NPC.velocity *= 0.88f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        NPC.PlayAnimationLooped(ANIM_SIRENIDLE);
        if (Timer >= BlitzReadyTime)
        {
            SwitchState(AIState.Blitz);
        }
    }

    void AI_Blitz()
    {
        MadSightLine();
        _sightLineColor = Color.Transparent;
        _outliner.attacking = true;
        Timer++;
        if(Timer % 7 == 0 && Timer < BlitzTime)
        {
            FXUtil.ShakeCamera(NPC.Center, 256, 8);
            _fireVelocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(14);
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<FlyingSpewerBullet>(NPC);
                firer.damage = Damage_SpewBullet;
                firer.velocity = SightLineRot.ToRotationVector2() * 16;
                firer.position = GunPosition + SightLineRot.ToRotationVector2() * 58;
                firer.New();
            }
            NPC.velocity = -_fireVelocity.SafeNormalize(Vector2.Zero) * 1.5f;
        }
        NPC.PlayAnimationLooped(ANIM_SIRENIDLE);
        NPC.velocity *= 0.88f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.025f, 0.01f);
        if(Timer >= BlitzTime * 1.52f)
        {
            SwitchState(AIState.Chase);
        }
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for(var i =0; i < NPC.oldPos.Length; i++)
        {
            var pod = NPC.oldPos[i] + NPC.Size * 0.5f;
            var ratio = (float)i / (float)NPC.oldPos.Length;
            var color = Color.Lerp(Color.Red, Color.Transparent, ratio) * 0.15f;
            color.A = 0;
            NPC.DrawAnimator(spriteBatch, color, pod);
        }

        var dist = Vector2.Distance(NPC.Center, GunPosition);
        var chainTex = AssetReferences.Assets.Textures.NPC.FlyingSpewerChain.Asset;
        var chainDrawer = SpritebatchDrawer.FromTextureAsset(chainTex, NPC.Center);
        var chainLength = chainTex.Value.Width;
        var chainSteps = dist / chainLength;
        chainSteps *= 2;
        for(var i = 0; i < chainSteps; i++)
        {
            var pos = Vector2.Lerp(NPC.Center, GunPosition, (float)i / (float)chainSteps * 1f);
            chainDrawer.worldPosition = pos;
            spriteBatch.Draw(chainDrawer);
        }

        var gunDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.NPC.FlyingSpewerGun.Asset, GunPosition);
        gunDrawer.VerticalFrame(0, 2);
        gunDrawer.drawOrigin = new Vector2(28, 16);
        gunDrawer.rotation = SightLineRot;

        spriteBatch.Draw(gunDrawer);
        NPC.DrawAnimator(spriteBatch, drawColor);

        return false;
    }
    void DeathFX()
    {
        FXUtil.ShakeCamera(NPC.Center, 1024, 16);

        var sound = AssetReferences.Assets.Sounds.Explosions.ExplosionGeneric1.Asset with { PitchVariance = 0.6f };
        SoundEngine.PlaySound(sound, NPC.position);

        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Fiery, new Vector2(5, 15f));
        fac.particleCount *= 2;
        ParticleUtils.CreateSwirlingDustCircle(fac);

        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.Gold, Color.DarkOrange, Color.Black, duration: 25, baseSize: 0.24f);
        fx.Scale *= 1.5f;
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 16))
        {
            Particles.GlowDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.15f, 1f) * 0.3f,
                color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextFloat()),
                timeLeft = Main.rand.Next(45, 120),
                scale = Main.rand.NextFloat(0.15f, 0.25f)
            });
        }

        var color = Color.Lerp(Color.OrangeRed, Color.Black, 0.75f);
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 16))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                color = color,
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.5f, 2f)
            });
        }
    }

    private void DrawSightLine(GraphicsDevice graphicsDevice)
    {
        var quad = new TexturedQuad();

        var left = Vector2.UnitX.RotatedBy(SightRadians / -2f) * SightDistance;
        var right = Vector2.UnitX.RotatedBy(SightRadians / 2f) * SightDistance;
        var dist = Vector2.Distance(left, right);
        quad.Cone(GunPosition + SightLineRot.ToRotationVector2() * 16, 0, dist, SightDistance, SightLineRot);
        quad.vertices[1].Color = quad.vertices[3].Color = Color.Transparent;
        quad.vertices[0].Color = quad.vertices[2].Color = _useSightLineColor;
     //   quad.vertices[0].TextureCoordinate = quad.vertices[2].TextureCoordinate = Vector2.One;
     //   quad.vertices[1].TextureCoordinate = quad.vertices[3].TextureCoordinate = Vector2.Zero;


        var pass = AssetReferences.Effects.Generic.SightLine.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = -Main.GlobalTimeWrappedHourly * 2;
        pass.Parameters.alpha = 1f - (AngerTimer / AngerTime);
        pass.Apply();
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        quad.Draw();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        DeathFX();
        
        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(-1, 0));
        this.MakeGore("_Gore_Butt_Left", NPC.position, NPC.velocity + new Vector2(1, 0));
        this.MakeGore("_Gore_Butt_Right", NPC.position, NPC.velocity + new Vector2(1, 1));
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
    }

    public void DrawToRenderTargets()
    {
        void DrawGunOutline(SpriteBatch sb)
        {
            var gunDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.NPC.FlyingSpewerGun.Asset, GunPosition);
            gunDrawer.VerticalFrame(0, 2);
            gunDrawer.drawOrigin = new Vector2(28, 16);
            gunDrawer.rotation = SightLineRot;
            gunDrawer.color = _outliner.outlineColor;
            sb.Draw(gunDrawer);
        }
        NPC.DrawOutlineByAnimator(_outliner);
        OutlineRenderer.Queue(DrawGunOutline);
        PixelationManager.QueuePrimitivesDrawAction(DrawSightLine, DrawLayer.OverNPCsAdditive);
    }


}
