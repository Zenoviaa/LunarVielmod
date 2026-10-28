using Stellamod.Common.GunSystem;
using Stellamod.Common.MeleeSwingSystem;
using Stellamod.Common.StaminaSystem;
using Stellamod.Content.Scrolls;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.MeteorHammerSystem;

/*So for meteor hammers, after swinging them they'll sit out and drag behind you
To keep the system flexible, setting an item to being a meteor hammer should only give this functionality:

- Holding the item will automatically summon the associated held projectile
- Attacking will cause the projectile to go into swing state, towards your cursor. It can do whatever the hell it wants
- ALL Meteor Hammers have a tipper mechanic where enemies that take a direct hit will take bonus damage, (the hitbox enables when it reaches its destination)
    
*/

public class MeteorHammerSets : ModSystem
{
    public static int[] MeteorHammerStaminaCost;
    public static bool[] IsMeteorHammer;
    public override void ResizeArrays()
    {
        base.ResizeArrays();
        IsMeteorHammer = ItemID.Sets.Factory.CreateBoolSet();
        MeteorHammerStaminaCost = ItemID.Sets.Factory.CreateIntSet();
    }
}

public static class MeteorHammerExtensions
{
    extension(ItemID.Sets)
    {
        public static int[] MeteorHammerStaminaCost => MeteorHammerSets.MeteorHammerStaminaCost;
        public static bool[] IsMeteorHammer => MeteorHammerSets.IsMeteorHammer;
    }

    extension(Item item)
    {
        public int MeteorHammerStaminaCost
        {
            get
            {
                return ItemID.Sets.MeteorHammerStaminaCost[item.type];
            }
            set
            {
                ItemID.Sets.MeteorHammerStaminaCost[item.type] = value;
            }
        }
        public bool IsMeteorHammer
        {
            get
            {
                return ItemID.Sets.IsMeteorHammer[item.type];
            }
            set
            {
                ItemID.Sets.IsMeteorHammer[item.type] = value;
            }
        }

        public void DefaultToMeteorHammer<ProjType>() 
            where ProjType : ModProjectile
        {
            item.DamageType = DamageClass.Ranged;
            item.useTime = item.useAnimation = 36;
            item.damage = 13;
            item.knockBack = 1;
            item.useStyle = ItemUseStyleID.Swing;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = false;
            item.autoReuse = false;
            item.shootSpeed = 1;
            item.shoot = ModContent.ProjectileType<ProjType>();
        }
    }

    extension(Projectile projectile)
    {
        /// <summary>
        /// AI0, the state of the meteor hammer
        /// </summary>
        public MeteorHammerState HammerState
        {
            get => (MeteorHammerState)projectile.ai[0];
            set => projectile.ai[0] = (float)value;
        }

        public int HammerHeldItemType
        {
            get => (int)projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public ref float HammerTimer => ref projectile.ai[2];
        public void DefaultToMeteorHammerBall()
        {
            projectile.DamageType = DamageClass.Ranged;
            projectile.LocalHitOnce = true;
            projectile.ignoreWater = true;
            projectile.light = 0.3f;
            projectile.friendly = true;
            projectile.WidthAndHeight = 64;

        }
    }
}

public enum MeteorHammerState : byte
{
    SwingOut,
    StaminaSwingOut,
    Hold,
}

public class MeteorHammerGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool isMeteorHammerProjectile;

}
public class MeteorHammerGlobalItem : GlobalItem
{
    public override bool AltFunctionUse(Item item, Player player)
    {
        if (item.IsMeteorHammer)
            return true;
        return base.AltFunctionUse(item, player);
    }
    public override bool CanUseItem(Item item, Player player)
    {
        if (item.IsMeteorHammer)
        {
            if (player.altFunctionUse == 2)
            {
                return player.Stamina >= item.MeteorHammerStaminaCost;
            }
        }

        return base.CanUseItem(item, player);
    }
    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (item.IsMeteorHammer)
        {
            if (player.ownedProjectileCounts[item.shoot] == 0)
            {
                Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    type,
                    damage,
                    knockback,
                    player.whoAmI,
                    ai0: (int)MeteorHammerState.SwingOut,
                    ai1: item.type);
            }

            if (player.altFunctionUse == 2)
            {
                player.StaminaPlayer.Consume(item.MeteorHammerStaminaCost);
                player.StaminaPlayer.StaminaEffects(player);
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI)
                        continue;
                    if (proj.type != item.shoot)
                        continue;
                    proj.ai[0] = (int)MeteorHammerState.StaminaSwingOut;
                    proj.ai[2] = 0;
                }
            }
            else
            {
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI)
                        continue;
                    if (proj.type != item.shoot)
                        continue;
                    proj.ai[0] = (int)MeteorHammerState.SwingOut;
                    proj.ai[2] = 0;
                }
            }
            return false;
        }
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}