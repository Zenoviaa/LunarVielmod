using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Summons.VoidMonsters;
using Stellamod.Projectiles.Swords;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class XScissorComboPlayer : ModPlayer
	{
		public float speed = 1f;
		public float timer;
        public override void UpdateEquips()
        {
            base.UpdateEquips();
			timer++;
			if(timer >= 120)
			{
				speed = 1;
                timer = 0;
			}
        }
    }

	public class XScissor : ModItem
    {
		private int _attackStyle;
		private int _dir;
        public override void SetDefaults()
        {
			Item.damage = 100;
			Item.knockBack = 3f;
			Item.mana = 20;
			Item.width = 76;
			Item.height = 80;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon;

			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<XScissorMinion>();
		}

        private void ChangeForm(int newForm)
        {
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
                Item.damage = 166;
				Item.UseSound = null; 
				Item.DamageType = DamageClass.Melee;
                Item.mana = 4;
                Item.useTime = 5;
                Item.useAnimation = 5;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noUseGraphic = true;

                Item.mana = 0;

                Item.shoot = ModContent.ProjectileType<XScissorMiracleProj>();
                Item.shootSpeed = 1;
            }
            else if (_attackStyle == 0)
            {
      
                Item.damage = 100;
                Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph");
                Item.DamageType = DamageClass.Summon;
                Item.useTime = 18;
                Item.useAnimation = 18;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noUseGraphic = false;
                Item.mana = 20;

                Item.shoot = ModContent.ProjectileType<XScissorMinion>();
                Item.shootSpeed = 0;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            if (_attackStyle == 0)
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Right click to change form, requires a Sewing Kit")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }
            else
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Changed by Sewing Kit, effects may be incorrect...")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.GetModPlayer<SewingKitPlayer>().hasSewingKit)
            {
                if (_attackStyle == 0)
                {
                    ChangeForm(1);
                }
                else
                {
                    ChangeForm(0);
                }

                int sound = Main.rand.Next(0, 3);
                switch (sound)
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), player.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), player.position);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), player.position);
                        break;
                }
                return false;
            }

            return base.CanUseItem(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            if (_attackStyle == 1)
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Melee/XScissorMiracle").Value;
                Vector2 size = new Vector2(52, 52);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
            }

            if (_attackStyle == 0)
			{
                //Spawn at the mouse cursor position
                position = Main.MouseWorld;

                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                projectile.originalDamage = Item.damage;

                player.UpdateMaxTurrets();
			}
			else
			{
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
				float speed = player.GetModPlayer<XScissorComboPlayer>().speed;
				float speedProgress = speed / 3;
                //Sound
                if (Main.rand.NextBool(2))
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
                    soundStyle.PitchVariance = 0.15f;
					soundStyle.Pitch = 0.75f + speedProgress * 0.2f;
                    SoundEngine.PlaySound(soundStyle, player.position);
                }
                else
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2");
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = 0.75f + speedProgress * 0.2f;
                    SoundEngine.PlaySound(soundStyle, player.position);
                }

                if (_dir == -1)
                {
                    _dir = 1;
                }
                else if (_dir == 1)
                {
                    _dir = -1;
                }
                else
                {
                    _dir = 1;
                }

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, _dir);
            }

			return false;
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.Excalibur, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 12)
				.AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }

	public class XScissorMinion : ModProjectile
	{
		private int _counter;
		private Projectile _voidRiftProjectile1;
		private Projectile _voidRiftProjectile2;
		private const int Void_Eater_Big_Chance = 12;
		private const int Fire_Range = 768;
		private enum SummonState
        {
			X_Slash_Telegraph,
			X_Slash,
			Void_Rift
        }
		private SummonState _summonState = SummonState.X_Slash_Telegraph;
		//Lower = faster
		private const int Fire_Rate = 49;
		public override void SetStaticDefaults()
		{
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;

			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 28;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;

			// Only determines the damage type
			//Projectile.minion = true;
			Projectile.sentry = true;
			Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 0f;

			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
            switch (_summonState)
            {
				case SummonState.Void_Rift:
					_counter++;
					NPC npcToTarget = NPCHelper.FindClosestNPC(Projectile.position, Fire_Range);
					if (_counter >= Fire_Rate && npcToTarget != null)
					{
						Player owner = Main.player[Projectile.owner];
						int projToFire = ModContent.ProjectileType<VoidEaterMini>();
						if (Main.rand.NextBool(Void_Eater_Big_Chance))
						{
							projToFire = ModContent.ProjectileType<VoidEater>();
						}

						Vector2 velocityToTarget = VectorHelper.VelocityDirectTo(Projectile.position, npcToTarget.position, 5);
						var proj = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, velocityToTarget,
							projToFire, Projectile.damage, Projectile.knockBack, owner.whoAmI);
						proj.DamageType = DamageClass.Summon;

						//Cool little circle visual
						for (int i = 0; i < 16; i++)
						{
							Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
							Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<VoidParticle>(),
								default(Color), 1 / 3f);
							p.layer = Particle.Layer.BeforeProjectiles;
						}

						//Firing Sound :P
						SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
						_counter = 0;
					}
					break;
            }

			Visuals();
		}

		private void SpawnVoidRiftProjectiles()
        {
			Player owner = Main.player[Projectile.owner];
			_voidRiftProjectile1 = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, Vector2.Zero,
				ModContent.ProjectileType<VoidRift>(), Projectile.damage*2, Projectile.knockBack,
				owner.whoAmI);
			_voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);
			_voidRiftProjectile1.DamageType = DamageClass.Summon;

			_voidRiftProjectile2 = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, Vector2.Zero,
				ModContent.ProjectileType<VoidRift>(), Projectile.damage*2, Projectile.knockBack, owner.whoAmI);
			_voidRiftProjectile2.rotation = MathHelper.ToRadians(45);
			_voidRiftProjectile2.DamageType = DamageClass.Summon;
		}

		private void KillVoidRiftProjectiles()
        {
			_voidRiftProjectile1?.Kill();
			_voidRiftProjectile2?.Kill();
		}

		private void Visuals()
		{
            switch (_summonState)
            {
				case SummonState.X_Slash_Telegraph:
					Particle telegraphPart1 = ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, 
						ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);
					Particle telegraphPart2 = ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, 
						ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), default(Color), 1f);
					telegraphPart1.rotation = MathHelper.ToRadians(-45);
					telegraphPart2.rotation = MathHelper.ToRadians(45);
					_summonState = SummonState.X_Slash;
					break;
				case SummonState.X_Slash:
					_counter++;
					if(_counter > RipperSlashTelegraphParticle.Animation_Length)
					{
						Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner, 0f, 0f);
						Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
							ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner, 
							ai1: MathHelper.ToRadians(90));

						_counter = 0;
						_summonState = SummonState.Void_Rift;
						SpawnVoidRiftProjectiles();
					}
                    break;
                case SummonState.Void_Rift:
					_voidRiftProjectile1.timeLeft = 2;
					_voidRiftProjectile1.Center = Projectile.Center;

					_voidRiftProjectile2.timeLeft = 2;
					_voidRiftProjectile2.Center = Projectile.Center;
					break;
            }

            //It needs to make two of those particles
            //Then have a delay before actually enabling the AI and void rift particle
			Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
		}

        public override void OnKill(int timeLeft)
        {
			KillVoidRiftProjectiles();
		}
    }
}
