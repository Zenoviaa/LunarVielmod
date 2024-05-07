using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class CinderedQuiverPlayer : ModPlayer
    {
        public bool hasQuiver;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasQuiver = false;
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly && hasQuiver)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordThrow"), position);
                type = ProjectileID.HellfireArrow;
                damage += 7;
                velocity *= 2f;
            }
        }
    }

    internal class CinderedQuiver : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<CinderedQuiverPlayer>().hasQuiver = true;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
            recipe.AddIngredient(ModContent.ItemType<MoltenScrap>(), 7);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ItemID.HellfireArrow, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
