using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    /*
		 * This minion shows a few mandatory things that make it behave properly. 
		 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
		 * If the player targets a certain NPC with right-click, it will fly through tiles to it
		 * If it isn't attacking, it will float near the player with minimal movement
		 */
    public class TheBurningRodMinionProj : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("HMArncharMinion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
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

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
            return true;
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8, float accel = 16)
        {
            //chase target
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;
            if (Projectile.velocity.X < targetVelocity.X)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X >= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }
            else if (Projectile.velocity.X > targetVelocity.X)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X <= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }

            if (Projectile.velocity.Y < targetVelocity.Y)
            {
                Projectile.velocity.Y += accel;
                if (Projectile.velocity.Y >= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
            else if (Projectile.velocity.Y > targetVelocity.Y)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y <= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<TheBurningRodMinionBuff>(player, Projectile))
                return;

            SummonHelper.SearchForTargets(player, Projectile, 
                out bool foundTarget, 
                out float distanceFromTarget, 
                out Vector2 targetCenter);
            if (!foundTarget)
            {
                SummonHelper.CalculateIdleValues(player, Projectile, 
                    out Vector2 vectorToIdlePosition, 
                    out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            else
            {
                if (distanceFromTarget > 252)
                {
                    AI_MoveToward(targetCenter, 10, 1);
                }
                else
                {
                    Projectile.velocity *= 0.9f;
                    Timer++;
                    if (Timer >= 30 && distanceFromTarget < 252)
                    {
                        Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 8;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4 / 3),
                            ProjectileID.GoldenShowerFriendly, Projectile.damage, 0f, Projectile.owner);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4 / 3),
                            ProjectileID.GoldenShowerFriendly, Projectile.damage, 0f, Projectile.owner);
                        Timer = 0;
                    }
                }
            }

            Visuals();
        }

        private void Visuals()
        {
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
