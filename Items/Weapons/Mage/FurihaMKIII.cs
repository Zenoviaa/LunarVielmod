using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{

    public class FurihaMKIII : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Ranged;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 0;
            Item.damage = 50;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
                + "\n'Triple Threat!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.mana = 100;
            Item.height = 25;
            Item.useTime = 9999;
            Item.useAnimation = 9999;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<ScissorianHold>();
            Item.scale = 1f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.sellPrice(gold: 2);
            Item.noUseGraphic = true;
            Item.channel = true;
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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

    }
}