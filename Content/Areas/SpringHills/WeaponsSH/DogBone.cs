using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Animations;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ProjectileHelpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class DogBone : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<Cupcake>(), isGuardian: true);
        Item.damage = 16;
        Item.knockBack = 3f;
    }
}

public class Cupcake : AbstractBellSummon
{
    Vector2 _startDashPos;
    Vector2 _endDashPos;
    Vector2 _targetOldPos;
    Pathfinder _pathfinder;
    int _targetNpc;
    float _targetafAlpha;
    float _afAlpha;
    bool _attacking;
    NPC Target
    {
        get
        {
            if (_targetNpc == -1)
                return Main.npc[0];
            return Main.npc[_targetNpc];
        }
    }

    enum AIState : byte
    {
        Idle,
        Patrol,
        GoHome,
        ChaseTarget,
        LungeTarget,
        FlyHome
    }
    ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float AttackCycle => ref Projectile.ai[2];
    const string ANIM_IDLE = "idle";
    const string ANIM_RUN = "run";
    const string ANIM_WALK = "walk";
    const string ANIM_DASH = "dash";
    const string ANIM_CROUCH = "crouch";
    const string ANIM_JUMP = "jump";
    const string ANIM_BALL = "ball";
    public override string Texture => TextureRegistry.EmptyTexture;
    float RunSpeed
    {
        get
        {
            if (State == AIState.GoHome)
                return 5;
            return 7;
        }
    }
    float MaxJumpSpeed => 13;
    float JumpRange => 232 * 232;
    float JumpTime => 29;
    float Gravity => 0.35f;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.UsesAseprite[Type] = true;
        Projectile.StaticDefaultToMinionProjectile();
        Projectile.SetTrailCacheLength(16);
    }

    //by separating the general hitbox from the collision hitbox we can avoid the minion getting stuck while pathfinding since its hitbox is not bigger than a tile
    //a bit of a hacky way to do it and we'll have to offset its drawing so it's not in the ground though
    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        Projectile.WidthAndHeight = 8;
        Projectile.DefaultToMinionProjectile();
        Projectile.LocalPiercingImmunityTime = 20;
        Projectile.light = 0.6f;
        Projectile.minionSlots = 0;

    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var myHitbox = DrawUtilities.CenterRectangle(Projectile.Center, 32, 32);
        return myHitbox.Intersects(targetHitbox);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_targetNpc);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetNpc = reader.ReadInt32();
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        AttackCycle = 0;
        State = state;
    }

    public override void AI()
    {
        base.AI();
        _attacking = false;
        _targetafAlpha = 0;
        Owner.GetModPlayer<BellPlayer>().incomingDamageMultiplier -= 0.5f;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.GoHome:
                AI_GoHome();
                break;
            case AIState.ChaseTarget:
                AI_ChaseTarget();
                break;
            case AIState.LungeTarget:
                AI_LungeTarget();
                break;
            case AIState.FlyHome:
                AI_FlyHome();
                break;
        }

        var distToOwner = Vector2.DistanceSquared(Projectile.Center, Owner.Center);
        if(distToOwner >= 780 * 780 && State != AIState.FlyHome)
        {
            SwitchState(AIState.FlyHome);
        }

        _afAlpha = MathHelper.Lerp(_afAlpha, _targetafAlpha, 0.1f);
        Projectile.velocity.Y += Gravity;
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
        this.AseAnimator.DrawOrigin = new Vector2(62, 51);
        this.AseAnimator.Update();
    }

    void AI_FlyHome()
    {
        Timer++;
        if(Timer == 1)
        {
            _startDashPos = Projectile.Center;

        }
        _targetafAlpha = 1f;
        var ease = EasingFunction.InOutSine(Timer / 60f);
        var pos = Vector2.Lerp(_startDashPos, Owner.Center + new Vector2(0, -32), ease);
        this.AseAnimator.PlayAnimation(ANIM_BALL, AnimationParams.Default);
        Projectile.rotation += 0.3f;
        Projectile.velocity = Vector2.Zero;
        Projectile.Center = pos;
        if(Timer >= 60f)
        {
            SwitchState(AIState.Idle);
        }
    }
    void AI_Idle()
    {
        Timer++;
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        Projectile.velocity.X *= 0.9f;
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.1f);
      
        var sqrDistToOwner = Vector2.DistanceSquared(Projectile.Center, Owner.Center);
        var maxAwayDistance = 32 * 32;
        if (sqrDistToOwner > maxAwayDistance)
        {
            SwitchState(AIState.GoHome);
        }
        if (Timer >= 15)
            ChaseTargetIfOneFound();

    }

    void HandleWalkingAnimation()
    {
        if (!IsGrounded())
        {
            this.AseAnimator.PlayAnimation(ANIM_JUMP, AnimationParams.Default);
        }
        else
        {
            this.AseAnimator.PlayAnimation(ANIM_WALK, AnimationParams.Default);
        }
    }
    void HandleRunningAnimation()
    {
        if (!IsGrounded())
        {
            this.AseAnimator.PlayAnimation(ANIM_JUMP, AnimationParams.Default);
        }
        else
        {
            this.AseAnimator.PlayAnimation(ANIM_RUN, AnimationParams.Default);
        }
    }
    void PathfindWalkTo(Vector2 destination)
    {
        MoonUtils.AIWalk_IvynStabber(_pathfinder, Projectile, destination, IsGrounded(), RunSpeed, MaxJumpSpeed, ref _targetOldPos);
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
    }
    void SearchForNewTarget()
    {
        _targetNpc = -1;
        var closestEnemy = MoonUtils.TargetClosestEnemy(Owner.Center, 512);
        if (closestEnemy == null)
            return;
        if (!Collision.CanHitLine(Projectile.position, 1, 1, closestEnemy.position, 1, 1))
            return;

        _targetNpc = closestEnemy.whoAmI;
    }

    void ChaseTargetIfOneFound()
    {
        SearchForNewTarget();
        if (_targetNpc != -1)
        {
            SwitchState(AIState.ChaseTarget);
        }
    }
    bool IsGrounded()
    {
        return MoonUtils.IsGrounded(Projectile);
    }

    void AI_GoHome()
    {
        Timer++;
        Projectile.rotation *= 0.8f;
        var poAroundPlayer = Owner.Center;
        poAroundPlayer.X += ExtraMath.Osc(-24, 24, speed: 0, Projectile.minionPos);
        PathfindWalkTo(poAroundPlayer);
        if (IsGrounded())
        {

 
        }
        var distSqr = Vector2.DistanceSquared(Projectile.Center, poAroundPlayer);
        if (distSqr < 32 * 32)
        {
            SwitchState(AIState.Idle);
        }
        HandleWalkingAnimation();
        if (Timer >= 15)
            ChaseTargetIfOneFound();

    }

    void AI_ChaseTarget()
    {
        Timer++;
        if (IsGrounded())
        {
            PathfindWalkTo(Target.Center);

        }
        Projectile.rotation *= 0.8f;
        HandleRunningAnimation();
        var distSqr = Vector2.DistanceSquared(Projectile.Center, Target.Center);
        if(distSqr <= JumpRange)
        {
            Projectile.velocity *= 0.9f;
        }
        if (distSqr <= JumpRange && Timer >= 20)
        {
            SwitchState(AIState.LungeTarget);
        }
    }
    void AI_LungeTarget()
    {
        Timer++;
        _targetafAlpha = 1f;
        switch (AttackCycle)
        {
            case 0:
                {
                    Projectile.velocity.X *= 0.85f;
                    if (IsGrounded())
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 1:
                {
                    _startDashPos = Projectile.Center;
                    _endDashPos = Target.Center;
                    _endDashPos = MoonUtils.RayCast(_startDashPos, (_endDashPos - _startDashPos), Vector2.Distance(_startDashPos, _endDashPos));
                    Projectile.velocity.X *= 0.92f;
                    this.AseAnimator.PlayAnimation(ANIM_CROUCH, AnimationParams.NoLooping);
                    if(Timer >= 45)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer % 3 == 0)
                    {
                        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 3, TriColorPalette.Bloody, new Vector2(5, 15f));
                        factory.particleCount = 4;
                        ParticleUtils.CreateSwirlingDustBurst(factory);
                    }

                    _attacking = true;
                    var posToMoveTo = Vector2.Lerp(_startDashPos, _endDashPos, EasingFunction.OutCirc(Timer / JumpTime));
                    var targetVelocity = _endDashPos - _startDashPos;
                    targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                    Projectile.velocity = targetVelocity * 20;
                    this.AseAnimator.PlayAnimation(ANIM_DASH, AnimationParams.Default);

                    Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                    if (Timer >= JumpTime / 3f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    Projectile.velocity *= 0.9f;
                    if(Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }

  
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
    }
    public override bool MinionContactDamage()
    {
        return _attacking;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        //Recoil a bit after hitting the enemy, exploding with some particles and a little sound effect
        Projectile.velocity *= 1.4f;

        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, target.Center, TriColorPalette.Bloody, new Vector2(5, 15f));
        factory.particleCount = 8;
        ParticleUtils.CreateSwirlingDustBurst(factory);

        var throwSound = AssetReferences.Assets.Sounds.SpearHit1.Asset with { PitchVariance = 0.5f, Volume = 0.55f };
        SoundEngine.PlaySound(throwSound, target.position);

        var firer = ProjFirer.From<GuardBite>(Projectile);
        firer.position = target.Center;
        firer.New();
    }
    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _afAlpha * 5.0f;
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(40, 0, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.FlameTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, Projectile.Size * 0.5f);
    }
    public override void DrawSpectral_Inner(SpriteBatch spriteBatch, Color drawColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail, DrawLayer.OverNPCs);
        var drawer = Projectile.GetAnimatorDrawInfo(drawColor);
        var offsetY = -18;
        drawer.worldPosition.Y += offsetY;
        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            var drawer2 = drawer;
            drawer2.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            Vector2 offset = drawer2.drawOrigin - this.AseAnimator.centerDrawOrigin;
            drawer2.worldPosition += offset;
            drawer2.worldPosition.Y += offsetY;
            drawer2.color = Color.Lerp(Color.DarkRed, Color.Transparent, oldPos.progress) * 0.3f * _afAlpha;
            spriteBatch.Draw(drawer2);
        }
        var shader = SpriteWhiteShader.Instance;
        using(spriteBatch.Ctx(spriteBatch.Parameters with { effect = shader }))
        {
            for (var i = 0F; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                var offset = i.ToRotationVector2();
                var drawer3 = drawer;
                drawer3.worldPosition += offset * 2;
                drawer3.color = Color.LightGreen;
                spriteBatch.Draw(drawer3);
            }
        }
    
        spriteBatch.Draw(drawer);
      //  _pathfinder.DebugDrawPath(spriteBatch, Color.White * 0.5f);

        /*
        var myRectangle = Projectile.getRect();

        //The rectangle is padded slightly prevent the entity getting stuck if it's hitbox is slightly smaller than the rectangles
        myRectangle = myRectangle.CenterPad(64);
        var drawer3 = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Vector2.Zero);
        drawer3.color = Color.White;
        myRectangle.Location -= new Point((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
        drawer3.drawOrigin = Vector2.Zero;
        drawer3.dstRect = myRectangle;
   
        spriteBatch.Draw(drawer3);*/
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class GuardBite : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    float Time => 15;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 47;
        Projectile.LocalHitOnce = true;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.friendly = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 21)
        {
            var biteSound = AssetReferences.Assets.Sounds.SpearSlash2.Asset with { PitchVariance = 0.6f, Volume = 0.4f, Pitch = -0.7f };
            SoundEngine.PlaySound(biteSound, Projectile.position);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var ease = EasingFunction.InOutSine(Timer / Time);
        var drawer = Projectile.Drawer;
        drawer.worldPosition.Y += MathHelper.Lerp(-32, 0, ease);
        var red = Color.Lerp(Color.Red, Color.DarkRed, 0.15f);
        drawer.color = Color.Lerp(Color.Transparent, red, EasingFunction.QuickOutSlowIn(Timer / Time)) * ExtraMath.Osc(0.6f, 1f, speed: 12);

        var drawer2 = drawer;
        drawer2.VerticalFrame(1, Main.projFrames[Type]);
        drawer2.worldPosition.Y += MathHelper.Lerp(64, 0, ease);

        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.Draw(drawer2);
        return false;
    }
}