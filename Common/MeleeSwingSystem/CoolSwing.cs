using Mono.Cecil;
using Stellamod.Common.StaminaSystem;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Common.MeleeSwingSystem;

public static class CoolSwingExtenstions
{
    extension(Item item)
    {
        public int StaminaShoot
        {
            get
            {
                if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
                {
                    return cs.meleeStaminaShoot;
                }
                return item.shoot;
            }
            set
            {
                if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
                {
                    cs.meleeStaminaShoot = value;
                }
            }
        }
        public float StaminaDamageMultiplier
        {
            get
            {
                if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
                {
                    return cs.meleeSwingStaminaDamageMultiplier;
                }
                return 1f;
            }
            set
            {
                if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
                {
                    cs.meleeSwingStaminaDamageMultiplier = value;
                }
            }
        }

        public void DefaultToCoolSwingItem<BasicSwing, StaminaSwing>(MeleeWeaponType meleeWeaponType)
            where BasicSwing : ModProjectile
            where StaminaSwing : ModProjectile
        {
            if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
            {
                cs.meleeWeaponType = meleeWeaponType;
                cs.meleeStaminaShoot = ModContent.ProjectileType<StaminaSwing>();
            }
            item.shoot = ModContent.ProjectileType<BasicSwing>();

            DefaultToCoolSwingItem(item);
        }

        public void DefaultToCoolSwingItem()
        {
            item.autoReuse = true;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useTime = 32;
            item.useAnimation = 32;
            item.useStyle = ItemUseStyleID.Swing;
            item.knockBack = 6;
            item.rare = ItemRarityID.Blue;
            item.shootSpeed = 10;
            item.damage = 13;
            item.DamageType = DamageClass.Melee;
            if (item.TryGetGlobalItem<CoolSwingGlobalItem>(out var cs))
            {
                cs.isMeleeSwingItem = true;
                cs.meleeSwingStaminaCost = 1;
                cs.meleeSwingStaminaDamageMultiplier = 2;
            }
        }
    }

    extension(Projectile projectile)
    {
        /// <summary>
        /// The number of distinct attacks in the combo
        /// </summary>
        public int CoolSwingComboLength
        {
            get
            {
                if (projectile.TryGetGlobalProjectile<CoolSwingGlobalProjectile>(out var cs) && cs.isMeleeSwing)
                {
                    //TODO:
                    //For accessories that add moves, search for them here and add them to the base combo length
                    return cs.baseComboLength;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// A drawer that automatically handles fixing the flip direction and rotation and when facing the blade upward
        /// </summary>
        public SpritebatchDrawer CoolSwingDrawer
        {
            get
            {
                var drawer = projectile.Drawer;
                float rotation = projectile.rotation;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.CoolSwingDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                    rotation -= MathHelper.PiOver2;
                }



                drawer.spriteEffects = spriteEffects;
                drawer.rotation = rotation;
                return drawer;
            }
        }

        /// <summary>
        /// Sugar for AI0, this timer is incremented every tick for swing projectiles
        /// </summary>
        public ref float CoolSwingTimer => ref projectile.ai[0];

        /// <summary>
        /// Sugar for AI1, this controls whether the blade is facing up or down 
        /// </summary>
        public ref float CoolSwingDirection => ref projectile.ai[1];

        /// <summary>
        /// Sugar for AI2, this is the index in the combo sequence
        /// </summary>
        public int CoolSwingComboIndex => (int)projectile.ai[2];

        /// <summary>
        /// Returns whether the projectile has been initialized as a comboing projectile
        /// Combo projectiles have different functionality depending on the combo index
        /// </summary>
        public bool IsCoolSwing => projectile.TryGetGlobalProjectile<CoolSwingGlobalProjectile>(out var cs) && cs.isMeleeSwing;
        public void DefaultToCoolSwing(CoolSwingHandler swingHandler) => DefaultToCoolSwing(projectile, swingHandler.Count);
        public void DefaultToCoolSwing(int baseComboLength)
        {
            //The width variable will control how far the projectile hits
            projectile.width = 16;

            //The height variable will control the length of the extender
            projectile.height = 0;
            projectile.friendly = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.light = 0.2f;
            projectile.timeLeft = 600;
            projectile.LocalHitOnce = true;
            projectile.DamageType = DamageClass.Melee;

            if (projectile.TryGetGlobalProjectile<CoolSwingGlobalProjectile>(out var cs))
            {
                cs.isMeleeSwing = true;
                cs.baseComboLength = baseComboLength;
            }
        }

    }
}

public class Swinger
{
    public required ISwordMovement movement;
    public float swingTime;
    public int direction;
    public bool fixRot;
    public SwordMovement GetMovement(Projectile projectile)
    {
        return GetMovement(ref projectile.ai[0], direction);
    }
    public SwordMovement GetMovement(ref float timer, in int dir)
    {
        float uneasedLerpValue = timer / swingTime;
        var input = new SwingInput(uneasedLerpValue, dir);
        return movement.CalculateSwordMovement(input);
    }
}
public record struct SwordLine(Vector2 Start, Vector2 End, Vector2 ProjectionEnd);
public class CoolSwingHandler
{
    /// <summary>
    /// A buffer for calculating trail points
    /// </summary>
    public static readonly Vector2[] TrailCacheBuffer = new Vector2[64];
    public readonly List<Swinger> Movements;
    public CoolSwingHandler()
    {
        Movements = new List<Swinger>();
    }
    public void Add(Swinger movement)
    {
        Movements.Add(movement);
    }



    public ProjFirer KillThisSwingGetNext(Projectile projectile)
    {
        SwingPlayerV2 comboPlayer = projectile.PlayerOwner.GetModPlayer<SwingPlayerV2>();
        int combo = projectile.CoolSwingComboIndex;
        int dir = (int)projectile.CoolSwingDirection;
        comboPlayer.IncreaseCombo();
        projectile.Kill();
        var firer = ProjFirer.Copy(projectile);
        firer.ai0 = 0;
        firer.ai1 = dir;
        firer.ai2 = combo + 1;
        return firer;
    }

    /// <summary>
    /// Sets the correct position and rotation of the swing.
    /// Assumes that this is a swing projectile and has the ai values 1 and 2 populated
    /// </summary>
    /// <param name="p"></param>
    public void ApplyTo(Projectile p, bool allowKill = true)
    {
        float targetRotation = p.velocity.ToRotation();
        var movement = this[p];

            movement.Offset = movement.Offset.RotatedBy(targetRotation);
        if (Movements[(int)p.CoolSwingComboIndex].fixRot)
        {
            if(p.PlayerOwner.direction == -1)
            {
                movement.Offset = movement.Offset.RotatedBy(MathHelper.Pi);
                movement.Offset *= new Vector2(-1, 1);
            }
        }



        p.friendly = movement.CanHurt;
        p.Center = p.PlayerOwner.Center + movement.Offset;
        p.rotation = (p.Center - p.PlayerOwner.Center).ToRotation() + MathHelper.PiOver4;
        if (movement.OverrideRotation.HasValue)
        {
            p.rotation = movement.OverrideRotation.Value + targetRotation + MathHelper.PiOver4;
        }
        if (movement.EasedInterpolant >= 1f && allowKill)
        {
            p.Kill();
        }
    }

    public int Count => Movements.Count;
    public float GetTime(Projectile comboIndex) => this[comboIndex.CoolSwingComboIndex].swingTime * (1f / comboIndex.PlayerOwner.GetTotalAttackSpeed(comboIndex.DamageType));
    public float GetInterpolant(Projectile comboIndex) => comboIndex.CoolSwingTimer /GetTime(comboIndex);
    public SwordMovement this[Projectile p]
    {
        get
        {
            var combo = p.CoolSwingComboIndex;
            if (combo >= Movements.Count)
                combo = 0;
            return this[combo].movement.CalculateSwordMovement(new()
            {
                dir = this[combo].direction == 0 ? combo % 2 == 0 ? 1 : -1 : this[combo].direction,
                uneasedLerpValue = (p.CoolSwingTimer / GetTime(p))
            });
        }
    }

    public Swinger GetSwinger(Projectile swingProjectile) => Movements[(int)swingProjectile.CoolSwingComboIndex];
    public void CalculateTrailingPoints(Projectile swingProjectile, ref Vector2[] trailCache, float trailOffset = 1.25f)
    {
        float time = GetInterpolant(swingProjectile);
        Vector2 velocity = swingProjectile.velocity;

        //Alright, calculating trail points
        //The points will be offset by the position matrix
        //So we just calculate the local points here
        float length = (float)trailCache.Length;
        var swingTime = GetTime(swingProjectile);
        for (int t = trailCache.Length - 1; t >= 0; t--)
        {
            var timer = swingProjectile.CoolSwingTimer;
            timer -= t * 0.2f;
            timer = MathHelper.Clamp(timer, 0, swingTime);
            var targetRotation = velocity.ToRotation();
            var interpolant = timer / swingTime;

            var combo = swingProjectile.CoolSwingComboIndex;
            if (combo >= Movements.Count)
                combo = 0;

            var swordMovement = this[combo].movement.CalculateSwordMovement(new()
            {
                dir = this[combo].direction == 0 ? combo % 2 == 0 ? 1 : -1 : this[combo].direction,
                uneasedLerpValue = (timer / swingTime)
            });

       


            var o = swordMovement.Offset;

            if (Movements[(int)swingProjectile.CoolSwingComboIndex].fixRot)
            {
                if (swingProjectile.PlayerOwner.direction == -1)
                {
                    o = o.RotatedBy(MathHelper.Pi);
                    o *= new Vector2(-1, 1);
                }
            }


            Vector2 offset = new Vector2(o.X * trailOffset, o.Y * trailOffset).RotatedBy(targetRotation);
            trailCache[t] = offset;
        }
    }
    public Swinger this[int i]
    {
        get => Movements[i];
        set
        {
            Movements[i] = value;

        }
    }
}

public class CoolSwingGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public bool isMeleeSwingItem;
    public int meleeSwingStaminaCost;
    public int meleeStaminaShoot;
    public float meleeSwingStaminaDamageMultiplier;
    public MeleeWeaponType meleeWeaponType;
    public override bool CanShoot(Item item, Player player)
    {
        if (player.altFunctionUse == 2 && isMeleeSwingItem)
        {
            StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            return dashPlayer.CanConsume(meleeSwingStaminaCost);
        }
        return base.CanShoot(item, player);
    }
    public override bool AltFunctionUse(Item item, Player player)
    {
        if (isMeleeSwingItem)
            return true;
        return base.AltFunctionUse(item, player);
    }



}

public static class CoolSwingHelper
{

    /// <summary>
    /// Performs a cool swing!
    /// </summary>
    /// <param name="player"></param>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    /// <param name="type"></param>
    /// <param name="damage"></param>
    /// <param name="knockback"></param>
    public static void DoACoolSwing(
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
        int combo = comboPlayer.ComboCounter;
        int dir = comboPlayer.ComboDirection;
        Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
            player.whoAmI, ai1: dir, ai2: combo);
        comboPlayer.IncreaseCombo();
    }
    public static void DoAStaminaSwing(
        Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
        SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
        int combo = comboPlayer.StaminaComboCounter;
        int dir = comboPlayer.ComboDirection;

        Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
            player.whoAmI, ai1: dir, ai2: combo);
        comboPlayer.IncreaseCombo();


    }

    public static void DoBasicOrStaminaSwing(Item item, Player player,
        EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            if(item.TryGetGlobalItem(out CoolSwingGlobalItem cs))
            {
                player.GetModPlayer<StaminaNDashPlayer>().Consume(cs.meleeSwingStaminaCost);
                player.GetModPlayer<StaminaNDashPlayer>().StaminaEffects(player);
                DoAStaminaSwing(player, source, position, velocity, item.StaminaShoot, (int)(damage * item.StaminaDamageMultiplier), knockback);
                PixelPrimitiveCircleFactory.CreateGenericInBoom(player.Center, Color.White, Color.White, 24, 128);
            }

        }
        else
        {
            DoACoolSwing(player, source, position, velocity, type, damage, knockback);
        }
    }
}

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
                _hitboxStart.Start, _hitboxStart.End, 16, ref collisionPoint);
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
