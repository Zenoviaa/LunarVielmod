using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Miracle
{
    [AutoloadEquip(EquipType.Head)]
    public class MiracleHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.maxTurrets += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+1 Max Sentry\nWIP";  // This is the setbonus tooltip
            player.maxTurrets += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MiracleBody>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes() 
        {
            CreateRecipe()
                .AddIngredient(ItemID.WizardHat, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 10)
                .AddIngredient(ModContent.ItemType<WanderingFlame>(), 8)
                .AddIngredient(ModContent.ItemType<DarkEssence>(), 4)
                .AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.PreDrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }
    }
}
