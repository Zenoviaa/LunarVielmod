using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Tiles.Ishtar;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class IshtarCandle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 76;
            Item.rare = ModContent.RarityType<Helpers.GoldenSpecialRarity>();
            Item.maxStack = Item.CommonMaxStack;
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 25);
            Item.defense = 6;
           
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            Lighting.AddLight(player.Center, Color.Purple.ToVector3() * 10.75f * Main.essScale);
        }
    

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }
    }
}
