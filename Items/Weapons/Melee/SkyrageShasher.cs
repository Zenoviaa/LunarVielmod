using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Melee
{
    public class SkyrageShasher : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Magic;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.damage = 30;
			Item.useAnimation = 25;
			Item.useTime = 30;
		}
		public float ArrowCount = 0;
		public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skyrage Shasher");
        }

        public override void SetDefaults() 
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 1, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.shoot = ProjectileType<WindSythe>();
			Item.shootSpeed = 15f;
			
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			ArrowCount += 1;
			if (ArrowCount >= 4)
			{
				Item.shootSpeed = 20f;
				for (int index1 = 0; index1 < 19; ++index1)
				{
					int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.Electric, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
					Main.dust[index2].noGravity = true;
					Main.dust[index2].velocity *= 0.5f;
					Main.dust[index2].scale *= 1.2f;
				}
				type = ModContent.ProjectileType<WindSytheBlue>();
				ArrowCount = 0;
            }
            else
			{
				Item.shootSpeed = 15f;
				for (int index1 = 0; index1 < 19; ++index1)
				{
					int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.Cloud, velocity.X, velocity.X, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
					Main.dust[index2].noGravity = true;
					Main.dust[index2].velocity *= 0.5f;
					Main.dust[index2].scale *= 1.2f;
				}
				type = ModContent.ProjectileType<WindSythe>();
			}
			Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
			if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
			{
				position += Offset;
            }
            var EntitySource = player.GetSource_FromThis();
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(2));
			Projectile proj = Projectile.NewProjectileDirect(EntitySource, position, velocity, type, Item.damage, Item.knockBack, Item.playerIndexTheItemIsReservedFor, 0, 0);
			proj.netUpdate = true;
			return false;
		}
		
	}
}