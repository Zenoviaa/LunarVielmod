using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class AuroreanStarI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Kaeva Clot");
			/* Tooltip.SetDefault("Flesh-like substance used for many items!" +
			"\nObtained from blood-typed enemies"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 5);
		}

        public override void PostUpdate()
        {
            if (Main.dayTime)
            {
                Item.active = false;
                NetMessage.SendData(MessageID.WorldData);
                NetMessage.SendData(MessageID.SyncItem);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Lighting.AddLight(Item.Center, Color.HotPink.ToVector3() * 1.25f * Main.essScale);
            Rectangle frame;




            if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Item.Center, Item.width, Item.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].noLight = false;
            }
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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(197, 149, 183, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 216, 174, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

    }
}
