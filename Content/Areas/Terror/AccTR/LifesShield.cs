using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.AccTR
{
    public class LifesShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<LifesShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankCard>();
        }
    }

   
    public class LifeShieldGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool isTouchingShield;
        public override void ResetEffects(NPC npc)
        {
            base.ResetEffects(npc);
            isTouchingShield = false;
        }
    }

    public class LifeShieldPlayer : ModPlayer
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            LifeShieldGlobalNPC lifeShieldGlobalNPC = target.GetGlobalNPC<LifeShieldGlobalNPC>();
            if (lifeShieldGlobalNPC.isTouchingShield)
            {
                Player.Heal(2);
            }
        }
    }

    public class LifesShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);
            npc.GetGlobalNPC<LifeShieldGlobalNPC>().isTouchingShield = true;
        }
    }
}
