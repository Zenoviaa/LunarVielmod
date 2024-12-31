using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Winterborn
{
    internal class WinterbornPlayer : ModPlayer
    {
        private int _timer;
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }

        public override void PostUpdateEquips()
        {
            if (!hasSetBonus)
                return;
            _timer--;
            if(_timer <= 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<WinterbornIcicleProj>()] < 3)
            {
                //Spawn one
                int damage = 15;
                int knockback = 2;
                float health = 100;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<WinterbornIcicleProj>(), damage, knockback, Player.whoAmI, ai0: health);
                _timer = 60 * 10;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class WinterbornHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Winterborn Head");
            // Tooltip.SetDefault("Increases Mana Regen by 4%");


            //Drawing hair full
            //ArmorIDs.Head.Sets.DrawFullHair[Type] = true;


            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaRegen += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WinterbornBody>() && legs.type == ModContent.ItemType<WinterbornLegs>();
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Up to three icicles surround you to protect you from attacks!");
            player.GetModPlayer<WinterbornPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes() 
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BorealWood, 8);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
