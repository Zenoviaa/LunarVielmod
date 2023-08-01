using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Terraria.DataStructures;
using Terraria.Audio;
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Weapons.Swords
{
    public class AssassinsSlash : ModItem
    {
        public int Hits;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ivyen Saber");
            // Tooltip.SetDefault("Has a chance to poison enemies.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {
            Item.damage = 29;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<MyPlayer>().AssassinsSlashnpc != null && Hits == 7)
                {
                    player.GetModPlayer<MyPlayer>().AssassinsSlash = true;
                    Hits = 0;
                    Item.noUseGraphic = true;
                }


            }
            else
            {
                Item.noUseGraphic = false;
            }
  
            return base.CanUseItem(player);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MeleeDrive>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 10);
            recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 15);
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Plantera_Green);
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hits != 7)
            {
                Hits += 1;
            }

            if(Hits == 6)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/AssassinsSlashCharge"), player.position);              
                Hits = 7;
            }
            player.GetModPlayer<MyPlayer>().AssassinsSlashnpc = target;
            player.AddBuff(ModContent.BuffType<AssassinsSlashBuff>(), 480);
            target.AddBuff(ModContent.BuffType<AssassinsSlashBuff>(), 480);
        }
    }
}