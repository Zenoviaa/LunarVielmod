using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicSystem
{
    internal abstract class Enchantment : ModItem
    {
        public bool isTimedEnchantment;
        public override void SetDefaults()
        {
            //Set some defaults for the enchantments
            Item.width = 36;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(gold: 1);
            Item.accessory = false;
        }

        public virtual void AI(MagicProjectile mProj)
        {

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}
