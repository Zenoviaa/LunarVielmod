using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Summons.Minions;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class VampirePlayer : ModPlayer
    {
		public bool lifesteal;
		public bool isMagic;
		public float cooldown;
        public override void ResetEffects()
        {
            base.ResetEffects();
			lifesteal = false;
        }

        public override void UpdateEquips()
        {
            base.UpdateEquips();
			cooldown--;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
			if(lifesteal && (!isMagic && proj.DamageType == DamageClass.Summon) || (isMagic && proj.DamageType == DamageClass.Magic))
            {
				float distanceToTarget = Vector2.Distance(Player.position, target.position);
				//10 tile radius
				if(distanceToTarget <= 320 && Main.rand.NextBool(6) && cooldown <= 0)
                {
					cooldown = 30;
                    //Life steal for 5% of the damage
                    float healFactor = damageDone * 0.08f;
					int healthToHeal = (int)healFactor;
					healthToHeal = Math.Clamp(healthToHeal, 1, 20);
					Player.Heal(healthToHeal);

					int count = 8;
					float degreesPer = 360 / (float)count;
					for (int k = 0; k < count; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 2;
						Dust.NewDust(target.Center, 0, 0, DustID.BloodWater, vel.X, vel.Y);
					}
					Dust.QuickDustLine(Player.Center, target.Center, 100f, Color.Red);
					SoundEngine.PlaySound(SoundID.NPCHit18, target.Center);
				}
			}
        }
    }

	public class VampireScepter : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

		public override void SetDefaults()
		{
			Item.damage = 34;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 40;
			Item.height = 48;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<VampireTorchMinionBuff>();
			Item.shoot = ModContent.ProjectileType<VampireTorchMinionProj>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Only allow one
			if (player.ownedProjectileCounts[Item.shoot] > 0)
				return false;
				// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;
            player.GetModPlayer<VampirePlayer>().isMagic = IsSwapped;
            if (IsSwapped)
            {
                projectile.DamageType = Item.DamageType;
            }

		
            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<StickOfWisdom>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 12);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ModContent.ItemType<TerrorFragments>(), 10);
			recipe.AddIngredient(ItemID.BloodMoonStarter, 1);
			recipe.Register();
		}
    }
}