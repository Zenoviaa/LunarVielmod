using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class BloodLamp : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetDefaults()
		{
			Item.damage = 4;
			Item.mana = 15;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Summon;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/bloodlamp");
			Item.autoReuse = true;
			Item.shoot = ProjectileID.RainbowCrystalExplosion;
			Item.autoReuse = true;
			Item.scale = 0.8f;
			Item.crit = 15;
			Item.value = 1200;
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(player, target, hit, damageDone);
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<DeathMultiplierBloodLamp>(), 480);
		}
		public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<DeathMultiplierBloodLamp>(), 480);
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