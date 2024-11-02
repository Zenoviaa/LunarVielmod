using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Summons.Minions;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.HeavyMetal
{
    internal class HeavyMetalPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSetBonus = false;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if(hasSetBonus && Player.ownedProjectileCounts[ModContent.ProjectileType<HMArncharMinionLeftProj>()] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone3"), Player.position);
                var EntitySource = Player.GetSource_FromThis();

                int damage = 9;
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, 
                    ModContent.ProjectileType<HMArncharMinionRightProj>(), damage, 1, Player.whoAmI, 0, 0);
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0, 
                    ModContent.ProjectileType<HMArncharMinionLeftProj>(), damage, 1, Player.whoAmI, 0, 0);
                Player.AddBuff(ModContent.BuffType<HMMinionBuff>(), 99999);
            }
            else if(!hasSetBonus)
            {
                Player.ClearBuff(ModContent.BuffType<HMMinionBuff>());
            }
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class HeavyMetalHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Heavy Metal Hat");
			// Tooltip.SetDefault("Increases throwing critical strike chance by 4%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

    

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.maxMinions += 1;
            player.GetModPlayer<HeavyMetalPlayer>().hasSetBonus = true;
            player.setBonus = LangText.SetBonus(this);//"2 Gintze Guards come to fight for you"  + "\n+1 Summons!");  // This is the setbonus tooltip
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
