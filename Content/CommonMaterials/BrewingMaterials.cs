using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials
{
    public class IllurineScale : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.rare = ModContent.RarityType<IllurineScaleRarity>();
            Item.maxStack = Item.CommonMaxStack;
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

    public class RadiantNectar : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<RadiantNectarRarity>();
        }
    }

    public class MarshScrap : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<MarshScrapRarity>();
        }
    }
    public class MechanizedSoul : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<MechanizedSoulRarity>();
        }
    }
}
