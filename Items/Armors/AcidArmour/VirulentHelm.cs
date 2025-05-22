using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
            if (Player.velocity == Vector2.Zero
                && Player.ownedProjectileCounts[ModContent.ProjectileType<AcidAuraProj>()] == 0)
            {
                _acidTimer++;
            }
            else
            {
                _acidTimer = 0;
            }

            if (_acidTimer >= 30)
            {
                int damage = 18;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<AcidAuraProj>(), damage, 1, Player.whoAmI);
                _acidTimer = 0;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class VirulentHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05F;
            player.GetCritChance(DamageClass.Generic) += 8;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VirulentArmor>() && legs.type == ModContent.ItemType<VirulentLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Stand still to emit a toxic aura!");
            player.moveSpeed += 0.2f;
            player.GetModPlayer<AcidPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
