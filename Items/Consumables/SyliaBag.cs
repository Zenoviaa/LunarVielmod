
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Wings;
using Stellamod.Items.Armors.Miracle;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class SyliaBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 36; // The item texture's width
            Item.height = 34; // The item texture's height
            Item.rare = ItemRarityID.Expert;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.consumable = true;
            Item.expert = true;
        }

        public override bool CanRightClick() //this make so you can right click this item
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            base.ModifyItemLoot(itemLoot);
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LittleScissor>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiracleThread>(), minimumDropped: 30, maximumDropped: 40));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SewingKit>(), chanceDenominator: 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiracleWings>(), chanceDenominator: 4));
            
            IItemDropRule armorRule = ItemDropRule.Common(ModContent.ItemType<MiracleHead>(), chanceDenominator: 4);
            armorRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MiracleBody>(), 1));
            itemLoot.Add(armorRule);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(220, 126, 58, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 216, 121, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
