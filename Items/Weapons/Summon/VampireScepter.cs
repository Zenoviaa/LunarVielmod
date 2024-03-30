using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class VampirePlayer : ModPlayer
    {
		public bool lifesteal;
		public bool isMagic;
        public override void ResetEffects()
        {
            base.ResetEffects();
			lifesteal = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
			if(lifesteal && (!isMagic && proj.DamageType == DamageClass.Summon) || (isMagic && proj.DamageType == DamageClass.Magic))
            {
				float distanceToTarget = Vector2.Distance(Player.position, target.position);
				//10 tile radius
				if(distanceToTarget <= 320)
                {
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

	public class VampireTorchMinionBuff : ModBuff
	{
		private int _vampiricTimer;
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		private void SearchForTargets(Player player, out bool foundTarget, out float distanceFromTarget)
		{
			// Starting search distance
			distanceFromTarget = 700f;
			foundTarget = false;
			if (!foundTarget)
			{
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					float between = Vector2.Distance(npc.Center, player.Center);
					bool inRange = between < distanceFromTarget;
					if (npc.CanBeChasedBy() && inRange)
					{
						foundTarget = true;
						distanceFromTarget = between;
					}
				}
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<VampireTorchMinion>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
				player.lifeRegenCount = 0;

				//Health Loss
				SearchForTargets(player, out bool foundTarget, out float distanceFromTarget);
                if (foundTarget)
				{
					_vampiricTimer++;
					if (_vampiricTimer >= 9 && player.statLife > 10)
					{
						player.statLife += -1;
						_vampiricTimer = 0;
					}
				}

				if (player.GetModPlayer<VampirePlayer>().isMagic)
				{
                    player.GetDamage(DamageClass.Magic) += 0.33f;
                    player.GetModPlayer<VampirePlayer>().lifesteal = true;
				}
				else
				{
                    player.GetDamage(DamageClass.Summon) += 0.33f;
                    player.GetModPlayer<VampirePlayer>().lifesteal = true;
                }

			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
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
			Item.buffType = BuffType<VampireTorchMinionBuff>();
			Item.shoot = ProjectileType<VampireTorchMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Only allow one
			if (player.ownedProjectileCounts[ProjectileType<VampireTorchMinion>()] > 0)
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
            recipe.AddIngredient(ItemType<StickOfWisdom>(), 1);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 12);
			recipe.AddIngredient(ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemType<TerrorFragments>(), 10);
			recipe.AddIngredient(ItemID.BloodMoonStarter, 1);
			recipe.Register();
		}
    }

	public class VampireTorchMinion : ModProjectile,
		IPixelPrimitiveDrawer
	{
		public Vector2[] CirclePos = new Vector2[48];
		public const float Beam_Width = 8;
		public override void SetStaticDefaults()
		{
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 4;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 42;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)// Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			if (!SummonHelper.CheckMinionActive<VampireTorchMinionBuff>(owner, Projectile))
				return;

			//This minion doesn't attack
			Projectile.Center = owner.Center - new Vector2(0, 96);
			Visuals();
		}

		private void Visuals()
        {
            Player owner = Main.player[Projectile.owner];
            DrawHelper.AnimateTopToBottom(Projectile, 5);
			if (Main.rand.NextBool(12))
			{
				int count = 3;
				for (int k = 0; k < count; k++)
				{
					Dust.NewDust(Projectile.position, 8, 8, DustID.Blood);
				}
			}

            DrawHelper.DrawCircle(owner.Center, 320, CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}

        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * Beam_Width;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Red;
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}