using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Buffs.Dusteffects;
using Stellamod.Buffs.PocketDustEffects;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterEx
{
    public class IgniterStart : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("IgniterStart");
		}
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 30;


		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
			float projSpeed = 22f; // The speed at which the projectile moves towards the target

			// Trying to find NPC closest to the projectile
			NPC closestNPC = FindClosestNPCS(maxDetectRadius);
			if (closestNPC == null)
				return;

			// If found, change the velocity of the projectile and turn it in the direction of the target
			// Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
			Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
			Projectile.tileCollide = false;
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPCS(float maxDetectDistance)
		{
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs(max always 200)
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC target = Main.npc[k];

				// Check if NPC able to be targeted. It means that NPC is
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				if (target.CanBeChasedBy() && target.HasBuff<Dusted>())
				{
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}
			Projectile.rotation += 0.1f;
			{

				Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
				Projectile.rotation = Projectile.velocity.ToRotation();
				if (Projectile.velocity.Y > 16f)
				{
					Projectile.velocity.Y = 16f;
				}
			}
			return closestNPC;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			NPC npc = target;
			if (npc.active && npc.HasBuff<ArcaneDust>())
			{
				ShakeModSystem.Shake = 8;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode"));
				for (int j = 0; j < 7; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 5, ParticleManager.NewInstance<BurnParticle2>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					ParticleManager.NewParticle(Projectile.Center, speed * 3, ParticleManager.NewInstance<BurnParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					npc.SimpleStrikeNPC((int)(Projectile.damage * 1f), 1, crit: false, Projectile.knockBack);
					npc.RequestBuffRemoval(ModContent.BuffType<ArcaneDust>());
				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
                {
					if (Main.rand.NextBool(7))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC((int)(Projectile.damage * 1f), 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}




			if (npc.active && npc.HasBuff<AivanDust>())
			{
				ShakeModSystem.Shake = 6;

		
				for (int j = 0; j < 1; j++)
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/GhostExcalibur1"));
					Projectile.scale = 1.5f;
					ShakeModSystem.Shake = 6;
					npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<AivanKaboom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(7))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
						npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}






            if (npc.active && npc.HasBuff<IshyBuff>())
            {
                ShakeModSystem.Shake = 6;


                for (int j = 0; j < 1; j++)
                {
                    //ExplosionHolyNecklaceWave
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ExplosionGaseous") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
                    Projectile.scale = 1.5f;
                    ShakeModSystem.Shake = 6;
                    npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
                    npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
                    float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                    float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<IshBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                }

                if (player.GetModPlayer<MyPlayer>().LuckyW)
                {
                    if (Main.rand.NextBool(7))
                    {
                        CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
                        npc.SimpleStrikeNPC(Projectile.damage * 13, 1, crit: false, Projectile.knockBack);
                        npc.SimpleStrikeNPC(Projectile.damage * 13, 1, crit: false, Projectile.knockBack);
                    }

                }

                if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
                {
                    if (Main.rand.NextBool(10))
                    {
                        CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
                        npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
                        float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                        float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                        npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

                    }

                }
            }



            if (npc.active && npc.HasBuff<JungleDust>())
			{
				ShakeModSystem.Shake = 6;


				for (int j = 0; j < 1; j++)
				{
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StaalkerDescend") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
                    Projectile.scale = 1.5f;
					ShakeModSystem.Shake = 6;
					npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					npc.SimpleStrikeNPC(Projectile.damage * 3, 1, crit: false, Projectile.knockBack);
					float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<JungleBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(7))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 7, 1, crit: false, Projectile.knockBack);
						npc.SimpleStrikeNPC(Projectile.damage * 7, 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}



			if (npc.active && npc.HasBuff<CrystalDust>())
			{
				ShakeModSystem.Shake = 6;


				for (int j = 0; j < 1; j++)
				{
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
                    Projectile.scale = 1.5f;
					ShakeModSystem.Shake = 5;
					npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
					npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
					float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
					float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<CrystalBloom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(15))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
						npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}

			if (npc.active && npc.HasBuff<FlameDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.OnFire, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 250;
					Timer++;
					if (Timer == 150)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 170)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);



						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
							}

						}
						if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
						{
							
								
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, 
									ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							

						}

					}
					if (Timer == 200)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}






            if (npc.active && npc.HasBuff<MushyBuff>())
            {

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
                for (int j = 0; j < 20; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                    target.AddBuff(BuffID.OnFire, 720);
                    target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

                    Projectile.timeLeft = 250;
                    Timer++;
                    if (Timer == 150)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

                    }


                    if (Timer == 170)
                    {
                        Vector2 velocity = npc.velocity;
                        if (npc.active && npc.HasBuff<EXPtime>())
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Green") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
                            Projectile.scale = 1.5f;
                            ShakeModSystem.Shake = 10;
                            npc.SimpleStrikeNPC(Projectile.damage * 9, 1, crit: false, Projectile.knockBack);
                            npc.SimpleStrikeNPC(Projectile.damage * 9, 1, crit: false, Projectile.knockBack);
                            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<MushyBoom>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);



                        }

                        if (player.GetModPlayer<MyPlayer>().LuckyW)
                        {
                            if (Main.rand.NextBool(10))
                            {
                                CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
                                npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
                                npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
                            }

                        }
                        if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
                        {


                            npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                            npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);



                        }

                    }
                    if (Timer == 200)
                    {
                        Projectile.Kill();
                        npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

                    }


                }
            }


            if (npc.active && npc.HasBuff<IlluBuff>())
            {

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
                for (int j = 0; j < 20; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<BurnParticle2>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                    target.AddBuff(BuffID.OnFire, 720);
                    target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

                    Projectile.timeLeft = 250;
                    Timer++;
                    if (Timer == 150)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

                    }


                    if (Timer == 170)
                    {
                        Vector2 velocity = npc.velocity;
                        if (npc.active && npc.HasBuff<EXPtime>())
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Green") { Pitch = Main.rand.NextFloat(-10f, 10f) }, player.Center);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
                            Projectile.scale = 1.5f;
                            ShakeModSystem.Shake = 10;
                            npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                            npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
                            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<IlluredBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);



                        }

                        if (player.GetModPlayer<MyPlayer>().LuckyW)
                        {
                            if (Main.rand.NextBool(3))
                            {
                                CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
                                npc.SimpleStrikeNPC(Projectile.damage * 24, 1, crit: false, Projectile.knockBack);
                                npc.SimpleStrikeNPC(Projectile.damage * 24, 1, crit: false, Projectile.knockBack);
                            }

                        }
                        if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
                        {


                            npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                            npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                                ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);



                        }

                    }
                    if (Timer == 200)
                    {
                        Projectile.Kill();
                        npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

                    }


                }
            }





            if (npc.active && npc.HasBuff<SpiritDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<VoidParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.Confused, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 250;
					Timer++;
					if (Timer == 140)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Briskfly"));

					}

					if (Timer == 140)
                    {
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, 
							ModContent.ProjectileType<KaBoomSpirit>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
					}
					if (Timer == 170)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Yumiko4"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.NebulaArcanumExplosionShot, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							



						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
							}
						}

						if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
						{
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, 
								ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
						}

					}
					if (Timer == 200)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}







			if (npc.active && npc.HasBuff<VoiddDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<ShadeParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.OnFire, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 250;
					Timer++;
					if (Timer == 150)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 170)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.NebulaArcanumExplosionShot, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 14, 1, crit: false, Projectile.knockBack);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa + 129, Projectile.position.Y + speedYa + 129, speedXa * 0, speedYa * 0, ModContent.ProjectileType<VoidKaboom>(), Projectile.damage * 6, 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 19, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 19, 1, crit: false, Projectile.knockBack);
							}

						}

					}
					if (Timer == 200)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}




			if (npc.active && npc.HasBuff<AlcadDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<FabledParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.OnFire3, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 300;
					Timer++;
					if (Timer == 200)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 220)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 7;
							npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
						
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, 
								ModContent.ProjectileType<KaBoomAlcadizz>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, 
									ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}
						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 11, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
							}

						}

					}


					if (Timer == 250)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}


			if (npc.active && npc.HasBuff<GovheilDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion"));
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<morrowstar>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.Poisoned, 720);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);

					Projectile.timeLeft = 300;
					Timer++;
					if (Timer == 200)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 220)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));

							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 7;
							npc.SimpleStrikeNPC(Projectile.damage * 13, 1, crit: false, Projectile.knockBack);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa + 200, Projectile.position.Y + speedYa + 200, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GovheilKaboom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 23, 1, crit: false, Projectile.knockBack);
							}

						}

					

					}

					if (Timer == 230)
                    {
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));
						npc.SimpleStrikeNPC(Projectile.damage * 7, 1, crit: false, Projectile.knockBack);
					}
					if (Timer == 240)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));
						npc.SimpleStrikeNPC(Projectile.damage * 6, 1, crit: false, Projectile.knockBack);
					}
					if (Timer == 250)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));
						npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
					}
					if (Timer == 251)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}










			if (npc.active && npc.HasBuff<ShadeDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ShadowExplosion"));
				for (int d = 0; d < 20; d++)
				{
					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedea * 4, ParticleManager.NewInstance<ShadeParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(ModContent.BuffType<EXPtime2>(), 1000);

					Projectile.timeLeft = 200;
					Timer++;
					if (Timer == 100)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 140)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime2>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ExplosionBurstBomb"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							float speedXab = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYab = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoomShade>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
							}

						}

					


					}
					if (Timer == 180)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}




			if (npc.active && npc.HasBuff<EldritchDust>())
			{
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SpidrSummon"));
				
				for (int d = 0; d < 20; d++)
				{
					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedea * 4, ParticleManager.NewInstance<BurnParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(ModContent.BuffType<EXPtime2>(), 1000);

					Projectile.timeLeft = 200;
					Timer++;
					if (Timer == 100)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StormDragon_WaveCharge"));

					}


					if (Timer == 140)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime2>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/StormDragon_LightingZap"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.LostSoulFriendly, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 16, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 16, 1, crit: false, Projectile.knockBack);
							float speedXab = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYab = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<EldritchBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 12, 1, crit: false, Projectile.knockBack);
							}

						}




					}
					if (Timer == 180)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}



			if (npc.active && npc.HasBuff<AgreviDust>())
			{

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SpidrSummon"));
				for (int d = 0; d < 20; d++)
				{
					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedea * 4, ParticleManager.NewInstance<UnderworldParticle1>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(ModContent.BuffType<EXPtime2>(), 1000);

					Projectile.timeLeft = 200;
					Timer++;
					if (Timer == 100)
					{
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

					}


					if (Timer == 140)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime2>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
							npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
							float speedXab = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYab = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<AgreviBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXab, Projectile.position.Y + speedYab, speedXab * 0, speedYab * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 24, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 24, 1, crit: false, Projectile.knockBack);
							}

						}




					}
					if (Timer == 180)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}





			if (npc.active && npc.HasBuff<IceDust>())
			{

				float speedXabx = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedYabx = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

				for (int d = 0; d < 20; d++)
				{

					Vector2 speedeaax = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedeaax * 7, ParticleManager.NewInstance<IceParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));

					target.AddBuff(ModContent.BuffType<EXPtime3>(), 1000);

					Projectile.timeLeft = 80;
					Timer++;
					if (Timer == 1)
					{

						ShakeModSystem.Shake = 3;
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabx, Projectile.position.Y + speedYabx, speedXabx * 0, speedYabx * 0, ModContent.ProjectileType<FrostbiteProj>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Frosty"));
						for (int da = 0; da < 4; da++)
						{

							npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
						}

						if (player.GetModPlayer<MyPlayer>().LuckyW)
						{
							if (Main.rand.NextBool(3))
							{
								CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
							}

						}

						if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
						{
							if (Main.rand.NextBool(10))
							{
								CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
								npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
								float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
								float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

							}

						}
					}


					if (Timer == 45)
					{

						if (npc.active && npc.HasBuff<EXPtime3>())
						{
							ShakeModSystem.Shake = 3;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabx, Projectile.position.Y + speedYabx, speedXabx * 0, speedYabx * 0, ModContent.ProjectileType<FrostbiteProj>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Frosty"));
							for (int da = 0; da < 6; da++)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

								if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
								{
									if (Main.rand.NextBool(10))
									{
										CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
										npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
										float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
										float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
										npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
										Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

									}

								}
							}






						}

					}
					if (Timer == 70)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}





					





				}
			}

			NPC npcaa = target;
			if (npcaa.active && npcaa.HasBuff<GrassBuff>())
			{
				ShakeModSystem.Shake = 3;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Dirt"));
				for (int jf = 0; jf < 3; jf++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 5, ParticleManager.NewInstance<LeafParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					ParticleManager.NewParticle(Projectile.Center, speed * 3, ParticleManager.NewInstance<DustaParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(3))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
					}

				}
			}


			NPC npcaab = target;
			if (npcaab.active && npcaab.HasBuff<FlowerBuff>())
			{
				ShakeModSystem.Shake = 3;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/windpetal"));
				for (int jf = 0; jf < 2; jf++)
				{
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Vector2 speed2 = Main.rand.NextVector2Circular(1f, 1f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 5, ParticleManager.NewInstance<ArcanalParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					ParticleManager.NewParticle(Projectile.Center, speed * 7, ParticleManager.NewInstance<FlowerParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(3))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}



			NPC npca = target;
			if (npca.active && npca.HasBuff<SongDust>())
			{
				ShakeModSystem.Shake = 6;

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/LenaSongEx"));
				for (int j = 0; j < 2; j++)
				{

					Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
					ParticleManager.NewParticle(Projectile.Center, speed2 * 7, ParticleManager.NewInstance<LenaSongParticle>(), Color.RoyalBlue, Main.rand.NextFloat(0.2f, 0.8f));
					npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);

				}

				if (player.GetModPlayer<MyPlayer>().LuckyW)
				{
					if (Main.rand.NextBool(3))
					{
						CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
					}

				}

				if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
				{
					if (Main.rand.NextBool(10))
					{
						CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
						npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
						float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
						npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

					}

				}
			}






			if (npc.active && npc.HasBuff<TrickDust>())
			{

				float speedXabxa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedYabxa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

				for (int d = 0; d < 20; d++)
				{

					Vector2 speedeaaxa = Main.rand.NextVector2CircularEdge(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedeaaxa * 7, ParticleManager.NewInstance<GildParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));

					target.AddBuff(ModContent.BuffType<EXPtime4>(), 1000);

					Projectile.timeLeft = 130;
					Timer++;
					if (Timer == 1)
					{

						ShakeModSystem.Shake = 3;
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<TrickbiteProj>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Burnbefore"));

						for (int da = 0; da < 4; da++)
						{

							npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

							if (player.GetModPlayer<MyPlayer>().LuckyW)
							{
								if (Main.rand.NextBool(3))
								{
									CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
									npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
								}

							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{
								if (Main.rand.NextBool(10))
								{
									CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
									npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);

									npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
									Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

								}

							}
						}



					}


					if (Timer == 45)
					{

						if (npc.active && npc.HasBuff<EXPtime4>())
						{
							ShakeModSystem.Shake = 8;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoomTrick>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/trickbomb"));


							npc.SimpleStrikeNPC(Projectile.damage * 9, 1, crit: false, Projectile.knockBack);



							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{
								if (Main.rand.NextBool(1))
								{
									CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
									npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);

									npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
									Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

								}

							}



						}

					}


					if (Timer == 90)
					{

						if (npc.active && npc.HasBuff<EXPtime4>())
						{
							ShakeModSystem.Shake = 8;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoomTrick>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/trickbomb"));


							npc.SimpleStrikeNPC(Projectile.damage * 17, 1, crit: false, Projectile.knockBack);


							if (player.GetModPlayer<MyPlayer>().LuckyW)
							{
								if (Main.rand.NextBool(3))
								{
									CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
									npc.SimpleStrikeNPC(Projectile.damage * 17, 1, crit: false, Projectile.knockBack);
								}

							}


							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXabxa, Projectile.position.Y + speedYabxa, speedXabxa * 0, speedYabxa * 0, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}

						}

					}
					if (Timer == 120)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}





				}
			}





			if (npc.active && npc.HasBuff<CoalBuff>())
			{
				for (int da = 0; da < 3; da++)
				{

					npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);

					if (player.GetModPlayer<MyPlayer>().LuckyW)
					{
						if (Main.rand.NextBool(5))
						{
							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
							npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
						}

					}


					if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
					{
						if (Main.rand.NextBool(10))
						{
							CombatText.NewText(player.getRect(), Color.LightSkyBlue, LangText.Misc("IgniterStart.Magic"), true, false);
							npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

						}

					}
				}

				for (int d = 0; d < 20; d++)
				{
					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedea * 7, ParticleManager.NewInstance<GeodeParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));
					ParticleManager.NewParticle(Projectile.Center, speedea * 4, ParticleManager.NewInstance<CoalParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));


					






				}
			}



			if (npc.active && npc.HasBuff<PocketSandBuff>())
			{
				Projectile.ArmorPenetration = 20;
				for (int da = 0; da < 3; da++)
				{

					npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);

					if (player.GetModPlayer<MyPlayer>().LuckyW)
					{
						if (Main.rand.NextBool(3))
						{
							CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
							npc.SimpleStrikeNPC(Projectile.damage * 1, 1, crit: false, Projectile.knockBack);
						}

					}
				}

				for (int d = 0; d < 15; d++)
				{
					Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speedea * 4, ParticleManager.NewInstance<DustaParticle>(), Color.HotPink, Main.rand.NextFloat(0.2f, 0.8f));


					






				}
			}


			if (npc.active && npc.HasBuff<KaevDust>())
			{

				
				for (int j = 0; j < 20; j++)
				{
					Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
					ParticleManager.NewParticle(Projectile.Center, speed * 4, ParticleManager.NewInstance<MoonTrailParticle>(), Color.DarkRed, Main.rand.NextFloat(0.2f, 0.8f));
					target.AddBuff(BuffID.Slow, 1120);
					target.AddBuff(BuffID.BrainOfConfusionBuff, 1120);
					target.AddBuff(ModContent.BuffType<EXPtime>(), 1000);


					Projectile.timeLeft = 250;
					Timer++;
	


					if (Timer == 170)
					{
						Vector2 velocity = npc.velocity;
						if (npc.active && npc.HasBuff<EXPtime>())
						{
							SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Suckler"));
							Projectile.NewProjectile(npc.GetSource_FromThis(), npc.position, velocity * 0, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);
							Projectile.scale = 1.5f;
							ShakeModSystem.Shake = 10;
							npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
			
							float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
							float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoomKaev>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
							
							if (player.GetModPlayer<MyPlayer>().LuckyW)
							{
								if (Main.rand.NextBool(3))
								{
									CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("IgniterStart.Strike"), true, false);
									npc.SimpleStrikeNPC(Projectile.damage * 16, 1, crit: false, Projectile.knockBack);
									npc.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
								}

							}
							for (int r = 0; r < 37; r++)
							{
							
								Vector2 speed2 = Main.rand.NextVector2CircularEdge(1f, 1f);
								ParticleManager.NewParticle(Projectile.Center, speed2 * 8, ParticleManager.NewInstance<MoonTrailParticle>(), Color.DarkRed, Main.rand.NextFloat(0.2f, 0.8f));
								
								npc.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
							
							}




							if (player.GetModPlayer<MyPlayer>().FlamedTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 4, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}
							if (player.GetModPlayer<MyPlayer>().MagicTomeDusts)
							{

								npc.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
								npc.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KaBoomMagic>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


							}
						}

					}
					if (Timer == 200)
					{
						Projectile.Kill();
						npc.RequestBuffRemoval(ModContent.BuffType<Dusted>());

					}


				}
			}










			base.OnHitNPC(target, hit, damageDone);
			
				
			
		}
	}
}
		
	
