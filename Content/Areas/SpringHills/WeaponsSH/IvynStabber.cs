using Microsoft.Xna.Framework.Input;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Core.ProjectileHelpers;
using Stellamod.Items;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class SongofIvyn : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<IvynStabber>());
        Item.damage = 9;
        Item.knockBack = 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(),
            material: ModContent.ItemType<Ivythorn>());
    }
}

public class IvynStabber : AbstractBellSummon
{
    float _extraSpeed;
    int _targetNpc;
    NPC Target
    {
        get
        {
            if (_targetNpc == -1)
                return Main.npc[0];
            return Main.npc[_targetNpc];
        }
    }

    Pathfinder _pathfinder;
    enum AIState : byte
    {
        GoHome,
        Idle,
        FindTarget,
        JumpToTarget,
        FlyHome
    }
    ref float Timer => ref Projectile.ai[0];
    AIState State
    {
        get
        {
            return (AIState)Projectile.ai[1];
        }
        set
        {
            Projectile.ai[1] = (float)value;
        }
    }
    ref float AttackCycle => ref Projectile.ai[2];
    float Gravity => 0.4f;
    float MaxJumpSpeed => 7;
    float RunSpeed => 4 * ExtraMath.Osc(0.8F, 1F, speed: 0, Projectile.minionPos) + _extraSpeed;
    float JumpTime => 23;
  
    public override string Texture => TextureRegistry.EmptyTexture;

    Vector2 _targetOldPos;

    const float REPATH_DISTANCE = 32 * 32;
    const float JUMP_RANGE = 96 * 96;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_JUMPFRAME = "Jumpframe";
    const string ANIM_STABFRAME = "Stabframe";
    public override void SetStaticDefaults()
    {
        Projectile.StaticDefaultToMinionProjectile();
        ProjectileID.Sets.UsesAseprite[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        Projectile.DefaultToMinionProjectile();
        Projectile.WidthAndHeight = 32;
        Projectile.width = 12;
        Projectile.LocalPiercingImmunityTime = 20;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.light = 0.67f;
    }
   

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
        Projectile.netUpdate = true;
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

    public override bool MinionContactDamage()
    {
        return State == AIState.JumpToTarget;
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

    public override void AI()
    {
        base.AI();
        _extraSpeed = 0;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.GoHome:
                AI_GoHome();
                break;
            case AIState.FindTarget:
                AI_FindTarget();
                break;
            case AIState.JumpToTarget:
                AI_JumpToTarget();
      
                break;
            case AIState.FlyHome:
                AI_FlyHome();
                break;
        }


        Projectile.velocity.Y += Gravity;
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.02f, 0.1f);
        this.AseAnimator.DrawOrigin = new Vector2(15, 36);
        this.AseAnimator.Update();
    }

    void ChaseTargetIfOneFound()
    {
        SearchForNewTarget();
        if(_targetNpc != -1)
        {
            SwitchState(AIState.FindTarget);
        }
    }

    void AI_Idle()
    {
        Timer++;
        Projectile.velocity.X *= 0.9f;
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        if (Timer >= 30)
            ChaseTargetIfOneFound();
        var sqrDist = Vector2.DistanceSquared(Projectile.Center, Owner.Center);
        if(sqrDist > 64 * 64)
        {
            SwitchState(AIState.GoHome);
        }
    }

    bool IsGrounded()
    {
        return MoonUtils.IsGrounded(Projectile);
        /*
        var tilePointBelow = Projectile.Bottom.ToTileCoordinates();
        var tileBelow = Main.tile[tilePointBelow];
        tilePointBelow.Y++;
        var tileBelow2 = Main.tile[tilePointBelow];
        return WorldGen.SolidOrSlopedTile(tileBelow) || WorldGen.SolidOrSlopedTile(tileBelow2);*/
    }

    void PathfindWalkTo(Vector2 destination)
    {
        //Everytime the target moves, a new path should be re calculated
        if (Vector2.DistanceSquared(_targetOldPos, destination) > REPATH_DISTANCE)
        {
            _targetOldPos = destination;
            var startPost = TileUtilities.FallToSolidTile(Projectile.Center);
//            startPost.Y -= 16;
            _pathfinder.NewPath(destination, startPost, 50);
        }

        if (_pathfinder.currentNode != Vector2.Zero)
        {
            var target = _pathfinder.currentNode;
            if(Collision.CanHitLine(Projectile.position, 1, 1, destination, 1, 1))
            {
                target = destination;
            }
            //We can make the assumption that whatever node we're moving too is VERY close to our actor
            //So here's how it works, we create a 16x16 rectangle around the the point we're moving to
            //If that rectangle intersects our hitbox rectangle, then the destination has been reached.
            var targetRectangle = DrawUtilities.CenterRectangle(_pathfinder.currentNode, 16, 16);
            var nextRectangle = DrawUtilities.CenterRectangle(_pathfinder.nextNode, 16, 16);
            var myRectangle = Projectile.getRect();

            //The rectangle is padded slightly prevent the entity getting stuck if it's hitbox is slightly smaller than the rectangles
            myRectangle = myRectangle.CenterPad(4);

            float distanceToCurrentNode = Vector2.Distance(Projectile.Center, _pathfinder.currentNode);
            float distanceToNextNode = Vector2.Distance(Projectile.Center, _pathfinder.nextNode);

            if (myRectangle.Intersects(targetRectangle) || distanceToNextNode < distanceToCurrentNode || myRectangle.Intersects(nextRectangle))
            {
                _pathfinder.Pop();
            }

            //Since this is a grounded entity, we can't just directly move towards the point we wish to reach
            //First we'll try to reach the target destination on the X axis, and once the X axis has been satisfied, we'll try to reach it on the y Axis
            //If the y axis is above, then we'll jump
            var diffX = (target.X - Projectile.Center.X);
            var distX = MathF.Abs(diffX);
            if(distX <= Projectile.width)
            {
                var diffY = (target.Y - Projectile.Center.Y);
                var distY = MathF.Abs(diffY);
                if(diffY < 0 && distY > myRectangle.Height && IsGrounded())
                {
                    var maxSpeed = MathF.Min(MaxJumpSpeed, distY / 8);
                    Projectile.velocity.Y = -maxSpeed;
                }
            }
           
            var dirX = MathF.Sign(diffX);
            var targetXVelocity = dirX * RunSpeed;
            var accel = ExtraMath.Osc(0.05f, 0.1f, speed: 0, offset: Projectile.identity);
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetXVelocity, accel);
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        }
        else if (_pathfinder.path != null && _pathfinder.path.Count > 0)
        {
            _pathfinder.Pop();
        }
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
    }

    void HandleWalkingAnimation()
    {
        if (!IsGrounded())
        {
            this.AseAnimator.PlayAnimation(ANIM_JUMPFRAME, AnimationParams.Default);
        }
        else
        {
            this.AseAnimator.PlayAnimation(ANIM_RUN, AnimationParams.Default);
        }
    }

    void AI_GoHome()
    {
        //alright
        Timer++;
        var poAroundPlayer = Owner.Center;
        poAroundPlayer.X += ExtraMath.Osc(-24, 24, speed: 0, Projectile.minionPos);
        PathfindWalkTo(poAroundPlayer);
        HandleWalkingAnimation();
        if(Timer >= 30)
            ChaseTargetIfOneFound();
        var distSqr = Vector2.DistanceSquared(Projectile.Center, poAroundPlayer);
        if(distSqr < 32 * 32)
        {
            SwitchState(AIState.Idle);
        }
    }


    void AI_FindTarget()
    {
        Timer++;
        _extraSpeed = 2;
        PathfindWalkTo(Target.Center);
        HandleWalkingAnimation();
        var distSqr = Vector2.DistanceSquared(Projectile.Center, Target.Center);
        if(distSqr <= JUMP_RANGE && Timer >= 20)
        {
            SwitchState(AIState.JumpToTarget);
        }
    }

    void AI_JumpToTarget()
    {
        Timer++;
        if(AttackCycle == 0 && IsGrounded())
        {
            Projectile.velocity = MoonUtils.VelocityTo(Projectile, Target, 9);
            AttackCycle++;
        }

        Projectile.velocity.X *= 0.95F;
        Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
        this.AseAnimator.PlayAnimation(ANIM_STABFRAME, AnimationParams.Default);

        if (Timer >= JumpTime && IsGrounded())
        {
            SwitchState(AIState.Idle);
        }
        Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
    }

    void AI_FlyHome()
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        //Recoil a bit after hitting the enemy, exploding with some particles and a little sound effect
        Projectile.velocity = MoonUtils.VelocityTo(target, Projectile, 5);

        var factory = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, target.Center, TriColorPalette.Foresty, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustBurst(factory);

        var throwSound = AssetReferences.Assets.Sounds.Jack_Throw.Asset with { PitchVariance = 0.5f, Volume = 0.55f };
        SoundEngine.PlaySound(throwSound, target.position);
    }

    public override void DrawSpectral_Inner(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawer = Projectile.GetAnimatorDrawInfo(drawColor);
       
        spriteBatch.Draw(drawer);
        var kb = Keyboard.GetState();
        if(kb.IsKeyDown(Keys.LeftShift))
            _pathfinder.DebugDrawPath(spriteBatch, Color.White  * 0.3f);
    }
    
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

}
