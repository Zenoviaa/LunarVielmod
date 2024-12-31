
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Reks
{
    public class RekEye2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = 27;
            Projectile.height = 34;
            Projectile.scale = 2.5f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Timer2;
        public override void AI()
        {
            if (Projectile.timeLeft <= 10)
            {
                Projectile.timeLeft = 600;
            }

            Timer2++;
            Timer++;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC player = Main.npc[k];
                if (!player.active && player.type == ModContent.NPCType<Rek>())
                {
                    if (!player.active)
                    {
                        Projectile.Kill();
                    }
                }
                // Check if NPC able to be targeted. It means that NPC is
                if (player.active && player.type == ModContent.NPCType<Rek>())
                {
                    if (!player.active)
                    {
                        Projectile.Kill();
                    }

                    Vector2 idlePosition;
                    idlePosition.X = player.Center.X - 240;
                    idlePosition.Y = player.Center.Y + 30;
                    // Go up 48 coordinates (three tiles from the center of the player)

                    // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
                    // The index is projectile.minionPos
                    float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;

                    // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

                    // Teleport to player if distance is too big
                    Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
                    float distanceToIdlePosition = vectorToIdlePosition.Length();
                    if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 1000f)
                    {
                        // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                        // and then set netUpdate to true
                        Projectile.position = idlePosition;
                        Projectile.velocity *= 0.20f;
                        Projectile.netUpdate = true;
                    }

                    // If your minion is flying, you want to do this independently of any conditions
                    float overlapVelocity = 0.04f;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        // Fix overlap with other minions
                        Projectile other = Main.projectile[i];
                        if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                        {
                            if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
                            else Projectile.velocity.X += overlapVelocity;

                            if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
                            else Projectile.velocity.Y += overlapVelocity;
                        }
                    }


                    // Starting search distance

                    Vector2 targetCenter = Projectile.position;
                    bool foundTarget = false;

                    // This code is required if your minion weapon has the targeting feature


                    // friendly needs to be set to true so the minion can deal contact damage
                    // friendly needs to be set to false so it doesn't damage things like target dummies while idling
                    // Both things depend on if it has a target or not, so it's just one assignment here
                    // You don't need this assignment if your minion is shooting things instead of dealing contact damage





                    // Default movement parameters (here for attacking)
                    float speed = 11f;
                    float inertia = 20f;
                    if (foundTarget)
                    {
                        Projectile.rotation = Projectile.velocity.X * 0.08f;
                        // Minion has a target: attack (here, fly towards the enemy)

                        if (distanceToIdlePosition > 250f)
                        {
                            // Speed up the minion if it's away from the player
                            speed = 30f;
                            inertia = 60f;
                        }
                        else
                        {
                            // Slow down the minion if closer to the player
                            speed = 4f;
                            inertia = 80f;
                        }
                        if (distanceToIdlePosition > 5f)
                        {
                            // The immediate range around the player (when it passively floats about)

                            // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            // If there is a case where it's not moving at all, give it a little "poke"
                            Projectile.velocity.X = -0.15f;
                            Projectile.velocity.Y = -0.05f;
                        }
                    }
                    else
                    {
                        Projectile.rotation = Projectile.velocity.X * 0.08f;
                        // Minion doesn't have a target: return to player and idle
                        if (distanceToIdlePosition > 240f)
                        {
                            // Speed up the minion if it's away from the player
                            speed = 30f;
                            inertia = 60f;
                        }
                        else
                        {
                            // Slow down the minion if closer to the player
                            speed = 7f;
                            inertia = 80f;
                        }
                        if (distanceToIdlePosition > 5f)
                        {
                            // The immediate range around the player (when it passively floats about)

                            // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            // If there is a case where it's not moving at all, give it a little "poke"
                            Projectile.velocity.X = -0.15f;

                        }
                    }
                }
            }
        }
    }
}