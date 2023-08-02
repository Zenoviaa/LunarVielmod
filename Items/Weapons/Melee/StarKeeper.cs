using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Bow;
using Terraria.DataStructures;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Magic;
using Stellamod.Items.Accessories.Runes;

using Stellamod.Projectiles.Spears;
using Terraria.Audio;

namespace Stellamod.Items.Weapons.Melee
{
    public class StarKeeper : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 21;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<StarKeeperStar>();
            Item.shootSpeed = 20f;
            Item.DamageType = DamageClass.Melee;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 8);
            recipe.AddIngredient(ItemID.EnchantedSword, 1);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RedTorch);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarKeeper"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarKeeper2"));
            }

            return true;
        }
    }
}