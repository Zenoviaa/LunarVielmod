using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;

using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Weapons.Magic;
using Stellamod.Items.Materials;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Stellamod.Projectiles.Weapons.Bow
{
    public class CyroBolt : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyro Bolt");
            // Tooltip.SetDefault("Casts an uncontrollable ice bolt");
        }

        public override void SetDefaults()
        {

            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<Zap>();
            Item.shootSpeed = 10f;
            Item.mana = 20;

        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/CyroBolt1"), player.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/CyroBolt2"), player.position);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.IceBlock, 15);
            recipe.AddIngredient(ItemID.FrostCore, 1);
            recipe.AddIngredient(ItemID.WaterBolt, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
