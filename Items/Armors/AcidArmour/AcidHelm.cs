using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.AcidArmour
{
    internal class AcidPlayer : ModPlayer
    {
        /*
         * Immunity to acid water contamination, 
standing still gives you an acid aura that stays where you were when you leave The aura will deal damage to enemies for a certain amount of time
        */

        private int _acidTimer;
        public bool hasSetBonus;

        public override void ResetEffects()
        {
            hasSetBonus = false;
        }

        public override void PostUpdateEquips()
        {
            if (!hasSetBonus)
                return;

            //Immunity to contamination
            Player.ClearBuff(ModContent.BuffType<AcidFlame>());
            Player.ClearBuff(ModContent.BuffType<Irradiation>());

            //Standing still for the acid aura
            if(Player.velocity == Vector2.Zero 
                && Player.ownedProjectileCounts[ModContent.ProjectileType<AcidAuraProj>()] == 0)
            {
                _acidTimer++;
            }
            else
            {
                _acidTimer = 0;
            }

            if(_acidTimer >= 30)
            {
                int damage = 18;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<AcidAuraProj>(), damage, 1, Player.whoAmI);
                _acidTimer = 0;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class AcidHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("AcidBody").Type && legs.type == Mod.Find<ModItem>("AcidLegs").Type;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Stand still to emit a toxic aura!";
            player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
            player.GetModPlayer<AcidPlayer>().hasSetBonus = true;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
