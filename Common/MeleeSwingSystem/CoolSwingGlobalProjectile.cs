using Stellamod.Core.SwingSystem;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Common.MeleeSwingSystem;

public class CoolSwingGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    /// <summary>
    /// Indicates if this projectile is a melee swing
    /// </summary>
    public bool isMeleeSwing;
    public int baseComboLength;
    SwordLine CalculateHitboxStart(Projectile projectile)
    {
        float swordLength = projectile.width;
        float edgeLength = swordLength;
        edgeLength *= 1.6f;
        edgeLength += projectile.height;
        //edgeLength += extraLength;
        /*
        if (MeleeEffectsPlayer.superExtender)
            edgeLength += 152;
        */
        float rotation = projectile.rotation;
        rotation -= MathHelper.PiOver4;

        Vector2 rotationVec = rotation.ToRotationVector2();
        return new(
            projectile.Center - rotationVec * swordLength,
            projectile.Center + rotationVec * swordLength,
            projectile.Center + rotationVec * edgeLength / 1.6f);
    }

    public override bool PreAI(Projectile projectile)
    {
        if (isMeleeSwing)
        {
            projectile.CoolSwingTimer++;
            projectile.PlayerOwner.GetModPlayer<SwingPlayerV2>().MaxCombo = projectile.CoolSwingComboLength;
        }
        return base.PreAI(projectile);
    }

    public override void PostAI(Projectile projectile)
    {
        base.PostAI(projectile);
        if (isMeleeSwing)
        {
            AI_OrientHand(projectile, Main.player[projectile.owner]);
        }
    }

    public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (isMeleeSwing)
        {
            //One issue that the other swing system had was that we were using extra updates to get more accurate collision checks,
            //Since fast swings could overshoot their targets and simple not hit
            //However, this made it annoying to add additional effects onto melee weapons, so we will be banned the use of extra updates in this instance
            //To fix collisions intead, we will be creating a hitbox that takes into the account the projectiles old position
            float collisionPoint = 0f;
            var _hitboxStart = CalculateHitboxStart(projectile);
            bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                _hitboxStart.Start, _hitboxStart.End, 36, ref collisionPoint);
            return check;
        }
        else
        {
            return base.Colliding(projectile, projHitbox, targetHitbox);
        }
    }

    void AI_OrientHand(Projectile projectile, Player owner)
    {
        float rotation = projectile.rotation;
        projectile.direction = projectile.velocity.X > 0 ? 1 : -1;
        owner.ChangeDir(projectile.direction);
        projectile.spriteDirection = owner.direction;


        //  Owner.GetModPlayer<SwingPlayerV2>().isSwinging = true;
        owner.itemRotation = rotation * owner.direction;
        owner.itemTime = 2;
        owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, projectile.rotation - MathHelper.ToRadians(135));// set arm position (90 degree offset since arm starts lowered)
    }
}
