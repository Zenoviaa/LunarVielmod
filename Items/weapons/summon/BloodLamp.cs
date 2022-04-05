using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Projectiles;
using Stellamod.Items.Materials;
using Terraria.DataStructures;
using Terraria.Audio;
using Stellamod.UI.systems;
using Stellamod.Buffs;

namespace Stellamod.Items.weapons.summon
{
	internal class BloodLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Lamp");
			Tooltip.SetDefault("Enrages your summons by hitting enemies, shows a red mark on them!" +
				"\nSummons a red explosion crystal that hurts foes" +
			"\nEven makes a bloodthirst trail for the summon being empowered!");
		}


		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.mana = 15;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Summon;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/bloodlamp");
			Item.autoReuse = true;
			Item.shoot = ProjectileID.RainbowCrystalExplosion;
			Item.autoReuse = true;
			Item.scale = 0.8f;
			Item.crit = 15;
		}



        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            base.OnHitNPC(player, target, damage, knockBack, crit);
			if (Main.rand.Next(5) == 0)
				target.AddBuff(ModContent.BuffType<DeathmultiplierBloodLamp>(), 480);
			

		}

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
		
			if (Main.rand.Next(5) == 0)
				target.AddBuff(ModContent.BuffType<DeathmultiplierBloodLamp>(), 480);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)

		{

			
			
			int projectileID = Projectile.NewProjectile(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
			Projectile projectile = Main.projectile[projectileID];

			BloodLampProjectileModifications globalProjectile = projectile.GetGlobalProjectile<BloodLampProjectileModifications>();
			globalProjectile.sayTimesHitOnThirdHit = false;
			globalProjectile.applyBuffOnHit = true;



			


			return false;

		}

       
	}
}