using Stellamod.Core.Bases;
using Terraria;
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
