using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.Bow;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
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
    public class IceboundMinionProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private Projectile Leader
        {
            get
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active)
                        continue;
                    if (p.type == Type && p.owner == Projectile.owner)
                        return p;
                }

                return Projectile;
            }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projFrames[Projectile.type] = 4;
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

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 26;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0.5f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.RoyalBlue, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return base.PreDraw(ref lightColor);
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
            Projectile.spriteDirection = Projectile.direction;

            if (!SummonHelper.CheckMinionActive<IceboundMinionBuff>(player, Projectile))
                return;

            bool isLeader = Leader.whoAmI == Projectile.whoAmI;
            if (isLeader)
            {
                SummonHelper.SearchForTargets(player, Projectile,
                    out bool foundTarget,
                    out float distanceFromTarget, 
                    out Vector2 targetCenter);
                if (foundTarget)
                {
                    AI_MoveToward(targetCenter, 12, 2);
                }
                else
                {
                    Vector2 idlePosition = player.Center + new Vector2(0, -48);
                    SummonHelper.CalculateIdleValuesWithOverlap(player, Projectile,
                        out Vector2 vectorToIdlePosition,
                        out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                }
            }
            else
            {
                SummonHelper.SearchForTargets(player, Leader,
                    out bool foundTarget,
                    out float distanceFromTarget,
                    out Vector2 foundTargetCenter);
                if (!foundTarget)
                {
                    SummonHelper.CalculateIdleValues(player, Projectile,
                        Leader.Center,
                            
                           out Vector2 vectorToIdlePosition,
                           out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                }
                else
                {
                    Vector2 targetCenter = Leader.Center;
                    float distanceToLeader = Vector2.Distance(Projectile.Center, targetCenter);
                    if (distanceToLeader > 64)
                    {
                        AI_MoveToward(targetCenter, 16, 1);
                    }
                }
            }

            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity = Main.rand.NextVector2CircularEdge(16, 16);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
            if (Main.rand.NextBool(16))
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                    ModContent.ProjectileType<WinterboundArrowFlake>(), Projectile.damage / 2, 1, Projectile.owner);
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);


            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
