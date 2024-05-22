using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Helpers
{
    public static class SummonHelper
    {
        public static void CalculateIdleValuesWithOverlap(Player owner, Projectile projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                projectile.position = idlePosition;
                projectile.velocity *= 0.1f;
                //Projectile.netUpdate = true;
            }
        }
        public static void CalculateIdleValues(Player owner, Projectile projectile, Vector2 idlePosition, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                projectile.position = idlePosition;
                projectile.velocity *= 0.1f;
                //Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != projectile.whoAmI && other.active && other.owner == projectile.owner && Math.Abs(projectile.position.X - other.position.X) + Math.Abs(projectile.position.Y - other.position.Y) < projectile.width)
                {
                    if (projectile.position.X < other.position.X)
                    {
                        projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        projectile.velocity.X += overlapVelocity;
                    }

                    if (projectile.position.Y < other.position.Y)
                    {
                        projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        public static void CalculateIdleValues(Player owner, Projectile projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
		{
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
			{
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				projectile.position = idlePosition;
				projectile.velocity *= 0.1f;
				//Projectile.netUpdate = true;
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile other = Main.projectile[i];

				if (i != projectile.whoAmI && other.active && other.owner == projectile.owner && Math.Abs(projectile.position.X - other.position.X) + Math.Abs(projectile.position.Y - other.position.Y) < projectile.width)
				{
					if (projectile.position.X < other.position.X)
					{
						projectile.velocity.X -= overlapVelocity;
					}
					else
					{
						projectile.velocity.X += overlapVelocity;
					}

					if (projectile.position.Y < other.position.Y)
					{
						projectile.velocity.Y -= overlapVelocity;
					}
					else
					{
						projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		/// <summary>
		/// Makes the projectile hover around the target position
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="distanceToIdlePosition"></param>
		/// <param name="vectorToIdlePosition"></param>
		public static void Idle(Projectile projectile, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
		{
			float speed;
			float inertia;

			// Minion doesn't have a target: return to player and idle
			if (distanceToIdlePosition > 100f)
			{
				// Speed up the minion if it's away from the player
				speed = 20f;
				inertia = 80f;
			}
			else
			{
				// Slow down the minion if closer to the player
				speed = 3f;
				inertia = 100f;
			}

			if (distanceToIdlePosition > 20f)
			{
				// The immediate range around the player (when it passively floats about)
				// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
			}
			else if (projectile.velocity == Vector2.Zero)
			{
				// If there is a case where it's not moving at all, give it a little "poke"
				projectile.velocity.X = -0.28f;
				projectile.velocity.Y = -0.14f;
			}
		}


		/// <summary>
		/// Refreshes the minion buff, call this in the update function for your minion's buff
		/// <br>Returns true if the buff is active, false if not</br>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="player"></param>
		/// <param name="buffIndex"></param>
		/// <returns></returns>
		public static bool UpdateMinionBuff<T>(Player player, ref int buffIndex) where T : ModProjectile
        {
			if (player.ownedProjectileCounts[ProjectileType<T>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
				return true;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return false;
			}
		}

		/// <summary>
		/// Checks if a minion is active, and clears the buff if it is not
		/// <br>Returns true if the minion is active, false if not</br>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="owner"></param>
		/// <param name="minion"></param>
		/// <returns></returns>
		public static bool CheckMinionActive<T>(Player owner, Projectile minion) where T : ModBuff
        {
			if (owner.dead || !owner.active)
			{
				owner.ClearBuff(BuffType<T>());
				return false;
			}

			if (owner.HasBuff(BuffType<T>()))
			{
				minion.timeLeft = 2;
			}

			return true;
		}

		/// <summary>
		/// Returns the index of the projectile relative to the owner and its type. 
		/// <br>Example: If you own 7 fireflies, it would return an index 0-6 depending on which is passed.</br>
		/// </summary>
		/// <param name="minion"></param>
		/// <returns></returns>
		public static int GetProjectileIndex(Projectile minion) 
        {
			int minionIndex = 0;
			// Fix overlap with other minions
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile other = Main.projectile[i];

				//Ignore projectiles that are not fireflies and are from a different owner.
				if (other.type != minion.type)
					continue;
				if (other.owner != minion.owner)
					continue;

				//If the project is not me, then increase the index.
				if (i != minion.whoAmI)
				{
					minionIndex++;
				}
				else
				{
					//We found ourselves, therefore we have our minion index.
					break;
				}
			}

			return minionIndex;
		}

		private static bool IsCorrectType(int targetType, int[] types)
		{
			for(int i = 0; i < types.Length; i++)
			{
				if (targetType == types[i])
				{
					return true;
				}
			}
			return false;
		}

        public static int GetProjectileIndexMulti(Projectile projectile, params int[] types)
        {
            int minionIndex = 0;
            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.owner != projectile.owner)
                    continue;

                //Ignore projectiles that are not fireflies and are from a different owner.
				if(!IsCorrectType(other.type, types))
                    continue;
      
                //If the project is not me, then increase the index.
                if (i != projectile.whoAmI)
                {
                    minionIndex++;
                }
                else
                {
                    //We found ourselves, therefore we have our minion index.
                    break;
                }
            }

            return minionIndex;
        }

        /// <summary>
        /// Basic search function that finds a valid target that can be chased by the given projectile
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="minion"></param>
        /// <param name="foundTarget"></param>
        /// <param name="distanceFromTarget"></param>
        /// <param name="targetCenter"></param>
        /// <param name="startingDistanceFromTarget"></param>
        public static void SearchForTargets(Player owner, Projectile minion, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, float startingSearchDistanceFromTarget = 700f)
		{
			// Starting search distance
			distanceFromTarget = startingSearchDistanceFromTarget;
			targetCenter = minion.position;
			foundTarget = false;
			if (owner.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, minion.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f)
				{
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget)
			{
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, minion.Center);
						bool closest = Vector2.Distance(minion.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(minion.position, minion.width, minion.height, npc.position, npc.width, npc.height);
						if (((closest && inRange) || !foundTarget) && lineOfSight)
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
		}

        public static void SearchForTargetsThroughTiles(Player owner, Projectile minion, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, float startingSearchDistanceFromTarget = 700f)
        {
            // Starting search distance
            distanceFromTarget = startingSearchDistanceFromTarget;
            targetCenter = minion.position;
            foundTarget = false;
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, minion.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, minion.Center);
                        bool closest = Vector2.Distance(minion.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        if (((closest && inRange) || !foundTarget))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
        }
    }
}
