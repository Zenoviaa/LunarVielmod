using Stellamod.Common;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Astar;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
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
    float Gravity => 0.2f;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        Projectile.StaticDefaultToMinionProjectile();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        Projectile.DefaultToMinionProjectile();
        Projectile.WidthAndHeight = 16;
        Projectile.LocalPiercingImmunityTime = 20;
        Projectile.tileCollide = true;
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
       // Projectile.velocity.Y += Gravity;
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.02f, 0.1f);
    }


    void AI_Idle()
    {
        Timer++;
        Projectile.velocity.X *= 0.96f;
    }

    void AI_GoHome()
    {
        //alright
        Timer++;
        if (Timer % 30 == 0)
            _pathfinder.NewPath(Projectile.Center, Owner.Center, 50);
        if (_pathfinder.currentNode != Vector2.Zero)
        {
            Vector2 targetVelocity = (_pathfinder.currentNode - Projectile.Center).SafeNormalize(Vector2.Zero);
            targetVelocity *= 5f;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.direction = (_pathfinder.currentNode.X > Projectile.Center.X) ? 1 : -1;

            //node has been crossed
            //not sure howe ewll this is gonna work but we'll see

            float distanceToCurrentNode = Vector2.Distance(Projectile.Center, _pathfinder.currentNode);
            float distanceToNextNode = Vector2.Distance(Projectile.Center, _pathfinder.nextNode);
            if (distanceToNextNode <= distanceToCurrentNode)
            {
                _pathfinder.currentNode = Vector2.Zero;
            }

            if(Projectile.getRect().Contains(_pathfinder.currentNode.ToPoint()) || Collision.CanHitLine(Projectile.position, 1, 1, _pathfinder.nextNode, 1, 1))
            {
                _pathfinder.Pop();
            }
        }
        else if (_pathfinder.path != null && _pathfinder.path.Count > 0)
        {
            _pathfinder.Pop();
        }
    }

    void AI_FindTarget()
    {

    }

    void AI_JumpToTarget()
    {

    }

    void AI_FlyHome()
    {

    }


    
    public override void DrawSpectral_Inner(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color = drawColor;
        spriteBatch.Draw(drawer);
        if (_pathfinder.path == null)
            return;

        foreach(var pos in _pathfinder.path)
        {
            var drawer2 = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, pos);
            spriteBatch.Draw(drawer2);
        }
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
