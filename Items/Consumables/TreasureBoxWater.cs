using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Accessories.Catacombs;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    internal class TreasureBoxWater : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 46; // The item texture's width
            Item.height = 42; // The item texture's height
            Item.rare = ItemRarityID.Expert;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.consumable = true;
        }

        public override bool CanRightClick() //this make so you can right click this item
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            base.ModifyItemLoot(itemLoot);
            itemLoot.Add(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<AquaticEmblem>(),
                ModContent.ItemType<WaterproofHeadphones>(),
                ModContent.ItemType<OceancrestShield>(),
                ModContent.ItemType<Aquamare>(),
                ModContent.ItemType<MagicalAxe>(),
                ModContent.ItemType<WaterSong>(),
                ModContent.ItemType<TyphoonInABottle>()));
            //itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiracleThread>(), chanceDenominator: 1, minimumDropped: 30, maximumDropped: 40));
        }

        public override void RightClick(Player player)
        {
            // We have to replicate the expert drops from MinionBossBody here via QuickSpawnItem

            var entitySource = player.GetSource_OpenItem(Type);



            if (Main.rand.NextBool(1))
            {
                player.QuickSpawnItem(entitySource, ItemID.SoulofFlight, Main.rand.Next(1, 17));
            }
            if (Main.rand.NextBool(1))
            {
                player.QuickSpawnItem(entitySource, ItemID.MythrilBar, Main.rand.Next(1, 12));
            }
            if (Main.rand.NextBool(1))
            {
                player.QuickSpawnItem(entitySource, ItemID.AdamantiteBar, Main.rand.Next(1, 7));
            }
            if (Main.rand.NextBool(1))
            {
                player.QuickSpawnItem(entitySource, ItemID.CobaltBar, Main.rand.Next(1, 17));
            }

            if (Main.rand.NextBool(1))
            {
                player.QuickSpawnItem(entitySource, ModContent.ItemType<KaleidoscopicInk>(), Main.rand.Next(1, 13));
            }

            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(entitySource, ModContent.ItemType<ArtisanBar>(), Main.rand.Next(1, 3));
            }


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
