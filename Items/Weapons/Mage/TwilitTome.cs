using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class TwilitTome : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public int Star;
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
     
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<TwilitTomeRed>();
            Item.shootSpeed = 10f;
            Item.mana = 5;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.player[Main.myPlayer];
            if (Main.dayTime)
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Mage/TwilitTomeDay").Value;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
                return false;
            }
            else
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Mage/TwilitTome").Value;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Star += 1;
            if (Main.dayTime)
            {
                if (Star >= 2)
                {
                    Star = 0;
                    type = ModContent.ProjectileType<TwilitTomeRed>();
                }
                if (Star == 1)
                {
                    type = ModContent.ProjectileType<TwilitTomeGreen>();
                }

            }
            else
            {
                if (Star >= 2)
                {
                    Star = 0;
                    type = ModContent.ProjectileType<TwilitTomeBlue>();
                }
                if (Star == 1)
                {
                    type = ModContent.ProjectileType<TwilitTomePurple>();
                }
            }

        }



    }
}
